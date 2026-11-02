using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrawingAnalyzer.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projects", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "drawings",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    project_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    file_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    storage_path = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    mime_type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    file_size = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_drawings", x => x.id);
                    table.ForeignKey(
                        name: "FK_drawings_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "processing_jobs",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    drawing_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    attempt_count = table.Column<int>(type: "int", nullable: false),
                    started_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    completed_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    error_message = table.Column<string>(type: "longtext", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_processing_jobs", x => x.id);
                    table.ForeignKey(
                        name: "FK_processing_jobs_drawings_drawing_id",
                        column: x => x.drawing_id,
                        principalTable: "drawings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "takeoff_items",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    processing_job_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    object_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    object_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    confidence = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true),
                    metadata_json = table.Column<string>(type: "json", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_takeoff_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_takeoff_items_processing_jobs_processing_job_id",
                        column: x => x.processing_job_id,
                        principalTable: "processing_jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_drawings_project_id",
                table: "drawings",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_processing_jobs_drawing_id",
                table: "processing_jobs",
                column: "drawing_id");

            migrationBuilder.CreateIndex(
                name: "IX_takeoff_items_processing_job_id",
                table: "takeoff_items",
                column: "processing_job_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "takeoff_items");

            migrationBuilder.DropTable(
                name: "processing_jobs");

            migrationBuilder.DropTable(
                name: "drawings");

            migrationBuilder.DropTable(
                name: "projects");
        }
    }
}
