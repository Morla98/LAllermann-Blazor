using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAllermannREST.Migrations.Password
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Password_User_UserId",
                table: "Password");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "Password",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Password_User_UserId",
                table: "Password",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Password_User_UserId",
                table: "Password");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "Password",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Password_User_UserId",
                table: "Password",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
