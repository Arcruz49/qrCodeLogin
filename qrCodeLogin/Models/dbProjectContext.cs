using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace qrCodeLogin.Models;

public partial class dbProjectContext : DbContext
{
    public dbProjectContext()
    {
    }

    public dbProjectContext(DbContextOptions<dbProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CadPerfil> CadPerfils { get; set; }

    public virtual DbSet<CadUsuario> CadUsuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-BNHCASD;Database=dbProject;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CadPerfil>(entity =>
        {
            entity.HasKey(e => e.CdPerfil).HasName("PK__cadPerfi__08F8A08B41E2E715");

            entity.ToTable("cadPerfil");

            entity.Property(e => e.CdPerfil).HasColumnName("cdPerfil");
            entity.Property(e => e.DtAlteracao)
                .HasColumnType("datetime")
                .HasColumnName("dtAlteracao");
            entity.Property(e => e.DtCriacao)
                .HasColumnType("datetime")
                .HasColumnName("dtCriacao");
            entity.Property(e => e.NmPerfil)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nmPerfil");
            entity.Property(e => e.UsuarioA).HasColumnName("usuarioA");
            entity.Property(e => e.UsuarioC).HasColumnName("usuarioC");
        });

        modelBuilder.Entity<CadUsuario>(entity =>
        {
            entity.HasKey(e => e.CdUsuario).HasName("PK__cadUsuar__0C5725AAD38AA4DC");

            entity.ToTable("cadUsuario");

            entity.Property(e => e.CdUsuario).HasColumnName("cdUsuario");
            entity.Property(e => e.CdPerfilUsuario).HasColumnName("cdPerfilUsuario");
            entity.Property(e => e.DtAlteracao)
                .HasColumnType("datetime")
                .HasColumnName("dtAlteracao");
            entity.Property(e => e.DtCriacao)
                .HasColumnType("datetime")
                .HasColumnName("dtCriacao");
            entity.Property(e => e.Email)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.NmUsuario)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nmUsuario");
            entity.Property(e => e.Senha)
                .IsUnicode(false)
                .HasColumnName("senha");
            entity.Property(e => e.UsuarioA).HasColumnName("usuarioA");
            entity.Property(e => e.UsuarioC).HasColumnName("usuarioC");

            entity.HasOne(d => d.CdPerfilUsuarioNavigation).WithMany(p => p.CadUsuarios)
                .HasForeignKey(d => d.CdPerfilUsuario)
                .HasConstraintName("FK_cadUsuario_cadPerfil");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
