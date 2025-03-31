using AutoMapper;
using be.Dtos.Category;
using be.Models;
using be.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace be.Controllers;

[ApiController]
[Route("api/category")]
public class CategoryController : ControllerBase
{
    private readonly ILogger<CategoryController> _logger;
    private readonly IMapper _mapper;
    private readonly ICategoryRepository _categoryRepository;

    public CategoryController(ILogger<CategoryController> logger, IMapper mapper, ICategoryRepository categoryRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _categoryRepository = categoryRepository;       
    }

    [HttpGet]
    [EnableQuery]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAsync(cancellationToken);
        var categoriesDto = _mapper.Map<IEnumerable<CategoryReadDTO>>(categories);
        return Ok(categoriesDto);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);

        if (category == null)
        {
            return NotFound();
        }

        var categoryDto = _mapper.Map<CategoryReadDTO>(category);
        return Ok(categoryDto);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CategoryCreateDTO categoryDto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var category = _mapper.Map<Category>(categoryDto);
        await _categoryRepository.AddAsync(category, cancellationToken);

        var categoryReadDto = _mapper.Map<CategoryReadDTO>(category);
        return CreatedAtAction(nameof(GetById), new { id = categoryReadDto.CategoryId }, categoryReadDto);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CategoryUpdateDTO categoryDto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingCategory = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (existingCategory == null)
        {
            return NotFound();
        }

        _mapper.Map(categoryDto, existingCategory);
        await _categoryRepository.UpdateAsync(existingCategory, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        
        if (category == null)
        {
            return NotFound();
        }

        await _categoryRepository.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}