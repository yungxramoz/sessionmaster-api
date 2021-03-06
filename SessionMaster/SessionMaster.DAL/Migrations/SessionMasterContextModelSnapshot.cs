// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SessionMaster.DAL;

namespace SessionMaster.DAL.Migrations
{
    [DbContext(typeof(SessionMasterContext))]
    partial class SessionMasterContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SessionMaster.DAL.Entities.AnonymousUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("AnonymousUsers");
                });

            modelBuilder.Entity("SessionMaster.DAL.Entities.Session", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("SessionplanId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("StartTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("SessionplanId");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("SessionMaster.DAL.Entities.SessionAnonymousUser", b =>
                {
                    b.Property<Guid>("SessionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AnonymousUserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("SessionId", "AnonymousUserId");

                    b.HasIndex("AnonymousUserId");

                    b.ToTable("SessionAnonymousUser");
                });

            modelBuilder.Entity("SessionMaster.DAL.Entities.SessionUser", b =>
                {
                    b.Property<Guid>("SessionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("SessionId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("SessionUser");
                });

            modelBuilder.Entity("SessionMaster.DAL.Entities.Sessionplan", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Sessionplans");
                });

            modelBuilder.Entity("SessionMaster.DAL.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Firstname")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Lastname")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("varbinary(64)");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varbinary(128)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("SessionMaster.DAL.Entities.UserBoardGame", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BoardGameId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId", "BoardGameId")
                        .IsUnique();

                    b.ToTable("UserBoardGame");
                });

            modelBuilder.Entity("SessionMaster.DAL.Entities.Session", b =>
                {
                    b.HasOne("SessionMaster.DAL.Entities.Sessionplan", "Sessionplan")
                        .WithMany("Sessions")
                        .HasForeignKey("SessionplanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sessionplan");
                });

            modelBuilder.Entity("SessionMaster.DAL.Entities.SessionAnonymousUser", b =>
                {
                    b.HasOne("SessionMaster.DAL.Entities.AnonymousUser", "AnonymousUser")
                        .WithMany("SessionAnonymousUsers")
                        .HasForeignKey("AnonymousUserId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("SessionMaster.DAL.Entities.Session", "Session")
                        .WithMany("SessionAnonymousUsers")
                        .HasForeignKey("SessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AnonymousUser");

                    b.Navigation("Session");
                });

            modelBuilder.Entity("SessionMaster.DAL.Entities.SessionUser", b =>
                {
                    b.HasOne("SessionMaster.DAL.Entities.Session", "Session")
                        .WithMany("SessionUsers")
                        .HasForeignKey("SessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SessionMaster.DAL.Entities.User", "User")
                        .WithMany("SessionUsers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("Session");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SessionMaster.DAL.Entities.Sessionplan", b =>
                {
                    b.HasOne("SessionMaster.DAL.Entities.User", "User")
                        .WithMany("Sessionplans")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("User");
                });

            modelBuilder.Entity("SessionMaster.DAL.Entities.UserBoardGame", b =>
                {
                    b.HasOne("SessionMaster.DAL.Entities.User", "User")
                        .WithMany("BoardGames")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("SessionMaster.DAL.Entities.AnonymousUser", b =>
                {
                    b.Navigation("SessionAnonymousUsers");
                });

            modelBuilder.Entity("SessionMaster.DAL.Entities.Session", b =>
                {
                    b.Navigation("SessionAnonymousUsers");

                    b.Navigation("SessionUsers");
                });

            modelBuilder.Entity("SessionMaster.DAL.Entities.Sessionplan", b =>
                {
                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("SessionMaster.DAL.Entities.User", b =>
                {
                    b.Navigation("BoardGames");

                    b.Navigation("Sessionplans");

                    b.Navigation("SessionUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
