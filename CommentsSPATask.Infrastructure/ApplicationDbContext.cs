using CommentsSPATask.Application.Abstractions.Data;
using CommentsSPATask.Domain.Abstractions;
using CommentsSPATask.Domain.Attachments;
using CommentsSPATask.Domain.Captchas;
using CommentsSPATask.Domain.Comments;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CommentsSPATask.Infrastructure;

public sealed class ApplicationDbContext(DbContextOptions options, IPublisher publisher)
    : DbContext(options), IUnitOfWork, IApplicationDbContext
{
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
        var result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync();

        return result;
    }
    
    private async Task PublishDomainEventsAsync()
    {
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.GetDomainEvents();

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent);
        }
    }
}