using System;
using System.Threading;
using System.Threading.Tasks;
using CommentsSPATask.Api.Contracts;
using CommentsSPATask.Api.Extensions;
using CommentsSPATask.Application.Abstractions.Captcha;
using CommentsSPATask.Application.Captchas.Commands.CreateCaptcha;
using CommentsSPATask.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CommentsSPATask.Api.Endpoints.Captchas;

public static class CaptchasEndpoints
{
    public static IEndpointRouteBuilder MapCaptchaEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/captchas", CreateCaptcha)
            .WithName(nameof(CreateCaptcha))
            .WithTags("Captchas")
            .WithSummary("Create a captcha challenge")
            .WithDescription("Generates a new captcha image and returns an ApiResponse where data contains the captcha identifier, image link, and expiration time.")
            .Produces<ApiResponse<CreateCaptchaApiResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<CreateCaptchaApiResponse>>(StatusCodes.Status400BadRequest);

        builder.MapGet("api/captchas/{id:guid}/image", GetCaptchaImage)
            .WithName(nameof(GetCaptchaImage))
            .WithTags("Captchas")
            .WithSummary("Get a captcha image")
            .WithDescription("Returns the captcha image as a PNG file.")
            .Produces(StatusCodes.Status200OK, contentType: "image/png")
            .Produces(StatusCodes.Status404NotFound);

        return builder;
    }

    public static async Task<IResult> CreateCaptcha(
        CreateCaptchaRequest _,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateCaptchaCommand();

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return Results.BadRequest(new ApiResponse<CreateCaptchaApiResponse>
            {
                Error = result.Error
            });
        }

        var response = new CreateCaptchaApiResponse(
            result.Value.CaptchaId,
            $"/api/captchas/{result.Value.CaptchaId}/image",
            result.Value.ExpiresAtUtc);

        return Results.Ok(Result.Success(response).MapToApiResponse());
    }

    public static async Task<IResult> GetCaptchaImage(
        Guid id,
        ICaptchaImageStore captchaImageStore,
        CancellationToken cancellationToken)
    {
        var imageBytes = await captchaImageStore.GetAsync(id, cancellationToken);

        return imageBytes is not null
            ? Results.File(imageBytes, "image/png")
            : Results.NotFound();
    }
}
