using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Element> Elements { get; set; }
        public DbSet<TierList> TierLists { get; set; }
        public DbSet<ContenuTierList> ContenusTierList { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // IMPORTANT : Mapping exact vers tes noms de tables SQL
            modelBuilder.Entity<Utilisateur>().ToTable("Utilisateur");
            modelBuilder.Entity<Element>().ToTable("Element");
            modelBuilder.Entity<TierList>().ToTable("TierList");
            modelBuilder.Entity<ContenuTierList>().ToTable("ContenuTierList");

            // Configuration des relations
            modelBuilder.Entity<TierList>()
                .HasOne(t => t.Utilisateur)
                .WithMany()
                .HasForeignKey(t => t.UtilisateurId);

            modelBuilder.Entity<ContenuTierList>()
                .HasOne(c => c.TierList)
                .WithMany(t => t.Contenus)
                .HasForeignKey(c => c.TierListId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<ContenuTierList>()
                .HasOne(c => c.Element)
                .WithMany()
                .HasForeignKey(c => c.ElementId);
        }
    }
}