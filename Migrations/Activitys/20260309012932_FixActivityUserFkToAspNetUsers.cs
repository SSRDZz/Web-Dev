using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcMovie.Migrations.Activitys
{
    /// <inheritdoc />
    public partial class FixActivityUserFkToAspNetUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET @fk := (
    SELECT CONSTRAINT_NAME
    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
    WHERE CONSTRAINT_SCHEMA = DATABASE()
      AND TABLE_NAME = 'ActivityCoOwners'
      AND CONSTRAINT_NAME = 'FK_ActivityCoOwners_UserAccount_CoOwnersId'
    LIMIT 1
);
SET @sql := IF(@fk IS NULL, 'SELECT 1', 'ALTER TABLE `ActivityCoOwners` DROP FOREIGN KEY `FK_ActivityCoOwners_UserAccount_CoOwnersId`');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
");

            migrationBuilder.Sql(@"
SET @fk := (
    SELECT CONSTRAINT_NAME
    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
    WHERE CONSTRAINT_SCHEMA = DATABASE()
      AND TABLE_NAME = 'ActivityParticipants'
      AND CONSTRAINT_NAME = 'FK_ActivityParticipants_UserAccount_ParticipantsId'
    LIMIT 1
);
SET @sql := IF(@fk IS NULL, 'SELECT 1', 'ALTER TABLE `ActivityParticipants` DROP FOREIGN KEY `FK_ActivityParticipants_UserAccount_ParticipantsId`');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityCoOwners_AspNetUsers_CoOwnersId",
                table: "ActivityCoOwners",
                column: "CoOwnersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityParticipants_AspNetUsers_ParticipantsId",
                table: "ActivityParticipants",
                column: "ParticipantsId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityCoOwners_AspNetUsers_CoOwnersId",
                table: "ActivityCoOwners");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityParticipants_AspNetUsers_ParticipantsId",
                table: "ActivityParticipants");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityCoOwners_UserAccount_CoOwnersId",
                table: "ActivityCoOwners",
                column: "CoOwnersId",
                principalTable: "UserAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityParticipants_UserAccount_ParticipantsId",
                table: "ActivityParticipants",
                column: "ParticipantsId",
                principalTable: "UserAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
