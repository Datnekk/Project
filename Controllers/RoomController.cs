using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using be.Dtos.Rooms;
using be.Models;
using be.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace be.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomController : ControllerBase
    {
        private readonly ILogger<RoomController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;
        private readonly IRoomRepository _roomRepository;

        public RoomController(ILogger<RoomController> logger, IRoomRepository roomRepository, IMapper mapper, IUserContext userContext)
        {
            _logger = logger;
            _roomRepository = roomRepository;
            _mapper = mapper;
            _userContext = userContext;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var rooms = await _roomRepository.GetAsync(cancellationToken);

            var roomsDto = _mapper.Map<IEnumerable<RoomReadDTO>>(rooms);

            return Ok(roomsDto);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            var room = await _roomRepository.GetByIdAsync(id, cancellationToken);

            if (room == null)
            {
                return NotFound();
            }

            var roomsDto = _mapper.Map<RoomReadDTO>(room);

            return Ok(roomsDto);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] RoomCreateDTO roomDto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = await _userContext.GetCurrentUserIdAsync();

            var room = _mapper.Map<Room>(roomDto);

            room.UserId = userId;

            await _roomRepository.AddAsync(room, cancellationToken);

            var roomReadDTO = _mapper.Map<RoomReadDTO>(room);

            return CreatedAtAction(nameof(GetById), new { id = room.RoomId }, roomReadDTO);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] RoomUpdateDTO roomDto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = await _userContext.GetCurrentUserIdAsync();

            var existingRoom = await _roomRepository.GetByIdAsync(id, cancellationToken);

            if(existingRoom == null)
            {
                return NotFound("Room Not Found!");
            }

            if (existingRoom.UserId != userId)
            {
                return Forbid("You are not authorized to update this room.");
            }

            _mapper.Map(roomDto, existingRoom);

            await _roomRepository.UpdateAsync(existingRoom, cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default)
        {
           var room = await _roomRepository.GetByIdAsync(id);

           if (room == null){
            return NotFound("Room Not Found!"); 
           }

           await _roomRepository.DeleteAsync(id, cancellationToken);

           return NoContent();
        }
    }
}