using Microsoft.EntityFrameworkCore;
using SessionMaster.DAL.Configurations;
using SessionMaster.DAL.Entities;

namespace SessionMaster.DAL
{
    public class SessionMasterContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<AnonymousUser> AnonymousUsers { get; set; }
        public DbSet<UserBoardGame> UserBoardGames { get; set; }
        public DbSet<Sessionplan> Sessionplans { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<SessionUser> SessionUsers { get; set; }
        public DbSet<SessionAnonymousUser> SessionAnonymousUsers { get; set; }

        public SessionMasterContext(DbContextOptions<SessionMasterContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // apply configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new AnonymousUserConfiguration());
            modelBuilder.ApplyConfiguration(new UserBoardGameConfiguration());
            modelBuilder.ApplyConfiguration(new SessionplanConfigurations());
            modelBuilder.ApplyConfiguration(new SessionConfigurations());
            modelBuilder.ApplyConfiguration(new SessionUserConfiguration());
            modelBuilder.ApplyConfiguration(new SessionAnonymousUserConfiguration());
        }
    }
}