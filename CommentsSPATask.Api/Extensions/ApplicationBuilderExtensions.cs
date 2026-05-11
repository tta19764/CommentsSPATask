using CommentsSPATask.Api.Middleware;
using CommentsSPATask.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

namespace CommentsSPATask.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        const int maxAttempts = 10;
        var delay = TimeSpan.FromSeconds(5);

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                dbContext.Database.Migrate();
                return;
            }
            catch (SqlException ex) when (attempt < maxAttempts && IsRetryableStartupError(ex))
            {
                logger.LogWarning(
                    ex,
                    "Database migration attempt {Attempt}/{MaxAttempts} failed. Retrying in {DelaySeconds} seconds.",
                    attempt,
                    maxAttempts,
                    delay.TotalSeconds);

                Thread.Sleep(delay);
            }
        }

        dbContext.Database.Migrate();
    }

    private static bool IsRetryableStartupError(SqlException exception)
    {
        foreach (SqlError error in exception.Errors)
        {
            if (error.Number is 1801 or 4060 or 18456 or 233 or 1205)
            {
                return true;
            }
        }

        return false;
    }

    public static void UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    public static IApplicationBuilder UseRequestContextLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestContextLoggingMiddleware>();

        return app;
    }

    public static IApplicationBuilder UseUploadedFiles(this IApplicationBuilder app)
    {
        var uploadsPath = Path.Combine(AppContext.BaseDirectory, "uploads");
        Directory.CreateDirectory(uploadsPath);

        var contentTypeProvider = new FileExtensionContentTypeProvider();
        contentTypeProvider.Mappings[".txt"] = "text/plain; charset=utf-8";

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(uploadsPath),
            RequestPath = "/uploads",
            ContentTypeProvider = contentTypeProvider
        });

        return app;
    }

    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Comments SPA Task API v1");
            options.RoutePrefix = "swagger";
        });

        return app;
    }
}
