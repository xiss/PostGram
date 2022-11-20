using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostGram.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddQuotesToComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "QuotedCommentId",
                table: "Comments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuotedText",
                table: "Comments",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_QuotedCommentId",
                table: "Comments",
                column: "QuotedCommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_QuotedCommentId",
                table: "Comments",
                column: "QuotedCommentId",
                principalTable: "Comments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_QuotedCommentId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_QuotedCommentId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "QuotedCommentId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "QuotedText",
                table: "Comments");
        }
    }
}
