using System.ComponentModel.DataAnnotations;

namespace LibPro.Models
{
    public class Categories
    {
        [Key]
        public int CatID { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name ="類別名稱")]
        [StringLength(20,ErrorMessage ="類別名稱最多20個字元")]
        public string CatName { get; set; } = null!;
    }
}
