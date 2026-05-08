using CommentsSPATask.Application.Abstractions.Attachments;
using CommentsSPATask.Application.Abstractions.Captcha;
using CommentsSPATask.Application.Abstractions.Clock;
using CommentsSPATask.Application.Abstractions.Comments;
using CommentsSPATask.Application.Abstractions.Messaging;
using CommentsSPATask.Domain.Abstractions;
using CommentsSPATask.Domain.Attachments;
using CommentsSPATask.Domain.Captchas;
using CommentsSPATask.Domain.Comments;
using Microsoft.Extensions.Logging;

namespace CommentsSPATask.Application.Comments.Commands.CreateComment;

public sealed class CreateCommentCommandHandler(
    ICommentRepository commentRepository,
    ICaptchaRepository captchaRepository,
    IAttachmentRepository attachmentRepository,
    IAttachmentProcessor attachmentProcessor,
    ICommentHtmlPolicy commentHtmlPolicy,
    ICaptchaChallengeService captchaChallengeService,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork,
    ILogger<CreateCommentCommandHandler> logger)
    : ICommandHandler<CreateCommentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Creating comment. ParentId: {ParentId}, CaptchaId: {CaptchaId}, HasAttachment: {HasAttachment}",
            request.ParentId,
            request.CaptchaId,
            request.Attachment is not null);

        if (request.ParentId.HasValue)
        {
            var parentComment = await commentRepository.GetByIdAsync(request.ParentId.Value, cancellationToken);

            if (parentComment is null)
            {
                logger.LogWarning("Comment creation rejected because parent comment {ParentId} was not found", request.ParentId.Value);
                return Result.Failure<Guid>(CommentErrors.ParentCommentNotFound);
            }
        }

        var captcha = await captchaRepository.GetByIdAsync(request.CaptchaId, cancellationToken);

        if (captcha is null)
        {
            logger.LogWarning("Comment creation rejected because captcha {CaptchaId} was not found", request.CaptchaId);
            return Result.Failure<Guid>(CaptchaErrors.NotFound);
        }

        var nowUtc = dateTimeProvider.UtcNow;

        if (captcha.IsUsed.Value)
        {
            logger.LogWarning("Comment creation rejected because captcha {CaptchaId} is already used", request.CaptchaId);
            return Result.Failure<Guid>(CaptchaErrors.AlreadyUsed);
        }

        if (captcha.MarkAsExpiredIfNeeded(nowUtc))
        {
            logger.LogWarning("Comment creation rejected because captcha {CaptchaId} is expired", request.CaptchaId);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Failure<Guid>(CaptchaErrors.Expired);
        }

        if (captcha.IsBlocked())
        {
            logger.LogWarning("Comment creation rejected because captcha {CaptchaId} is blocked", request.CaptchaId);
            return Result.Failure<Guid>(CaptchaErrors.Blocked);
        }

        if (!captchaChallengeService.VerifyCode(request.CaptchaInput, captcha.CodeHash.Value))
        {
            logger.LogWarning("Comment creation rejected because captcha {CaptchaId} input verification failed", request.CaptchaId);
            captcha.RegisterFailedAttempt();
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Failure<Guid>(CaptchaErrors.InvalidInput);
        }

        var sanitizedTextResult = commentHtmlPolicy.ValidateAndSanitize(request.Text);

        if (sanitizedTextResult.IsFailure)
        {
            logger.LogWarning(
                "Comment creation rejected because text sanitization failed with error {ErrorCode}",
                sanitizedTextResult.Error.Code);
            return Result.Failure<Guid>(sanitizedTextResult.Error == Error.None
                ? CommentErrors.InvalidHtml
                : sanitizedTextResult.Error);
        }

        var comment = Comment.Create(
            request.ParentId,
            new UserName(request.UserName),
            new Email(request.Email),
            string.IsNullOrWhiteSpace(request.HomePage) ? null : new HomePage(request.HomePage),
            new CommentText(sanitizedTextResult.Value));

        await commentRepository.AddAsync(comment, cancellationToken);
        logger.LogInformation("Comment {CommentId} was created and queued for persistence", comment.Id);

        if (request.Attachment is not null)
        {
            logger.LogInformation("Preparing attachment for comment {CommentId}", comment.Id);
            var preparedAttachmentResult = await attachmentProcessor.PrepareAsync(request.Attachment, cancellationToken);

            if (preparedAttachmentResult.IsFailure)
            {
                logger.LogWarning(
                    "Attachment preparation failed for comment {CommentId} with error {ErrorCode}",
                    comment.Id,
                    preparedAttachmentResult.Error.Code);
                return Result.Failure<Guid>(preparedAttachmentResult.Error);
            }

            var preparedAttachment = preparedAttachmentResult.Value;

            var attachment = Attachment.Create(
                comment.Id,
                new OriginalFileName(preparedAttachment.OriginalFileName),
                new StoredFileName(preparedAttachment.StoredFileName),
                preparedAttachment.ContentType,
                nowUtc);

            await attachmentRepository.AddAsync(attachment, cancellationToken);
            logger.LogInformation("Attachment {AttachmentId} was created for comment {CommentId}", attachment.Id, comment.Id);
        }

        captcha.MarkAsUsed();

        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Comment {CommentId} was persisted successfully", comment.Id);

        return Result.Success(comment.Id);
    }
}
