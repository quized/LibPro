using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibPro.Models
{
    public class Staff
    {
        [Key]        
        [RegularExpression("S[0-9]{7}[1-9]")]
        public string StaffID { get; set; } = null!;

        [Required(ErrorMessage = "必填欄位")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = ("姓名需2~40個字元"))]
        [Display(Name = "姓名")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "必填欄位")]
        [StringLength(20, ErrorMessage = ("學歷最多只接受20個字元"))]
        [Display(Name = "學歷")]
        public string Education { get; set; } = null!;

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "生日")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime Birthday { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "性別")]
        [UIHint("GenderFormat")]
        public byte Gender { get; set; }

        [Display(Name = "電子郵件")]
        [EmailAddress(ErrorMessage = "請輸入正確的電子郵件格式")]
        [StringLength(100, ErrorMessage = ("電子郵件最多只接受100個字元"))]
        public string? Email { get; set; }

        [Display(Name = "電話")]
        [RegularExpression("0[2-9][0-9]{7,8}")]
        [Phone]
        [Required(ErrorMessage = "必填欄位")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "地址")]
        [StringLength(100, ErrorMessage = "地址最多只可填100個字元")]
        public string Address { get; set; } = null!;

        [Display(Name = "郵遞區號")]
        [StringLength(6,ErrorMessage = "需填6碼郵遞區號")]
        [RegularExpression("[1-9][0-9]{5}")]
        public string? ZipCode { get; set; }

        [Display(Name = "是否離職")]
        public bool IsResigned { get; set; } = false;

        [Display(Name = "登入ID")]
        public string? UserID { get; set; }

        [Display(Name = "縣市")]
        public byte CityID { get; set; }

        [Display(Name = "部門")]
        public string DeptID { get; set; } = null!;

        [ForeignKey("UserID")]
        [Display(Name = "登入ID")]
        public virtual UserAccounts? UserAccount { get; set; }

        [ForeignKey("CityID")]
        [Display(Name = "縣市")]
        public virtual Cities? City { get; set; }

        [ForeignKey("DeptID")]
        [Display(Name = "部門")]
        public virtual Departments? Department { get; set; }

        public virtual List<Announcements>? Announcements { get; set; }

    }
}
