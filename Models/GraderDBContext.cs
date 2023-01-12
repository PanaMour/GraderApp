using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GraderApp.Models;

public partial class GraderDBContext : DbContext
{
    public GraderDBContext()
    {
    }

    public GraderDBContext(DbContextOptions<GraderDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseHasStudent> CourseHasStudents { get; set; }

    public virtual DbSet<Professor> Professors { get; set; }

    public virtual DbSet<Secretary> Secretaries { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-HRDAQCNN;Database=GraderDB;Trusted_Connection=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Latin1_General_CI_AS");

        modelBuilder.Entity<Course>(entity =>
        {
            entity.Property(e => e.IdCourse).ValueGeneratedNever();

            entity.HasOne(d => d.ProfessorsAfmNavigation).WithMany(p => p.Courses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_course_professors");
        });

        modelBuilder.Entity<CourseHasStudent>(entity =>
        {
            entity.Property(e => e.CourseIdCourse).ValueGeneratedNever();

            entity.HasOne(d => d.CourseIdCourseNavigation).WithOne(p => p.CourseHasStudent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_course_has_students_course");

            entity.HasOne(d => d.StudentsRegistrationNumberNavigation).WithMany(p => p.CourseHasStudents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_course_has_students_students");
        });

        modelBuilder.Entity<Professor>(entity =>
        {
            entity.Property(e => e.Afm).ValueGeneratedNever();

            entity.HasOne(d => d.UsersUsernameNavigation).WithMany(p => p.Professors)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_professors_users");
        });

        modelBuilder.Entity<Secretary>(entity =>
        {
            entity.Property(e => e.Phonenumber).ValueGeneratedNever();

            entity.HasOne(d => d.UsersUsernameNavigation).WithMany(p => p.Secretaries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_secretaries_users");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.Property(e => e.RegistrationNumber).ValueGeneratedNever();

            entity.HasOne(d => d.UsersUsernameNavigation).WithMany(p => p.Students)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_students_users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
