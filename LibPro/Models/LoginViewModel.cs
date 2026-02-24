using System.ComponentModel.DataAnnotations;

namespace LibPro.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "請輸入登入帳號")]
        [Display(Name = "登入帳號")]
        public string Account { get; set; } = null!;

        [Required(ErrorMessage = "請輸入密碼")]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }= null!;
    }
}
