using CommentsSPATask.Application.Abstractions.Captcha;
using CommentsSPATask.Domain.Abstractions;
using CommentsSPATask.Domain.Captchas;
using CommentsSPATask.Domain.Captchas.Events;
using MediatR;

namespace CommentsSPATask.Application.Captchas.EventHandlers;

public sealed class CaptchaUsedDomainEventHandler(
    ICaptchaRepository captchaRepository,
    ICaptchaImageStore captchaImageStore,
    IUnitOfWork unitOfWork) : INotificationHandler<CaptchaUsedDomainEvent>
{
    public async Task Handle(CaptchaUsedDomainEvent notification, CancellationToken cancellationToken)
    {
        var captcha = await captchaRepository.GetByIdAsync(notification.CaptchaId, cancellationToken);

        if (captcha is null)
        {
            return;
        }

        captchaRepository.Remove(captcha);
        await captchaImageStore.RemoveAsync(notification.CaptchaId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
