using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalSignService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class updateusersignimg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "img",
                schema: "public",
                table: "template_paper_user_sign",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "img",
                schema: "public",
                table: "template_paper_user_sign");
        }
    }
}
