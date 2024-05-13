using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        public DbSet<Ledger> Ledger { get; set; }
        public DbSet<NetworkNode> NetworkNodes { get; set; }
        public DbSet<Balance> Balances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();

            ////modelBuilder.Entity<Ledger>().HasNoKey().HasIndex(x => x.BlockID).IsUnique();
            //modelBuilder.Entity<Ledger>().HasNoKey().Property(p => p.Amount).HasPrecision(18,2);
            modelBuilder.Entity<Ledger>().Property(p => p.Amount).HasPrecision(16, 8);
            modelBuilder.Entity<Balance>().Property(p => p.Amount).HasPrecision(16, 8);

            ////GET ALL FK FROM MY CODE MODEL AND SET FK CASADE ON DELETE
            ////0=CLIENT SET NULL //1=RESTRICT //2=SET NULL //3=CASCADE
            ////ADD MIGRATION AND UPDATE DATABASE
            //foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
            //    .SelectMany(e => e.GetForeignKeys()))
            //{
            //    foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            //}
        }
    }
}
