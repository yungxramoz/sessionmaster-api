using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SessionMaster.DAL.Configurations;
using SessionMaster.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionMaster.DAL
{
    public class SessionMasterContext : DbContext
    {
        protected readonly IConfiguration _configuration;

        public DbSet<User> Users { get; set; }

        public SessionMasterContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // apply configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration.GetConnectionString("SessionMasterContext");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
