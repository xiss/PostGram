#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace PostGram.DAL.Migrations
{
    /// <inheritdoc />
    public partial class LikesToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Likes_AuthorId_CommentId",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Likes_AuthorId_PostId",
                table: "Likes");

            migrationBuilder.AddColumn<Guid>(
                name: "EntityId",
                table: "Likes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "EntityType",
                table: "Likes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Likes_AuthorId_EntityId",
                table: "Likes",
                columns: new[] { "AuthorId", "EntityId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Likes_AuthorId_EntityId",
                table: "Likes");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "Likes");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "Likes");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_AuthorId_CommentId",
                table: "Likes",
                columns: new[] { "AuthorId", "CommentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Likes_AuthorId_PostId",
                table: "Likes",
                columns: new[] { "AuthorId", "PostId" },
                unique: true);
        }
    }
}
