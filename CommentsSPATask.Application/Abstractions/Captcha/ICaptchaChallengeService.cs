namespace CommentsSPATask.Application.Abstractions.Captcha;

public interface ICaptchaChallengeService
{
    string GenerateCode(int length);

    string HashCode(string code);

    bool VerifyCode(string input, string hash);

    byte[] RenderImage(string code);
}
