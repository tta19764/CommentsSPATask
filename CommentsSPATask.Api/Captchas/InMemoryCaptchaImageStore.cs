using System;
using System.Threading;
using System.Threading.Tasks;
using CommentsSPATask.Application.Abstractions.Captcha;
using Microsoft.Extensions.Caching.Memory;

namespace CommentsSPATask.Api.Captchas;

public sealed class InMemoryCaptchaImageStore(IMemoryCache cache) : ICaptchaImageStore
{
    public Task StoreAsync(
        Guid captchaId,
        byte[] imageBytes,
        DateTime expiresAtUtc,
        CancellationToken cancellationToken = default)
    {
        cache.Set(GetCacheKey(captchaId), imageBytes, expiresAtUtc);
        return Task.CompletedTask;
    }

    public Task<byte[]?> GetAsync(Guid captchaId, CancellationToken cancellationToken = default)
    {
        cache.TryGetValue(GetCacheKey(captchaId), out byte[]? imageBytes);
        return Task.FromResult(imageBytes);
    }

    public Task RemoveAsync(Guid captchaId, CancellationToken cancellationToken = default)
    {
        cache.Remove(GetCacheKey(captchaId));
        return Task.CompletedTask;
    }

    private static string GetCacheKey(Guid captchaId) => $"captcha-image:{captchaId:N}";
}
