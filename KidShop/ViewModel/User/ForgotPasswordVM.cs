using System.ComponentModel.DataAnnotations;

namespace KidShop.ViewModel.User
{
    public class ForgotPasswordVM
    {
        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; }
    }
}
