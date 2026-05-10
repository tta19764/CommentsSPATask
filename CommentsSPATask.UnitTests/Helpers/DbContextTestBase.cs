using CommentsSPATask.Infrastructure;

namespace CommentsSPATask.UnitTests.Helpers;

public abstract class DbContextTestBase
{
    protected ApplicationDbContext CreateContext()
    {
        return TestDbContextFactory.Create(Guid.NewGuid().ToString());
    }
}
