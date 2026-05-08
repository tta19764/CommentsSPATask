using CommentsSPATask.Domain.Comments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommentsSPATask.Infrastructure.Configurations;

public sealed class CommentsConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");
        
        builder.HasKey(comment => comment.Id);
        
        builder.Property(comment => comment.Text)
            .IsRequired()
            .HasMaxLength(2500)
            .HasConversion(text => text.Value, value => new CommentText(value));
        
        builder.Property(comment => comment.UserName)
            .IsRequired()
            .HasMaxLength(50)
            .HasConversion(userName => userName.Value, value => new UserName(value));
        
        builder.Property(comment => comment.Email)
            .IsRequired()
            .HasMaxLength(100)
            .HasConversion(email => email.Value, value => new Email(value));
        
        builder.Property(comment => comment.HomePage)
            .HasMaxLength(250)
            .HasConversion(
                homePage => homePage == null ? null : homePage.Value,
                value => value == null ? null : new HomePage(value));
        
        builder.HasOne<Comment>()
            .WithMany()
            .HasForeignKey(comment => comment.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(comment => comment.CreatedAtUtc)
            .IsRequired();
    }
}