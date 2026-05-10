using CommentsSPATask.Infrastructure.Clock;

namespace CommentsSPATask.UnitTests.Infrastructure.Clock;

public sealed class DateTimeProviderTests
{
    [Fact]
    public void UtcNow_ShouldReturnCurrentUtcTime()
    {
        var provider = new DateTimeProvider();
        var before = DateTime.UtcNow.AddSeconds(-1);

        var value = provider.UtcNow;

        Assert.InRange(value, before, DateTime.UtcNow.AddSeconds(1));
    }
}
