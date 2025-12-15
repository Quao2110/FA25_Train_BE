using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Notification
{
    public Guid NotificationId { get; set; }

    public Guid UserId { get; set; }

    public Guid ActorId { get; set; }

    public string Type { get; set; } = null!;

    public Guid? ReferenceId { get; set; }

    public string? ReferenceType { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User Actor { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
