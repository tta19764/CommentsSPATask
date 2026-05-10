using CommentsSPATask.Api.Contracts;
using CommentsSPATask.Api.Endpoints.Captchas;
using CommentsSPATask.Application.Abstractions.Captcha;
using CommentsSPATask.Application.Captchas.Commands.CreateCaptcha;
using CommentsSPATask.Domain.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace CommentsSPATask.UnitTests.Api.Endpoints;

public sealed class CaptchasEndpointsTests : EndpointTestBase
{
    [Fact]
    public async Task CreateCaptcha_ShouldReturnOk_WhenCommandSucceeds()
    {
        var sender = CreateSender();
        var response = new CreateCaptchaResponse(Guid.NewGuid(), DateTime.UtcNow.AddMinutes(5));

        sender.Setup(x => x.Send(It.IsAny<CreateCaptchaCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(response));

        var result = await CaptchasEndpoints.CreateCaptcha(
            sender.Object,
            CancellationToken.None);

        var ok = Assert.IsType<Ok<ApiResponse<CreateCaptchaApiResponse>>>(result);
        Assert.Equal(response.CaptchaId, ok.Value?.Data?.CaptchaId);
        Assert.Equal(response.ExpiresAtUtc, ok.Value?.Data?.ExpiresAtUtc);
        Assert.Equal($"/api/captchas/{response.CaptchaId}/image", ok.Value?.Data?.ImageUrl);
    }

    [Fact]
    public async Task CreateCaptcha_ShouldReturnBadRequest_WhenCommandFails()
    {
        var sender = CreateSender();
        var error = new Error("Captcha.Error", "Captcha generation failed");

        sender.Setup(x => x.Send(It.IsAny<CreateCaptchaCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<CreateCaptchaResponse>(error));

        var result = await CaptchasEndpoints.CreateCaptcha(
            sender.Object,
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequest<ApiResponse<CreateCaptchaApiResponse>>>(result);
        Assert.Equal(error, badRequest.Value?.Error);
    }

    [Fact]
    public async Task GetCaptchaImage_ShouldReturnFile_WhenImageExists()
    {
        var captchaImageStore = new Mock<ICaptchaImageStore>();
        var captchaId = Guid.NewGuid();
        var imageBytes = new byte[] { 1, 2, 3 };

        captchaImageStore.Setup(x => x.GetAsync(captchaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(imageBytes);

        var result = await CaptchasEndpoints.GetCaptchaImage(
            captchaId,
            captchaImageStore.Object,
            CancellationToken.None);

        var file = Assert.IsType<FileContentHttpResult>(result);
        Assert.Equal("image/png", file.ContentType);
        Assert.Equal(imageBytes, file.FileContents);
    }

    [Fact]
    public async Task GetCaptchaImage_ShouldReturnNotFound_WhenImageDoesNotExist()
    {
        var captchaImageStore = new Mock<ICaptchaImageStore>();

        captchaImageStore.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        var result = await CaptchasEndpoints.GetCaptchaImage(
            Guid.NewGuid(),
            captchaImageStore.Object,
            CancellationToken.None);

        Assert.IsType<NotFound>(result);
    }
}
