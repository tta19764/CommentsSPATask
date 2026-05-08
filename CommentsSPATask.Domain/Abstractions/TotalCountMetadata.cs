namespace CommentsSPATask.Domain.Abstractions;

public record TotalCountMetadata : IResultMetadata
{
    public TotalCountMetadata(int totalCount)
    {
        Value.Add("totalCount", totalCount);
    }
    
    public TotalCountMetadata(long totalCount)
    {
        Value.Add("totalCount", totalCount);
    }

    public Dictionary<string, object> Value { get; } = [];
}