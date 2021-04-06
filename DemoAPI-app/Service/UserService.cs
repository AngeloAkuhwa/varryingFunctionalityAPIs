using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DemoAPI_app.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DemoAPI_app.Service
{
    public class UserService:IUserService
    {
        private readonly UserManager<IdentityUser> _userMgr;
        private readonly SignInManager<IdentityUser> _signInMgr;
        private readonly IConfiguration _configuration;

        public UserService(UserManager<IdentityUser> userMgr, SignInManager<IdentityUser> signInMgr, IConfiguration configuration)
        {
            _userMgr = userMgr;
            _signInMgr = signInMgr;
            _configuration = configuration;
        }

        public async Task<UsermanagerResponseDTO> RegisterUserAsync(RegisterDTO model)
        {
            if (model == null)
            {
                throw new NullReferenceException("All fields are required");
            }

            if (model.Password != model.ConfirmPassword)
            {
                return new UsermanagerResponseDTO
                {
                    Message = "Confirm password doesn't match password",
                    IsSuccessful = false,
                        
                };
            }

            var user = new IdentityUser()
            {
                UserName = model.Email,
                Email = model.Email,
            };

            var result =await _userMgr.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                //Todo: send a confirmation email
                return new UsermanagerResponseDTO()
                {
                    Message = "user created successfully",
                    IsSuccessful = true,
                };
            }
            return new UsermanagerResponseDTO()
            {
                Message = "",
                IsSuccessful = false,
                Errors = result.Errors.Select(e => e.Description),
            };
        }

        public async Task<UsermanagerResponseDTO> ChangePasswordAsync(ChangePasswordDTO model)
        {
            if (model == null)
            {
                throw new NullReferenceException("All fields are required");
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                return new UsermanagerResponseDTO()
                {
                    Message = "Confirm password doesn't match new password",
                    IsSuccessful = false,
                };
            }

            var user = _userMgr.FindByEmailAsync(model.Email);
          
            var result = await _userMgr.ChangePasswordAsync(await user, model.OldPassword, model.NewPassword);

            if (result != null)
            {
                return new UsermanagerResponseDTO()
                {
                    Message = "user password changed successfully",
                    IsSuccessful = true,
                };
            }

            return new UsermanagerResponseDTO()
            {
                Message = "",
                IsSuccessful = false,
                Errors = result.Errors.Select(e => e.Description),
            };


        }


        public async Task<UsermanagerResponseDTO> LoginUserAsync(LoginDTO model)
        {
            var user = await _userMgr.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new UsermanagerResponseDTO()
                {
                    Message = "User doesn't exist",
                    IsSuccessful = false,
                };
            }

            var result = await _userMgr.CheckPasswordAsync(user, model.Password);

            if (!result)
            {
                return new UsermanagerResponseDTO()
                {
                    Message = "Invalid password",
                    IsSuccessful = false,
                };
            }

            var claims = new[]
            {
                new Claim("Email", model.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["AuthSettings:Issuer"],
                audience:_configuration["AuthSettings:Audience"],
                claims:claims,
                expires:DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(key ,SecurityAlgorithms.HmacSha256 ));

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return  new UsermanagerResponseDTO()
            {
                Message = tokenString,
                IsSuccessful = true,
                ExpireDate = token.ValidTo
            };

        }
    }
}
