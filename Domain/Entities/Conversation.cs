using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Conversation
{
    public Guid ConversationId { get; set; }

    public string? Name { get; set; }

    public bool? IsGroup { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ConversationParticipant> ConversationParticipants { get; set; } = new List<ConversationParticipant>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
