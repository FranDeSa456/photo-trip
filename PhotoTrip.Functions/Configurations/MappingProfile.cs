using AutoMapper;
using PhotoTrip.BLL.Models;
using PhotoTrip.DAL.Entities;

namespace PhotoTrip.Functions.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Place, PlaceModel>().ReverseMap();
            CreateMap<Review, ReviewModel>().ReverseMap();
        }
    }
}
