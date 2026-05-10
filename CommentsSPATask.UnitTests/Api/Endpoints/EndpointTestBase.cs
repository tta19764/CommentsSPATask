using MediatR;
using Moq;

namespace CommentsSPATask.UnitTests.Api.Endpoints;

public abstract class EndpointTestBase
{
    protected static Mock<ISender> CreateSender() => new();
}
