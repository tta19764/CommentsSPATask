using CommentsSPATask.Domain.Abstractions;
using CommentsSPATask.Domain.Comments.Events;

namespace CommentsSPATask.Domain.Comments;

public sealed class Comment : Entity
{
    private Comment(
        Guid id,
        Guid? parentId,
        UserName userName,
        Email email,
        HomePage? homePage,
        CommentText text,
        DateTime createdAtUtc)  
        : base(id)
    {
        ParentId = parentId;
        UserName = userName;
        Email = email;
        HomePage = homePage;
        Text = text;
        CreatedAtUtc = createdAtUtc;       
    }
    
    public Guid? ParentId { get; private set; }
    
    public UserName UserName { get; private set; }
    
    public Email Email { get; private set; }
    
    public HomePage? HomePage { get; private set; }
    
    public CommentText Text { get; private set; }
    
    public DateTime CreatedAtUtc { get; private set; }
    
    public static Comment Create(Guid? parentId, UserName userName, Email email, HomePage? homePage, CommentText text)
    {
        var comment = new Comment(
            Guid.NewGuid(), 
            parentId,
            userName,
            email,
            homePage,
            text,
            DateTime.UtcNow);
        
        comment.RaiseDomainEvent(new CommentCreatedDomainEvent(comment.Id));
        
        return comment;
    }
}