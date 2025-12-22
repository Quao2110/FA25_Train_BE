using Application.DTOs.RequestDTOs.ConversationDTO;
using Application.DTOs.ResponseDTOs;

namespace Application.Interfaces.Service;

public interface IConversationService
{
    Task<ConversationResponseDTO> CreateConversationAsync(CreateConversationRequestDTO createDto);
    Task<ConversationResponseDTO?> GetConversationByIdAsync(Guid conversationId, Guid userId);
    Task<IEnumerable<ConversationResponseDTO>> GetUserConversationsAsync(Guid userId);
    Task<ConversationResponseDTO?> UpdateConversationAsync(UpdateConversationRequestDTO updateDto, Guid userId);
    Task<bool> DeleteConversationAsync(Guid conversationId, Guid userId);
    Task<bool> AddParticipantsToConversationAsync(Guid conversationId, List<Guid> userIds, Guid currentUserId);
    Task<bool> RemoveParticipantFromConversationAsync(Guid conversationId, Guid participantId, Guid currentUserId);
}
