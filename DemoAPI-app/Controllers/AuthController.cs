using DemoAPI_app.DTOs;
using DemoAPI_app.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DemoAPI_app.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(IServiceProvider serviceProvider)
        {
            _userService = serviceProvider.GetRequiredService<IUserService>();
            _userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>(); }


        //api/auth/Register
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid) return BadRequest("some properties are not valid"); //status code 400

            var result = await _userService.RegisterUserAsync(model);
          
            if (!result.IsSuccessful) return BadRequest(result);

            return Created("",result); //status code 201;

        }

        //api/auth/Register

        [HttpPatch("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePasswordAsync([FromBody]ChangePasswordDTO model)
        {
            if (!ModelState.IsValid) return BadRequest("some properties are not valid"); //status code 400

            //var user = User.Claims.FirstOrDefault(userId => userId.Type == ClaimTypes.NameIdentifier);

            var result = await _userService.ChangeUserPasswordAsync(model);

            if (!result.IsSuccessful) return BadRequest(result);

            return Ok(result); //status code 200;

        }


        //api/auth/Login
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody]LoginDTO model)
        {
            if (!ModelState.IsValid) return BadRequest("some properties are not valid");
            
            var result = await _userService.LoginUserAsync(model);

            if (result.IsSuccessful) return Ok(result);

            return BadRequest(result);

        }
    }
}
