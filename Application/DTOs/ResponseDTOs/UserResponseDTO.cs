namespace Application.DTOs.ResponseDTOs
{
    public class UserResponseDTO
    {

        /// <summary>
        /// UserId
        /// </summary>
        public Guid UserId { get; set; }


        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; } = null!;


        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; } = null!;


        /// <summary>
        /// PhoneNumber
        /// </summary>
        public string? PhoneNumber { get; set; }


        /// <summary>
        /// IsActive
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// LastLogin
        /// </summary>
        public DateTime? LastLogin { get; set; }

    }
}
