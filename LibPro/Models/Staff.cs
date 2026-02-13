using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
        public byte Gender { get; set; }

        [Display(Name = "電子郵件")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "請輸入正確的電子郵件格式")]
        [StringLength(100, ErrorMessage = ("電子郵件最多只接受100個字元"))]
        public string? Email { get; set; }

        [Display(Name = "電話")]
        [RegularExpression("0[2-9][0-9]{7,8}")]
        [Phone]
        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "必填欄位")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "地址")]
        [StringLength(100, ErrorMessage = "地址最多只可填100個字元")]
        public string Address { get; set; } = null!;

        [Display(Name = "郵遞區號")]
        [StringLength(6,MinimumLength =3,ErrorMessage = "需填3碼或6碼郵遞區號")]
        [RegularExpression("[1-9][0-9]{2}|[1-9][0-9]{5}")]
        public string? ZipCode { get; set; }

        public string UserID { get; set; } = null!;

        public byte CityID { get; set; }

        public string DeptID { get; set; } = null!;

        public virtual UserAccounts? UserAccount { get; set; }

        public virtual Cities? City { get; set; }

        public virtual Departments? Department { get; set; }

        public virtual List<Announcements>? Announcements { get; set; }

    }
}
