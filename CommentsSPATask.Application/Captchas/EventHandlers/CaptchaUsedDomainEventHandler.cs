using CommentsSPATask.Domain.Abstractions;
using CommentsSPATask.Domain.Captchas;
using CommentsSPATask.Domain.Captchas.Events;
using MediatR;

namespace CommentsSPATask.Application.Captchas.EventHandlers;

public sealed class CaptchaUsedDomainEventHandler(
    ICaptchaRepository captchaRepository,
    IUnitOfWork unitOfWork) : INotificationHandler<CaptchaUsedDomainEvent>
{
    public async Task Handle(CaptchaUsedDomainEvent notification, CancellationToken cancellationToken)
    {
        var captcha = await captchaRepository.GetByIdAsync(notification.CaptchaId, cancellationToken);

        if (captcha is null)
        {
            return;
        }

        await captchaRepository.RemoveAsync(captcha, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
