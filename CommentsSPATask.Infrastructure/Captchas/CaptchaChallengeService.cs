using System.Security.Cryptography;
using System.Text;
using CommentsSPATask.Application.Abstractions.Captcha;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace CommentsSPATask.Infrastructure.Captchas;

internal sealed class CaptchaChallengeService : ICaptchaChallengeService
{
    private const string AllowedCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public string GenerateCode(int length)
    {
        var chars = new char[length];

        for (var i = 0; i < length; i++)
        {
            chars[i] = AllowedCharacters[RandomNumberGenerator.GetInt32(AllowedCharacters.Length)];
        }

        return new string(chars);
    }

    public string HashCode(string code)
    {
        var normalized = Normalize(code);
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(normalized));
        return Convert.ToHexString(bytes);
    }

    public bool VerifyCode(string input, string hash)
    {
        var inputHash = HashCode(input);

        return CryptographicOperations.FixedTimeEquals(
            Convert.FromHexString(inputHash),
            Convert.FromHexString(hash));
    }

    public byte[] RenderImage(string code)
    {
        using var image = new Image<Rgba32>(160, 60, Color.White);
        var font = SystemFonts.CreateFont("Arial", 24, FontStyle.Bold);
        var random = Random.Shared;

        image.Mutate(context =>
        {
            for (var i = 0; i < 6; i++)
            {
                context.DrawLine(
                    Color.LightGray,
                    1,
                    new PointF(random.Next(image.Width), random.Next(image.Height)),
                    new PointF(random.Next(image.Width), random.Next(image.Height)));
            }

            for (var i = 0; i < code.Length; i++)
            {
                var x = 12 + (i * 26);
                var y = random.Next(8, 18);
                context.DrawText(code[i].ToString(), font, Color.FromRgb(40, 40, 40), new PointF(x, y));
            }
        });

        using var stream = new MemoryStream();
        image.SaveAsPng(stream);
        return stream.ToArray();
    }

    private static string Normalize(string value) => value.Trim().ToUpperInvariant();
}
