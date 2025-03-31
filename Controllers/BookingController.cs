using AutoMapper;
using be.Dtos.Booking;
using be.Models;
using be.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace be.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;
        private readonly IMapper _mapper;
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;

        public BookingController(ILogger<BookingController> logger, IBookingRepository bookingRepository, IRoomRepository roomRepository, IMapper mapper)
        {
            _logger = logger;
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [EnableQuery]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var bookings = await _bookingRepository.GetAsync(cancellationToken);

            var bookingsDto = _mapper.Map<IEnumerable<BookingReadDTO>>(bookings);

            return Ok(bookingsDto);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            var booking = await _bookingRepository.GetByIdAsync(id, cancellationToken);

            if (booking == null)
            {
                return NotFound();
            }

            var bookingsDto = _mapper.Map<BookingReadDTO>(booking);

            return Ok(bookingsDto);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] BookingCreateDTO bookingDto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var booking = _mapper.Map<Booking>(bookingDto);

            await _bookingRepository.AddAsync(booking, cancellationToken);

            var room = await _roomRepository.GetByIdAsync(booking.RoomId, cancellationToken);
            if (room == null)
            {
                return NotFound("Room not found.");
            }

            room.IsAvailable = false;

            await _roomRepository.UpdateAsync(room, cancellationToken);

            var bookingReadDto = _mapper.Map<BookingReadDTO>(booking);

            return CreatedAtAction(nameof(GetById), new { id = bookingReadDto.BookingId }, bookingReadDto);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] BookingUpdateDTO bookingDto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingBooking = await _bookingRepository.GetByIdAsync(id, cancellationToken);

            if (existingBooking == null)
            {
                return NotFound();
            }

            _mapper.Map(bookingDto, existingBooking);

            await _bookingRepository.UpdateAsync(existingBooking, cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            var booking = await _bookingRepository.GetByIdAsync(id, cancellationToken);
            
            if(booking == null){
                
                return NotFound();
            }
        
            await _bookingRepository.DeleteAsync(id, cancellationToken);

            var room = await _roomRepository.GetByIdAsync(booking.RoomId, cancellationToken);

            if (room == null)
            {
                return NotFound("Room not found.");
            }

            room.IsAvailable = true;

            await _roomRepository.UpdateAsync(room, cancellationToken);

            return NoContent();
        }
    }
}