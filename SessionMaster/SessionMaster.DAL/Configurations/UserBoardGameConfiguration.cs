using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SessionMaster.DAL.Entities;

namespace SessionMaster.DAL.Configurations
{
    public class UserBoardGameConfiguration : IEntityTypeConfiguration<UserBoardGame>
    {
        public void Configure(EntityTypeBuilder<UserBoardGame> builder)
        {
            builder.ToTable("UserBoardGame");
            builder.HasIndex(e => new { e.UserId, e.BoardGameId }).IsUnique();
        }
    }
}