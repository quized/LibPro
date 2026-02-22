using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibPro.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePatronAndStaffUserID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patrons_UserAccounts_UserID",
                table: "Patrons");

            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_UserAccounts_UserID",
                table: "Staffs");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Staffs",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Patrons",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Patrons_UserAccounts_UserID",
                table: "Patrons",
                column: "UserID",
                principalTable: "UserAccounts",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_UserAccounts_UserID",
                table: "Staffs",
                column: "UserID",
                principalTable: "UserAccounts",
                principalColumn: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patrons_UserAccounts_UserID",
                table: "Patrons");

            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_UserAccounts_UserID",
                table: "Staffs");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Staffs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Patrons",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Patrons_UserAccounts_UserID",
                table: "Patrons",
                column: "UserID",
                principalTable: "UserAccounts",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_UserAccounts_UserID",
                table: "Staffs",
                column: "UserID",
                principalTable: "UserAccounts",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
