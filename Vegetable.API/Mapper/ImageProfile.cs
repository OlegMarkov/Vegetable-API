using AutoMapper;
using Vegetable.API.ViewModels;
using Vegetable.Entities;

namespace Vegetable.API.Mapper
{
    public class ImageProfile : Profile
    {
        public ImageProfile()
        {
            CreateMap<ImageInfo, Image>();
        }
    }
}
