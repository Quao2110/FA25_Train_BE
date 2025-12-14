namespace Application.DTOs.RequestDTOs
{
    public class UpdateUserDTO
    {
        public Guid UserId { get; set; }

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? Avatar { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
