using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SGSP.Models;

#nullable disable

namespace SGSP.Data
{
    public partial class dbSpotsContext : IdentityDbContext
    {
        public dbSpotsContext()
        {
        }

        public dbSpotsContext(DbContextOptions<dbSpotsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Canal> Canals { get; set; }
        public virtual DbSet<Cliente> Clientes { get; set; }
        public virtual DbSet<Janela> Janelas { get; set; }
        public virtual DbSet<Spot> Spots { get; set; }
        public virtual DbSet<SpotPlanificacao> SpotPlanificacaos { get; set; }
        public virtual DbSet<TipoSpot> TipoSpots { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=DefaultConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Canal>(entity =>
            {
                entity.ToTable("Canal");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("code");

                entity.Property(e => e.DataCreate)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCreate");

                entity.Property(e => e.Designacao)
                    .IsUnicode(false)
                    .HasColumnName("designacao");

                entity.Property(e => e.IsActive).HasColumnName("isActive");
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("Cliente");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataCreate)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCreate");

                entity.Property(e => e.Designacao)
                    .IsUnicode(false)
                    .HasColumnName("designacao");

                entity.Property(e => e.EMail)
                    .IsUnicode(false)
                    .HasColumnName("eMail");

                entity.Property(e => e.IdClientePrincipal).HasColumnName("idClientePrincipal");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.Nuit)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("nuit");

                entity.Property(e => e.Telefone)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("telefone");

                entity.HasOne(d => d.IdClientePrincipalNavigation)
                    .WithMany(p => p.InverseIdClientePrincipalNavigation)
                    .HasForeignKey(d => d.IdClientePrincipal)
                    .HasConstraintName("FK_Cliente_Cliente");
            });

            modelBuilder.Entity<Janela>(entity =>
            {
                entity.ToTable("Janela");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Designacao)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("designacao");

                entity.Property(e => e.DiasSemana)
                    .IsUnicode(false)
                    .HasColumnName("diasSemana")
                    .HasComment("dias da semana, \r\ndomingo=1\r\nsegunda=2\r\nterca=3\r\nquarta=4\r\nquinta=5\r\nsexta=6\r\nsabado=7");

                entity.Property(e => e.HoraFim).HasColumnName("horaFim");

                entity.Property(e => e.HoraInicio).HasColumnName("horaInicio");

                entity.Property(e => e.IdCanal).HasColumnName("idCanal");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.HasOne(d => d.IdCanalNavigation)
                    .WithMany(p => p.Janelas)
                    .HasForeignKey(d => d.IdCanal)
                    .HasConstraintName("FK_Janela_Canal1");
            });

            modelBuilder.Entity<Spot>(entity =>
            {
                entity.ToTable("Spot");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("code");

                entity.Property(e => e.DataCreate)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCreate");

                entity.Property(e => e.DataFim)
                    .HasColumnType("datetime")
                    .HasColumnName("dataFim");

                entity.Property(e => e.DataInicio)
                    .HasColumnType("datetime")
                    .HasColumnName("dataInicio");

                entity.Property(e => e.Designacao)
                    .IsUnicode(false)
                    .HasColumnName("designacao");

                entity.Property(e => e.Duracao).HasColumnName("duracao");

                entity.Property(e => e.IdCanal).HasColumnName("idCanal");

                entity.Property(e => e.IdCliente).HasColumnName("idCliente");

                entity.Property(e => e.IdSpotPrincipal).HasColumnName("idSpotPrincipal");

                entity.Property(e => e.IdTipoSpot).HasColumnName("idTipoSpot");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.Periodicidade).HasColumnName("periodicidade");

                entity.Property(e => e.Estado).HasColumnName("estado");

                entity.HasOne(d => d.IdCanalNavigation)
                    .WithMany(p => p.Spots)
                    .HasForeignKey(d => d.IdCanal)
                    .HasConstraintName("FK_Spot_Canal1");

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.Spots)
                    .HasForeignKey(d => d.IdCliente)
                    .HasConstraintName("FK_Spot_Cliente");

                entity.HasOne(d => d.IdSpotPrincipalNavigation)
                    .WithMany(p => p.InverseIdSpotPrincipalNavigation)
                    .HasForeignKey(d => d.IdSpotPrincipal)
                    .HasConstraintName("FK_Spot_Spot");

