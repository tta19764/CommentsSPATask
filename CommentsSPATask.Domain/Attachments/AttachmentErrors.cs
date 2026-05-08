using CommentsSPATask.Domain.Abstractions;

namespace CommentsSPATask.Domain.Attachments;

public static class AttachmentErrors
{
    public static readonly Error UnsupportedFileType =
        new("Attachments.UnsupportedFileType", "The attachment file type is not supported.");

    public static readonly Error InvalidTextFileSize =
        new("Attachments.InvalidTextFileSize", "The text attachment exceeds the 100 KB size limit.");

    public static readonly Error InvalidImage =
        new("Attachments.InvalidImage", "The image attachment is invalid.");
}
