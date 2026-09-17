using PhotoTrip.BLL.Models;
using PhotoTrip.DAL.Entities;

namespace PhotoTrip.BLL.Extensions
{
    public static class RegionMappingExtensions
    {
        // Da Entity (DAL) a Model (BLL)
        public static RegionModel ToModel(this Region entity)
        {
            if (entity == null) return null!;

            return new RegionModel
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        // Da Model (BLL) a Entity (DAL)
        public static Region ToEntity(this RegionModel model)
        {
            if (model == null) return null!;

            return new Region
            {
                Id = model.Id,
                Name = model.Name
            };
        }
    }
}