using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrawingAnalyzer.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDrawingAnalysisResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "processing_job_id",
                table: "takeoff_items",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldMaxLength: 36);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "takeoff_items",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldMaxLength: 36);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "projects",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldMaxLength: 36);

            migrationBuilder.AlterColumn<Guid>(
                name: "drawing_id",
                table: "processing_jobs",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldMaxLength: 36);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "processing_jobs",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldMaxLength: 36);

            migrationBuilder.AlterColumn<Guid>(
                name: "project_id",
                table: "drawings",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldMaxLength: 36);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "drawings",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldMaxLength: 36);

            migrationBuilder.CreateTable(
                name: "drawing_analysis_results",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    drawing_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    processing_job_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    analysis_mode = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    total_pages = table.Column<int>(type: "int", nullable: false),
                    processing_time_ms = table.Column<int>(type: "int", nullable: false),
                    result_json = table.Column<string>(type: "json", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_drawing_analysis_results", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "uq_analysis_processing_job",
                table: "drawing_analysis_results",
                column: "processing_job_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "drawing_analysis_results");

            migrationBuilder.AlterColumn<string>(
                name: "processing_job_id",
                table: "takeoff_items",
                type: "varchar(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "takeoff_items",
                type: "varchar(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "projects",
                type: "varchar(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "drawing_id",
                table: "processing_jobs",
                type: "varchar(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "processing_jobs",
                type: "varchar(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "project_id",
                table: "drawings",
                type: "varchar(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "drawings",
                type: "varchar(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)");
        }
    }
}
