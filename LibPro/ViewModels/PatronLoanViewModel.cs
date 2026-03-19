namespace LibPro.ViewModels
{
    public class PatronLoanViewModel
    {
        public string LoanID { get; set; } = null!;
        public string BookTitle { get; set; } = null!;
        public string Author { get; set; } = null!;

        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public byte RenewalCount { get; set; }
        public bool CanRenew { get; set; }
        public string RenewMessage { get; set; } = null!;
        public string ButtonClass { get; set; } = null!;
    }
}
