using AutoMapper;
using be.Dtos.Users;
using be.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IUserContext _userContext;

        public UserController(ILogger<UserController> logger, IMapper mapper, IUserRepository userRepository, IUserContext userContext)
        {
            _logger = logger;
            _userRepository = userRepository;
            _mapper = mapper;
            _userContext = userContext;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var users = await _userRepository.GetAsync(cancellationToken);
            var usersDto = _mapper.Map<IEnumerable<UserReadDTO>>(users);
            return Ok(usersDto);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
            {
                return NotFound();
            }
            var userDto = _mapper.Map<UserReadDTO>(user);
            return Ok(userDto);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UserUpdateDTO userDto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
            {
                return NotFound("User Not Found!");
            }

            _mapper.Map(userDto, user);
            var success = await _userRepository.UpdateAsync(user, cancellationToken);

            if (!success)
            {
                return BadRequest("Failed to update user.");
            }

            var userDtoResponse = _mapper.Map<UserReadDTO>(user);
            
            return Ok(userDtoResponse);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            var deleted = await _userRepository.DeleteAsync(id, cancellationToken);
            if (!deleted)
            {
                return NotFound("User Not Found!");
            }
            return NoContent();
        }

        [HttpPost("{id:int}/assign-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole([FromRoute] int id, [FromBody] string role, CancellationToken cancellationToken = default)
        {
            var success = await _userRepository.AssignRoleAsync(id, role, cancellationToken);
            if (!success)
            {
                return BadRequest("User Not Found or Role Does Not Exist!");
            }
            return Ok($"User with ID {id} assigned to role {role}");
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser(){
            try
            {
                var userDto = await _userContext.GetCurrentUserAsync();

                return Ok(userDto);
                
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving user: {ex.Message}");
            }
        }
    }
}