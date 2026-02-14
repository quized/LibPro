using Microsoft.EntityFrameworkCore;

namespace LibPro.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (LibproContext context = new LibproContext(serviceProvider.GetRequiredService<DbContextOptions<LibproContext>>()))
            {
                if (context.UserRoles.Any())
                {
                    return;
                }

                context.UserRoles.AddRange(
                    new UserRoles
                    {
                        RoleID = 1,
                        RoleName = "Admin"
                    },
                    new UserRoles
                    {
                        RoleID = 2,
                        RoleName = "Staff"
                    },
                    new UserRoles
                    {
                        RoleID = 3,
                        RoleName = "Patron"
                    }
                );

                context.SaveChanges();

                context.Departments.AddRange(
                    new Departments
                    {
                        DeptID = "D01",
                        DeptName = "採編組"
                    },
                    new Departments
                    {
                        DeptID = "D02",
                        DeptName = "典閱組"
                    },
                    new Departments
                    {
                        DeptID = "D03",
                        DeptName = "參考組"
                    },
                    new Departments
                    {
                        DeptID = "D04",
                        DeptName = "系統組"
                    },
                    new Departments
                    {
                        DeptID = "D05",
                        DeptName = "行政組"
                    }

                    );
                context.SaveChanges();

                context.PatronsStatus.AddRange(
                    new PatronsStatus
                    {
                        StatusCode = 1,
                        StatusName = "正常"
                    },
                    new PatronsStatus
                    {
                        StatusCode = 2,
                        StatusName = "停權中"
                    },
                    new PatronsStatus
                    {
                        StatusCode = 3,
                        StatusName = "已掛失"
                    },
                    new PatronsStatus
                    {
                        StatusCode = 4,
                        StatusName = "註銷"
                    }

                    );
                context.ReserveStatus.AddRange(
                    new ReserveStatus
                    {
                        StatusCode = 1,
                        StatusName = "排隊中"
                    },
                    new ReserveStatus
                    {
                        StatusCode = 2,
                        StatusName = "待取"
                    },
                    new ReserveStatus
                    {
                        StatusCode = 3,
                        StatusName = "結案"
                    },
                    new ReserveStatus
                    {
                        StatusCode = 4,
                        StatusName = "取消"
                    }
                    );
                context.SystemStatus.AddRange(
                    new SystemStatus
                    {
                        StatusCode = 1,
                        StatusName = "待審核"
                    },
                    new SystemStatus
                    {
                        StatusCode = 2,
                        StatusName = "通過"
                    },
                    new SystemStatus
                    {
                        StatusCode = 3,
                        StatusName = "刪除"
                    }
                    );

                context.SaveChanges();
                context.Locations.AddRange(
                    new Locations
                    {
                        LocationID = 1,
                        LocationName = "總館",
                        Depth = 0,
                        SortOrder = 10,
                        ParentID = null
                    },
                    new Locations
                    {
                        LocationID = 2,
                        LocationName = "一樓",
                        Depth = 1,
                        SortOrder = 10,
                        ParentID = 1
                    },
                    new Locations
                    {
                        LocationID = 3,
                        LocationName = "二樓",
                        Depth = 1,
                        SortOrder = 20,
                        ParentID = 1
                    }
                    );
                context.ItemStatus.AddRange(
                    new ItemStatus
                    {
                        StatusCode = 1,
                        StatusName = "在架"
                    },
                    new ItemStatus
                    {
                        StatusCode = 2,
                        StatusName = "借出"
                    },
                    new ItemStatus
                    {
                        StatusCode = 3,
                        StatusName = "預約保留"
                    },
                    new ItemStatus
                    {
                        StatusCode = 4,
                        StatusName = "遺失"
                    },
                    new ItemStatus
                    {
                        StatusCode = 5,
                        StatusName = "整理"
                    },
                    new ItemStatus
                    {
                        StatusCode = 6,
                        StatusName = "報廢"
                    }
                    );

                context.SaveChanges();

                context.Categories.AddRange(
                   new Categories
                   {
                       CatID = 0,
                       CatName = "總類"
                   },
                    new Categories
                    {
                        CatID = 100,
                        CatName = "哲學類"
                    },
                    new Categories
                    {
                        CatID = 200,
                        CatName = "宗教類"
                    },
                    new Categories
                    {
                        CatID = 300,
                        CatName = "科學類"
                    },
                    new Categories
                    {
                        CatID = 400,
                        CatName = "應用科學類"
                    },
                    new Categories
                    {
                        CatID = 500,
                        CatName = "社會科學類"
                    },
                    new Categories
                    {
                        CatID = 600,
                        CatName = "中國史地類"
                    },
                    new Categories
                    {
                        CatID = 700,
                        CatName = "世界史地類"
                    },
                    new Categories
                    {
                        CatID = 800,
                        CatName = "語文類"
                    },
                    new Categories
                    {
                        CatID = 900,
                        CatName = "藝術類"
                    }
                    );
                context.SaveChanges();

                context.FineTypes.AddRange(
                    new FineTypes
                    {
                        FTID = 1,
                        FTName = "逾期",
                        UnitPrice = 5
                    },
                    new FineTypes
                    {
                        FTID = 2,
                        FTName = "遺失",
                        UnitPrice = 300
                    },
                    new FineTypes
                    {
                        FTID = 3,
                        FTName = "毀損",
                        UnitPrice = 150
                    },
                    new FineTypes
                    {
                        FTID = 4,
                        FTName = "附件遺失",
                        UnitPrice = 50
                    }
                    );
                context.SaveChanges();

                context.Cities.AddRange(
                    new Cities
                    {
                        CityID = 1,
                        CityName = "基隆市"
                    },
                    new Cities
                    {
                        CityID = 2,
                        CityName = "台北市"
                    },
                    new Cities
                    {
                        CityID = 3,
                        CityName = "新北市"
                    },
                    new Cities
                    {
                        CityID = 4,
                        CityName = "桃園市"
                    },
                    new Cities
                    {
                        CityID = 5,
                        CityName = "新竹市"
                    },
                    new Cities
                    {
                        CityID = 6,
                        CityName = "新竹縣"
                    },
                    new Cities
                    {
                        CityID = 7,
                        CityName = "苗栗縣"
                    },
                    new Cities
                    {
                        CityID = 8,
                        CityName = "台中市"
                    },
                    new Cities
                    {
                        CityID = 9,
                        CityName = "彰化縣"
                    },
                    new Cities
                    {
                        CityID = 10,
                        CityName = "南投縣"
                    },
                    new Cities
                    {
                        CityID = 11,
                        CityName = "雲林縣"
                    },
                    new Cities
                    {
                        CityID = 12,
                        CityName = "嘉義市"
                    },
                    new Cities
                    {
                        CityID = 13,
                        CityName = "嘉義縣"
                    },
                    new Cities
                    {
                        CityID = 14,
                        CityName = "台南市"
                    },
                    new Cities
                    {
                        CityID = 15,
                        CityName = "高雄市"
                    },
                    new Cities
                    {
                        CityID = 16,
                        CityName = "屏東縣"
                    },
                    new Cities
                    {
                        CityID = 17,
                        CityName = "台東縣"
                    },
                    new Cities
                    {
                        CityID = 18,
                        CityName = "花蓮縣"
                    },
                    new Cities
                    {
                        CityID = 19,
                        CityName = "宜蘭縣"
                    },
                    new Cities
                    {
                        CityID = 20,
                        CityName = "澎湖縣"
                    },
                    new Cities
                    {
                        CityID = 21,
                        CityName = "金門縣"
                    },
                    new Cities
                    {
                        CityID = 22,
                        CityName = "連江縣"
                    }
                );
                context.SaveChanges();                
            }
        }
    }
}
