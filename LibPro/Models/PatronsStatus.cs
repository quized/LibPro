using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibPro.Models
{
    public class PatronsStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte StatusCode { get; set; }

        [Display(Name = "狀態名稱")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(10)]
        public string StatusName { get; set; } = null!;

        public virtual List<Patrons>? Patrons { get; set; }
    }
}
