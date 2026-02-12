using System.ComponentModel.DataAnnotations;

namespace LibPro.Models
{
    public class PatronsStatus
    {
        [Key]
        public byte StatusCode { get; set; }

        [Display(Name = "狀態名稱")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(10)]
        public string StatusName { get; set; } = null!;

        public List<Patrons>? Patrons { get; set; }
    }
}
