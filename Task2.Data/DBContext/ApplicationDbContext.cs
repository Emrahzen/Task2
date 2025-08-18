using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task2.Data.BOL;

namespace Task2.Data.DBContext
{
    public class ApplicationDbContext : DbContext, IDisposable
    {
        public readonly bool IsMigration = false;

        public ApplicationDbContext() : base()
        {
            IsMigration = true;
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }


        public virtual DbSet<UserTBL> UserTBL { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (IsMigration)
            {
                optionsBuilder.UseNpgsql("Data Source=localhost;Database=Task2;integrated security=false;User ID=sa;password=19370031017aA;TrustServerCertificate=True;");
            }
            base.OnConfiguring(optionsBuilder);
        }


        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
