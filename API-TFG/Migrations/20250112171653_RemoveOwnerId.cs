using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_TFG.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOwnerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Users_OwnerID",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "OwnerID",
                table: "Files",
                newName: "OwnerUserID");

            migrationBuilder.RenameIndex(
                name: "IX_Files_OwnerID",
                table: "Files",
                newName: "IX_Files_OwnerUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Users_OwnerUserID",
                table: "Files",
                column: "OwnerUserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Users_OwnerUserID",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "OwnerUserID",
                table: "Files",
                newName: "OwnerID");

            migrationBuilder.RenameIndex(
                name: "IX_Files_OwnerUserID",
                table: "Files",
                newName: "IX_Files_OwnerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Users_OwnerID",
                table: "Files",
                column: "OwnerID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
