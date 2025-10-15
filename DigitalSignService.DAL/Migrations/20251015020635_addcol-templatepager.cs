using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalSignService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addcoltemplatepager : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "template_url",
                schema: "public",
                table: "template_paper",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "template_url",
                schema: "public",
                table: "template_paper");
        }
    }
}
