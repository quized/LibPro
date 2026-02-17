using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibPro.Models
{
    public class Biblios
    {
        [Key]
        public long BibID { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        [StringLength(200, ErrorMessage = "書名最多200個字元")]
        [Display(Name = "書名")]
        public string BTitle { get; set; } = null!;

        [Display(Name = "國際標準書號(ISBN)")]     
        [RegularExpression("97[89][0-9]{10}|[0-9]{9}[0-9xX]")]
        public string? ISBN { get; set; }

        [Display(Name ="作者")]
        [StringLength(100,ErrorMessage ="作者最多接受100字元")]
        public string? Author { get; set; }

        [Display(Name = "內容簡介")]
        [StringLength(500, ErrorMessage = "內容簡介最多接受100字元")]
        [DataType(DataType.MultilineText)]
        public string? Summary { get; set; }

        [Display(Name = "出版日期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? PubDate { get; set; }

        [Display(Name = "圖片")]
        [StringLength(40, ErrorMessage = "圖片最多接受40字元")]
        public string? ImgPath { get; set; }

        [HiddenInput]
        public byte isDeleted { get; set; } = 0;

        [Display(Name = "書類")]
        [ForeignKey("CatID")]
        public int CatID { get; set; }

        [Display(Name = "出版社")]      
        [ForeignKey("PubID")]
        public long? PubID { get; set; }

        [Display(Name = "書類")]
        public virtual Categories? Category { get; set; }

        [Display(Name = "出版社")]
        public virtual Publishers? Publisher { get; set; }

        public virtual List<Reviews>? Reviews { get; set; }

        public virtual List<BookItems>? BookItems { get; set; }

    }
}
