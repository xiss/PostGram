#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace PostGram.DAL.Migrations
{
    /// <inheritdoc />
    public partial class removeCommentHeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Header",
                table: "Comments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Header",
                table: "Comments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
