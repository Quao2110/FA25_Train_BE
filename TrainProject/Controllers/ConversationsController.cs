using Application.DTOs.RequestDTOs.ConversationDTO;
using Application.DTOs.ResponseDTOs;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TrainProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ConversationsController : ControllerBase
    {
        private readonly IConversationService _conversationService;
        private readonly ILogger<ConversationsController> _logger;

        public ConversationsController(
            IConversationService conversationService,
            ILogger<ConversationsController> logger)
        {
            _conversationService = conversationService ?? throw new ArgumentNullException(nameof(conversationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID claim not found in the token");
                throw new UnauthorizedAccessException("User ID claim not found in the token");
            }
            return Guid.Parse(userId);
        }

        [HttpPost]
        public async Task<IActionResult> CreateConversation([FromBody] CreateConversationRequestDTO createDto)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                
                // Add current user to participants if not already included
                if (!createDto.ParticipantIds.Contains(currentUserId))
                {
                    createDto.ParticipantIds.Add(currentUserId);
                }

                var result = await _conversationService.CreateConversationAsync(createDto);
                return CreatedAtAction(nameof(GetConversation), new { id = result.ConversationId }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetConversation(Guid id)
        {
            var currentUserId = GetCurrentUserId();
            var conversation = await _conversationService.GetConversationByIdAsync(id, currentUserId);
            
            if (conversation == null)
            {
                return NotFound("Conversation not found or access denied");
            }

            return Ok(conversation);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserConversations()
        {
            var currentUserId = GetCurrentUserId();
            var conversations = await _conversationService.GetUserConversationsAsync(currentUserId);
            return Ok(conversations);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateConversation(Guid id, [FromBody] UpdateConversationRequestDTO updateDto)
        {
            if (id != updateDto.ConversationId)
            {
                return BadRequest("Conversation ID mismatch");
            }

            var currentUserId = GetCurrentUserId();
            var result = await _conversationService.UpdateConversationAsync(updateDto, currentUserId);
            
            if (result == null)
            {
                return NotFound("Conversation not found or access denied");
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConversation(Guid id)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _conversationService.DeleteConversationAsync(id, currentUserId);
            
            if (!result)
            {
                return NotFound("Conversation not found or access denied");
            }

            return NoContent();
        }

        [HttpPost("{conversationId}/participants")]
        public async Task<IActionResult> AddParticipants(Guid conversationId, [FromBody] List<Guid> participantIds)
        {
            if (participantIds == null || !participantIds.Any())
            {
                return BadRequest("At least one participant ID is required");
            }

            var currentUserId = GetCurrentUserId();
            var result = await _conversationService.AddParticipantsToConversationAsync(
                conversationId, participantIds, currentUserId);
            
            if (!result)
            {
                return NotFound("Conversation not found or access denied");
            }

            return NoContent();
        }

        [HttpDelete("{conversationId}/participants/{participantId}")]
        public async Task<IActionResult> RemoveParticipant(Guid conversationId, Guid participantId)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _conversationService.RemoveParticipantFromConversationAsync(
                conversationId, participantId, currentUserId);
            
            if (!result)
            {
                return NotFound("Conversation/participant not found or access denied");
            }

            return NoContent();
        }
    }
}
