using Application.DTOs.RequestDTOs.PostDTO;
using Application.DTOs.ResponseDTOs;
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

            //Reflect to DB
            await _unitOfWork.PostRepository.CreateAsync(entity);
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
        public async Task<IEnumerable<PostResponseDTO>> GetAllPostsAsync()
        {
            var posts = await _unitOfWork.PostRepository.GetAllAsync();
            var activePosts = posts.Where(p => p.IsDeleted == false || p.IsDeleted == null);

            return _mapper.Map<IEnumerable<PostResponseDTO>>(activePosts);
        }

        /// <summary>
        /// Get Post By ID
        /// </summary>
        public async Task<PostResponseDTO?> GetPostByIdAsync(Guid id)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(id);
            if (post == null || post.IsDeleted == true) return null;

            return _mapper.Map<PostResponseDTO>(post);
        }

        /// <summary>
        /// Update Post
        /// </summary>
        public async Task UpdatePostAsync(UpdatePostRequestDTO
            postDTO)
        {
            var existingPost = await _unitOfWork.PostRepository.GetByIdAsync(postDTO.PostId);
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
