using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibPro.Models
{
    public class BookItems
    {
        [Key]
        [Display(Name = "館藏編號")]
        public string ItemID { get; set; } = null!;

        [Display(Name = "購入日期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime PurDate { get; set; }

        [Display(Name = "備註")]
        [StringLength(200, ErrorMessage = "備註最多接受200個字元")]
        [DataType(DataType.MultilineText)]
        public string? Remarks { get; set; }

        [Display(Name = "館藏狀態")]
        public byte ItmStatus { get; set; }

        [Display(Name = "書名")]
        public long BibID { get; set; }

        [Display(Name = "放置位置")]
        public int LocID { get; set; }

        [ForeignKey("ItmStatus")]
        [Display(Name = "館藏狀態")]
        public virtual ItemStatus? ItemStatus { get; set; }

        [ForeignKey("BibID")]
        [Display(Name = "書名")]
        public virtual Biblios? Biblio { get; set; }

        [ForeignKey("LocID")]
        [Display(Name = "放置位置")]
        public virtual Locations? Location { get; set; }

        public virtual List<Loans>? Loans { get; set; }
         
        public virtual List<Reserves>? Reserves { get; set; }
    }
}
