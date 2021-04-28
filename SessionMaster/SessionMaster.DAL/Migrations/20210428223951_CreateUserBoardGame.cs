using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SessionMaster.DAL.Migrations
{
    public partial class CreateUserBoardGame : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserBoardGame",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoardGameId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBoardGame", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBoardGame_User_Id",
                        column: x => x.Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBoardGame_UserId_BoardGameId",
                table: "UserBoardGame",
                columns: new[] { "UserId", "BoardGameId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserBoardGame");
        }
    }
}