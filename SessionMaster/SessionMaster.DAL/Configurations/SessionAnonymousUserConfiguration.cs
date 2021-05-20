using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SessionMaster.DAL.Entities;

namespace SessionMaster.DAL.Configurations
{
    public class SessionAnonymousUserConfiguration : IEntityTypeConfiguration<SessionAnonymousUser>
    {
        public void Configure(EntityTypeBuilder<SessionAnonymousUser> builder)
        {
            builder.ToTable("SessionAnonymousUser");

            builder.HasKey(e => new { e.SessionId, e.AnonymousUserId });

            builder.HasOne(e => e.AnonymousUser)
                .WithMany(e => e.SessionAnonymousUsers)
                .OnDelete(DeleteBehavior.ClientCascade);

            builder.HasOne(e => e.Session)
                .WithMany(e => e.SessionAnonymousUsers)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}