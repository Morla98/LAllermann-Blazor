using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LAllermannShared.Models.Entities
{
    public class User
    {
        [Key]
        public long Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = "";
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = "";
        [Required(ErrorMessage = "APIKEY is required.")]
        public string APIKEY { get; set; } = "";
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }

		
		public long RoleId { get; set; }
		[ForeignKey("RoleId")]
		public virtual Role Role { get; set; }
    }
}
