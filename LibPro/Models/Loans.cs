using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibPro.Models
{
    public class Loans
    {
        [Key]
        [RegularExpression("L[0-9]{12}")]
        public string LoanID { get; set; } = null!;

        [Display(Name = "借書日期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd }")]
        public DateTime LoanDate { get; set; }

        [Display(Name = "應還日期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd }")]
        public DateTime DueDate { get; set; }

        [Display(Name = "實際歸還日期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd }")]
        public DateTime? ReturnDate { get; set; }

        [Display(Name = "續借次數")]
        [Required(ErrorMessage = "必填欄位")]
        [Range(0,1,ErrorMessage ="最多只可續借一次")]
        public byte RenewalCount { get; set; } = 0;

        [Display(Name = "備註")]
        [StringLength(250, ErrorMessage = "備註最多接受250個字元")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; } 

        public string PatronID { get; set; } = null!;

        public string ItemID { get; set; } = null!;

        [ForeignKey("PatronID")]
        public virtual Patrons? Patron { get; set; }

        [ForeignKey("ItemID")]
        public virtual BookItems? BookItem { get; set; }

        public virtual List<Fines>? Fines { get; set; }
    }
}
