using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibPro.Models
{
    public class BookItems
    {
        [Key]
        [RegularExpression("[0-9]{10}")]
        public string ItemID { get; set; } = null!;

        [Display(Name = "購入日期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime PurDate { get; set; }

        [Display(Name = "備註")]
        [StringLength(200, ErrorMessage = "備註最多接受200個字元")]
        [DataType(DataType.MultilineText)]
        public string? Remarks { get; set; }

        public byte ItmStatus { get; set; }

        public long BibID { get; set; }

        public byte LocID { get; set; }

        public virtual ItemStatus? ItemStatus { get; set; }

        public virtual Biblios? Biblio { get; set; }

        [ForeignKey("LocID")]
        public virtual Locations? Location { get; set; }

        public virtual List<Loans>? Loans { get; set; }
         
        public virtual List<Reserves>? Reserves { get; set; }
    }
}
