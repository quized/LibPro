using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibPro.Models
{
    public class FineTypes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte FTID { get; set; }

        [Display(Name = "罰款類型")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(20, ErrorMessage = "罰款類型最多20個字元")]
        public string FTName { get; set; } = null!;

        [Display(Name ="金額")]
        [Required(ErrorMessage = "必填欄位")]
        [Precision(8, 0)]
        public decimal UnitPrice { get; set; } 

        public virtual List<Fines>? Fines { get; set; }
    }
}
