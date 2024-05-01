using Microsoft.EntityFrameworkCore.Migrations;

namespace Models.Migrations
{
    public partial class addNetworkNodeModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NetworkNodes",
                columns: table => new
                {
                    Id_Local = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Pubkx = table.Column<string>(type: "nvarchar(66)", maxLength: 66, nullable: false),
                    IP = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false),
                    IsTP = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkNodes", x => x.Id_Local);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NetworkNodes");
        }
    }
}
