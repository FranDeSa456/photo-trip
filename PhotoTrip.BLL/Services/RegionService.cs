using PhotoTrip.BLL.Services.Interfaces;
using PhotoTrip.DAL.Entities;
using PhotoTrip.DAL.Repositories.Interfaces;

namespace PhotoTrip.BLL.Services
{
    public class RegionService : IRegionService
    {
        private readonly IRegionRepository _regionRepository;

        public RegionService(IRegionRepository regionRepository)
        {
            _regionRepository = regionRepository;
        }

        public async Task<IEnumerable<Region>> GetAllAsync()
        {
            return await _regionRepository.GetAllAsync();
        }

        public async Task<Region?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Region ID must be greater than 0.", nameof(id));

            return await _regionRepository.GetByIdAsync(id);
        }

        public async Task<Region> CreateAsync(Region region)
        {
            if (string.IsNullOrWhiteSpace(region.Name))
                throw new ArgumentException("Region name cannot be empty.", nameof(region.Name));

            return await _regionRepository.CreateAsync(region);
        }

        public async Task<Region?> UpdateAsync(int id, Region region)
        {
            if (id <= 0)
                throw new ArgumentException("Region ID must be greater than 0.", nameof(id));

            if (string.IsNullOrWhiteSpace(region.Name))
                throw new ArgumentException("Region name cannot be empty.", nameof(region.Name));

            return await _regionRepository.UpdateAsync(id, region);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Region ID must be greater than 0.", nameof(id));

            return await _regionRepository.DeleteAsync(id);
        }
    }
}
