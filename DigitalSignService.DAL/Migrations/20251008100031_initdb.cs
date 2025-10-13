using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalSignService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class initdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "paper_size",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    paper_name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    height = table.Column<int>(type: "integer", nullable: false),
                    width = table.Column<int>(type: "integer", nullable: false),
                    paper_size_type = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_userId = table.Column<Guid>(type: "uuid", nullable: true),
                    created_dateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_userId = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_dateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_paper_size", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "template",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_userId = table.Column<Guid>(type: "uuid", nullable: true),
                    created_dateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_userId = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_dateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_template", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "template_paper",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    template_id = table.Column<Guid>(type: "uuid", nullable: false),
                    paper_size_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_userId = table.Column<Guid>(type: "uuid", nullable: true),
                    created_dateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_userId = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_dateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_template_paper", x => x.id);
                    table.ForeignKey(
                        name: "FK_template_paper_paper_size_paper_size_id",
                        column: x => x.paper_size_id,
                        principalSchema: "public",
                        principalTable: "paper_size",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_template_paper_template_template_id",
                        column: x => x.template_id,
                        principalSchema: "public",
                        principalTable: "template",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "template_paper_user_sign",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    template_paper_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_sign_id = table.Column<string>(type: "text", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    user_sign_positions = table.Column<string>(type: "jsonb", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_userId = table.Column<Guid>(type: "uuid", nullable: true),
                    created_dateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_userId = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_dateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_template_paper_user_sign", x => x.id);
                    table.ForeignKey(
                        name: "FK_template_paper_user_sign_template_paper_template_paper_id",
                        column: x => x.template_paper_id,
                        principalSchema: "public",
                        principalTable: "template_paper",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_template_paper_paper_size_id",
                schema: "public",
                table: "template_paper",
                column: "paper_size_id");

            migrationBuilder.CreateIndex(
                name: "IX_template_paper_template_id",
                schema: "public",
                table: "template_paper",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "IX_template_paper_user_sign_template_paper_id",
                schema: "public",
                table: "template_paper_user_sign",
                column: "template_paper_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "template_paper_user_sign",
                schema: "public");

            migrationBuilder.DropTable(
                name: "template_paper",
                schema: "public");

            migrationBuilder.DropTable(
                name: "paper_size",
                schema: "public");

            migrationBuilder.DropTable(
                name: "template",
                schema: "public");
        }
    }
}
