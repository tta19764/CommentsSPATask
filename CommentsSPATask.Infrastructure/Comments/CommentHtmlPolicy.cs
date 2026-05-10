using CommentsSPATask.Application.Abstractions.Comments;
using CommentsSPATask.Domain.Abstractions;
using CommentsSPATask.Domain.Comments;
using HtmlAgilityPack;

namespace CommentsSPATask.Infrastructure.Comments;

internal sealed class CommentHtmlPolicy : ICommentHtmlPolicy
{
    private static readonly HashSet<string> AllowedTags = new(StringComparer.OrdinalIgnoreCase)
    {
        "a",
        "code",
        "i",
        "strong"
    };

    private static readonly HashSet<string> AllowedAnchorAttributes = new(StringComparer.OrdinalIgnoreCase)
    {
        "href",
        "title"
    };

    public Result<string> ValidateAndSanitize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return Result.Failure<string>(Error.NullValue);
        }

        var original = input.Trim();
        var document = new HtmlDocument
        {
            OptionAutoCloseOnEnd = false,
            OptionFixNestedTags = true
        };

        document.LoadHtml(original);

        foreach (var node in document.DocumentNode.ChildNodes)
        {
            if (!ValidateNode(node))
            {
                return Result.Failure<string>(CommentErrors.InvalidHtml);
            }
        }

        return Result.Success(document.DocumentNode.InnerHtml);
    }

    private static bool ValidateNode(HtmlNode node)
    {
        if (node.NodeType == HtmlNodeType.Text)
        {
            return true;
        }

        if (node.NodeType != HtmlNodeType.Element)
        {
            return false;
        }

        if (!AllowedTags.Contains(node.Name))
        {
            return false;
        }

        if (node.Name == "a")
        {
            foreach (var attribute in node.Attributes)
            {
                if (!AllowedAnchorAttributes.Contains(attribute.Name))
                {
                    return false;
                }

                if (attribute.Name == "href" &&
                    !IsValidAnchorHref(HtmlEntity.DeEntitize(attribute.Value)))
                {
                    return false;
                }
            }
        }
        else if (node.HasAttributes)
        {
            return false;
        }

        return node.ChildNodes.All(ValidateNode);
    }

    private static bool IsValidAnchorHref(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (!Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var uri))
        {
            return false;
        }

        if (uri.IsAbsoluteUri)
        {
            return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
        }

        return !value.StartsWith("//", StringComparison.Ordinal) &&
               !value.Contains(':', StringComparison.Ordinal);
    }
}
