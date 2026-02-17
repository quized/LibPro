using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibPro.Models
{
    public class Reviews
    {
        [Key]
        public long ReviewID { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = ("標題需2~50個字元"))]
        [Display(Name = "標題")]
        public string Title { get; set; } = null!;

        [Display(Name = "內容")]
        [StringLength(500, ErrorMessage = "備註最多接受500個字元")]
        [DataType(DataType.MultilineText)]
        public string? Content { get; set; }

        [Display(Name = "發布日期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm:ss}")]
        public DateTime PostDate { get; set; }

        [Display(Name = "評分")]
        [Range(1, 5, ErrorMessage = "評分必須介於1到5之間")]
        [Required(ErrorMessage = "必填欄位")]
        public byte Rating { get; set; }

        public byte RevStatus { get; set; }

        public string PatronID { get; set; } = null!;

        public long BibID { get; set; }

        [ForeignKey("RevStatus")]
        public virtual SystemStatus? SystemStatus { get; set; }

        [ForeignKey("PatronID")]
        public virtual Patrons? Patron { get; set; }

        [ForeignKey("BibID")]
        public virtual Biblios? Biblio { get; set; }

    }
}
