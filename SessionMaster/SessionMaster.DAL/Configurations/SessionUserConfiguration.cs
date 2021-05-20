using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SessionMaster.DAL.Entities;

namespace SessionMaster.DAL.Configurations
{
    public class SessionUserConfiguration : IEntityTypeConfiguration<SessionUser>
    {
        public void Configure(EntityTypeBuilder<SessionUser> builder)
        {
            builder.ToTable("SessionUser");

            builder.HasKey(e => new { e.SessionId, e.UserId });

            builder.HasOne(e => e.User)
                .WithMany(e => e.SessionUsers)
                .OnDelete(DeleteBehavior.ClientCascade);

            builder.HasOne(e => e.Session)
                .WithMany(e => e.SessionUsers)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}