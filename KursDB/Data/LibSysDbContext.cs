using System;
using System.Collections.Generic;
using KursDB.Models;
using Microsoft.EntityFrameworkCore;

namespace KursDB.Data;

public partial class LibSysDbContext : DbContext
{
    public LibSysDbContext()
    {
    }

    public LibSysDbContext(DbContextOptions<LibSysDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Authorship> Authorships { get; set; }

    public virtual DbSet<Content> Contents { get; set; }

    public virtual DbSet<CurrentLoan> CurrentLoans { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeSchedule> EmployeeSchedules { get; set; }

    public virtual DbSet<Hall> Halls { get; set; }

    public virtual DbSet<HallType> HallTypes { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<InventoryDetail> InventoryDetails { get; set; }

    public virtual DbSet<Library> Libraries { get; set; }

    public virtual DbSet<Loan> Loans { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Publication> Publications { get; set; }

    public virtual DbSet<PublicationType> PublicationTypes { get; set; }

    public virtual DbSet<Reader> Readers { get; set; }

    public virtual DbSet<ReaderAttribute> ReaderAttributes { get; set; }

    public virtual DbSet<ReaderDetail> ReaderDetails { get; set; }

    public virtual DbSet<ReaderType> ReaderTypes { get; set; }

    public virtual DbSet<Work> Works { get; set; }

    public virtual DbSet<WorkCategory> WorkCategories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-NL9MST4;Database=LibKursach;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__Authors__70DAFC146A1915A2");

            entity.Property(e => e.AuthorId).HasColumnName("AuthorID");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.MiddleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Authorship>(entity =>
        {
            entity.HasKey(e => e.AuthorshipId).HasName("PK__Authorsh__CA26D9BA9DB5CF4C");

            entity.ToTable("Authorship");

            entity.Property(e => e.AuthorshipId).HasColumnName("AuthorshipID");
            entity.Property(e => e.AuthorId).HasColumnName("AuthorID");
            entity.Property(e => e.WorkId).HasColumnName("WorkID");

            entity.HasOne(d => d.Author).WithMany(p => p.Authorships)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK__Authorshi__Autho__5812160E");

            entity.HasOne(d => d.Work).WithMany(p => p.Authorships)
                .HasForeignKey(d => d.WorkId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Authorshi__WorkI__59063A47");
        });

        modelBuilder.Entity<Content>(entity =>
        {
            entity.HasKey(e => e.ContentId).HasName("PK__Contents__2907A87E311DF386");

            entity.Property(e => e.ContentId).HasColumnName("ContentID");
            entity.Property(e => e.PublicationId).HasColumnName("PublicationID");
            entity.Property(e => e.WorkId).HasColumnName("WorkID");

            entity.HasOne(d => d.Publication).WithMany(p => p.Contents)
                .HasForeignKey(d => d.PublicationId)
                .HasConstraintName("FK__Contents__Public__60A75C0F");

            entity.HasOne(d => d.Work).WithMany(p => p.Contents)
                .HasForeignKey(d => d.WorkId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Contents__WorkID__619B8048");
        });

        modelBuilder.Entity<CurrentLoan>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("CurrentLoans");

            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeName).HasMaxLength(152);
            entity.Property(e => e.LibraryName).HasMaxLength(100);
            entity.Property(e => e.LoanDate).HasColumnType("datetime");
            entity.Property(e => e.LoanId).HasColumnName("LoanID");
            entity.Property(e => e.PublicationTitle).HasMaxLength(200);
            entity.Property(e => e.ReaderName).HasMaxLength(152);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF1FBBC788E");

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.LibraryId).HasColumnName("LibraryID");
            entity.Property(e => e.MiddleName).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.PositionId).HasColumnName("PositionID");

            entity.HasOne(d => d.Library).WithMany(p => p.Employees)
                .HasForeignKey(d => d.LibraryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employees__Libra__4222D4EF");

            entity.HasOne(d => d.Position).WithMany(p => p.Employees)
                .HasForeignKey(d => d.PositionId)
                .HasConstraintName("FK__Employees__Posit__412EB0B6");
        });

        modelBuilder.Entity<EmployeeSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Employee__9C8A5B6968EE1371");

            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.HallId).HasColumnName("HallID");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeSchedules)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__EmployeeS__Emplo__44FF419A");

