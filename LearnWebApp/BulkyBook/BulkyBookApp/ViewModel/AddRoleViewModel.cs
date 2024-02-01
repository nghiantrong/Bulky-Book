using System.ComponentModel.DataAnnotations;

namespace BulkyBookApp.ViewModel
{
    public class AddRoleViewModel
    {
        [Required]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
}
