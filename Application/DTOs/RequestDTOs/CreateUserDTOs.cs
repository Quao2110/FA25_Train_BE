using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.RequestDTOs
{
    public class CreateUserDTOs
    {
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? Avatar { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