            entity.HasOne(d => d.Hall).WithMany(p => p.EmployeeSchedules)
                .HasForeignKey(d => d.HallId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeS__HallI__45F365D3");
        });

        modelBuilder.Entity<Hall>(entity =>
        {
            entity.HasKey(e => e.HallId).HasName("PK__Halls__7E60E274E2B4064E");

            entity.Property(e => e.HallId).HasColumnName("HallID");
            entity.Property(e => e.HallTypeId).HasColumnName("HallTypeID");
            entity.Property(e => e.LibraryId).HasColumnName("LibraryID");

            entity.HasOne(d => d.HallType).WithMany(p => p.Halls)
                .HasForeignKey(d => d.HallTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Halls__HallTypeI__3C69FB99");

            entity.HasOne(d => d.Library).WithMany(p => p.Halls)
                .HasForeignKey(d => d.LibraryId)
                .HasConstraintName("FK__Halls__LibraryID__3B75D760");
        });

        modelBuilder.Entity<HallType>(entity =>
        {
            entity.HasKey(e => e.HallTypeId).HasName("PK__HallType__490C8FE56DBF7F46");

            entity.Property(e => e.HallTypeId).HasColumnName("HallTypeID");
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("PK__Inventor__F5FDE6D3808B6A2A");

            entity.ToTable("Inventory");

            entity.Property(e => e.InventoryId).HasColumnName("InventoryID");
            entity.Property(e => e.CanTakeHome).HasDefaultValue(true);
            entity.Property(e => e.HallId).HasColumnName("HallID");
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.LibraryId).HasColumnName("LibraryID");
            entity.Property(e => e.LoanPeriodDays).HasDefaultValue(14);
            entity.Property(e => e.PublicationId).HasColumnName("PublicationID");

            entity.HasOne(d => d.Hall).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.HallId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__HallI__693CA210");

            entity.HasOne(d => d.Library).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.LibraryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Libra__68487DD7");

            entity.HasOne(d => d.Publication).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.PublicationId)
                .HasConstraintName("FK__Inventory__Publi__6754599E");
        });

        modelBuilder.Entity<InventoryDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("InventoryDetails");

            entity.Property(e => e.CanTakeHome).HasMaxLength(3);
            entity.Property(e => e.HallType).HasMaxLength(50);
            entity.Property(e => e.InventoryId).HasColumnName("InventoryID");
            entity.Property(e => e.LibraryName).HasMaxLength(100);
            entity.Property(e => e.PublicationTitle).HasMaxLength(200);
            entity.Property(e => e.PublicationType).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(8);
        });

        modelBuilder.Entity<Library>(entity =>
        {
            entity.HasKey(e => e.LibraryId).HasName("PK__Librarie__A13647BFC2A66AA5");

            entity.Property(e => e.LibraryId).HasColumnName("LibraryID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.LoanId).HasName("PK__Loans__4F5AD43797E828A7");

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("CheckTakeHomePermission");
                    tb.HasTrigger("UpdateInventoryStatus");
                });

            entity.Property(e => e.LoanId).HasColumnName("LoanID");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.InventoryId).HasColumnName("InventoryID");
            entity.Property(e => e.LoanDate).HasColumnType("datetime");
            entity.Property(e => e.ReaderId).HasColumnName("ReaderID");
            entity.Property(e => e.ReturnDate).HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.Loans)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Loans__EmployeeI__6E01572D");

            entity.HasOne(d => d.Inventory).WithMany(p => p.Loans)
                .HasForeignKey(d => d.InventoryId)
                .HasConstraintName("FK__Loans__Inventory__6C190EBB");

            entity.HasOne(d => d.Reader).WithMany(p => p.Loans)
                .HasForeignKey(d => d.ReaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Loans__ReaderID__6D0D32F4");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("PK__Position__60BB9A5957F16D06");

            entity.Property(e => e.PositionId).HasColumnName("PositionID");
            entity.Property(e => e.PositionName).HasMaxLength(50);
            entity.Property(e => e.Salary).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Publication>(entity =>
        {
            entity.HasKey(e => e.PublicationId).HasName("PK__Publicat__05E6DC587F9D3587");

            entity.Property(e => e.PublicationId).HasColumnName("PublicationID");
            entity.Property(e => e.Isbn)
                .HasMaxLength(20)
                .HasColumnName("ISBN");
            entity.Property(e => e.PublicationTypeId).HasColumnName("PublicationTypeID");
            entity.Property(e => e.Publisher).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.PublicationType).WithMany(p => p.Publications)
                .HasForeignKey(d => d.PublicationTypeId)
                .HasConstraintName("FK__Publicati__Publi__5DCAEF64");
        });

        modelBuilder.Entity<PublicationType>(entity =>
        {
            entity.HasKey(e => e.PublicationTypeId).HasName("PK__Publicat__52388F1F6677CFC7");

            entity.Property(e => e.PublicationTypeId).HasColumnName("PublicationTypeID");
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<Reader>(entity =>
        {
            entity.HasKey(e => e.ReaderId).HasName("PK__Readers__8E67A58146D53E03");

            entity.Property(e => e.ReaderId).HasColumnName("ReaderID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.LibraryId).HasColumnName("LibraryID");
            entity.Property(e => e.MiddleName).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.ReaderTypeId).HasColumnName("ReaderTypeID");

            entity.HasOne(d => d.Library).WithMany(p => p.Readers)
                .HasForeignKey(d => d.LibraryId)
                .HasConstraintName("FK__Readers__Library__4AB81AF0");

            entity.HasOne(d => d.ReaderType).WithMany(p => p.Readers)
                .HasForeignKey(d => d.ReaderTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Readers__ReaderT__4BAC3F29");
        });

        modelBuilder.Entity<ReaderAttribute>(entity =>
        {
            entity.HasKey(e => e.AttributeId).HasName("PK__ReaderAt__C189298AB9D16D9A");

            entity.Property(e => e.AttributeId).HasColumnName("AttributeID");
            entity.Property(e => e.AttributeName).HasMaxLength(50);
            entity.Property(e => e.AttributeValue).HasMaxLength(200);
            entity.Property(e => e.ReaderId).HasColumnName("ReaderID");

            entity.HasOne(d => d.Reader).WithMany(p => p.ReaderAttributes)
                .HasForeignKey(d => d.ReaderId)
                .HasConstraintName("FK__ReaderAtt__Reade__4E88ABD4");
        });

        modelBuilder.Entity<ReaderDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ReaderDetails");

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Attributes).HasMaxLength(4000);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(152);
            entity.Property(e => e.LibraryName).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.ReaderId).HasColumnName("ReaderID");
            entity.Property(e => e.ReaderType).HasMaxLength(50);
        });

        modelBuilder.Entity<ReaderType>(entity =>
        {
            entity.HasKey(e => e.ReaderTypeId).HasName("PK__ReaderTy__5EEFEDCEFAEDDABB");

            entity.Property(e => e.ReaderTypeId).HasColumnName("ReaderTypeID");
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<Work>(entity =>
        {
            entity.HasKey(e => e.WorkId).HasName("PK__Works__2DE6D215DA0D79C4");

            entity.Property(e => e.WorkId).HasColumnName("WorkID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Category).WithMany(p => p.Works)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Works__CategoryI__5535A963");
        });

        modelBuilder.Entity<WorkCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__WorkCate__19093A2BF6F205FC");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
