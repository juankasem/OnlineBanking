using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineBanking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SecondCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Address_AddressId",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_FastTransactions_BankAccounts_BankAccountId",
                table: "FastTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Branches_AddressId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Branches");

            migrationBuilder.AlterColumn<Guid>(
                name: "BankAccountId",
                table: "FastTransactions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "FastTransactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "RecipientIBAN",
                table: "FastTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientName",
                table: "FastTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Country",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_District",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Name",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Street",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_ZipCode",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "Address",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppUserId1",
                table: "Address",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Address",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CityId1",
                table: "Address",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CountryId1",
                table: "Address",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DistrictId1",
                table: "Address",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "District",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_District", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_AppUserId1",
                table: "Address",
                column: "AppUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Address_BranchId",
                table: "Address",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_CityId1",
                table: "Address",
                column: "CityId1");

            migrationBuilder.CreateIndex(
                name: "IX_Address_CountryId1",
                table: "Address",
                column: "CountryId1");

            migrationBuilder.CreateIndex(
                name: "IX_Address_DistrictId1",
                table: "Address",
                column: "DistrictId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_AspNetUsers_AppUserId1",
                table: "Address",
                column: "AppUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Branches_BranchId",
                table: "Address",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_City_CityId1",
                table: "Address",
                column: "CityId1",
                principalTable: "City",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Country_CountryId1",
                table: "Address",
                column: "CountryId1",
                principalTable: "Country",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_District_DistrictId1",
                table: "Address",
                column: "DistrictId1",
                principalTable: "District",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FastTransactions_BankAccounts_BankAccountId",
                table: "FastTransactions",
                column: "BankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_AspNetUsers_AppUserId1",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Address_Branches_BranchId",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Address_City_CityId1",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Address_Country_CountryId1",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Address_District_DistrictId1",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_FastTransactions_BankAccounts_BankAccountId",
                table: "FastTransactions");

            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "District");

            migrationBuilder.DropIndex(
                name: "IX_Address_AppUserId1",
                table: "Address");

            migrationBuilder.DropIndex(
                name: "IX_Address_BranchId",
                table: "Address");

            migrationBuilder.DropIndex(
                name: "IX_Address_CityId1",
                table: "Address");

            migrationBuilder.DropIndex(
                name: "IX_Address_CountryId1",
                table: "Address");

            migrationBuilder.DropIndex(
                name: "IX_Address_DistrictId1",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "FastTransactions");

            migrationBuilder.DropColumn(
                name: "RecipientIBAN",
                table: "FastTransactions");

            migrationBuilder.DropColumn(
                name: "RecipientName",
                table: "FastTransactions");

            migrationBuilder.DropColumn(
                name: "Address_City",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "Address_Country",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "Address_District",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "Address_Name",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "Address_Street",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "Address_ZipCode",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "AppUserId1",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "CityId1",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "CountryId1",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "DistrictId1",
                table: "Address");

            migrationBuilder.AlterColumn<Guid>(
                name: "BankAccountId",
                table: "FastTransactions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "AddressId",
                table: "Branches",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branches_AddressId",
                table: "Branches",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Address_AddressId",
                table: "Branches",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FastTransactions_BankAccounts_BankAccountId",
                table: "FastTransactions",
                column: "BankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id");
        }
    }
}
