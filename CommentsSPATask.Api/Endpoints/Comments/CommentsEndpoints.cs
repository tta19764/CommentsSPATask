using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommentsSPATask.Api.Contracts;
using CommentsSPATask.Api.Extensions;
using CommentsSPATask.Application.Abstractions.Attachments;
using CommentsSPATask.Application.Comments.Commands.CreateComment;
using CommentsSPATask.Application.Comments.Queries;
using CommentsSPATask.Application.Comments.Queries.GetCommentThread;
using CommentsSPATask.Application.Comments.Queries.GetCommentsPage;
using CommentsSPATask.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CommentsSPATask.Api.Endpoints.Comments;

public static class CommentsEndpoints
{
    public static IEndpointRouteBuilder MapCommentEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("api/comments")
            .WithTags("Comments");

        group.MapGet(string.Empty, GetCommentsPage)
            .WithName(nameof(GetCommentsPage))
            .WithSummary("Get paged root comments")
            .WithDescription("Returns an ApiResponse where data contains the page of root comments and metadata contains paging values such as totalCount.")
            .Produces<ApiResponse<IReadOnlyCollection<CommentSummaryResponse>>>(StatusCodes.Status200OK);

        group.MapGet("{id:guid}", GetCommentThread)
            .WithName(nameof(GetCommentThread))
            .WithSummary("Get a comment thread")
            .WithDescription("Returns an ApiResponse where data contains a comment and all of its nested replies. If the comment is not found, error is populated.")
            .Produces<ApiResponse<CommentThreadResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<CommentThreadResponse>>(StatusCodes.Status404NotFound);

        group.MapPost(string.Empty, CreateComment)
            .WithName(nameof(CreateComment))
            .WithSummary("Create a comment")
            .WithDescription("Creates a root comment or reply. Accepts multipart form data and an optional attachment file. Returns an ApiResponse where data contains the created comment identifier.")
            .Accepts<CreateCommentRequest>("multipart/form-data")
            .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
            .Produces<ApiResponse<Guid>>(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();

        return builder;
    }

    public static async Task<IResult> GetCommentsPage(
        [AsParameters] GetCommentsPageRequest request,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var query = new GetCommentsPageQuery(
            request.Page,
            request.PageSize,
            request.SortField,
            request.SortDirection);

        var result = await sender.Send(query, cancellationToken);

        return Results.Ok(result.MapToApiResponse());
    }

    public static async Task<IResult> GetCommentThread(
        [AsParameters] GetCommentThreadRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetCommentThreadQuery(request.Id);

        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? Results.Ok(result.MapToApiResponse()) : Results.NotFound(result.MapToApiResponse());
    }

    public static async Task<IResult> CreateComment(
        [FromForm] CreateCommentRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        FileUpload? attachment = null;

        if (request.Attachment is not null)
        {
            attachment = new FileUpload(
                request.Attachment.FileName,
                request.Attachment.ContentType,
                request.Attachment.Length,
                request.Attachment.OpenReadStream());
        }

        var command = new CreateCommentCommand(
            request.UserName,
            request.Email,
            request.HomePage,
            request.Text,
            request.CaptchaId,
            request.CaptchaInput,
            request.ParentId,
            attachment);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return Results.BadRequest(result.MapToApiResponse());
        }

        return Results.CreatedAtRoute(
            nameof(GetCommentThread),
            new { id = result.Value },
            result.MapToApiResponse());
    }
}
