using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Comment
{
    public Guid CommentId { get; set; }

    public Guid? PostId { get; set; }

    public Guid? ReelId { get; set; }

    public Guid UserId { get; set; }

    public string? Content { get; set; }

    public Guid? ParentCommentId { get; set; }

    public string? MediaUrl { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Comment> InverseParentComment { get; set; } = new List<Comment>();

    public virtual Comment? ParentComment { get; set; }

    public virtual Post? Post { get; set; }

    public virtual ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();

    public virtual Reel? Reel { get; set; }

    public virtual User User { get; set; } = null!;
}
