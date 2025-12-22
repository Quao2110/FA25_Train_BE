namespace Application.DTOs.RequestDTOs.ConversationDTO;

public class UpdateConversationRequestDTO
{
    public Guid ConversationId { get; set; }
    public string? Name { get; set; }
    public bool? IsGroup { get; set; }
}
