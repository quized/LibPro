using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibPro.Models
{
    public class UserRoles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [HiddenInput]
        public byte RoleID { get; set; }

        [Display(Name ="權限名稱")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(20)]
        public string RoleName { get; set; } = null!;

        public List<UserAccounts>? UserAccounts { get; set; } 

    }
}
