using System.ComponentModel.DataAnnotations;

namespace LAllermannREST.Models
{
    public class User
    {
        [Key]
        public long Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
        public string? APIKEY { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
