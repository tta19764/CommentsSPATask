using CommentsSPATask.Api.Captchas;
using CommentsSPATask.Api.BackgroundJobs;
using CommentsSPATask.Api.Realtime;
using CommentsSPATask.Application.Abstractions.Captcha;
using CommentsSPATask.Application.Abstractions.Realtime;
using System.Text.Json.Serialization;

namespace CommentsSPATask.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("Client", policy =>
            {
                var localOrigins = new[]
                {
                    "http://localhost:8080",
                    "http://127.0.0.1:8080",
                    "http://localhost:5173",
                    "http://127.0.0.1:5173"
                };

                var configuredOrigins = configuration
                    .GetSection("Cors:AllowedOrigins")
                    .Get<string[]>() ?? [];

                var allowedOrigins = localOrigins
                    .Concat(configuredOrigins)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        services.AddEndpointsApiExplorer();
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        services.AddMemoryCache();
        services.AddSignalR();
        services.AddSingleton<ICaptchaImageStore, InMemoryCaptchaImageStore>();
        services.AddHostedService<ExpiredCaptchaCleanupService>();
        services.AddSingleton<ICommentsRealtimeNotifier, SignalRCommentsRealtimeNotifier>();
        services.AddSwaggerGen();

        return services;
    }
}
