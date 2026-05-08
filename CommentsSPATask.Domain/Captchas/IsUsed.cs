namespace CommentsSPATask.Domain.Captchas;

public sealed record IsUsed
{
    public bool Value { get; private set; }
    
    internal void SetUsed() => Value = true;
};