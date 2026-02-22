using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibPro.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffIsResigned : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsResigned",
                table: "Staffs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsResigned",
                table: "Staffs");
        }
    }
}
