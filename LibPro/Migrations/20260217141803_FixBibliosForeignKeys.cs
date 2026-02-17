using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibPro.Migrations
{
    /// <inheritdoc />
    public partial class FixBibliosForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookItems_Biblios_BiblioBibID",
                table: "BookItems");

            migrationBuilder.DropForeignKey(
                name: "FK_BookItems_ItemStatus_ItemStatusStatusCode",
                table: "BookItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Fines_FineTypes_FineTypeFTID",
                table: "Fines");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_BookItems_BookItemItemID",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Patrons_UserAccounts_UserAccountUserID",
                table: "Patrons");

            migrationBuilder.DropForeignKey(
                name: "FK_Reserves_BookItems_BookItemItemID",
                table: "Reserves");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Biblios_BibliosBibID",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_Departments_DepartmentDeptID",
                table: "Staffs");

            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_UserAccounts_UserAccountUserID",
                table: "Staffs");

            migrationBuilder.DropIndex(
                name: "IX_Staffs_DepartmentDeptID",
                table: "Staffs");

            migrationBuilder.DropIndex(
                name: "IX_Staffs_UserAccountUserID",
                table: "Staffs");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_BibliosBibID",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reserves_BookItemItemID",
                table: "Reserves");

            migrationBuilder.DropIndex(
                name: "IX_Patrons_UserAccountUserID",
                table: "Patrons");

            migrationBuilder.DropIndex(
                name: "IX_Loans_BookItemItemID",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Fines_FineTypeFTID",
                table: "Fines");

            migrationBuilder.DropIndex(
                name: "IX_BookItems_BiblioBibID",
                table: "BookItems");

            migrationBuilder.DropIndex(
                name: "IX_BookItems_ItemStatusStatusCode",
                table: "BookItems");

            migrationBuilder.DropColumn(
                name: "DepartmentDeptID",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "UserAccountUserID",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "BibliosBibID",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "BookItemItemID",
                table: "Reserves");

            migrationBuilder.DropColumn(
                name: "UserAccountUserID",
                table: "Patrons");

            migrationBuilder.DropColumn(
                name: "BookItemItemID",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "FineTypeFTID",
                table: "Fines");

            migrationBuilder.DropColumn(
                name: "BiblioBibID",
                table: "BookItems");

            migrationBuilder.DropColumn(
                name: "ItemStatusStatusCode",
                table: "BookItems");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Staffs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DeptID",
                table: "Staffs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ItemID",
                table: "Reserves",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Patrons",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ItemID",
                table: "Loans",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_DeptID",
                table: "Staffs",
                column: "DeptID");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_UserID",
                table: "Staffs",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BibID",
                table: "Reviews",
                column: "BibID");

            migrationBuilder.CreateIndex(
                name: "IX_Reserves_ItemID",
                table: "Reserves",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_Patrons_UserID",
                table: "Patrons",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_ItemID",
                table: "Loans",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_FTID",
                table: "Fines",
                column: "FTID");

            migrationBuilder.CreateIndex(
                name: "IX_BookItems_BibID",
                table: "BookItems",
                column: "BibID");

            migrationBuilder.CreateIndex(
                name: "IX_BookItems_ItmStatus",
                table: "BookItems",
                column: "ItmStatus");

            migrationBuilder.AddForeignKey(
                name: "FK_BookItems_Biblios_BibID",
                table: "BookItems",
                column: "BibID",
                principalTable: "Biblios",
                principalColumn: "BibID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookItems_ItemStatus_ItmStatus",
                table: "BookItems",
                column: "ItmStatus",
                principalTable: "ItemStatus",
                principalColumn: "StatusCode",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Fines_FineTypes_FTID",
                table: "Fines",
                column: "FTID",
                principalTable: "FineTypes",
                principalColumn: "FTID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_BookItems_ItemID",
                table: "Loans",
                column: "ItemID",
                principalTable: "BookItems",
                principalColumn: "ItemID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Patrons_UserAccounts_UserID",
                table: "Patrons",
                column: "UserID",
                principalTable: "UserAccounts",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reserves_BookItems_ItemID",
                table: "Reserves",
                column: "ItemID",
                principalTable: "BookItems",
                principalColumn: "ItemID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Biblios_BibID",
                table: "Reviews",
                column: "BibID",
                principalTable: "Biblios",
                principalColumn: "BibID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_Departments_DeptID",
                table: "Staffs",
                column: "DeptID",
                principalTable: "Departments",
                principalColumn: "DeptID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_UserAccounts_UserID",
                table: "Staffs",
                column: "UserID",
                principalTable: "UserAccounts",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookItems_Biblios_BibID",
                table: "BookItems");

            migrationBuilder.DropForeignKey(
                name: "FK_BookItems_ItemStatus_ItmStatus",
                table: "BookItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Fines_FineTypes_FTID",
                table: "Fines");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_BookItems_ItemID",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Patrons_UserAccounts_UserID",
                table: "Patrons");

            migrationBuilder.DropForeignKey(
                name: "FK_Reserves_BookItems_ItemID",
                table: "Reserves");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Biblios_BibID",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_Departments_DeptID",
                table: "Staffs");

            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_UserAccounts_UserID",
                table: "Staffs");

            migrationBuilder.DropIndex(
                name: "IX_Staffs_DeptID",
                table: "Staffs");

            migrationBuilder.DropIndex(
                name: "IX_Staffs_UserID",
                table: "Staffs");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_BibID",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reserves_ItemID",
                table: "Reserves");

            migrationBuilder.DropIndex(
                name: "IX_Patrons_UserID",
                table: "Patrons");

            migrationBuilder.DropIndex(
                name: "IX_Loans_ItemID",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Fines_FTID",
                table: "Fines");

            migrationBuilder.DropIndex(
                name: "IX_BookItems_BibID",
                table: "BookItems");

            migrationBuilder.DropIndex(
                name: "IX_BookItems_ItmStatus",
                table: "BookItems");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Staffs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "DeptID",
                table: "Staffs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "DepartmentDeptID",
                table: "Staffs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAccountUserID",
                table: "Staffs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BibliosBibID",
                table: "Reviews",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ItemID",
                table: "Reserves",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "BookItemItemID",
                table: "Reserves",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Patrons",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserAccountUserID",
                table: "Patrons",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ItemID",
                table: "Loans",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "BookItemItemID",
                table: "Loans",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "FineTypeFTID",
                table: "Fines",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BiblioBibID",
                table: "BookItems",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "ItemStatusStatusCode",
                table: "BookItems",
                type: "tinyint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_DepartmentDeptID",
                table: "Staffs",
                column: "DepartmentDeptID");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_UserAccountUserID",
                table: "Staffs",
                column: "UserAccountUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BibliosBibID",
                table: "Reviews",
                column: "BibliosBibID");

            migrationBuilder.CreateIndex(
                name: "IX_Reserves_BookItemItemID",
                table: "Reserves",
                column: "BookItemItemID");

            migrationBuilder.CreateIndex(
                name: "IX_Patrons_UserAccountUserID",
                table: "Patrons",
                column: "UserAccountUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_BookItemItemID",
                table: "Loans",
                column: "BookItemItemID");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_FineTypeFTID",
                table: "Fines",
                column: "FineTypeFTID");

            migrationBuilder.CreateIndex(
                name: "IX_BookItems_BiblioBibID",
                table: "BookItems",
                column: "BiblioBibID");

            migrationBuilder.CreateIndex(
                name: "IX_BookItems_ItemStatusStatusCode",
                table: "BookItems",
                column: "ItemStatusStatusCode");

            migrationBuilder.AddForeignKey(
                name: "FK_BookItems_Biblios_BiblioBibID",
                table: "BookItems",
                column: "BiblioBibID",
                principalTable: "Biblios",
                principalColumn: "BibID");

            migrationBuilder.AddForeignKey(
                name: "FK_BookItems_ItemStatus_ItemStatusStatusCode",
                table: "BookItems",
                column: "ItemStatusStatusCode",
                principalTable: "ItemStatus",
                principalColumn: "StatusCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Fines_FineTypes_FineTypeFTID",
                table: "Fines",
                column: "FineTypeFTID",
                principalTable: "FineTypes",
                principalColumn: "FTID");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_BookItems_BookItemItemID",
                table: "Loans",
                column: "BookItemItemID",
                principalTable: "BookItems",
                principalColumn: "ItemID");

            migrationBuilder.AddForeignKey(
                name: "FK_Patrons_UserAccounts_UserAccountUserID",
                table: "Patrons",
                column: "UserAccountUserID",
                principalTable: "UserAccounts",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Reserves_BookItems_BookItemItemID",
                table: "Reserves",
                column: "BookItemItemID",
                principalTable: "BookItems",
                principalColumn: "ItemID");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Biblios_BibliosBibID",
                table: "Reviews",
                column: "BibliosBibID",
                principalTable: "Biblios",
                principalColumn: "BibID");

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_Departments_DepartmentDeptID",
                table: "Staffs",
                column: "DepartmentDeptID",
                principalTable: "Departments",
                principalColumn: "DeptID");

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_UserAccounts_UserAccountUserID",
                table: "Staffs",
                column: "UserAccountUserID",
                principalTable: "UserAccounts",
                principalColumn: "UserID");
        }
    }
}
