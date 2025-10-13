using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalSignService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addcolhistorysign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "provider",
                schema: "public",
                table: "history_sign",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "provider",
                schema: "public",
                table: "history_sign");
        }
    }
}
