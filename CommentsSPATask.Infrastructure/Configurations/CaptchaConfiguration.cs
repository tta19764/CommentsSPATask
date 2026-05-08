using CommentsSPATask.Domain.Captchas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommentsSPATask.Infrastructure.Configurations;

public sealed class CaptchaConfiguration : IEntityTypeConfiguration<Captcha>
{
    public void Configure(EntityTypeBuilder<Captcha> builder)
    {
        builder.ToTable("Captchas");
        
        builder.HasKey(captcha => captcha.Id);
        
        builder.Property(captcha => captcha.Id).IsRequired();
        
        builder.Property(captcha => captcha.CodeHash)
            .IsRequired()
            .HasConversion(code => code.Value, code => new CodeHash(code));
        
        builder.Property(captcha => captcha.IsUsed)
            .IsRequired()
            .HasConversion(isUsed => isUsed.Value, value => new IsUsed(value));
        
        builder.Property(captcha => captcha.CreatedAtUtc)
            .IsRequired();
        
        builder.Property(captcha => captcha.ExpiresAtUtc)
            .IsRequired();
        
        builder.Property(captcha => captcha.FailedAttemptsCount)
            .IsRequired()
            .HasConversion(failedAttemptsCount => failedAttemptsCount.Value, value => new FailedAttemptsCount(value));
    }
}