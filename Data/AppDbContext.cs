using BasicBilling.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BasicBilling.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public AppDbContext() { }

        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
    }
}
