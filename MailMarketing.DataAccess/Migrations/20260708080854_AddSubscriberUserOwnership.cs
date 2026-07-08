using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MailMarketing.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriberUserOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subscribers_Email",
                table: "Subscribers");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Subscribers",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM "Subscribers") AND NOT EXISTS (SELECT 1 FROM "Users") THEN
                        RAISE EXCEPTION 'Cannot assign existing subscribers to a default user because no users exist.';
                    END IF;
                END $$;
                """);

            migrationBuilder.Sql("""
                UPDATE "Subscribers"
                SET "UserId" = (SELECT "Id" FROM "Users" ORDER BY "Id" LIMIT 1)
                WHERE "UserId" IS NULL;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Subscribers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscribers_UserId_Email",
                table: "Subscribers",
                columns: new[] { "UserId", "Email" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscribers_Users_UserId",
                table: "Subscribers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscribers_Users_UserId",
                table: "Subscribers");

            migrationBuilder.DropIndex(
                name: "IX_Subscribers_UserId_Email",
                table: "Subscribers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Subscribers");

            migrationBuilder.CreateIndex(
                name: "IX_Subscribers_Email",
                table: "Subscribers",
                column: "Email",
                unique: true);
        }
    }
}
