using AutoMapper;
using be.Dtos.Rooms;
using be.Helpers;
using be.Models;
using be.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomController : ControllerBase
    {
        private readonly ILogger<RoomController> _logger;
        private readonly IMapper _mapper;
        private readonly IRoomRepository _roomRepository;

        public RoomController(ILogger<RoomController> logger, IRoomRepository roomRepository, IMapper mapper)
        {
            _logger = logger;
            _roomRepository = roomRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> getAll([FromQuery] RoomQueryObject query)
        {
            var rooms = await _roomRepository.GetAllAsync(query);

            var roomsDto = _mapper.Map<IEnumerable<RoomReadDTO>>(rooms);

            return Ok(roomsDto);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> getById([FromRoute] int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            var roomsDto = _mapper.Map<RoomReadDTO>(room);

            return Ok(roomsDto);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> create([FromBody] RoomCreateDTO roomDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var room = _mapper.Map<Room>(roomDto);

            var createdRoom = await _roomRepository.CreateAsync(room);

            var roomsDtoResponse = _mapper.Map<RoomReadDTO>(createdRoom);

            return CreatedAtAction(nameof(getById), new { id = room.RoomId }, roomsDtoResponse);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> update([FromRoute] int id, [FromBody] RoomUpdateDTO roomDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var room = _mapper.Map<Room>(roomDto);

            var updatedRoom = await _roomRepository.UpdateAsync(id, room);

            if (updatedRoom == null)
            {
                return NotFound("Room Not Found!");
            }

            var roomsDtoResponse = _mapper.Map<RoomReadDTO>(updatedRoom);

            return Ok(roomsDtoResponse);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> delete([FromRoute] int id)
        {
           var deleted = await _roomRepository.DeleteAsync(id);

           if (!deleted){
            return NotFound("Room Not Found!"); 
           }

           return NoContent();
        }
    }
}