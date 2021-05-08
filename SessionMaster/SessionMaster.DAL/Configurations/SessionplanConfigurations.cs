using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SessionMaster.DAL.Entities;

namespace SessionMaster.DAL.Configurations
{
    public class SessionplanConfigurations : IEntityTypeConfiguration<Sessionplan>
    {
        public void Configure(EntityTypeBuilder<Sessionplan> builder)
        {
            builder.ToTable("Sessionplans");
        }
    }
}