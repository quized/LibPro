using System.ComponentModel.DataAnnotations;

namespace LibPro.Models
{
    public class BookItems
    {
        [Key]
        [RegularExpression("[0-9]{10}")]
        public string ItemID { get; set; } = null!;

        [Display(Name = "購入日期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime PurDate { get; set; }

        [Display(Name = "備註")]
        [StringLength(200, ErrorMessage = "備註最多接受200個字元")]
        [DataType(DataType.MultilineText)]
        public string? Remarks { get; set; }
    }
}
