using CommentsSPATask.Domain.Attachments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommentsSPATask.Infrastructure.Configurations;

public sealed class AttachmentsConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.ToTable("Attachments");
        
        builder.HasKey(attachment => attachment.Id);
        
        builder.HasMany<Attachment>()
            .WithOne()
            .HasForeignKey(attachment => attachment.CommentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(attachment => attachment.CreatedAtUtc)
            .IsRequired();
        
        builder.Property(attachment => attachment.OriginalFileName)
            .IsRequired()
            .HasConversion(originalFileName => originalFileName.Value, value => new OriginalFileName(value));
        
        builder.Property(attachment => attachment.StoredFileName)
            .IsRequired()
            .HasConversion(storedFileName => storedFileName.Value, value => new StoredFileName(value));
        
        builder.Property(attachment => attachment.ContentType)
            .IsRequired()
            .HasConversion<int>();
    }
}