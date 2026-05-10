using CommentsSPATask.Application.Abstractions.Attachments;
using CommentsSPATask.Application.Abstractions.Captcha;
using CommentsSPATask.Application.Abstractions.Clock;
using CommentsSPATask.Application.Abstractions.Comments;
using CommentsSPATask.Application.Comments.Commands.CreateComment;
using CommentsSPATask.Domain.Abstractions;
using CommentsSPATask.Domain.Attachments;
using CommentsSPATask.Domain.Captchas;
using CommentsSPATask.Domain.Comments;
using Microsoft.Extensions.Logging;
using Moq;

namespace CommentsSPATask.UnitTests.Application.Comments;

public sealed class CreateCommentCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenParentCommentDoesNotExist()
    {
        var commentRepository = new Mock<ICommentRepository>();
        var captchaRepository = new Mock<ICaptchaRepository>();
        var attachmentRepository = new Mock<IAttachmentRepository>();
        var attachmentProcessor = new Mock<IAttachmentProcessor>();
        var commentHtmlPolicy = new Mock<ICommentHtmlPolicy>();
        var captchaService = new Mock<ICaptchaChallengeService>();
        var dateTimeProvider = new Mock<IDateTimeProvider>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var logger = new Mock<ILogger<CreateCommentCommandHandler>>();

        commentRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Comment?)null);

        var handler = new CreateCommentCommandHandler(
            commentRepository.Object,
            captchaRepository.Object,
            attachmentRepository.Object,
            attachmentProcessor.Object,
            commentHtmlPolicy.Object,
            captchaService.Object,
            dateTimeProvider.Object,
            unitOfWork.Object,
            logger.Object);

        var result = await handler.Handle(
            new CreateCommentCommand("User1", "user1@example.com", null, "text", Guid.NewGuid(), "ABC12", Guid.NewGuid()),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(CommentErrors.ParentCommentNotFound, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCaptchaInputIsInvalid()
    {
        var commentRepository = new Mock<ICommentRepository>();
        var captchaRepository = new Mock<ICaptchaRepository>();
        var attachmentRepository = new Mock<IAttachmentRepository>();
        var attachmentProcessor = new Mock<IAttachmentProcessor>();
        var commentHtmlPolicy = new Mock<ICommentHtmlPolicy>();
        var captchaService = new Mock<ICaptchaChallengeService>();
        var dateTimeProvider = new Mock<IDateTimeProvider>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var logger = new Mock<ILogger<CreateCommentCommandHandler>>();
        var captcha = Captcha.Create(new CodeHash("HASH"), DateTime.UtcNow, DateTime.UtcNow.AddMinutes(5));

        captchaRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(captcha);
        captchaService.Setup(x => x.VerifyCode("WRONG", "HASH")).Returns(false);
        dateTimeProvider.SetupGet(x => x.UtcNow).Returns(DateTime.UtcNow);

        var handler = new CreateCommentCommandHandler(
            commentRepository.Object,
            captchaRepository.Object,
            attachmentRepository.Object,
            attachmentProcessor.Object,
            commentHtmlPolicy.Object,
            captchaService.Object,
            dateTimeProvider.Object,
            unitOfWork.Object,
            logger.Object);

        var result = await handler.Handle(
            new CreateCommentCommand("User1", "user1@example.com", null, "text", Guid.NewGuid(), "WRONG"),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(CaptchaErrors.InvalidInput, result.Error);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCreateComment_WhenRequestIsValid()
    {
        var commentRepository = new Mock<ICommentRepository>();
        var captchaRepository = new Mock<ICaptchaRepository>();
        var attachmentRepository = new Mock<IAttachmentRepository>();
        var attachmentProcessor = new Mock<IAttachmentProcessor>();
        var commentHtmlPolicy = new Mock<ICommentHtmlPolicy>();
        var captchaService = new Mock<ICaptchaChallengeService>();
        var dateTimeProvider = new Mock<IDateTimeProvider>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var logger = new Mock<ILogger<CreateCommentCommandHandler>>();
        var captcha = Captcha.Create(new CodeHash("HASH"), DateTime.UtcNow, DateTime.UtcNow.AddMinutes(5));

        captchaRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(captcha);
        captchaService.Setup(x => x.VerifyCode("ABCDE", "HASH")).Returns(true);
        commentHtmlPolicy.Setup(x => x.ValidateAndSanitize("text"))
            .Returns(Result.Success("text"));
        dateTimeProvider.SetupGet(x => x.UtcNow).Returns(DateTime.UtcNow);

        var handler = new CreateCommentCommandHandler(
            commentRepository.Object,
            captchaRepository.Object,
            attachmentRepository.Object,
            attachmentProcessor.Object,
            commentHtmlPolicy.Object,
            captchaService.Object,
            dateTimeProvider.Object,
            unitOfWork.Object,
            logger.Object);

        var result = await handler.Handle(
            new CreateCommentCommand("User1", "user1@example.com", null, "text", Guid.NewGuid(), "ABCDE"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        commentRepository.Verify(x => x.Add(It.IsAny<Comment>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
