using CommentsSPATask.Domain.Attachments;
using CommentsSPATask.Infrastructure.Repositories;
using CommentsSPATask.UnitTests.Helpers;

namespace CommentsSPATask.UnitTests.Infrastructure.Repositories;

public sealed class AttachmentRepositoryTests : DbContextTestBase
{
    [Fact]
    public async Task GetByCommentIdAsync_ShouldReturnOnlyMatchingAttachments()
    {
        await using var context = CreateContext();
        var repository = new AttachmentRepository(context);
        var firstCommentId = Guid.NewGuid();
        var secondCommentId = Guid.NewGuid();

        var firstAttachment = Attachment.Create(firstCommentId, new OriginalFileName("a.txt"), new StoredFileName("1.txt"), ContentType.TextFile, DateTime.UtcNow);
        var secondAttachment = Attachment.Create(firstCommentId, new OriginalFileName("b.txt"), new StoredFileName("2.txt"), ContentType.TextFile, DateTime.UtcNow);
        var thirdAttachment = Attachment.Create(secondCommentId, new OriginalFileName("c.txt"), new StoredFileName("3.txt"), ContentType.TextFile, DateTime.UtcNow);
        firstAttachment.ClearDomainEvents();
        secondAttachment.ClearDomainEvents();
        thirdAttachment.ClearDomainEvents();

        repository.Add(firstAttachment);
        repository.Add(secondAttachment);
        repository.Add(thirdAttachment);
        await context.SaveChangesAsync();

        var result = await repository.GetByCommentIdAsync(firstCommentId);

        Assert.Equal(2, result.Count);
        Assert.All(result, attachment => Assert.Equal(firstCommentId, attachment.CommentId));
    }
}
