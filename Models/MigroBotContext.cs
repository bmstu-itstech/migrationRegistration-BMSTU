using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MigrationBot.Models;

public partial class MigroBotContext : DbContext
{
    public MigroBotContext()
    {
    }

    public MigroBotContext(DbContextOptions<MigroBotContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Entry> Entries { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  => optionsBuilder.UseNpgsql(Data.Strings.Tokens.SqlConnection);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entry>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id_fk");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.ChatId)
                .ValueGeneratedNever()
                .HasColumnName("chatId");
            entity.Property(e => e.ArrivalDate).HasColumnName("arrival_date");
            entity.Property(e => e.Comand)
                .HasMaxLength(255)
                .HasColumnName("comand");
            entity.Property(e => e.Country).HasColumnName("country ");
            entity.Property(e => e.Entry)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("entry");
            entity.Property(e => e.FioEn)
                .HasMaxLength(255)
                .HasColumnName("fio_en");
            entity.Property(e => e.FioRu)
                .HasMaxLength(255)
                .HasColumnName("fio_ru");
            entity.Property(e => e.Service).HasColumnName("service");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
