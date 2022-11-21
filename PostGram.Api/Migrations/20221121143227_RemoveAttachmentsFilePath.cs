using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostGram.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAttachmentsFilePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Attachments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Attachments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
