using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ResponseDTOs;

public class PostResponseDTO
{
    /// <summary>
    /// PostId
    /// </summary>
    public Guid PostId { get; set; }

    /// <summary>
    /// UserId
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Content
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// PrivacyLevel (Public, Friends, Private)
    /// </summary>
    public string PrivacyLevel { get; set; } = null!;

    /// <summary>
    /// CreatedAt
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// UpdatedAt
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}