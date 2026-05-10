using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommentsSPATask.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCaptchaFailedAttempts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedAttemptsCount",
                table: "Captchas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FailedAttemptsCount",
                table: "Captchas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
