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


        public bool IsPaid { get; set; } = false;

        
    }
}
