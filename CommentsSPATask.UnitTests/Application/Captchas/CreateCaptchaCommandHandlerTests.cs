using CommentsSPATask.Application.Abstractions.Captcha;
using CommentsSPATask.Application.Abstractions.Clock;
using CommentsSPATask.Application.Captchas.Commands.CreateCaptcha;
using CommentsSPATask.Domain.Abstractions;
using CommentsSPATask.Domain.Captchas;
using Microsoft.Extensions.Logging;
using Moq;

namespace CommentsSPATask.UnitTests.Application.Captchas;

public sealed class CreateCaptchaCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateAndPersistCaptcha()
    {
        var captchaRepository = new Mock<ICaptchaRepository>();
        var captchaService = new Mock<ICaptchaChallengeService>();
        var captchaImageStore = new Mock<ICaptchaImageStore>();
        var dateTimeProvider = new Mock<IDateTimeProvider>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var logger = new Mock<ILogger<CreateCaptchaCommandHandler>>();

        captchaService.Setup(x => x.GenerateCode(5)).Returns("ABCDE");
        captchaService.Setup(x => x.HashCode("ABCDE")).Returns("HASH");
        captchaService.Setup(x => x.RenderImage("ABCDE")).Returns([1, 2, 3]);
        dateTimeProvider.SetupGet(x => x.UtcNow).Returns(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        var handler = new CreateCaptchaCommandHandler(
            captchaRepository.Object,
            captchaService.Object,
            captchaImageStore.Object,
            dateTimeProvider.Object,
            unitOfWork.Object,
            logger.Object);

        var result = await handler.Handle(new CreateCaptchaCommand(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.CaptchaId);
        Assert.Equal(new DateTime(2026, 1, 1, 0, 5, 0, DateTimeKind.Utc), result.Value.ExpiresAtUtc);
        captchaRepository.Verify(x => x.Add(It.IsAny<Captcha>()), Times.Once);
        captchaImageStore.Verify(x => x.StoreAsync(
            result.Value.CaptchaId,
            It.Is<byte[]>(bytes => bytes.SequenceEqual(new byte[] { 1, 2, 3 })),
            result.Value.ExpiresAtUtc,
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
