using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ThirdMigration_UpdatedPoll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Polls",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Polls",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Polls_AuthorId",
                table: "Polls",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Polls_AspNetUsers_AuthorId",
                table: "Polls",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Polls_AspNetUsers_AuthorId",
                table: "Polls");

            migrationBuilder.DropIndex(
                name: "IX_Polls_AuthorId",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Polls");
        }
    }
}
