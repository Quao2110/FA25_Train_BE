using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class PostMedium
{
    public Guid MediaId { get; set; }

    public Guid PostId { get; set; }

    public string MediaUrl { get; set; } = null!;

    public string? MediaType { get; set; }

    public int? DisplayOrder { get; set; }

    public virtual Post Post { get; set; } = null!;
}
