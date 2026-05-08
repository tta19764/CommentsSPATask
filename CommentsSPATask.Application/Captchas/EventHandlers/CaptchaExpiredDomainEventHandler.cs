using CommentsSPATask.Domain.Abstractions;
using CommentsSPATask.Domain.Captchas;
using CommentsSPATask.Domain.Captchas.Events;
using MediatR;

namespace CommentsSPATask.Application.Captchas.EventHandlers;

public sealed class CaptchaExpiredDomainEventHandler(
    ICaptchaRepository captchaRepository,
    IUnitOfWork unitOfWork) : INotificationHandler<CaptchaExpiredDomainEvent>
{
    public async Task Handle(CaptchaExpiredDomainEvent notification, CancellationToken cancellationToken)
    {
        var captcha = await captchaRepository.GetByIdAsync(notification.CaptchaId, cancellationToken);

        if (captcha is null)
        {
            return;
        }

        captchaRepository.Remove(captcha);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
