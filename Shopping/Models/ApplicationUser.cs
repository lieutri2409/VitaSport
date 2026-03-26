using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [MaxLength(15)]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumberCustom { get; set; }
    }
}