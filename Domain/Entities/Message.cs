using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Message
{
    public Guid MessageId { get; set; }

    public Guid ConversationId { get; set; }

    public Guid SenderId { get; set; }

    public string? Content { get; set; }

    public string? MediaUrl { get; set; }

    public bool? IsRead { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
