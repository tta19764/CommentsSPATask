using CommentsSPATask.Domain.Comments;
using CommentsSPATask.Infrastructure.Comments;

namespace CommentsSPATask.UnitTests.Infrastructure.Comments;

public sealed class CommentHtmlPolicyTests
{
    private readonly CommentHtmlPolicy _policy = new();

    [Fact]
    public void ValidateAndSanitize_ShouldReturnSuccess_ForAllowedHtml()
    {
        var input = "<strong>Hello</strong> <a href=\"https://example.com\" title=\"site\">link</a>";

        var result = _policy.ValidateAndSanitize(input);

        Assert.True(result.IsSuccess);
        Assert.Equal(input, result.Value);
    }

    [Fact]
    public void ValidateAndSanitize_ShouldReturnFailure_ForForbiddenTag()
    {
        var result = _policy.ValidateAndSanitize("<script>alert(1)</script>");

        Assert.True(result.IsFailure);
        Assert.Equal(CommentErrors.InvalidHtml, result.Error);
    }

    [Fact]
    public void ValidateAndSanitize_ShouldReturnFailure_ForInvalidAnchorHref()
    {
        var result = _policy.ValidateAndSanitize("<a href=\"javascript:alert(1)\">x</a>");

        Assert.True(result.IsFailure);
        Assert.Equal(CommentErrors.InvalidHtml, result.Error);
    }
}
