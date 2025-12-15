using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Reel
{
    public Guid ReelId { get; set; }

    public Guid UserId { get; set; }

    public string? Caption { get; set; }

    public string VideoUrl { get; set; } = null!;

    public string? ThumbnailUrl { get; set; }

    public int? DurationSeconds { get; set; }

    public string? MusicTrack { get; set; }

    public int? ViewCount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();

    public virtual User User { get; set; } = null!;
}
