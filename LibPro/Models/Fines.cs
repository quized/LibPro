using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LibPro.Models
{
    public class Fines
    {
        [Key]
        [RegularExpression("F[0-9]{11}")]
        public string FineID { get; set; } = null!;

        [Display(Name = "產生日期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm:ss}")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "付款與否")]
        public bool ISPaid { get; set; } = false;

        public string PatronID { get; set; } = null!;

        public byte FTID { get; set; }

        public string LoanID { get; set; } = null!;

        public virtual Patrons? Patron { get; set; }

        public virtual FineTypes? FineType { get; set; }

        public virtual Loans? Loan { get; set; }
    }
}
