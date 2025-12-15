using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Post
{
    public Guid PostId { get; set; }

    public Guid UserId { get; set; }

    public string? Content { get; set; }

    public string? PrivacyLevel { get; set; }

    public Guid? OriginalPostId { get; set; }

    public int? LikeCount { get; set; }

    public int? CommentCount { get; set; }

    public int? ShareCount { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Post> InverseOriginalPost { get; set; } = new List<Post>();

    public virtual Post? OriginalPost { get; set; }

    public virtual ICollection<PostMedium> PostMedia { get; set; } = new List<PostMedium>();

    public virtual ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();

    public virtual User User { get; set; } = null!;
}
