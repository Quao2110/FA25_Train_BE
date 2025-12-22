namespace Application.DTOs.RequestDTOs.ConversationDTO;

public class CreateConversationRequestDTO
{
    public string? Name { get; set; }
    public bool IsGroup { get; set; }
    public List<Guid> ParticipantIds { get; set; } = new();
}
