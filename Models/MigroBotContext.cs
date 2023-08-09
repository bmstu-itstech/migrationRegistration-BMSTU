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

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
              => optionsBuilder.UseNpgsql(Data.Strings.Tokens.SqlConnection);


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.ChatId)
                .ValueGeneratedNever()
                .HasColumnName("chatId");
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
