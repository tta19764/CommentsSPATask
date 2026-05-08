using CommentsSPATask.Application.Abstractions.Clock;
using CommentsSPATask.Application.Abstractions.Data;
using CommentsSPATask.Application.Exceptions;
using CommentsSPATask.Domain.Abstractions;
using CommentsSPATask.Domain.Attachments;
using CommentsSPATask.Domain.Captchas;
using CommentsSPATask.Domain.Comments;
using CommentsSPATask.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CommentsSPATask.Infrastructure;

public sealed class ApplicationDbContext(DbContextOptions options, IDateTimeProvider dateTimeProvider)
    : DbContext(options), IUnitOfWork, IApplicationDbContext
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };
    
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    public DbSet<Comment> Comments { get; private set; }
    
    public DbSet<Attachment> Attachments { get; private set; }
    
    public DbSet<Captcha> Captchas { get; private set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddDomainEventsAsOutboxMessages();

        int result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }
    
    private void AddDomainEventsAsOutboxMessages()
    {
        var outboxMessages = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                IReadOnlyList<IDomainEvent> domainEvents = entity.GetDomainEvents();

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage(
                Guid.NewGuid(),
                _dateTimeProvider.UtcNow,
                domainEvent.GetType().Name,
                JsonConvert.SerializeObject(domainEvent, JsonSerializerSettings)))
            .ToList();

        AddRange(outboxMessages);
    }
}