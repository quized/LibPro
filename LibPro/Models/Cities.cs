using System.ComponentModel.DataAnnotations;

namespace LibPro.Models
{
    public class Cities
    {
        [Key]
        public byte CityID { get; set; }

        [Display(Name ="縣市名稱")]
        public string CityName { get; set; }= null!;
    }
}
