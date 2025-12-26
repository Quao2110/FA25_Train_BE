using Application.DTOs.RequestDTOs.PostDTO;
using Application.DTOs.ResponseDTOs;

namespace Application.Interfaces.Service
{
    public interface IPostService
    {
        Task<IEnumerable<PostResponseDTO>> GetAllPostsAsync();
        Task<PostResponseDTO?> GetPostByIdAsync(Guid id);
        Task CreatePostAsync(CreatePostRequestDTO requestDTO);
        Task UpdatePostAsync(UpdatePostRequestDTO requestDTO);
        Task DeletePostAsync(Guid id);
    }
}
