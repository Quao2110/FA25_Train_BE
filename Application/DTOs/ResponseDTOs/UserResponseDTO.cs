using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ResponseDTOs
{
    public class UserResponseDTO
    {
        public Guid UserId { get; set; }

        public string Username { get; set; } = null!;

        public string? Avatar { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
