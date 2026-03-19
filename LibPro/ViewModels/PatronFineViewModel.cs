namespace LibPro.ViewModels
{
    public class PatronFineViewModel
    {
        public string FineID { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string BookTitle { get; set; } = null!;
        public string FineTypeName { get; set; } = null!;
        public int OverdueDays { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
    }
}
