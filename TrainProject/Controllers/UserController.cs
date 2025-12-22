using Application.DTOs.ApiResponseDTO;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllUsers()
        {
            var listUser = await _userService.GetAllAsync();

            return Ok(ApiResponse.Success("Get all Successful", listUser));
        }
    }
}
