using CommentsSPATask.Domain.Abstractions;
using CommentsSPATask.Domain.Attachments.Events;

namespace CommentsSPATask.Domain.Attachments;

public sealed class Attachment : Entity
{
    private Attachment(
        Guid id,
        Guid commentId,
        OriginalFileName originalFileName,
        StoredFileName storedFileName,
        ContentType contentType,
        DateTime createdAtUtc)  
        : base(id)
    {
        CommentId = commentId;
        OriginalFileName = originalFileName;
        StoredFileName = storedFileName;
        ContentType = contentType;
        CreatedAtUtc = createdAtUtc;
    }
    
    public Guid CommentId { get; private set; }
    
    public OriginalFileName OriginalFileName { get; private set; }
    
    public StoredFileName StoredFileName { get; private set; }
    
    public ContentType ContentType { get; private set; }
    
    public DateTime CreatedAtUtc { get; private set; }
    
    public static Attachment Create(Guid commentId, OriginalFileName originalFileName, StoredFileName storedFileName, ContentType contentType, DateTime createdAtUtc)
    {
        var attachment = new Attachment(Guid.NewGuid(), commentId, originalFileName, storedFileName, contentType, createdAtUtc);
        
        attachment.RaiseDomainEvent(new AttachmentCreatedDomainEvent(attachment.Id));
        
        return attachment;
    }
}