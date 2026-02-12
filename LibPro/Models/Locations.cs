using System.ComponentModel.DataAnnotations;

namespace LibPro.Models
{
    public class Locations
    {
        [Key]
        public byte LocationID { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = ("名稱需2~50個字元"))]
        [Display(Name = "名稱")]
        public string LocationName { get; set; } = null!;

        [Display(Name ="層級深度")]
        public byte Depth { get; set; }

        [Display(Name = "排列序號")]
        public int SortOrder { get; set; }
    }
}
