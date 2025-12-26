using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.RequestDTOs.PostDTO
{
    public class CreatePostRequestDTO
    {
        /// <summary>
        /// UserId
        /// </summary>
        [Required(ErrorMessage = "User ID is required")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Content
        /// </summary>
        [StringLength(5000, ErrorMessage = "Content cannot exceed 5000 characters")]
        public string? Content { get; set; }

        /// <summary>
        /// PrivacyLevel (Public, Friends, Private)
        /// </summary>
        [Required(ErrorMessage = "Privacy level is required")]
        [RegularExpression("^(Public|Friends|Private)$", ErrorMessage = "Privacy level must be Public, Friends, or Private")]
        public string? PrivacyLevel { get; set; }
    }
}
