using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClientContactManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClientCodeLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clients_ClientCode",
                table: "Clients");

            migrationBuilder.AlterColumn<string>(
                name: "ClientCode",
                table: "Clients",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ClientCode",
                table: "Clients",
                column: "ClientCode",
                unique: true,
                filter: "[ClientCode] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clients_ClientCode",
                table: "Clients");

            migrationBuilder.AlterColumn<string>(
                name: "ClientCode",
                table: "Clients",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(6)",
                oldMaxLength: 6,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ClientCode",
                table: "Clients",
                column: "ClientCode",
                unique: true);
        }
    }
}
