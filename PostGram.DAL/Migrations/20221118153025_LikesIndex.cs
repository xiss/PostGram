#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace PostGram.DAL.Migrations
{
    /// <inheritdoc />
    public partial class LikesIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Likes_AuthorId",
                table: "Likes");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLike",
                table: "Likes",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Likes_AuthorId_CommentId",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Likes_AuthorId_PostId",
                table: "Likes");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLike",
                table: "Likes",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Likes_AuthorId",
                table: "Likes",
                column: "AuthorId");
        }
    }
}
