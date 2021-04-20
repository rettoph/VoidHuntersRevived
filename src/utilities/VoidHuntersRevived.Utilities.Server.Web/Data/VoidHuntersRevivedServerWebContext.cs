using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VoidHuntersRevived.Utilities.Server.Web.Models;

namespace VoidHuntersRevived.Utilities.Server.Web.Data
{
    public class VoidHuntersRevivedServerWebContext : DbContext
    {
        public VoidHuntersRevivedServerWebContext (DbContextOptions<VoidHuntersRevivedServerWebContext> options)
            : base(options)
        {
        }

        public DbSet<Release> Release { get; set; }
        public DbSet<Asset> Assets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Release>(builder =>
            {
                builder.ToTable("Releases");

                builder.Property(r => r.Id).IsRequired();
                builder.Property(r => r.ReleaseDate).IsRequired();
                builder.Property(r => r.Version).IsRequired();
            });

            modelBuilder.Entity<Asset>(builder =>
            {
                builder.ToTable("Assets");

                builder.Property(a => a.Id).IsRequired();
                builder.Property(a => a.RID).IsRequired();
                builder.Property(a => a.Type).IsRequired();
                builder.Property(a => a.DownloadURL).IsRequired();
                builder.HasOne(a => a.Release).WithMany(r => r.Assets);
            });
        }
    }
}
