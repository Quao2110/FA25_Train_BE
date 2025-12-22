using Application.DTOs.RequestDTOs.ConversationDTO;
using Application.DTOs.ResponseDTOs;
using Application.Interfaces.Repository;
using Application.Interfaces.Service;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces.UnitOfwork;


namespace Application.Services;

public class ConversationService : IConversationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConversationRepository _conversationRepository;
    private readonly IGenericRepository<ConversationParticipant> _participantRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IMapper _mapper;

    public ConversationService(
        IUnitOfWork unitOfWork,
        IConversationRepository conversationRepository,
        IGenericRepository<ConversationParticipant> participantRepository,
        IGenericRepository<User> userRepository,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _conversationRepository = conversationRepository ?? throw new ArgumentNullException(nameof(conversationRepository));
        _participantRepository = participantRepository ?? throw new ArgumentNullException(nameof(participantRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ConversationResponseDTO> CreateConversationAsync(CreateConversationRequestDTO createDto)
    {
        var conversation = new Conversation
        {
            ConversationId = Guid.NewGuid(),
            Name = createDto.Name,
            IsGroup = createDto.IsGroup,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Add participants
        foreach (var userId in createDto.ParticipantIds.Distinct())
        {
            var userExists = await _userRepository.GetByIdAsync(userId) != null;
            if (!userExists)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

            conversation.ConversationParticipants.Add(new ConversationParticipant
            {
                ConversationId = conversation.ConversationId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow
            });
        }

        await _conversationRepository.AddAsync(conversation);
        await _unitOfWork.CommitAsync();

        return ConversationResponseDTO.FromEntity(conversation);
    }

    public async Task<ConversationResponseDTO?> GetConversationByIdAsync(Guid conversationId, Guid userId)
    {
        var isInConversation = await _conversationRepository.IsUserInConversationAsync(conversationId, userId);
        if (!isInConversation)
        {
            return null;
        }

        var conversation = await _conversationRepository.GetByIdWithParticipantsAsync(conversationId);
        return conversation != null ? ConversationResponseDTO.FromEntity(conversation) : null;
    }

    public async Task<IEnumerable<ConversationResponseDTO>> GetUserConversationsAsync(Guid userId)
    {
        var conversations = await _conversationRepository.GetUserConversationsAsync(userId);
        return conversations.Select(ConversationResponseDTO.FromEntity);
    }

    public async Task<ConversationResponseDTO?> UpdateConversationAsync(UpdateConversationRequestDTO updateDto, Guid userId)
    {
        var isInConversation = await _conversationRepository.IsUserInConversationAsync(updateDto.ConversationId, userId);
        if (!isInConversation)
        {
            return null;
        }

        var conversation = await _conversationRepository.GetByIdAsync(updateDto.ConversationId);
        if (conversation == null)
        {
            return null;
        }

        if (updateDto.Name != null)
        {
            conversation.Name = updateDto.Name;
        }
        
        if (updateDto.IsGroup.HasValue)
        {
            conversation.IsGroup = updateDto.IsGroup.Value;
        }

        conversation.UpdatedAt = DateTime.UtcNow;

        _conversationRepository.Update(conversation);
        await _unitOfWork.CommitAsync();

        return await GetConversationByIdAsync(conversation.ConversationId, userId);
    }

    public async Task<bool> DeleteConversationAsync(Guid conversationId, Guid userId)
    {
        var isInConversation = await _conversationRepository.IsUserInConversationAsync(conversationId, userId);
        if (!isInConversation)
        {
            return false;
        }

        var conversation = await _conversationRepository.GetByIdWithParticipantsAsync(conversationId);
        if (conversation == null)
        {
            return false;
        }

        _conversationRepository.Delete(conversation);
        await _unitOfWork.CommitAsync();
        return true;
    }

    public async Task<bool> AddParticipantsToConversationAsync(Guid conversationId, List<Guid> userIds, Guid currentUserId)
    {
        var isInConversation = await _conversationRepository.IsUserInConversationAsync(conversationId, currentUserId);
        if (!isInConversation)
        {
            return false;
        }

        var conversation = await _conversationRepository.GetByIdWithParticipantsAsync(conversationId);
        if (conversation == null)
        {
            return false;
        }

        var existingParticipantIds = conversation.ConversationParticipants
            .Select(cp => cp.UserId)
            .ToHashSet();

        foreach (var userId in userIds.Distinct())
        {
            if (existingParticipantIds.Contains(userId))
            {
                continue;
            }

            var userExists = await _userRepository.GetByIdAsync(userId) != null;
            if (!userExists)
            {
                continue;
            }

            conversation.ConversationParticipants.Add(new ConversationParticipant
            {
                ConversationId = conversationId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow
            });
        }

        conversation.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.CommitAsync();
        return true;
    }

    public async Task<bool> RemoveParticipantFromConversationAsync(Guid conversationId, Guid participantId, Guid currentUserId)
    {
        // Only allow removing if current user is the participant being removed or has admin rights
        if (participantId != currentUserId)
        {
            // Here you might want to add additional checks for admin rights
            return false;
        }

        var participant = await _participantRepository.FindAsync(
            p => p.ConversationId == conversationId && p.UserId == participantId);

        if (participant == null)
        {
            return false;
        }

        _participantRepository.Delete(participant);
        
        // Update conversation's updatedAt
        var conversation = await _conversationRepository.GetByIdAsync(conversationId);
        if (conversation != null)
        {
            conversation.UpdatedAt = DateTime.UtcNow;
            _conversationRepository.Update(conversation);
        }

        await _unitOfWork.CommitAsync();
        return true;
    }
}
