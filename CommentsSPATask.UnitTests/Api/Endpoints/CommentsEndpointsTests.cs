using System.Text;
using CommentsSPATask.Api.Contracts;
using CommentsSPATask.Api.Endpoints.Comments;
using CommentsSPATask.Application.Abstractions.Attachments;
using CommentsSPATask.Application.Comments.Commands.CreateComment;
using CommentsSPATask.Application.Comments.Queries;
using CommentsSPATask.Application.Comments.Queries.GetCommentThread;
using CommentsSPATask.Application.Comments.Queries.GetCommentsPage;
using CommentsSPATask.Domain.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace CommentsSPATask.UnitTests.Api.Endpoints;

public sealed class CommentsEndpointsTests : EndpointTestBase
{
    [Fact]
    public async Task GetCommentsPage_ShouldReturnOk_WithWrappedResponse()
    {
        var sender = CreateSender();
        var httpContext = new DefaultHttpContext();
        var response = new[]
        {
            new CommentSummaryResponse(
                Guid.NewGuid(),
                "User1",
                "user1@example.com",
                null,
                "text",
                DateTime.UtcNow,
                [])
        };

        sender.Setup(x => x.Send(
                It.Is<GetCommentsPageQuery>(q =>
                    q.Page == 2 &&
                    q.PageSize == 10 &&
                    q.SortField == CommentSortField.UserName &&
                    q.SortDirection == SortDirection.Asc),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<IReadOnlyCollection<CommentSummaryResponse>>(response)
                .WithMetadata(new TotalCountMetadata(42)));

        var result = await CommentsEndpoints.GetCommentsPage(
            new GetCommentsPageRequest(2, 10, CommentSortField.UserName, SortDirection.Asc),
            sender.Object,
            httpContext,
            CancellationToken.None);

        var ok = Assert.IsType<Ok<ApiResponse<IReadOnlyCollection<CommentSummaryResponse>>>>(result);
        Assert.Equal(response, ok.Value?.Data);
        Assert.NotNull(ok.Value?.Metadata);
        Assert.Equal(42, ok.Value!.Metadata!["totalCount"]);
    }

    [Fact]
    public async Task GetCommentThread_ShouldReturnNotFound_WhenQueryFails()
    {
        var sender = CreateSender();
        var commentId = Guid.NewGuid();

        sender.Setup(x => x.Send(
                It.Is<GetCommentThreadQuery>(q => q.CommentId == commentId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<CommentThreadResponse>(new Error("Comments.NotFound", "Comment not found")));

        var result = await CommentsEndpoints.GetCommentThread(
            new GetCommentThreadRequest(commentId),
            sender.Object,
            CancellationToken.None);

        var notFound = Assert.IsType<NotFound<ApiResponse<CommentThreadResponse>>>(result);
        Assert.Equal("Comments.NotFound", notFound.Value?.Error?.Code);
    }

    [Fact]
    public async Task CreateComment_ShouldReturnCreatedAtRoute_WhenCommandSucceeds()
    {
        var sender = CreateSender();
        var commentId = Guid.NewGuid();
        var request = new CreateCommentRequest
        {
            UserName = "User1",
            Email = "user1@example.com",
            HomePage = "https://example.com",
            Text = "text",
            CaptchaId = Guid.NewGuid(),
            CaptchaInput = "ABCDE",
            Attachment = CreateFormFile("hello.txt", "text/plain", "hello")
        };

        sender.Setup(x => x.Send(
                It.Is<CreateCommentCommand>(command =>
                    command.UserName == request.UserName &&
                    command.Email == request.Email &&
                    command.HomePage == request.HomePage &&
                    command.Text == request.Text &&
                    command.CaptchaId == request.CaptchaId &&
                    command.CaptchaInput == request.CaptchaInput &&
                    command.ParentId == null &&
                    command.Attachment != null &&
                    command.Attachment.FileName == "hello.txt" &&
                    command.Attachment.ContentType == "text/plain" &&
                    command.Attachment.Length == 5),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(commentId));

        var result = await CommentsEndpoints.CreateComment(
            request,
            sender.Object,
            CancellationToken.None);

        var created = Assert.IsType<CreatedAtRoute<ApiResponse<Guid>>>(result);
        Assert.Equal(commentId, created.Value?.Data);
        Assert.Equal(nameof(CommentsEndpoints.GetCommentThread), created.RouteName);
        Assert.Equal(commentId, created.RouteValues?["id"]);
    }

    [Fact]
    public async Task CreateReply_ShouldReturnCreatedAtRoute_WhenCommandSucceeds()
    {
        var sender = CreateSender();
        var commentId = Guid.NewGuid();
        var parentId = Guid.NewGuid();
        var request = new CreateCommentRequest
        {
            UserName = "User1",
            Email = "user1@example.com",
            HomePage = "https://example.com",
            Text = "text",
            CaptchaId = Guid.NewGuid(),
            CaptchaInput = "ABCDE",
            Attachment = CreateFormFile("hello.txt", "text/plain", "hello")
        };

        sender.Setup(x => x.Send(
                It.Is<CreateCommentCommand>(command =>
                    command.UserName == request.UserName &&
                    command.Email == request.Email &&
                    command.HomePage == request.HomePage &&
                    command.Text == request.Text &&
                    command.CaptchaId == request.CaptchaId &&
                    command.CaptchaInput == request.CaptchaInput &&
                    command.ParentId == parentId &&
                    command.Attachment != null &&
                    command.Attachment.FileName == "hello.txt" &&
                    command.Attachment.ContentType == "text/plain" &&
                    command.Attachment.Length == 5),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(commentId));

        var result = await CommentsEndpoints.CreateReply(
            parentId,
            request,
            sender.Object,
            CancellationToken.None);

        var created = Assert.IsType<CreatedAtRoute<ApiResponse<Guid>>>(result);
        Assert.Equal(commentId, created.Value?.Data);
        Assert.Equal(nameof(CommentsEndpoints.GetCommentThread), created.RouteName);
        Assert.Equal(commentId, created.RouteValues?["id"]);
    }

    [Fact]
    public async Task CreateComment_ShouldReturnBadRequest_WhenCommandFails()
    {
        var sender = CreateSender();
        var error = new Error("Comments.Invalid", "Invalid comment");
        var request = new CreateCommentRequest
        {
            UserName = "User1",
            Email = "user1@example.com",
            Text = "text",
            CaptchaId = Guid.NewGuid(),
            CaptchaInput = "ABCDE"
        };

        sender.Setup(x => x.Send(It.IsAny<CreateCommentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Guid>(error));

        var result = await CommentsEndpoints.CreateComment(
            request,
            sender.Object,
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequest<ApiResponse<Guid>>>(result);
        Assert.Equal(error, badRequest.Value?.Error);
    }

    private static IFormFile CreateFormFile(string fileName, string contentType, string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);

        return new FormFile(stream, 0, bytes.Length, "Attachment", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }
}
