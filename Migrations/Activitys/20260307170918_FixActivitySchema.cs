using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcMovie.Migrations.Activitys
{
    /// <inheritdoc />
    public partial class FixActivitySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityKeywords_Activities_ActivityId1",
                table: "ActivityKeywords");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAccount_Activities_ActivityId",
                table: "UserAccount");

            migrationBuilder.DropIndex(
                name: "IX_UserAccount_ActivityId",
                table: "UserAccount");

            migrationBuilder.DropIndex(
                name: "IX_ActivityKeywords_ActivityId1",
                table: "ActivityKeywords");

            migrationBuilder.DropColumn(
                name: "ActivityId",
                table: "UserAccount");

            migrationBuilder.DropColumn(
                name: "ActivityId1",
                table: "ActivityKeywords");

            migrationBuilder.AlterColumn<Guid>(
                name: "ActivityId",
                table: "ActivityKeywords",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "ActivityCoOwners",
                columns: table => new
                {
                    CoOwnedActivitiesId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CoOwnersId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityCoOwners", x => new { x.CoOwnedActivitiesId, x.CoOwnersId });
                    table.ForeignKey(
                        name: "FK_ActivityCoOwners_Activities_CoOwnedActivitiesId",
                        column: x => x.CoOwnedActivitiesId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityCoOwners_UserAccount_CoOwnersId",
                        column: x => x.CoOwnersId,
                        principalTable: "UserAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityKeywords_ActivityId",
                table: "ActivityKeywords",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityCoOwners_CoOwnersId",
                table: "ActivityCoOwners",
                column: "CoOwnersId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityKeywords_Activities_ActivityId",
                table: "ActivityKeywords",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityKeywords_Activities_ActivityId",
                table: "ActivityKeywords");

            migrationBuilder.DropTable(
                name: "ActivityCoOwners");

            migrationBuilder.DropIndex(
                name: "IX_ActivityKeywords_ActivityId",
                table: "ActivityKeywords");

            migrationBuilder.AddColumn<Guid>(
                name: "ActivityId",
                table: "UserAccount",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "ActivityId",
                table: "ActivityKeywords",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "ActivityId1",
                table: "ActivityKeywords",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccount_ActivityId",
                table: "UserAccount",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityKeywords_ActivityId1",
                table: "ActivityKeywords",
                column: "ActivityId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityKeywords_Activities_ActivityId1",
                table: "ActivityKeywords",
                column: "ActivityId1",
                principalTable: "Activities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccount_Activities_ActivityId",
                table: "UserAccount",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id");
        }
    }
}
