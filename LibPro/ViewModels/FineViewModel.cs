using System.ComponentModel.DataAnnotations;

namespace LibPro.ViewModels
{
    public class FineViewModel
    {
        
        public string FineID { get; set; } 
        public DateTime CreatedDate { get; set; }
        public bool ISPaid { get; set; }
        public string FTName { get; set; }
        public string PatronID { get; set; }
        public string PatronName { get; set; }

        
        public int OverdueDays { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
