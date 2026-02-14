using Microsoft.EntityFrameworkCore;

namespace LibPro.Models
{
    public class LibproContext : DbContext
    {
        public LibproContext(DbContextOptions<LibproContext> options) : base(options)
        {
        }
        public virtual DbSet<Announcements> Announcements { get; set; }
        public virtual DbSet<Biblios> Biblios { get; set; }
        public virtual DbSet<BookItems> BookItems { get; set; }
        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<Cities> Cities { get; set; }
        public virtual DbSet<Departments> Departments { get; set; }
        public virtual DbSet<Fines> Fines { get; set; }
        public virtual DbSet<FineTypes> FineTypes { get; set; }
        public virtual DbSet<ItemStatus> ItemStatus { get; set; }
        public virtual DbSet<Loans> Loans { get; set; }
        public virtual DbSet<Locations> Locations { get; set; }
        public virtual DbSet<Patrons> Patrons { get; set; }
        public virtual DbSet<PatronsStatus> PatronsStatus { get; set; }
        public virtual DbSet<Publishers> Publishers { get; set; }      
        public virtual DbSet<Reserves> Reserves { get; set; }
        public virtual DbSet<ReserveStatus> ReserveStatus { get; set; }
        public virtual DbSet<Reviews> Reviews { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        public virtual DbSet<SystemStatus> SystemStatus { get; set; }
        public virtual DbSet<UserAccounts> UserAccounts { get; set; }
        public virtual DbSet<UserRoles> UserRoles { get; set; }
    }
}
