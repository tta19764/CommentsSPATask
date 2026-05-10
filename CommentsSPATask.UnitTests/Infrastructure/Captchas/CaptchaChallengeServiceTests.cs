using CommentsSPATask.Infrastructure.Captchas;

namespace CommentsSPATask.UnitTests.Infrastructure.Captchas;

public sealed class CaptchaChallengeServiceTests
{
    private readonly CaptchaChallengeService _service = new();

    [Fact]
    public void GenerateCode_ShouldReturnExpectedLengthAndCharset()
    {
        const string allowedCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        var code = _service.GenerateCode(6);

        Assert.Equal(6, code.Length);
        Assert.All(code, c => Assert.Contains(c, allowedCharacters));
    }

    [Fact]
    public void VerifyCode_ShouldReturnTrue_ForMatchingInput()
    {
        var hash = _service.HashCode("abc12");

        var result = _service.VerifyCode("ABC12", hash);

        Assert.True(result);
    }

    [Fact]
    public void VerifyCode_ShouldReturnFalse_ForDifferentInput()
    {
        var hash = _service.HashCode("abc12");

        var result = _service.VerifyCode("abc13", hash);

        Assert.False(result);
    }

    [Fact]
    public void RenderImage_ShouldReturnPngBytes()
    {
        var bytes = _service.RenderImage("ABC12");

        Assert.NotEmpty(bytes);
        Assert.Equal(0x89, bytes[0]);
        Assert.Equal(0x50, bytes[1]);
        Assert.Equal(0x4E, bytes[2]);
        Assert.Equal(0x47, bytes[3]);
    }
}
