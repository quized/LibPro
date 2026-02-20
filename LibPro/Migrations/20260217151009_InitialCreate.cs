using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibPro.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CatID = table.Column<int>(type: "int", nullable: false),
                    CatName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CatID);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    CityID = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.CityID);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DeptID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DeptName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DeptID);
                });

            migrationBuilder.CreateTable(
                name: "FineTypes",
                columns: table => new
                {
                    FTID = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FTName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(8,0)", precision: 8, scale: 0, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FineTypes", x => x.FTID);
                });

            migrationBuilder.CreateTable(
                name: "ItemStatus",
                columns: table => new
                {
                    StatusCode = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemStatus", x => x.StatusCode);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Depth = table.Column<byte>(type: "tinyint", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    ParentID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationID);
                    table.ForeignKey(
                        name: "FK_Locations_Locations_ParentID",
                        column: x => x.ParentID,
                        principalTable: "Locations",
                        principalColumn: "LocationID");
                });

            migrationBuilder.CreateTable(
                name: "PatronsStatus",
                columns: table => new
                {
                    StatusCode = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatronsStatus", x => x.StatusCode);
                });

            migrationBuilder.CreateTable(
                name: "ReserveStatus",
                columns: table => new
                {
                    StatusCode = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReserveStatus", x => x.StatusCode);
                });

            migrationBuilder.CreateTable(
                name: "SystemStatus",
                columns: table => new
                {
                    StatusCode = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemStatus", x => x.StatusCode);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    RoleID = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "Publishers",
                columns: table => new
                {
                    PubID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PubName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Pubtel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CityID = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publishers", x => x.PubID);
                    table.ForeignKey(
                        name: "FK_Publishers_Cities_CityID",
                        column: x => x.CityID,
                        principalTable: "Cities",
                        principalColumn: "CityID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAccounts",
                columns: table => new
                {
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Account = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastLoginTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserType = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccounts", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_UserAccounts_UserRoles_UserType",
                        column: x => x.UserType,
                        principalTable: "UserRoles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Biblios",
                columns: table => new
                {
                    BibID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Author = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PubDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImgPath = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    isDeleted = table.Column<byte>(type: "tinyint", nullable: false),
                    CatID = table.Column<int>(type: "int", nullable: false),
                    PubID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Biblios", x => x.BibID);
                    table.ForeignKey(
                        name: "FK_Biblios_Categories_CatID",
                        column: x => x.CatID,
                        principalTable: "Categories",
                        principalColumn: "CatID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Biblios_Publishers_PubID",
                        column: x => x.PubID,
                        principalTable: "Publishers",
                        principalColumn: "PubID");
                });

            migrationBuilder.CreateTable(
                name: "Patrons",
                columns: table => new
                {
                    PatronID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Education = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<byte>(type: "tinyint", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Profession = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    District = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    Memo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CityID = table.Column<byte>(type: "tinyint", nullable: false),
                    PtrStatus = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patrons", x => x.PatronID);
                    table.ForeignKey(
                        name: "FK_Patrons_Cities_CityID",
                        column: x => x.CityID,
                        principalTable: "Cities",
                        principalColumn: "CityID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Patrons_PatronsStatus_PtrStatus",
                        column: x => x.PtrStatus,
                        principalTable: "PatronsStatus",
                        principalColumn: "StatusCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Patrons_UserAccounts_UserID",
                        column: x => x.UserID,
                        principalTable: "UserAccounts",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Staffs",
                columns: table => new
                {
                    StaffID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Education = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<byte>(type: "tinyint", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CityID = table.Column<byte>(type: "tinyint", nullable: false),
                    DeptID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staffs", x => x.StaffID);
                    table.ForeignKey(
                        name: "FK_Staffs_Cities_CityID",
                        column: x => x.CityID,
                        principalTable: "Cities",
                        principalColumn: "CityID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Staffs_Departments_DeptID",
                        column: x => x.DeptID,
                        principalTable: "Departments",
                        principalColumn: "DeptID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Staffs_UserAccounts_UserID",
                        column: x => x.UserID,
                        principalTable: "UserAccounts",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookItems",
                columns: table => new
                {
                    ItemID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PurDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ItmStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    BibID = table.Column<long>(type: "bigint", nullable: false),
                    LocID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookItems", x => x.ItemID);
                    table.ForeignKey(
                        name: "FK_BookItems_Biblios_BibID",
                        column: x => x.BibID,
                        principalTable: "Biblios",
                        principalColumn: "BibID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookItems_ItemStatus_ItmStatus",
                        column: x => x.ItmStatus,
                        principalTable: "ItemStatus",
                        principalColumn: "StatusCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookItems_Locations_LocID",
                        column: x => x.LocID,
                        principalTable: "Locations",
                        principalColumn: "LocationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PostDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rating = table.Column<byte>(type: "tinyint", nullable: false),
                    RevStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    PatronID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BibID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewID);
                    table.ForeignKey(
                        name: "FK_Reviews_Biblios_BibID",
                        column: x => x.BibID,
                        principalTable: "Biblios",
                        principalColumn: "BibID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Patrons_PatronID",
                        column: x => x.PatronID,
                        principalTable: "Patrons",
                        principalColumn: "PatronID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_SystemStatus_RevStatus",
                        column: x => x.RevStatus,
                        principalTable: "SystemStatus",
                        principalColumn: "StatusCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Announcements",
                columns: table => new
                {
                    AnnID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Creator = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.AnnID);
                    table.ForeignKey(
                        name: "FK_Announcements_Staffs_Creator",
                        column: x => x.Creator,
                        principalTable: "Staffs",
                        principalColumn: "StaffID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    LoanID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RenewalCount = table.Column<byte>(type: "tinyint", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    PatronID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ItemID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.LoanID);
                    table.ForeignKey(
                        name: "FK_Loans_BookItems_ItemID",
                        column: x => x.ItemID,
                        principalTable: "BookItems",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Loans_Patrons_PatronID",
                        column: x => x.PatronID,
                        principalTable: "Patrons",
                        principalColumn: "PatronID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reserves",
                columns: table => new
                {
                    ResID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ResDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ResStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    PatronID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ItemID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reserves", x => x.ResID);
                    table.ForeignKey(
                        name: "FK_Reserves_BookItems_ItemID",
                        column: x => x.ItemID,
                        principalTable: "BookItems",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reserves_Patrons_PatronID",
                        column: x => x.PatronID,
                        principalTable: "Patrons",
                        principalColumn: "PatronID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reserves_ReserveStatus_ResStatus",
                        column: x => x.ResStatus,
                        principalTable: "ReserveStatus",
                        principalColumn: "StatusCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fines",
                columns: table => new
                {
                    FineID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ISPaid = table.Column<bool>(type: "bit", nullable: false),
                    FTID = table.Column<byte>(type: "tinyint", nullable: false),
                    LoanID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fines", x => x.FineID);
                    table.ForeignKey(
                        name: "FK_Fines_FineTypes_FTID",
                        column: x => x.FTID,
                        principalTable: "FineTypes",
                        principalColumn: "FTID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fines_Loans_LoanID",
                        column: x => x.LoanID,
                        principalTable: "Loans",
                        principalColumn: "LoanID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_Creator",
                table: "Announcements",
                column: "Creator");

            migrationBuilder.CreateIndex(
                name: "IX_Biblios_CatID",
                table: "Biblios",
                column: "CatID");

            migrationBuilder.CreateIndex(
                name: "IX_Biblios_PubID",
                table: "Biblios",
                column: "PubID");

            migrationBuilder.CreateIndex(
                name: "IX_BookItems_BibID",
                table: "BookItems",
                column: "BibID");

            migrationBuilder.CreateIndex(
                name: "IX_BookItems_ItmStatus",
                table: "BookItems",
                column: "ItmStatus");

            migrationBuilder.CreateIndex(
                name: "IX_BookItems_LocID",
                table: "BookItems",
                column: "LocID");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_FTID",
                table: "Fines",
                column: "FTID");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_LoanID",
                table: "Fines",
                column: "LoanID");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_ItemID",
                table: "Loans",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_PatronID",
                table: "Loans",
                column: "PatronID");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_ParentID",
                table: "Locations",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_Patrons_CityID",
                table: "Patrons",
                column: "CityID");

            migrationBuilder.CreateIndex(
                name: "IX_Patrons_PtrStatus",
                table: "Patrons",
                column: "PtrStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Patrons_UserID",
                table: "Patrons",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Publishers_CityID",
                table: "Publishers",
                column: "CityID");

            migrationBuilder.CreateIndex(
                name: "IX_Reserves_ItemID",
                table: "Reserves",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_Reserves_PatronID",
                table: "Reserves",
                column: "PatronID");

            migrationBuilder.CreateIndex(
                name: "IX_Reserves_ResStatus",
                table: "Reserves",
                column: "ResStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BibID",
                table: "Reviews",
                column: "BibID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_PatronID",
                table: "Reviews",
                column: "PatronID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_RevStatus",
                table: "Reviews",
                column: "RevStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_CityID",
                table: "Staffs",
                column: "CityID");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_DeptID",
                table: "Staffs",
                column: "DeptID");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_UserID",
                table: "Staffs",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_UserType",
                table: "UserAccounts",
                column: "UserType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Announcements");

            migrationBuilder.DropTable(
                name: "Fines");

            migrationBuilder.DropTable(
                name: "Reserves");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Staffs");

            migrationBuilder.DropTable(
                name: "FineTypes");

            migrationBuilder.DropTable(
                name: "Loans");

            migrationBuilder.DropTable(
                name: "ReserveStatus");

            migrationBuilder.DropTable(
                name: "SystemStatus");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "BookItems");

            migrationBuilder.DropTable(
                name: "Patrons");

            migrationBuilder.DropTable(
                name: "Biblios");

            migrationBuilder.DropTable(
                name: "ItemStatus");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "PatronsStatus");

            migrationBuilder.DropTable(
                name: "UserAccounts");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Publishers");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Cities");
        }
    }
}
