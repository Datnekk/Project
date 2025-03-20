using AutoMapper;
using be.Dtos.Booking;
using be.Dtos.Users;
using be.Models;
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

        public UserController(ILogger<UserController> logger, IMapper mapper, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> getAll([FromQuery] UserQueryObject query){
            var users = await _userRepository.GetAllAsync(query);

            var usersDto = _mapper.Map<IEnumerable<UserReadDTO>>(users);

            return Ok(usersDto);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> getById([FromRoute] int id){
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userDto = _mapper.Map<UserReadDTO>(user);
            return Ok(userDto);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> update([FromRoute] int id, [FromBody] UserUpdateDTO userDto){
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _mapper.Map<User>(userDto);

            var updatedUser = await _userRepository.UpdateAsync(id, user);

            if (updatedUser == null)
            {
                return NotFound("User Not Found!");
            }

            var userDtoResponse = _mapper.Map<UserReadDTO>(updatedUser);
            
            return Ok(userDtoResponse);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> delete([FromRoute] int id){

            var deleted = await _userRepository.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound("User Not Found!");
            }

            return NoContent();
        }

        [HttpPost("{id:int}/assign-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole([FromRoute] int id, [FromBody] string role){

            var success = await _userRepository.AssignRoleAsync(id, role);

            if(!success)
            {
            return BadRequest("User Not Found Or Role Does Not Exist!!");
            }

            return Ok($"User with ID {id} assigned to role {role}");
        }
    }
}