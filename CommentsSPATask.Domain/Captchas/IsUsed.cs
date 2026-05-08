namespace CommentsSPATask.Domain.Captchas;

public sealed record IsUsed(bool Value = false)
{
    public bool Value { get; private set; } = Value;
    
    internal void SetUsed() => Value = true;
};