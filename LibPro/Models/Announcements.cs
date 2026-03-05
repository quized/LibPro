using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibPro.Models
{
    public class Announcements
    {
        [Key]
        [RegularExpression("A[0-9]{11}")]
        public string AnnID { get; set; } = null!;

        [Display(Name = "標題")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "標題限2~100個字元")]
        public string Title { get; set; } = null!;

        [Display(Name = "內容")]
        [Required(ErrorMessage = "必填欄位")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; } = null!;

        [Display(Name = "建立時間")]
        [HiddenInput]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm:ss}")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string Creator { get; set; } = null!;

        [Display(Name = "是否顯示")]
        public bool IsVisible { get; set; }

        [ForeignKey("Creator")]
        public virtual Staff? Staff { get; set; }

    }
}
