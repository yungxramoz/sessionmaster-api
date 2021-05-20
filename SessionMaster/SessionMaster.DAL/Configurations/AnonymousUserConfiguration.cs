using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SessionMaster.DAL.Entities;

namespace SessionMaster.DAL.Configurations
{
    public class AnonymousUserConfiguration : IEntityTypeConfiguration<AnonymousUser>
    {
        public void Configure(EntityTypeBuilder<AnonymousUser> builder)
        {
            builder.ToTable("AnonymousUsers");
        }
    }
}