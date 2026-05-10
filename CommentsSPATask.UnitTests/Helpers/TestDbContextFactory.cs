using CommentsSPATask.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CommentsSPATask.UnitTests.Helpers;

internal static class TestDbContextFactory
{
    public static ApplicationDbContext Create(string databaseName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        var publisher = new Mock<IPublisher>();

        return new ApplicationDbContext(options, publisher.Object);
    }
}
