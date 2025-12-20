using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.RequestDTOs.PostDTO;
using Application.Interfaces.Service;
using Application.Interfaces.UnitOfwork;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PostService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Create Post
        /// </summary>
        public async Task CreatePostAsync(CreatePostRequestDTO post)
        {
            var entity = _mapper.Map<Post>(post);

            entity.PostId = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsDeleted = false;
            entity.LikeCount = 0; 
            entity.CommentCount = 0;
            entity.ShareCount = 0;
            entity.UpdatedAt = null;

            await _unitOfWork.PostRepository.CreateAsync(entity);

            //Reflect to DB
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Delete Post
        /// </summary>
        public async Task DeletePostAsync(Guid id)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(id);
            if (post != null)
            {
                post.IsDeleted = true;
                post.UpdatedAt = DateTime.UtcNow;

                //Logical deletion
                await _unitOfWork.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Get All Posts
        /// </summary>
        public async Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            var posts = await _unitOfWork.PostRepository.GetAllAsync();
            return posts.Where(p => p.IsDeleted == false || p.IsDeleted == null);
        }

        /// <summary>
        /// Get Post By ID
        /// </summary>
        public async Task<Post?> GetPostByIdAsync(Guid id)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(id);
            return post != null && (post.IsDeleted == false || post.IsDeleted == null) ? post : null;
        }

        /// <summary>
        /// Update Post
        /// </summary>
        public async Task UpdatePostAsync(Guid id, UpdatePostRequestDTO
            postDTO)
        {
            var existingPost = await _unitOfWork.PostRepository.GetByIdAsync(id);
            if (existingPost != null)
            {
                // Map data from DTO to Entity if DTO has additional fields
                _mapper.Map(postDTO, existingPost);
                existingPost.UpdatedAt = DateTime.UtcNow;

                //Reflect to DB
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
