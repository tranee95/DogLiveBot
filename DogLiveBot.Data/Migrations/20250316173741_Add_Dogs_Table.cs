using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DogLiveBot.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Dogs_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserCallbackQuerys_UserTelegramId",
                table: "UserCallbackQuerys");

            migrationBuilder.CreateTable(
                name: "Dogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserTelegramId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dogs_Users_UserTelegramId",
                        column: x => x.UserTelegramId,
                        principalTable: "Users",
                        principalColumn: "TelegramId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCallbackQuerys_UserTelegramId",
                table: "UserCallbackQuerys",
                column: "UserTelegramId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dogs_UserTelegramId",
                table: "Dogs",
                column: "UserTelegramId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dogs");

            migrationBuilder.DropIndex(
                name: "IX_UserCallbackQuerys_UserTelegramId",
                table: "UserCallbackQuerys");

            migrationBuilder.CreateIndex(
                name: "IX_UserCallbackQuerys_UserTelegramId",
                table: "UserCallbackQuerys",
                column: "UserTelegramId");
        }
    }
}
