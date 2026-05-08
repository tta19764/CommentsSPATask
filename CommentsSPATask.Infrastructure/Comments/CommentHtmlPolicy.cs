using CommentsSPATask.Application.Abstractions.Comments;
using CommentsSPATask.Domain.Abstractions;
using CommentsSPATask.Domain.Comments;
using System.Xml;
using System.Xml.Linq;

namespace CommentsSPATask.Infrastructure.Comments;

internal sealed class CommentHtmlPolicy : ICommentHtmlPolicy
{
    private static readonly HashSet<string> AllowedTags = new(StringComparer.Ordinal)
    {
        "a",
        "code",
        "i",
        "strong"
    };

    private static readonly HashSet<string> AllowedAnchorAttributes = new(StringComparer.Ordinal)
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
        var wrapped = $"<root>{original}</root>";

        try
        {
            var document = XDocument.Parse(
                wrapped,
                LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);

            foreach (var element in document.Root!.Descendants())
            {
                if (!AllowedTags.Contains(element.Name.LocalName))
                {
                    return Result.Failure<string>(CommentErrors.InvalidHtml);
                }

                if (element.Name.LocalName == "a")
                {
                    foreach (var attribute in element.Attributes())
                    {
                        if (!AllowedAnchorAttributes.Contains(attribute.Name.LocalName))
                        {
                            return Result.Failure<string>(CommentErrors.InvalidHtml);
                        }

                        if (attribute.Name.LocalName == "href" &&
                            !IsValidAnchorHref(attribute.Value))
                        {
                            return Result.Failure<string>(CommentErrors.InvalidHtml);
                        }
                    }
                }
                else if (element.HasAttributes)
                {
                    return Result.Failure<string>(CommentErrors.InvalidHtml);
                }
            }
        }
        catch (XmlException)
        {
            return Result.Failure<string>(CommentErrors.InvalidHtml);
        }

        return Result.Success(original);
    }

    private static bool IsValidAnchorHref(string value)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            return false;
        }

        return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
    }
}
