using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public byte FTID { get; set; }

        public string LoanID { get; set; } = null!;

        [ForeignKey("FTID")]
        public virtual FineTypes? FineType { get; set; }

        [ForeignKey("LoanID")]
        public virtual Loans? Loan { get; set; }
    }
}
