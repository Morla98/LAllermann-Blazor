using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LAllermannREST.Models
{
    public class Password
    {
        [Key]
        public long Id { get; set; }
        [ForeignKey("User")]
        public long UserId  { get; set; }
        public string? Title { get; set; }
        public string? Username { get; set; }
        public string? UserPassword { get; set; }
        public string? URL { get; set; }
        public string? Notes { get; set; }
    }
}
