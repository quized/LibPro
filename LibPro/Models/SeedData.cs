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
                        RoleName = "Patron"
                    },
                    new UserRoles
                    {
                        RoleName = "Staff"
                    },
                    new UserRoles
                    {
                        RoleName = "Admin"
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
                         StatusName = "註銷"
                    },
                    new PatronsStatus
                    {
                        StatusName = "已掛失"
                    },
                    new PatronsStatus
                    {
                        StatusName = "停權中"
                    },
                    new PatronsStatus
                    {
                        StatusName = "正常"
                    }
                );


                context.ReserveStatus.AddRange(
                        new ReserveStatus
                        {
                            StatusName = "取消"
                        },
                        new ReserveStatus
                        {
                            StatusName = "結案"
                        },
                        new ReserveStatus
                        {
                            StatusName = "待取"
                        },
                        new ReserveStatus
                        {
                            StatusName = "排隊中"
                        }
                     );

                context.SystemStatus.AddRange(
                    new SystemStatus
                    {
                        StatusName = "刪除"
                    },
                    new SystemStatus
                    {
                        StatusName = "通過"
                    },
                    new SystemStatus
                    {
                        StatusName = "待審核"
                    }
                    );

                    context.SaveChanges();
                
                    context.Locations.AddRange(
                    new Locations
                    {
                        LocationName = "總館",
                        Depth = 0,
                        SortOrder = 10,
                        ParentID = null
                    },
                    new Locations
                    {
                        LocationName = "一樓",
                        Depth = 1,
                        SortOrder = 10,
                        ParentID = 1
                    },
                    new Locations
                    {
                        LocationName = "二樓",
                        Depth = 1,
                        SortOrder = 20,
                        ParentID = 1
                    }
                    );
                    context.ItemStatus.AddRange(
                       new ItemStatus
                        {
                           StatusName = "註銷"
                       },
                        new ItemStatus
                        {
                            StatusName = "整理"
                        },
                        new ItemStatus
                        {
                            StatusName = "遺失"
                        },
                        new ItemStatus
                        {
                            StatusName = "預約保留"
                        },
                        new ItemStatus
                        {
                            StatusName = "借出"
                        },
                        new ItemStatus
                        {
                            StatusName = "在架"
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
                        FTName = "附件遺失",
                        UnitPrice = 50
                    },
                      new FineTypes
                      {
                          FTName = "毀損",
                          UnitPrice = 150
                      },
                      new FineTypes
                      {
                          FTName = "遺失",
                          UnitPrice = 300
                      },
                      new FineTypes
                      {
                          FTName = "逾期",
                          UnitPrice = 5
                      }
                   );
                    context.SaveChanges();
               


                    context.Cities.AddRange(
                        new Cities
                        {
                            CityName = "連江縣"
                        },
                        new Cities
                        {
                            CityName = "金門縣"
                        },
                        new Cities
                        {
                            CityName = "澎湖縣"
                        },
                        new Cities
                        {
                            CityName = "宜蘭縣"
                        },
                        new Cities
                        {
                            CityName = "花蓮縣"
                        },
                        new Cities
                        {
                            CityName = "台東縣"
                        },
                        new Cities
                        {
                            CityName = "屏東縣"
                        },
                        new Cities
                        {
                            CityName = "高雄市"
                        },
                        new Cities
                        {
                            CityName = "台南市"
                        },
                        new Cities
                        {
                            CityName = "嘉義縣"
                        },
                        new Cities
                        {
                            CityName = "嘉義市"
                        },
                        new Cities
                        {
                            CityName = "雲林縣"
                        },
                        new Cities
                        {
                            CityName = "南投縣"
                        },
                        new Cities
                        {
                            CityName = "彰化縣"
                        },
                        new Cities
                        {
                            CityName = "台中市"
                        },
                        new Cities
                        {
                            CityName = "苗栗縣"
                        },
                        new Cities
                        {
                            CityName = "新竹縣"
                        },
                        new Cities
                        {
                            CityName = "新竹市"
                        },
                        new Cities
                        {
                            CityName = "桃園市"
                        },
                        new Cities
                        {
                            CityName = "新北市"
                        },
                        new Cities
                        {
                            CityName = "台北市"
                        },
                        new Cities
                        {
                            CityName = "基隆市"
                        }
                    );
                    context.SaveChanges();
                
            }
        }
    }
}
