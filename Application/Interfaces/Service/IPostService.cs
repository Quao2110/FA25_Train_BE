using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.RequestDTOs.PostDTO;
using Domain.Entities;

namespace Application.Interfaces.Service
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> GetAllPostsAsync();
        Task<Post?> GetPostByIdAsync(Guid id);
        Task CreatePostAsync(CreatePostRequestDTO requestDTO);
        Task UpdatePostAsync(Guid id, UpdatePostRequestDTO requestDTO);
        Task DeletePostAsync(Guid id);
    }
}
