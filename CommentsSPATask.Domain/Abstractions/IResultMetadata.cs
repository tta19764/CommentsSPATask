namespace CommentsSPATask.Domain.Abstractions;

public interface IResultMetadata
{
    Dictionary<string, object> Value { get; }
}