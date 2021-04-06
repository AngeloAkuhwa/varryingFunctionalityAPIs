using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoAPI_app.DTOs;

namespace DemoAPI_app.Service
{
    public interface IUserService
    {
        Task<UsermanagerResponseDTO> RegisterUserAsync(RegisterDTO model);
        Task<UsermanagerResponseDTO> ChangePasswordAsync(ChangePasswordDTO model);
        Task<UsermanagerResponseDTO> LoginUserAsync(LoginDTO model);
    }
}
