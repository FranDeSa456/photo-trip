using PhotoTrip.DAL.Entities;

namespace PhotoTrip.DAL.Repositories.Interfaces
{
    public interface IRegionRepository
    {
        Task<IEnumerable<Region>> GetAllAsync();
        Task<Region?> GetByIdAsync(int id);
        Task<Region> CreateAsync(Region region);
        Task<Region?> UpdateAsync(int id, Region region);
        Task<bool> DeleteAsync(int id);
    }
}
