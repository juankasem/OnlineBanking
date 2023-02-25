using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineBanking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FinalAppUserIdTypeUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Branches_BranchId",
                table: "Address");

            migrationBuilder.DropIndex(
                name: "IX_Address_BranchId",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Address");

            migrationBuilder.AlterColumn<Guid>(
                name: "AppUserId",
                table: "Address",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AppUserId",
                table: "Address",
                type: "int",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Address",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Address_BranchId",
                table: "Address",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Branches_BranchId",
                table: "Address",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");
        }
    }
}
