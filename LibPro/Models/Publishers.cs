using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static LibPro.ValidationAttributes.UniqueAttribute;

namespace LibPro.Models
{
    public class Publishers
    {
        [Key]
        public long PubID { get; set; }

        [Display(Name = "出版社")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(50, ErrorMessage = "出版社名稱最多50個字元")]
        [PubNameCheck]
        public string PubName { get; set; } = null!;

        [Display(Name = "地址")]
        [StringLength(100,ErrorMessage ="地址最多100個字元")]
        public string? Address { get; set; }
        
        [Display(Name = "電話")]
        [StringLength(20, ErrorMessage = "電話最多20個字元")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("0[2-9][0-9]{7,8}")]
        public string? Pubtel { get; set; }

        [Display(Name = "縣市")]
        public byte CityID { get; set; }

        [Display(Name = "縣市")]
        [ForeignKey("CityID")]
        public virtual Cities? City { get; set; }

        public virtual List<Biblios>? Biblios { get; set; }
    }
}
