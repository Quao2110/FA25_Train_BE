using Domain.Entities;

namespace Application.DTOs.ResponseDTOs;

public class ConversationResponseDTO
{
    public Guid ConversationId { get; set; }
    public string? Name { get; set; }
    public bool IsGroup { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<Guid> ParticipantIds { get; set; } = new List<Guid>();

    public static ConversationResponseDTO FromEntity(Conversation conversation)
    {
        return new ConversationResponseDTO
        {
            ConversationId = conversation.ConversationId,
            Name = conversation.Name,
            IsGroup = conversation.IsGroup ?? false,
            CreatedAt = conversation.CreatedAt,
            UpdatedAt = conversation.UpdatedAt,
            ParticipantIds = conversation.ConversationParticipants
                .Select(cp => cp.UserId)
                .ToList()
        };
    }
}
