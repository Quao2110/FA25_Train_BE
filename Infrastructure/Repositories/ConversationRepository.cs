using System.Data;
using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ConversationRepository : GenericRepository<Conversation>, IConversationRepository
{
    private readonly FakebookContext _context;

    public ConversationRepository(FakebookContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Conversation?> GetByIdWithParticipantsAsync(Guid conversationId)
    {
        return await _context.Conversations
            .Include(c => c.ConversationParticipants)
            .FirstOrDefaultAsync(c => c.ConversationId == conversationId);
    }

    public async Task<IEnumerable<Conversation>> GetUserConversationsAsync(Guid userId)
    {
        return await _context.ConversationParticipants
            .Where(cp => cp.UserId == userId)
            .Include(cp => cp.Conversation)
                .ThenInclude(c => c!.ConversationParticipants)
            .Select(cp => cp.Conversation!)
            .ToListAsync();
    }

    public async Task<bool> IsUserInConversationAsync(Guid conversationId, Guid userId)
    {
        return await _context.ConversationParticipants
            .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);
    }

    public async Task AddAsync(Conversation conversation)
    {
        await _context.Conversations.AddAsync(conversation);
    }
}
