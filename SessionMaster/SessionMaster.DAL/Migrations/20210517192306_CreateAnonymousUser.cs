using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SessionMaster.DAL.Migrations
{
    public partial class CreateAnonymousUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnonymousUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnonymousUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SessionAnonymousUser",
                columns: table => new
                {
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnonymousUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionAnonymousUser", x => new { x.SessionId, x.AnonymousUserId });
                    table.ForeignKey(
                        name: "FK_SessionAnonymousUser_AnonymousUsers_AnonymousUserId",
                        column: x => x.AnonymousUserId,
                        principalTable: "AnonymousUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SessionAnonymousUser_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SessionAnonymousUser_AnonymousUserId",
                table: "SessionAnonymousUser",
                column: "AnonymousUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessionAnonymousUser");

            migrationBuilder.DropTable(
                name: "AnonymousUsers");
        }
    }
}
