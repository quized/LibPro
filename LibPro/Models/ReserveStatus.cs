using System.ComponentModel.DataAnnotations;

namespace LibPro.Models
{
    public class ReserveStatus
    {
        [Key]
        public byte StatusCode { get; set; }

        [Display(Name = "狀態名稱")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(20)]
        public string StatusName { get; set; } = null!;

        public List<Reserves>? Reserves { get; set; }
    }
}
