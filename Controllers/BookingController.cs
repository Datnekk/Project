using AutoMapper;
using be.Dtos.Booking;
using be.Helpers;
using be.Models;
using be.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;
        private readonly IMapper _mapper;
        private readonly IBookingRepository _bookingRepository;

        public BookingController(ILogger<BookingController> logger, IBookingRepository bookingRepository, IMapper mapper)
        {
            _logger = logger;
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> getAll([FromQuery] BookingQueryObject query)
        {
            var bookings = await _bookingRepository.GetAllAsync(query);

            var bookingsDto = _mapper.Map<IEnumerable<BookingReadDTO>>(bookings);

            return Ok(bookingsDto);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> getById([FromRoute] int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            var bookingsDto = _mapper.Map<BookingReadDTO>(booking);

            return Ok(bookingsDto);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> create([FromBody] BookingCreateDTO bookingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var booking = _mapper.Map<Booking>(bookingDto);

            var createdBooking = await _bookingRepository.CreateAsync(booking);

            var bookingReadDto = _mapper.Map<BookingReadDTO>(createdBooking);

            return CreatedAtAction(nameof(getById), new { id = bookingReadDto.BookingId }, bookingReadDto);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> update([FromRoute] int id, [FromBody] BookingUpdateDTO bookingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var booking = _mapper.Map<Booking>(bookingDto);

            var updatedBooking = await _bookingRepository.UpdateAsync(id, booking);

            if (updatedBooking == null)
            {
                return NotFound();
            }

            var bookingReadDto = _mapper.Map<BookingReadDTO>(updatedBooking);

            return Ok(bookingReadDto);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> delete([FromRoute] int id)
        {
            var deleted = await _bookingRepository.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}