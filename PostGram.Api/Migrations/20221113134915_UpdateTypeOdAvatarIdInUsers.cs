using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostGram.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTypeOdAvatarIdInUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarId",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarId",
                table: "Users",
                type: "text",
                nullable: true);
        }
    }
}
