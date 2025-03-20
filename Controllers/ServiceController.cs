using AutoMapper;
using be.Dtos.Services;
using be.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/[controller]")]
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
     public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default){
        var services = await _serviceRepository.GetAsync(cancellationToken);
        var servicesDto = _mapper.Map<IEnumerable<ServiceReadDTO>>(services);
        return Ok(servicesDto);
     }

}