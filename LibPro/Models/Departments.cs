using System.ComponentModel.DataAnnotations;

namespace LibPro.Models
{
    public class Departments
    {
        [Key]
        [RegularExpression("D[0-9][1-9]")]
        public string DeptID { get; set; } = null!;

        [Display(Name ="部門名稱")]
        [StringLength(20,MinimumLength =3,ErrorMessage =("部門名稱需3~20個字元"))]
        [Required(ErrorMessage = "必填欄位")]
        public string DeptName { get; set; } = null!;
    }
}
