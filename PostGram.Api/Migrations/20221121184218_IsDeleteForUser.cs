using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostGram.Api.Migrations
{
    /// <inheritdoc />
    public partial class IsDeleteForUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Users_AuthorId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Users_AuthorId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_MasterId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_SlaveId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSessions_Users_UserId",
                table: "UserSessions");

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Likes_AuthorId",
                table: "Likes",
                column: "AuthorId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Users_AuthorId",
                table: "Attachments",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Users_AuthorId",
                table: "Likes",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_MasterId",
                table: "Subscriptions",
                column: "MasterId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_SlaveId",
                table: "Subscriptions",
                column: "SlaveId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessions_Users_UserId",
                table: "UserSessions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Users_AuthorId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Users_AuthorId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_MasterId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_SlaveId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSessions_Users_UserId",
                table: "UserSessions");

            migrationBuilder.DropIndex(
                name: "IX_Likes_AuthorId",
                table: "Likes");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Users_AuthorId",
                table: "Attachments",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Users_AuthorId",
                table: "Likes",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_MasterId",
                table: "Subscriptions",
                column: "MasterId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_SlaveId",
                table: "Subscriptions",
                column: "SlaveId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessions_Users_UserId",
                table: "UserSessions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
