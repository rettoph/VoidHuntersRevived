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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Release>(builder =>
            {
                builder.ToTable("Releases");

                builder.Property(r => r.Id).IsRequired();
                builder.Property(r => r.Version).IsRequired();
                builder.Property(a => a.RID).IsRequired();
                builder.Property(a => a.Type).IsRequired();
                builder.Property(a => a.DownloadUrl).IsRequired();
                builder.Property(r => r.CreatedAt).IsRequired();
            });
        }
    }
}
