using Application.DTOs.ApiResponseDTO;
using Application.DTOs.RequestDTOs.AuthDTO;
using Application.DTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Service
{
    public interface IAuthService
    {
        Task<ApiResponse> LoginAsync(LoginRequestDTO request);
        Task<ApiResponse> RegisterAsync(RegisterRequestDTO request);
        Task<ApiResponse> VerifyOtpAndCreateUserAsync(VerifyOtpRequestDTO request);
        Task<ApiResponse> LoginGoogleAsync(GoogleLoginRequestDTO request);
    }
}
