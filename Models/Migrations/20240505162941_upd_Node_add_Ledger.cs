using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Models.Migrations
{
    public partial class upd_Node_add_Ledger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Pubkx",
                table: "NetworkNodes",
                type: "varchar(66)",
                maxLength: 66,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(66)",
                oldMaxLength: 66);

            migrationBuilder.AlterColumn<string>(
                name: "IP",
                table: "NetworkNodes",
                type: "varchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.CreateTable(
                name: "Ledger",
                columns: table => new
                {
                    TID = table.Column<long>(type: "bigint", nullable: false),
                    LHash = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    Sender = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    Reciver = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Memo = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
                    TransactionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Signature = table.Column<byte[]>(type: "varbinary(128)", nullable: false),
                    Hash = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ledger");

            migrationBuilder.AlterColumn<string>(
                name: "Pubkx",
                table: "NetworkNodes",
                type: "nvarchar(66)",
                maxLength: 66,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(66)",
                oldMaxLength: 66);

            migrationBuilder.AlterColumn<string>(
                name: "IP",
                table: "NetworkNodes",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldMaxLength: 15);
        }
    }
}
