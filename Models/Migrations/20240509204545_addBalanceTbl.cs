using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Models.Migrations
{
    public partial class addBalanceTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Signature",
                table: "Ledger",
                type: "varbinary(70)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(128)");

            migrationBuilder.CreateTable(
                name: "Balances",
                columns: table => new
                {
                    Id_Local = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "varchar(34)", maxLength: 34, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(16,8)", precision: 16, scale: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Balances", x => x.Id_Local);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Balances");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Signature",
                table: "Ledger",
                type: "varbinary(128)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(70)");
        }
    }
}
