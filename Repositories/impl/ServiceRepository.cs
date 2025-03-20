using be.Models;

namespace be.Repositories.impl;

public class ServiceRepository : IServiceRepository
{
    private readonly IRepositoryAsync<Service> _serviceRepository;

    public ServiceRepository(IRepositoryAsync<Service> serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task AddAsync(Service entity, CancellationToken cancellationToken = default)
    {
        await _serviceRepository.AddAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _serviceRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Service>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _serviceRepository.GetAsync(cancellationToken);
    }

    public async Task<Service> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _serviceRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task UpdateAsync(Service entity, CancellationToken cancellationToken = default)
    {
        await _serviceRepository.UpdateAsync(entity, cancellationToken);
    }
}