                entity.HasOne(d => d.IdTipoSpotNavigation)
                    .WithMany(p => p.Spots)
                    .HasForeignKey(d => d.IdTipoSpot)
                    .HasConstraintName("FK_Spot_TipoSpot");
            });

            modelBuilder.Entity<SpotPlanificacao>(entity =>
            {
                entity.ToTable("SpotPlanificacao");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataPassagem)
                    .HasColumnType("datetime")
                    .HasColumnName("dataPassagem");

                entity.Property(e => e.DataPassagemConfirmacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataPassagemConfirmacao");

                entity.Property(e => e.DataPlanificacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataPlanificacao");

                entity.Property(e => e.DataSkip)
                    .HasColumnType("datetime")
                    .HasColumnName("dataSkip");

                entity.Property(e => e.Estado)
                    .HasColumnName("estado")
                    .HasComment("pendente =0;\r\npublicado=1;\r\nadiado=-1;");

                entity.Property(e => e.IdCanal).HasColumnName("idCanal");

                entity.Property(e => e.IdJanela).HasColumnName("idJanela");

                entity.Property(e => e.IdSpot).HasColumnName("idSpot");

                entity.Property(e => e.IdSpotPlanificacaoPrincipal).HasColumnName("idSpotPlanificacaoPrincipal");

                entity.Property(e => e.IsReagendamento).HasColumnName("isReagendamento");

                entity.Property(e => e.ParecerCoordenador)
                    .IsUnicode(false)
                    .HasColumnName("parecerCoordenador");

                entity.Property(e => e.Prioridade)
                    .HasColumnType("datetime")
                    .HasColumnName("prioridade");

                entity.Property(e => e.SkipMotivo)
                    .IsUnicode(false)
                    .HasColumnName("skipMotivo");

                entity.Property(e => e.UserCoordenador)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("userCoordenador");

                entity.Property(e => e.UserLocutor)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("userLocutor");

                entity.HasOne(d => d.IdCanalNavigation)
                    .WithMany(p => p.SpotPlanificacaos)
                    .HasForeignKey(d => d.IdCanal)
                    .HasConstraintName("FK_SpotPlanificacao_Canal1");

                entity.HasOne(d => d.IdJanelaNavigation)
                    .WithMany(p => p.SpotPlanificacaos)
                    .HasForeignKey(d => d.IdJanela)
                    .HasConstraintName("FK_SpotPlanificacao_Janela");

                entity.HasOne(d => d.IdSpotNavigation)
                    .WithMany(p => p.SpotPlanificacaos)
                    .HasForeignKey(d => d.IdSpot)
                    .HasConstraintName("FK_SpotPlanificacao_Spot");

                entity.HasOne(d => d.IdSpotPlanificacaoPrincipalNavigation)
                    .WithMany(p => p.InverseIdSpotPlanificacaoPrincipalNavigation)
                    .HasForeignKey(d => d.IdSpotPlanificacaoPrincipal)
                    .HasConstraintName("FK_SpotPlanificacao_SpotPlanificacao");
            });

            modelBuilder.Entity<TipoSpot>(entity =>
            {
                entity.ToTable("TipoSpot");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Designacao)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("designacao");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuario");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasColumnName("createdOn");

                entity.Property(e => e.IdAspNetUser).HasMaxLength(450);

                entity.Property(e => e.IdCanal).HasColumnName("idCanal");

                entity.Property(e => e.IdJanela).HasColumnName("idJanela");

                entity.Property(e => e.IdCliente).HasColumnName("idCliente");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.Role)
                        .IsUnicode(false)
                        .HasColumnName("role");

                entity.Property(e => e.RemovedOn)
                    .HasColumnType("datetime")
                    .HasColumnName("removedOn");

                entity.HasOne(d => d.IdCanalNavigation)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.IdCanal)
                    .HasConstraintName("FK_Usuario_Canal");

                entity.HasOne(d => d.IdJanelaNavigation)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.IdJanela)
                    .HasConstraintName("FK_Usuario_Janela");

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.IdCliente)
                    .HasConstraintName("FK_Usuario_Cliente");
            });

            OnModelCreatingPartial(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public DbSet<SGSP.Models.UpdatePasswordModel> UpdatePasswordModel { get; set; }
    }
}
