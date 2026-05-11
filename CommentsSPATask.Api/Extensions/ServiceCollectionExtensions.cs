using CommentsSPATask.Api.Captchas;
using CommentsSPATask.Api.Realtime;
using CommentsSPATask.Application.Abstractions.Captcha;
using CommentsSPATask.Application.Abstractions.Realtime;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace CommentsSPATask.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("Client", policy =>
            {
                policy
                    .WithOrigins(
                        "http://localhost:8080",
                        "http://127.0.0.1:8080",
                        "http://localhost:5173",
                        "http://127.0.0.1:5173")
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
        services.AddSingleton<ICommentsRealtimeNotifier, SignalRCommentsRealtimeNotifier>();
        services.AddSwaggerGen();

        return services;
    }
}
