using CommentsSPATask.Application.Abstractions.Attachments;
using CommentsSPATask.Domain.Attachments;
using CommentsSPATask.Infrastructure.Attachments;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace CommentsSPATask.UnitTests.Infrastructure.Attachments;

public sealed class FileSystemAttachmentProcessorTests : IDisposable
{
    private readonly FileSystemAttachmentProcessor _processor = new();
    private readonly string _uploadsPath = Path.Combine(AppContext.BaseDirectory, "uploads");

    public FileSystemAttachmentProcessorTests()
    {
        CleanupUploads();
    }

    [Fact]
    public async Task PrepareAsync_ShouldSaveTextFile_WhenFileIsValid()
    {
        await using var stream = new MemoryStream("hello"u8.ToArray());
        var upload = new FileUpload("note.txt", "text/plain", stream.Length, stream);

        var result = await _processor.PrepareAsync(upload);

        Assert.True(result.IsSuccess);
        Assert.Equal(ContentType.TextFile, result.Value.ContentType);
        Assert.True(File.Exists(Path.Combine(_uploadsPath, result.Value.StoredFileName)));
    }

    [Fact]
    public async Task PrepareAsync_ShouldFail_WhenTextFileTooLarge()
    {
        await using var stream = new MemoryStream(new byte[100 * 1024 + 1]);
        var upload = new FileUpload("big.txt", "text/plain", stream.Length, stream);

        var result = await _processor.PrepareAsync(upload);

        Assert.True(result.IsFailure);
        Assert.Equal(AttachmentErrors.InvalidTextFileSize, result.Error);
    }

    [Fact]
    public async Task PrepareAsync_ShouldResizeLargeImage()
    {
        await using var stream = new MemoryStream();
        using (var image = new Image<Rgba32>(640, 480))
        {
            await image.SaveAsPngAsync(stream);
        }

        stream.Position = 0;
        var upload = new FileUpload("large.png", "image/png", stream.Length, stream);

        var result = await _processor.PrepareAsync(upload);

        Assert.True(result.IsSuccess);
        Assert.Equal(ContentType.Image, result.Value.ContentType);

        await using var savedStream = File.OpenRead(Path.Combine(_uploadsPath, result.Value.StoredFileName));
        using var savedImage = await Image.LoadAsync(savedStream);

        Assert.True(savedImage.Width <= 320);
        Assert.True(savedImage.Height <= 240);
    }

    [Fact]
    public async Task PrepareAsync_ShouldFail_ForUnsupportedExtension()
    {
        await using var stream = new MemoryStream([1, 2, 3]);
        var upload = new FileUpload("archive.zip", "application/zip", stream.Length, stream);

        var result = await _processor.PrepareAsync(upload);

        Assert.True(result.IsFailure);
        Assert.Equal(AttachmentErrors.UnsupportedFileType, result.Error);
    }

    public void Dispose()
    {
        CleanupUploads();
    }

    private void CleanupUploads()
    {
        if (Directory.Exists(_uploadsPath))
        {
            Directory.Delete(_uploadsPath, recursive: true);
        }
    }
}
