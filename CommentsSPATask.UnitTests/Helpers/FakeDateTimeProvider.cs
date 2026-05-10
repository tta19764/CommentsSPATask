using CommentsSPATask.Application.Abstractions.Clock;

namespace CommentsSPATask.UnitTests.Helpers;

internal sealed class FakeDateTimeProvider(DateTime utcNow) : IDateTimeProvider
{
    public DateTime UtcNow { get; } = utcNow;
}
