using AutoMapper;
using be.Dtos.Services;
using be.Models;
using be.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/services")]
public class ServiceController : ControllerBase
{
    private readonly ILogger<ServiceController> _logger;
    private readonly IMapper _mapper;
    private readonly IServiceRepository _serviceRepository;

    public ServiceController(ILogger<ServiceController> logger, IServiceRepository serviceRepository, IMapper mapper)
    {
        _logger = logger;
        _serviceRepository = serviceRepository;
        _mapper = mapper;
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var services = await _serviceRepository.GetAsync(cancellationToken);

        var servicesDto = _mapper.Map<IEnumerable<ServiceReadDTO>>(services);

        return Ok(servicesDto);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var service = await _serviceRepository.GetByIdAsync(id, cancellationToken);

        if (service == null)
        {
            return NotFound();
        }

        var serviceDto = _mapper.Map<ServiceReadDTO>(service);

        return Ok(serviceDto);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] ServiceCreateDTO serviceDto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var service = _mapper.Map<Service>(serviceDto);

        await _serviceRepository.AddAsync(service, cancellationToken);

        var serviceReadDto = _mapper.Map<ServiceReadDTO>(service);

        return CreatedAtAction(nameof(GetById), new { id = service.ServiceId }, serviceReadDto);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ServiceUpdateDTO serviceDto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingService = await _serviceRepository.GetByIdAsync(id, cancellationToken);

        if (existingService == null)
        {
            return NotFound("Service Not Found!");
        }

        _mapper.Map(serviceDto, existingService);

        await _serviceRepository.UpdateAsync(existingService, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var service = await _serviceRepository.GetByIdAsync(id);

        if (service == null)
        {
            return NotFound("Service Not Found!"); 
        }

        await _serviceRepository.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}