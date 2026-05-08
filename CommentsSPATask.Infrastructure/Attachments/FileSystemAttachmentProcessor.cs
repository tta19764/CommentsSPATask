using CommentsSPATask.Application.Abstractions.Attachments;
using CommentsSPATask.Domain.Abstractions;
using CommentsSPATask.Domain.Attachments;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

namespace CommentsSPATask.Infrastructure.Attachments;

internal sealed class FileSystemAttachmentProcessor : IAttachmentProcessor
{
    private const int MaxTextFileSizeBytes = 100 * 1024;
    private const int MaxImageWidth = 320;
    private const int MaxImageHeight = 240;

    private static readonly HashSet<string> AllowedTextExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".txt"
    };

    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".gif"
    };

    private readonly string _uploadsRootPath = Path.Combine(AppContext.BaseDirectory, "uploads");

    public async Task<Result<PreparedAttachment>> PrepareAsync(FileUpload upload, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_uploadsRootPath);

        var originalFileName = Path.GetFileName(upload.FileName);
        var extension = Path.GetExtension(originalFileName);

        if (AllowedTextExtensions.Contains(extension))
        {
            return await SaveTextFileAsync(upload, originalFileName, extension, cancellationToken);
        }

        if (AllowedImageExtensions.Contains(extension))
        {
            return await SaveImageAsync(upload, originalFileName, extension, cancellationToken);
        }

        return Result.Failure<PreparedAttachment>(AttachmentErrors.UnsupportedFileType);
    }

    private async Task<Result<PreparedAttachment>> SaveTextFileAsync(
        FileUpload upload,
        string originalFileName,
        string extension,
        CancellationToken cancellationToken)
    {
        if (upload.Length > MaxTextFileSizeBytes)
        {
            return Result.Failure<PreparedAttachment>(AttachmentErrors.InvalidTextFileSize);
        }

        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var path = Path.Combine(_uploadsRootPath, storedFileName);

        await using var fileStream = File.Create(path);
        await upload.Content.CopyToAsync(fileStream, cancellationToken);

        return Result.Success(new PreparedAttachment(
            originalFileName,
            storedFileName,
            ContentType.TextFile));
    }

    private async Task<Result<PreparedAttachment>> SaveImageAsync(
        FileUpload upload,
        string originalFileName,
        string extension,
        CancellationToken cancellationToken)
    {
        try
        {
            await using var memoryStream = new MemoryStream();
            await upload.Content.CopyToAsync(memoryStream, cancellationToken);
            memoryStream.Position = 0;

            using var image = await Image.LoadAsync(memoryStream, cancellationToken);
            var resizeRequired = image.Width > MaxImageWidth || image.Height > MaxImageHeight;

            if (resizeRequired)
            {
                image.Mutate(context => context.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(MaxImageWidth, MaxImageHeight)
                }));
            }

            var storedFileName = $"{Guid.NewGuid():N}{extension}";
            var path = Path.Combine(_uploadsRootPath, storedFileName);
            memoryStream.Position = 0;

            await using var fileStream = File.Create(path);
            await SaveImageAsync(image, extension, fileStream, cancellationToken);

            return Result.Success(new PreparedAttachment(
                originalFileName,
                storedFileName,
                ContentType.Image));
        }
        catch
        {
            return Result.Failure<PreparedAttachment>(AttachmentErrors.InvalidImage);
        }
    }

    private static Task SaveImageAsync(
        Image image,
        string extension,
        Stream output,
        CancellationToken cancellationToken)
    {
        return extension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => image.SaveAsJpegAsync(output, cancellationToken),
            ".png" => image.SaveAsPngAsync(output, cancellationToken),
            ".gif" => image.SaveAsGifAsync(output, cancellationToken),
            _ => image.SaveAsPngAsync(output, cancellationToken)
        };
    }
}
