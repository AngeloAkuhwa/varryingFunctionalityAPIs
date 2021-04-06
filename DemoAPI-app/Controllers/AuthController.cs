using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DemoAPI_app.DTOs;
using DemoAPI_app.Service;
using Microsoft.AspNetCore.Identity;

namespace DemoAPI_app.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthController(IUserService userService, UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        //api/auth/Register
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("some properties are not valid"); //status code 400
            }

            var result = await _userService.RegisterUserAsync(model);
          
            

            if (result.IsSuccessful)
            {
                return Ok(result); //status code 200;
            }

            return BadRequest(result);

        }

        //api/auth/Register

        [HttpPatch("ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDTO model)
        {
            if (ModelState.IsValid)
            {
                //var user = User.Claims.FirstOrDefault(userId => userId.Type == ClaimTypes.NameIdentifier);
                
                var result = await _userService.ChangePasswordAsync(model);

                if (result.IsSuccessful)
                {
                    return Ok(result); //status code 200;
                }

                return BadRequest(result);


            }

            return BadRequest("some properties are not valid"); //status code 400
        }


        //api/auth/Login
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginDTO model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginUserAsync(model);

                if (result.IsSuccessful)
                {
                    return Ok(result);
                }

                return BadRequest(result);

            }

            return BadRequest("some properties are not valid");
        }
    }
}
