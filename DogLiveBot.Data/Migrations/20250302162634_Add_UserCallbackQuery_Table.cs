using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DogLiveBot.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_UserCallbackQuery_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "TelegramId",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Users_TelegramId",
                table: "Users",
                column: "TelegramId");

            migrationBuilder.CreateTable(
                name: "UserCallbackQuerys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CallbackQueryId = table.Column<string>(type: "text", nullable: false),
                    UserTelegramId = table.Column<long>(type: "bigint", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCallbackQuerys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCallbackQuerys_Users_UserTelegramId",
                        column: x => x.UserTelegramId,
                        principalTable: "Users",
                        principalColumn: "TelegramId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCallbackQuerys_UserTelegramId",
                table: "UserCallbackQuerys",
                column: "UserTelegramId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCallbackQuerys");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Users_TelegramId",
                table: "Users");

            migrationBuilder.AlterColumn<long>(
                name: "TelegramId",
                table: "Users",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
