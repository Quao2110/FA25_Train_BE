using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.RequestDTOs.PostDTO
{
    public class UpdatePostRequestDTO
    {
        /// <summary>
        /// PostId
        /// </summary>
        [Required(ErrorMessage = "Post ID is required")]
        public Guid PostId { get; set; }

        /// <summary>
        /// Content
        /// </summary>
        [StringLength(5000, ErrorMessage = "Content cannot exceed 5000 characters")]
        public string? Content { get; set; }

        /// <summary>
        /// PrivacyLevel (Public, Friends, Private)
        /// </summary> 
        [RegularExpression("^(Public|Friends|Private)$", ErrorMessage = "Privacy level must be Public, Friends, or Private")]
        public string? PrivacyLevel { get; set; }
    }
}
