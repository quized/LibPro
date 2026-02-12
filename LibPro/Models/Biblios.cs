using System.ComponentModel.DataAnnotations;

namespace LibPro.Models
{
    public class Biblios
    {
        [Key]
        public long BibID { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        [StringLength(200, ErrorMessage = ("書名最多200個字元"))]
        [Display(Name = "書名")]
        public string BTitle { get; set; } = null!;

        [Display(Name = "國際標準書號(ISBN)")]     
        [RegularExpression("97[89][0-9]{10} | [0-9]{9}[0-9xX]")]
        public string? ISBN { get; set; }

        public string? Author { get; set; } 

        public string? Summary { get; set; }

        public DateTime? PubDate { get; set; }

        public string? ImgPath { get; set; }



    }
}
