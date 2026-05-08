using CommentsSPATask.Domain.Abstractions;

namespace CommentsSPATask.Domain.Captchas;

public sealed record FailedAttemptsCount(int Value = 0)
{
    public static readonly Error Invalid = new("FailedAttempts.Invalid", "The FailedAttempts count is invalid");

    public int Value { get; private set; } = Value;

    private const int Max = 5;
    
    internal void Increment() => Value++;
    public bool IsMaxReached() => Value >= Max;
}