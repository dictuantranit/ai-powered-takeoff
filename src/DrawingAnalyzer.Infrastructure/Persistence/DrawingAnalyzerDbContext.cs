
using DrawingAnalyzer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DrawingAnalyzer.Infrastructure.Persistence;

public class DrawingAnalyzerDbContext(
    DbContextOptions<DrawingAnalyzerDbContext> options)
    : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();

    public DbSet<Drawing> Drawings => Set<Drawing>();

    public DbSet<ProcessingJob> ProcessingJobs =>
        Set<ProcessingJob>();

    public DbSet<TakeoffItem> TakeoffItems =>
        Set<TakeoffItem>();

    public DbSet<DrawingAnalysisRecord> DrawingAnalysisRecords =>
        Set<DrawingAnalysisRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("projects");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("char(36)");

            entity.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasColumnName("description")
                .HasMaxLength(1000);

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Drawing>(entity =>
        {
            entity.ToTable("drawings");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("char(36)");

            entity.Property(x => x.ProjectId)
                .HasColumnName("project_id")
                .HasColumnType("char(36)");

            entity.Property(x => x.FileName)
                .HasColumnName("file_name")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(x => x.StoragePath)
                .HasColumnName("storage_path")
                .HasMaxLength(1000);

            entity.Property(x => x.MimeType)
                .HasColumnName("mime_type")
                .HasMaxLength(100);

            entity.Property(x => x.FileSize)
                .HasColumnName("file_size");

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at");

            entity.HasOne(x => x.Project)
                .WithMany(x => x.Drawings)
                .HasForeignKey(x => x.ProjectId);
        });

        modelBuilder.Entity<ProcessingJob>(entity =>
        {
            entity.ToTable("processing_jobs");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("char(36)");

            entity.Property(x => x.DrawingId)
                .HasColumnName("drawing_id")
                .HasColumnType("char(36)");

            entity.Property(x => x.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(30);

            entity.Property(x => x.ErrorMessage)
                .HasColumnName("error_message");

            entity.Property(x => x.AttemptCount)
                .HasColumnName("attempt_count");

            entity.Property(x => x.StartedAt)
                .HasColumnName("started_at");

            entity.Property(x => x.CompletedAt)
                .HasColumnName("completed_at");

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at");

            entity.HasOne(x => x.Drawing)
                .WithMany(x => x.ProcessingJobs)
                .HasForeignKey(x => x.DrawingId);
        });

        modelBuilder.Entity<TakeoffItem>(entity =>
        {
            entity.ToTable("takeoff_items");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("char(36)");

            entity.Property(x => x.ProcessingJobId)
                .HasColumnName("processing_job_id")
                .HasColumnType("char(36)");

            entity.Property(x => x.ObjectType)
                .HasColumnName("object_type")
                .HasMaxLength(50);

            entity.Property(x => x.ObjectName)
                .HasColumnName("object_name")
                .HasMaxLength(100);

            entity.Property(x => x.Quantity)
                .HasColumnName("quantity")
                .HasPrecision(18, 3);

            entity.Property(x => x.Confidence)
                .HasColumnName("confidence")
                .HasPrecision(5, 4);

            entity.Property(x => x.MetadataJson)
                .HasColumnName("metadata_json")
                .HasColumnType("json");

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at");

            entity.HasOne(x => x.ProcessingJob)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.ProcessingJobId);
        });

        modelBuilder.Entity<DrawingAnalysisRecord>(entity =>
        {
            entity.ToTable("drawing_analysis_results");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("char(36)");

            entity.Property(x => x.DrawingId)
                .HasColumnName("drawing_id")
                .HasColumnType("char(36)");

            entity.Property(x => x.ProcessingJobId)
                .HasColumnName("processing_job_id")
                .HasColumnType("char(36)");

            entity.Property(x => x.AnalysisMode)
                .HasColumnName("analysis_mode")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Status)
                .HasColumnName("status")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.TotalPages)
                .HasColumnName("total_pages");

            entity.Property(x => x.ProcessingTimeMs)
                .HasColumnName("processing_time_ms");

            entity.Property(x => x.ResultJson)
                .HasColumnName("result_json")
                .HasColumnType("json")
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at");

            entity.HasIndex(x => x.ProcessingJobId)
                .IsUnique()
                .HasDatabaseName("uq_analysis_processing_job");
        });
    }
}
