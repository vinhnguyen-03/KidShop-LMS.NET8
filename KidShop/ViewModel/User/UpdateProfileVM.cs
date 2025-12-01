using System.ComponentModel.DataAnnotations;

namespace KidShop.ViewModel.User
{
    public class UpdateProfileVM
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}
