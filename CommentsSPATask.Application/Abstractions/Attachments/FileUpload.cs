namespace CommentsSPATask.Application.Abstractions.Attachments;

public sealed record FileUpload(
    string FileName,
    string ContentType,
    long Length,
    Stream Content);
