using System.Text.RegularExpressions;
using FluentValidation;

namespace CommentsSPATask.Application.Comments.Commands.CreateComment;

public sealed partial class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    private const int MaxUserNameLength = 50;
    private const int MaxEmailLength = 100;
    private const int MaxHomePageLength = 250;
    private const int MaxCaptchaLength = 10;
    private const int MaxTextLength = 2500;

    public CreateCommentCommandValidator()
    {
        RuleFor(command => command.UserName)
            .NotEmpty()
            .MaximumLength(MaxUserNameLength)
            .Matches(UserNameRegex())
            .WithMessage("User name must contain only Latin letters and digits.");

        RuleFor(command => command.Email)
            .NotEmpty()
            .MaximumLength(MaxEmailLength)
            .EmailAddress();

        RuleFor(command => command.HomePage)
            .MaximumLength(MaxHomePageLength)
            .Must(BeValidHomePage)
            .When(command => !string.IsNullOrWhiteSpace(command.HomePage))
            .WithMessage("Home page must be a valid absolute HTTP or HTTPS URL.");

        RuleFor(command => command.CaptchaId)
            .NotEmpty();

        RuleFor(command => command.CaptchaInput)
            .NotEmpty()
            .MaximumLength(MaxCaptchaLength)
            .Matches(CaptchaRegex())
            .WithMessage("Captcha must contain only Latin letters and digits.");

        RuleFor(command => command.Text)
            .NotEmpty()
            .MaximumLength(MaxTextLength);

        When(command => command.Attachment is not null, () =>
        {
            RuleFor(command => command.Attachment!.FileName)
                .NotEmpty();

            RuleFor(command => command.Attachment!.ContentType)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(command => command.Attachment!.Length)
                .GreaterThan(0);

            RuleFor(command => command.Attachment!.Content)
                .NotNull();
        });
    }

    private static bool BeValidHomePage(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            return false;
        }

        return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
    }

    [GeneratedRegex("^[A-Za-z0-9]+$")]
    private static partial Regex UserNameRegex();

    [GeneratedRegex("^[A-Za-z0-9]+$")]
    private static partial Regex CaptchaRegex();
}
