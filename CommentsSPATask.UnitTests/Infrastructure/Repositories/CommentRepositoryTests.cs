using CommentsSPATask.Domain.Comments;
using CommentsSPATask.Infrastructure.Repositories;
using CommentsSPATask.UnitTests.Helpers;

namespace CommentsSPATask.UnitTests.Infrastructure.Repositories;

public sealed class CommentRepositoryTests : DbContextTestBase
{
    [Fact]
    public async Task Add_AndGetByIdAsync_ShouldPersistComment()
    {
        await using var context = CreateContext();
        var repository = new CommentRepository(context);
        var comment = Comment.Create(null, new UserName("User1"), new Email("user1@example.com"), null, new CommentText("text"));
        comment.ClearDomainEvents();

        repository.Add(comment);
        await context.SaveChangesAsync();

        var result = await repository.GetByIdAsync(comment.Id);

        Assert.NotNull(result);
        Assert.Equal(comment.Id, result!.Id);
    }
}
