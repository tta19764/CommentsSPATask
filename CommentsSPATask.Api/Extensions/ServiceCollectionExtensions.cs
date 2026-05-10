using CommentsSPATask.Api.Captchas;
using CommentsSPATask.Api.Realtime;
using CommentsSPATask.Application.Abstractions.Captcha;
using CommentsSPATask.Application.Abstractions.Realtime;
using System.Text.Json.Serialization;

namespace CommentsSPATask.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
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
