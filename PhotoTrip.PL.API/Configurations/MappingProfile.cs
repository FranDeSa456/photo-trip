using AutoMapper;
using PhotoTrip.BLL.Models;
using PhotoTrip.DAL.Entities;

namespace PhotoTrip.PL.API.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Region, RegionModel>().ReverseMap();
            CreateMap<Place, PlaceModel>().ReverseMap();
            CreateMap<Review, ReviewModel>().ReverseMap();
        }
    }
}
