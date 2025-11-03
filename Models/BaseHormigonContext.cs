using Microsoft.EntityFrameworkCore;

namespace CalculoBasesAIE.Models
{
    public class BaseHormigonContext(DbContextOptions<BaseHormigonContext> options) : DbContext(options)
    {
        public DbSet<BaseHormigon> BasesHormigon { get; set; }
        public DbSet<BaseHormigonDimensiones> BasesHormigonDimensiones { get; set; }
        public DbSet<BaseHormigonEsfuerzos> BasesHormigonEsfuerzos { get; set; }
        public DbSet<BaseHormigonVerificaciones> BasesHormigonVerificaciones { get; set; }
        public DbSet<BaseHormigonArmadura> BasesHormigonArmaduras { get; set; }
        public DbSet<BaseHormigonCuantia> BasesHormigonCuantias { get; set; }
        public DbSet<BaseHormigonDiametrosBarras> BasesHormigonDiametrosBarras { get; set; }
        public DbSet<BaseHormigonVerificacionCorte> BasesHormigonVerificacionCorte { get; set; }
        public DbSet<BaseHormigonVerificacionPunzonado> BasesHormigonVerificacionPunzonado { get; set; }
        public DbSet<BaseHormigonComputo> BasesHormigonComputos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BaseHormigon>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.EsfuerzoAxil);
            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.PorcentajeCargaD);
            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.PorcentajeCargaL);
            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.AnchoColumnaX);
            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.AnchoColumnaY);
            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.CargaAdmisible);
            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.PesoEspecificoSuelo);
            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.NivelFundacion);
            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.PesoEspecificoHormigon);
            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.ResistenciaCaracteristicaHormigon);
            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.RecubrimientoHormigon);
            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.TensionFluenciaAcero);
            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.DiametroBarrasX);
            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.DiametroBarrasY);
            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.MomentoX);
            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.MomentoY);
            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.CorteX);
            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.CorteY);
            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.ModuloBalasto);
            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.CostoM3Hormigon);
            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.CostoKgAcero);
            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.CostoM3Excavacion);
            modelBuilder.Entity<BaseHormigon>().OwnsOne(b => b.CoeficienteEsponjamiento);
        }
    }
}
