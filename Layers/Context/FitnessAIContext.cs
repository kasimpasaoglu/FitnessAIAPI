using System;
using System.Collections.Generic;
using API.DMO;
using Microsoft.EntityFrameworkCore;

namespace API.DataAccessLayer;

public partial class FitnessAIContext : DbContext
{
    public FitnessAIContext()
    {
    }

    public FitnessAIContext(DbContextOptions<FitnessAIContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CompletedExercise> CompletedExercises { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WorkoutPlan> WorkoutPlans { get; set; }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //     => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CompletedExercise>(entity =>
        {
            entity.HasKey(e => e.CompletionId).HasName("PK__Complete__77FA708F967DE997");

            entity.Property(e => e.CompletionId).ValueGeneratedNever();
            entity.Property(e => e.CompletedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExerciseName).HasMaxLength(100);

            entity.HasOne(d => d.Plan).WithMany(p => p.CompletedExercises)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompletedExercises_WorkoutPlans");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C2FEA8F1C");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.AvailableDays).HasMaxLength(200);
            entity.Property(e => e.ExperienceLevel).HasMaxLength(50);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Goal).HasMaxLength(50);
            entity.Property(e => e.HeightCm).HasColumnName("Height_cm");
            entity.Property(e => e.MedicationsUsing).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Surname).HasMaxLength(100);
            entity.Property(e => e.WeightKg).HasColumnName("Weight_kg");
        });

        modelBuilder.Entity<WorkoutPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__WorkoutP__755C22B7ABD27108");

            entity.Property(e => e.PlanId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.WorkoutPlans)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkoutPlans_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
