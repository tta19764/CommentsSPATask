using CommentsSPATask.Application.Abstractions.Clock;

namespace CommentsSPATask.Infrastructure.Clock;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}