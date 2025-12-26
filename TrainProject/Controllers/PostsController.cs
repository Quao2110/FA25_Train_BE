using Application.DTOs.ApiResponseDTO;
using Application.DTOs.RequestDTOs.PostDTO;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    /// <summary>
    /// PostsController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        public PostsController(IPostService postService)
        {
            _postService = postService;
        }
        /// <summary>
        /// Get All The Posts
        /// </summary>
        [HttpGet("PostGetAll")]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postService.GetAllPostsAsync();

            //Normal response
            return Ok(ApiResponse.Success("Get all posts successful", posts));
        }

        /// <summary>
        /// Get Post By ID
        /// </summary>
        [HttpGet("PostGetByID{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null) return NotFound(ApiResponse.Fail("Post not found"));

            //Normal response
            return Ok(ApiResponse.Success("Get post successful", post));
        }

        /// <summary>
        /// Create Post
        /// </summary>
        [HttpPost("PostCreate")]
        public async Task<IActionResult> Create([FromBody] CreatePostRequestDTO dto)
        {
            if (dto == null) return BadRequest(ApiResponse.Fail("Data is null"));

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse.Fail(string.Join(", ", errors)));
            }

            await _postService.CreatePostAsync(dto);

            //Normal response
            return Ok(ApiResponse.Success("Create post successful", dto));
        }

        /// <summary>
        /// Update Post
        /// </summary>
        [HttpPut("PostUpdate{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePostRequestDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse.Fail(string.Join(", ", errors)));
            }

            var existing = await _postService.GetPostByIdAsync(id);
            if (existing == null) return NotFound(ApiResponse.Fail("Post not found"));

            await _postService.UpdatePostAsync(dto);

            //Normal response
            return Ok(ApiResponse.Success("Update post successful", dto));
        }

        /// <summary>
        /// Delete Post
        /// </summary>
        [HttpDelete("PostDelete{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _postService.GetPostByIdAsync(id);
            if (existing == null) return NotFound(ApiResponse.Fail("Post not found"));

            await _postService.DeletePostAsync(id);

            //Normal response
            return Ok(ApiResponse.Success("Delete post successful", existing));
        }
    }
}
