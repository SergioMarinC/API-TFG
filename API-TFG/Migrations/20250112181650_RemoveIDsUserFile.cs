using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_TFG.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIDsUserFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Eliminar el índice relacionado con FileID
            migrationBuilder.DropIndex(
                name: "IX_UserFiles_FileID",
                table: "UserFiles");

            // Eliminar la clave foránea relacionada con FileID
            migrationBuilder.DropForeignKey(
                name: "FK_UserFiles_Files_FileID",
                table: "UserFiles");

            // Ahora es seguro eliminar la columna FileID
            migrationBuilder.DropColumn(
                name: "FileID",
                table: "UserFiles");

            // Realizar otras modificaciones necesarias
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restaurar la columna FileID
            migrationBuilder.AddColumn<Guid>(
                name: "FileID",
                table: "UserFiles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Restaurar la clave foránea
            migrationBuilder.AddForeignKey(
                name: "FK_UserFiles_Files_FileID",
                table: "UserFiles",
                column: "FileID",
                principalTable: "Files",
                principalColumn: "FileID",
                onDelete: ReferentialAction.Cascade);

            // Restaurar el índice
            migrationBuilder.CreateIndex(
                name: "IX_UserFiles_FileID",
                table: "UserFiles",
                column: "FileID");
        }
    }
}
