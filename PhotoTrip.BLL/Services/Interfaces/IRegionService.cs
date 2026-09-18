using PhotoTrip.DAL.Entities;

namespace PhotoTrip.BLL.Services.Interfaces
{
    public interface IRegionService
    {
        Task<IEnumerable<Region>> GetAllAsync();
        Task<Region?> GetByIdAsync(int id);
        Task<Region> CreateAsync(Region region);
        Task<Region?> UpdateAsync(int id, Region region);
        Task<bool> DeleteAsync(int id);
    }
}
