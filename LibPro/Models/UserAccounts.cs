using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibPro.Models
{
    public class UserAccounts
    {
        [Key]
        [RegularExpression("L[PS][0-9]{6}[1-9]")]        
        public string UserID { get; set; } = null!;

        [Required(ErrorMessage ="必填欄位")]
        [StringLength(30,MinimumLength =8,ErrorMessage =("帳號需8~30個字元"))]
        [Display(Name ="帳號")]
        public string Account { get; set; } = null!;

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name ="密碼")]
        [StringLength(50,MinimumLength =8,ErrorMessage ="密碼需8~50個字元")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name ="最後登入時間")]
        [HiddenInput]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm:ss}")]
        public DateTime? LastLoginTime { get; set; }

        [Display(Name = "建立時間")]
        [HiddenInput]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm:ss}")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        
        public byte UserType { get; set; }

        [ForeignKey("UserType")]
        public virtual UserRoles? UserRole { get; set; }

    }
}
