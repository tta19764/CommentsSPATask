using CommentsSPATask.Application.Abstractions.Attachments;
using CommentsSPATask.Application.Abstractions.Captcha;
using CommentsSPATask.Application.Abstractions.Clock;
using CommentsSPATask.Application.Abstractions.Comments;
using CommentsSPATask.Application.Abstractions.Data;
using CommentsSPATask.Application.Abstractions.Realtime;
using CommentsSPATask.Domain.Abstractions;
using CommentsSPATask.Domain.Attachments;
using CommentsSPATask.Domain.Captchas;
using CommentsSPATask.Domain.Comments;
using CommentsSPATask.Infrastructure.Attachments;
using CommentsSPATask.Infrastructure.Captchas;
using CommentsSPATask.Infrastructure.Clock;
using CommentsSPATask.Infrastructure.Comments;
using CommentsSPATask.Infrastructure.Data;
using CommentsSPATask.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommentsSPATask.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IAttachmentProcessor, FileSystemAttachmentProcessor>();
        services.AddSingleton<ICaptchaChallengeService, CaptchaChallengeService>();
        services.AddSingleton<ICommentHtmlPolicy, CommentHtmlPolicy>();

        AddPersistence(services, configuration);

        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IAttachmentRepository, AttachmentRepository>();
        services.AddScoped<ICaptchaRepository, CaptchaRepository>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddSingleton<ISqlConnectionFactory>(_ =>
            new SqlConnectionFactory(connectionString));
    }
}
