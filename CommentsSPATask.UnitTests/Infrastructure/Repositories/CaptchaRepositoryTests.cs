using CommentsSPATask.Domain.Captchas;
using CommentsSPATask.Infrastructure.Repositories;
using CommentsSPATask.UnitTests.Helpers;

namespace CommentsSPATask.UnitTests.Infrastructure.Repositories;

public sealed class CaptchaRepositoryTests : DbContextTestBase
{
    [Fact]
    public async Task Remove_ShouldDeleteCaptcha()
    {
        await using var context = CreateContext();
        var repository = new CaptchaRepository(context);
        var captcha = Captcha.Create(new CodeHash("HASH"), DateTime.UtcNow, DateTime.UtcNow.AddMinutes(5));
        captcha.ClearDomainEvents();

        repository.Add(captcha);
        await context.SaveChangesAsync();

        repository.Remove(captcha);
        await context.SaveChangesAsync();

        var result = await repository.GetByIdAsync(captcha.Id);

        Assert.Null(result);
    }
}
