using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Reaction
{
    public Guid ReactionId { get; set; }

    public Guid UserId { get; set; }

    public string? Type { get; set; }

    public Guid? PostId { get; set; }

    public Guid? CommentId { get; set; }

    public Guid? ReelId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Comment? Comment { get; set; }

    public virtual Post? Post { get; set; }

    public virtual Reel? Reel { get; set; }

    public virtual User User { get; set; } = null!;
}
