using CommentsSPATask.Domain.Abstractions;
using CommentsSPATask.Domain.Captchas.Events;

namespace CommentsSPATask.Domain.Captchas;

public sealed class Captcha : Entity
{
    private Captcha(
        Guid id,
        CodeHash codeHash,
        DateTime createdAtUtc,
        DateTime expiresAtUtc,
        IsUsed isUsed)
        : base(id)
    {
        CodeHash = codeHash;
        CreatedAtUtc = createdAtUtc;
        ExpiresAtUtc = expiresAtUtc;
        IsUsed = isUsed;
    }

    public CodeHash CodeHash { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime ExpiresAtUtc { get; private set; }

    public IsUsed IsUsed { get; private set; }

    public static Captcha Create(
        CodeHash codeHash,
        DateTime createdAtUtc,
        DateTime expiresAtUtc)
    {
        var captcha = new Captcha(
            Guid.NewGuid(),
            codeHash,
            createdAtUtc,
            expiresAtUtc,
            new IsUsed());
        
        return captcha;
    }

    public bool IsExpired(DateTime utcNow) => utcNow >= ExpiresAtUtc;

    public void MarkAsUsed()
    {
        IsUsed.SetUsed();
        RaiseDomainEvent(new CaptchaUsedDomainEvent(Id));
    }

    public bool MarkAsExpiredIfNeeded(DateTime utcNow)
    {
        if (!IsExpired(utcNow))
        {
            return false;
        }

        RaiseDomainEvent(new CaptchaExpiredDomainEvent(Id));
        return true;
    }
}
