using Domain.Entities;

namespace Application.Interfaces.Repository;

public interface IConversationRepository : IGenericRepository<Conversation>
{
    Task<Conversation?> GetByIdWithParticipantsAsync(Guid conversationId);
    Task<IEnumerable<Conversation>> GetUserConversationsAsync(Guid userId);
    Task<bool> IsUserInConversationAsync(Guid conversationId, Guid userId);
    Task AddAsync(Conversation conversation);
}
