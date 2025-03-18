using AutoMapper;
using be.Dtos.Auth;
using be.Interfaces;
using be.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace be.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        private readonly SignInManager<User> _signInManager;

        public AuthController(IMapper mapper, UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager)
        {
            _mapper = mapper;
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO loginDTO){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDTO.UserName);

            if(user == null) return Unauthorized("Invalid UserName!");

            var res = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);

            if(!res.Succeeded) return Unauthorized("UserName/Password Not Found");

            return Ok(
                new NewUserDTO {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user),
                 }
            );
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    throw new Exception("Invalid data");
                }

                var user = _mapper.Map<User>(registerDTO);

                var createdUser = await _userManager.CreateAsync(user, registerDTO.Password);

                if(createdUser.Succeeded)
                {
                    var roleUser = await _userManager.AddToRoleAsync(user, "User");

                    if(roleUser.Succeeded){
                        return Ok(
                            new NewUserDTO
                            {
                                UserName = user.UserName,
                                Email = user.Email,
                                Token = _tokenService.CreateToken(user),
                            }
                        );
                    }
                    else
                    {
                        return StatusCode(500, roleUser.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);

                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}