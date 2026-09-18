using Microsoft.EntityFrameworkCore;
using PhotoTrip.DAL.Data;
using PhotoTrip.DAL.Entities;
using PhotoTrip.DAL.Repositories.Interfaces;

namespace PhotoTrip.DAL.Repositories
{
    public class RegionRepository : IRegionRepository
    {
        private readonly PhotoDbContext _context;

        public RegionRepository(PhotoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Region>> GetAllAsync()
        {
            return await _context.Regions
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Region?> GetByIdAsync(int id)
        {
            return await _context.Regions
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Region> CreateAsync(Region region)
        {
            _context.Regions.Add(region);
            await _context.SaveChangesAsync();
            return region;
        }

        public async Task<Region?> UpdateAsync(int id, Region region)
        {
            var existingRegion = await _context.Regions.FindAsync(id);
            if (existingRegion == null)
                return null;

            existingRegion.Name = region.Name;

            _context.Regions.Update(existingRegion);
            await _context.SaveChangesAsync();
            return existingRegion;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var region = await _context.Regions.FindAsync(id);
            if (region == null)
                return false;

            _context.Regions.Remove(region);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
