using CommentsSPATask.Application.Abstractions.Clock;
using CommentsSPATask.Domain.Abstractions;
using CommentsSPATask.Domain.Captchas;
using CommentsSPATask.Domain.Captchas.Events;
using MediatR;

namespace CommentsSPATask.Application.Captchas.EventHandlers;

public sealed class CaptchaBlockedDomainEventHandler(
    ICaptchaRepository captchaRepository,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : INotificationHandler<CaptchaBlockedDomainEvent>
{
    public async Task Handle(CaptchaBlockedDomainEvent notification, CancellationToken cancellationToken)
    {
        var captcha = await captchaRepository.GetByIdAsync(notification.CaptchaId, cancellationToken);

        if (captcha is null)
        {
            return;
        }

        captchaRepository.Remove(captcha);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var removedCount = await captchaRepository.RemoveExpiredCaptchasAsync(
            dateTimeProvider.UtcNow.Subtract(TimeSpan.FromMinutes(30)),
            cancellationToken);

        if (removedCount > 0)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
