using CommentsSPATask.Application.Abstractions.Attachments;
using CommentsSPATask.Application.Abstractions.Messaging;

namespace CommentsSPATask.Application.Comments.Commands.CreateComment;

public sealed record CreateCommentCommand(
    string UserName,
    string Email,
    string? HomePage,
    string Text,
    Guid CaptchaId,
    string CaptchaInput,
    Guid? ParentId = null,
    FileUpload? Attachment = null) : ICommand<Guid>;
