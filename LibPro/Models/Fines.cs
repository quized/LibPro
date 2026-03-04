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

        [Display(Name = "罰金類型")]
        public byte FTID { get; set; }

        [Display(Name = "借閱紀錄")]
        public string LoanID { get; set; } = null!;

        [ForeignKey("FTID")]
        [Display(Name = "罰金類型")]
        public virtual FineTypes? FineType { get; set; }

        [ForeignKey("LoanID")]
        [Display(Name = "借閱紀錄")]
        public virtual Loans? Loan { get; set; }
    }
}
