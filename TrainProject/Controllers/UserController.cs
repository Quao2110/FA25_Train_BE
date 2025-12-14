using Application.DTOs.ApiResponseDTO;
using Application.Interfaces.ServiceProviders;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IServiceProviders _serviceProviders;

        public UserController(IServiceProviders serviceProviders)
        {
            _serviceProviders = serviceProviders;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllUsers()
        {
            var listUser = await _serviceProviders.UserService.GetAllAsync();

            return Ok(ApiResponse.Success("Get all Successful", listUser));
        }
    }
}
