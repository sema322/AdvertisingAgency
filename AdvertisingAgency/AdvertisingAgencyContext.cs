
using AdvertisingAgency.Models;
using Microsoft.EntityFrameworkCore;

namespace AdvertisingAgency
{
    public class AdvertisingAgencyContext : DbContext
    {
        public DbSet<Advertising> Advertisings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<User> Users { get; set; }

        public AdvertisingAgencyContext(DbContextOptions options) : base(options) { }
    }
}
