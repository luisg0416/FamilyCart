namespace FamilyCart.API.Controllers
{
    using FamilyCart.Core.DTOs;
    using FamilyCart.Core.Interfaces;
    using FamilyCart.Core.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if(user == null)
            {
                return Unauthorized("Invalid email or password");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var token = _tokenService.GetToken(user);
                var authResponseDto = new AuthResponseDto
                {
                    Token = token.Token,
                    Expiration = token.Expiration,
                    UserId = user.Id,
                    Email = user.Email ?? "",
                    FirstName = user.FirstName
                };

                return Ok(authResponseDto);
            }
            else
            {
                if (result.IsLockedOut)
                {
                    return Unauthorized("Account Locked");
                }
                else
                {
                    return Unauthorized("Invalid Password");
                }
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if(!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var token = _tokenService.GetToken(user);
            var authResponseDto = new AuthResponseDto
                {
                    Token = token.Token,
                    Expiration = token.Expiration,
                    UserId = user.Id,
                    Email = user.Email ?? "",
                    FirstName = user.FirstName
                };
            
            return Ok(authResponseDto);
        }
    }
}