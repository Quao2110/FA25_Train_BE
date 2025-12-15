using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Friendship
{
    public Guid RequesterId { get; set; }

    public Guid AddresseeId { get; set; }

    public string Status { get; set; } = null!;

    public Guid ActionUserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User Addressee { get; set; } = null!;

    public virtual User Requester { get; set; } = null!;
}
