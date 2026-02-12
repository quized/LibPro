using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LibPro.Models
{
    public class Reserves
    {
        [Key]
        [RegularExpression("R[0-9]{11}")]
        public string ResID { get; set; } = null!;

        [Display(Name = "預約時間")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm:ss}")]
        public DateTime ResDate { get; set; } = DateTime.Now;

        [Display(Name = "預定取書日期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime ExpiryDate { get; set; }

        [Display(Name = "備註")]
        [StringLength(200, ErrorMessage = "備註最多接受200個字元")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }
    }
}
