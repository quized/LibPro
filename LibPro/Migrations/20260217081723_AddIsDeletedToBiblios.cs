using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibPro.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToBiblios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "isDeleted",
                table: "Biblios",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "Biblios");
        }
    }
}
