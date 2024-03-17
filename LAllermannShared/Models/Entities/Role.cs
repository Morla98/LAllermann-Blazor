using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAllermannShared.Models.Entities
{
	public class Role
	{
		[Key]
		public long Id { get; set; }
		[Required(ErrorMessage = "Role is required.")]
		public string RoleName { get; set; } = "";
	}

	
}
