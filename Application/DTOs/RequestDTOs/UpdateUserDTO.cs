namespace Application.DTOs.RequestDTOs
{
    public class UpdateUserDTO
    {
        public Guid UserId { get; set; }

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
