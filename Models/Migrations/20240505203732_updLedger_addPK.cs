using Microsoft.EntityFrameworkCore.Migrations;

namespace Models.Migrations
{
    public partial class updLedger_addPK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
                name: "PK_Ledger",
                table: "Ledger",
                column: "TID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Ledger",
                table: "Ledger");
        }
    }
}
