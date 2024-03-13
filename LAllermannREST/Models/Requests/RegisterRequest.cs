using System.ComponentModel.DataAnnotations;

namespace LAllermannREST.Models.Requests
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm password is required.")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Owner key is required.")]
        public string OwnerKey { get; set; }
    }
}
