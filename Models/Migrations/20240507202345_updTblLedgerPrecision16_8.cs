using Microsoft.EntityFrameworkCore.Migrations;

namespace Models.Migrations
{
    public partial class updTblLedgerPrecision16_8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Ledger",
                type: "decimal(16,8)",
                precision: 16,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(13,6)",
                oldPrecision: 13,
                oldScale: 6);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Ledger",
                type: "decimal(13,6)",
                precision: 13,
                scale: 6,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(16,8)",
                oldPrecision: 16,
                oldScale: 8);
        }
    }
}
