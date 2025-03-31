using be.Models;

namespace be.Repositories.impl;

public class CategoryRepository : ICategoryRepository
{
    private readonly IRepositoryAsync<Category> _categoryRepository;
        public CategoryRepository(IRepositoryAsync<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

    public async Task AddAsync(Category entity, CancellationToken cancellationToken = default)
    {
        await _categoryRepository.AddAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _categoryRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _categoryRepository.GetAsync(cancellationToken);
    }

    public async Task<Category> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _categoryRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task UpdateAsync(Category entity, CancellationToken cancellationToken = default)
    {
        await _categoryRepository.UpdateAsync(entity, cancellationToken);
    }
}