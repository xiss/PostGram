#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace PostGram.DAL.Migrations
{
    /// <inheritdoc />
    public partial class QueryFiltersForCommentsAndPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostContents_Posts_PostId",
                table: "PostContents");

            migrationBuilder.AddForeignKey(
                name: "FK_PostContents_Posts_PostId",
                table: "PostContents",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostContents_Posts_PostId",
                table: "PostContents");

            migrationBuilder.AddForeignKey(
                name: "FK_PostContents_Posts_PostId",
                table: "PostContents",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
