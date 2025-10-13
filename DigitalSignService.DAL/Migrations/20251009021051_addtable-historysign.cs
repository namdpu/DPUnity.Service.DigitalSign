using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalSignService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addtablehistorysign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "history_sign",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_url = table.Column<string>(type: "text", nullable: false),
                    document_id = table.Column<string>(type: "text", nullable: false),
                    document_name = table.Column<string>(type: "text", nullable: false),
                    template_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    img = table.Column<string>(type: "text", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: false),
                    serial_number = table.Column<string>(type: "text", nullable: true),
                    user_sign_positions = table.Column<string>(type: "jsonb", nullable: false),
                    signing_status = table.Column<int>(type: "integer", nullable: false),
                    document_signed_url = table.Column<string>(type: "text", nullable: true),
                    transaction_id = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_userId = table.Column<Guid>(type: "uuid", nullable: true),
                    created_dateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_userId = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_dateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_history_sign", x => x.id);
                    table.ForeignKey(
                        name: "FK_history_sign_template_template_id",
                        column: x => x.template_id,
                        principalSchema: "public",
                        principalTable: "template",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_history_sign_template_id",
                schema: "public",
                table: "history_sign",
                column: "template_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "history_sign",
                schema: "public");
        }
    }
}
