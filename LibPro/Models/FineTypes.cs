using System.ComponentModel.DataAnnotations;

namespace LibPro.Models
{
    public class FineTypes
    {
        [Key]
        public byte FTID { get; set; }

        [Display(Name = "罰款類型")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(20, ErrorMessage = "罰款類型最多20個字元")]
        public string FTName { get; set; } = null!;

        [Display(Name ="金額")]
        [Required(ErrorMessage = "必填欄位")]
        public decimal UnitPrice { get; set; } 

        public virtual List<Fines>? Fines { get; set; }
    }
}
