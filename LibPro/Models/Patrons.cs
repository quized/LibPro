using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibPro.Models
{
    public class Patrons
    {
        [Key]
        [RegularExpression("P[0-9]{7}[1-9]")]
        public string PatronID { get; set; } = null!;


        [StringLength(40, MinimumLength = 2, ErrorMessage = ("姓名需2~40個字元"))]
        [Display(Name = "姓名")]
        [Required(ErrorMessage = "必填欄位")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "必填欄位")]
        [StringLength(20, ErrorMessage = ("學歷最多只接受20個字元"))]
        [Display(Name = "學歷")]
        public string Education { get; set; } = null!;

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "生日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.DateTime)]
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

        [Display(Name = "職業")]
        [StringLength(20, ErrorMessage = ("職業最多只接受20個字元"))]
        public string? Profession { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "行政區")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "行政區需填2~10個字元")]
        public string District { get; set; } = null!;

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "詳細地址")]
        [StringLength(100, ErrorMessage = "詳細地址最多接受100個字元")]
        public string Address { get; set; } = null!;

        [Display(Name = "郵遞區號")]
        [StringLength(6, MinimumLength = 3, ErrorMessage = "需填3碼或6碼郵遞區號")]
        [RegularExpression("[1-9][0-9]{2}|[1-9][0-9]{5}")]
        public string? ZipCode { get; set; }

        [Display(Name = "備註")]
        [StringLength(100, ErrorMessage = "備註最多接受100個字元")]
        [DataType(DataType.MultilineText)]
        public string? Memo { get; set; }

        public string UserID { get; set; } = null!;

        public byte CityID { get; set; }

        public byte PtrStatus { get; set; }

        [ForeignKey("UserID")]
        public virtual UserAccounts? UserAccount { get; set; }

        [ForeignKey("CityID")]
        public virtual Cities? City { get; set; }

        [ForeignKey("PtrStatus")]
        public virtual PatronsStatus? PatronsStatus { get; set; }      

        public virtual List<Loans>? Loans { get; set; }

        public virtual List<Reserves>? Reserves { get; set; }

        public virtual List<Reviews>? Reviews { get; set; }
    }
}
