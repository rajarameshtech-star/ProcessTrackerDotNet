using Microsoft.EntityFrameworkCore;
using ProcessTracker.API.Entities;

namespace ProcessTracker.API.Data;

public class ProcessTrackerDbContext : DbContext
{
    public ProcessTrackerDbContext(DbContextOptions<ProcessTrackerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<ProcessDefinition> ProcessDefinitions => Set<ProcessDefinition>();
    public DbSet<ProcessDefinitionProjectMapping> ProcessDefinitionProjectMappings => Set<ProcessDefinitionProjectMapping>();
    public DbSet<ProcessField> ProcessFields => Set<ProcessField>();
    public DbSet<ServiceItem> ServiceItems => Set<ServiceItem>();
    public DbSet<ProcessRecord> ProcessRecords => Set<ProcessRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Project
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Application
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasOne(d => d.Project)
                  .WithMany(p => p.Applications)
                  .HasForeignKey(d => d.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ProcessDefinition
        modelBuilder.Entity<ProcessDefinition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProcessCode).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.ProcessCode).IsUnique();
            entity.Property(e => e.ProcessName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // ProcessDefinitionProjectMapping (Many-to-Many)
        modelBuilder.Entity<ProcessDefinitionProjectMapping>(entity =>
        {
            entity.HasKey(e => new { e.ProcessDefinitionId, e.ProjectId });

            entity.HasOne(d => d.ProcessDefinition)
                  .WithMany(p => p.ProcessDefinitionProjectMappings)
                  .HasForeignKey(d => d.ProcessDefinitionId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Project)
                  .WithMany(p => p.ProcessDefinitionProjectMappings)
                  .HasForeignKey(d => d.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ProcessField
        modelBuilder.Entity<ProcessField>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FieldName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Label).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Placeholder).HasMaxLength(200);
            entity.Property(e => e.DefaultValue).HasMaxLength(200);

            entity.HasOne(d => d.ProcessDefinition)
                  .WithMany(p => p.ProcessFields)
                  .HasForeignKey(d => d.ProcessDefinitionId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            // Unique FieldName per ProcessDefinition
            entity.HasIndex(e => new { e.ProcessDefinitionId, e.FieldName }).IsUnique();
        });

        // ServiceItem
        modelBuilder.Entity<ServiceItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReferenceNumber).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.ReferenceNumber).IsUnique();
            
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Priority).IsRequired().HasMaxLength(50);
            entity.Property(e => e.AssignedTo).HasMaxLength(100);

            entity.HasOne(d => d.Application)
                  .WithMany(p => p.ServiceItems)
                  .HasForeignKey(d => d.ApplicationId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.ProcessDefinition)
                  .WithMany(p => p.ServiceItems)
                  .HasForeignKey(d => d.ProcessDefinitionId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ProcessRecord
        modelBuilder.Entity<ProcessRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DataJson).IsRequired();
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);

            // IsJson Constraint
            entity.ToTable(t => t.HasCheckConstraint("CK_ProcessRecord_DataJson", "ISJSON(DataJson) = 1"));

            entity.HasOne(d => d.ServiceItem)
                  .WithOne(p => p.ProcessRecord)
                  .HasForeignKey<ProcessRecord>(d => d.ServiceItemId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.ProcessDefinition)
                  .WithMany(p => p.ProcessRecords)
                  .HasForeignKey(d => d.ProcessDefinitionId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
