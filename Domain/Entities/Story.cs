using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Story
{
    public Guid StoryId { get; set; }

    public Guid UserId { get; set; }

    public string MediaUrl { get; set; } = null!;

    public string? MediaType { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
