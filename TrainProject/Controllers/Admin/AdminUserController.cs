using Application.DTOs.ApiResponseDTO;
using Application.DTOs.RequestDTOs;
using Application.DTOs.ResponseDTOs;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Admin
{
    /// <summary>
    /// Admin User Management Controller
    /// </summary>
    [Route("api/admin/[controller]")]
    [ApiController]
    public class AdminUserController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminUserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get All Users (Admin Only)
        /// </summary>
        [HttpGet("UserGetAll")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();

            //Normal response
            return Ok(ApiResponse.Success("Get all users successful", users));
        }

        /// <summary>
        /// Get User By ID (Admin Only)
        /// </summary>
        [HttpGet("UserGetByID{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetById(id);
            if (user == null) return NotFound(ApiResponse.Fail("User not found"));

            //Normal response
            return Ok(ApiResponse.Success("Get user successful", user));
        }

        /// <summary>
        /// Create New User (Admin Only)
        /// </summary>
        [HttpPost("UserCreate")]
        public async Task<IActionResult> Create([FromBody] CreateUserDTOs dto)
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

            await _userService.CreateAsync(dto);

            //Normal response
            return Ok(ApiResponse.Success("Create user successful", dto));
        }

        /// <summary>
        /// Update User (Admin Only)
        /// </summary>
        [HttpPut("UserUpdate")]
        public async Task<IActionResult> Update([FromBody] UpdateUserDTO dto)
        {
            if (dto == null) return BadRequest(ApiResponse.Fail("Data is null"));

            var user = await _userService.GetById(dto.UserId);
            if (user == null) return NotFound(ApiResponse.Fail("User not found to update"));

            await _userService.UpdateAsync(dto);

            //Normal response
            return Ok(ApiResponse.Success("Update user successful", dto));
        }

        /// <summary>
        /// Delete User (Admin Only)
        /// </summary>
        [HttpDelete("UserDelete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userService.GetById(id);
            if (user == null) return NotFound(ApiResponse.Fail("User not found to delete"));

            await _userService.DeleteAsync(id);

            //Normal response
            return Ok(ApiResponse.Success("Delete user successful", id));
        }
    }
}
