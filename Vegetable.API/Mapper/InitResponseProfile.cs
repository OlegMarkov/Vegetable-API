using AutoMapper;
using Newtonsoft.Json;
using System;
using Vegetable.API.ViewModels.Payment;
using Vegetable.Entities;

namespace Vegetable.API.Mapper
{
    public class InitResponseProfile : Profile
    {
        public InitResponseProfile()
        {
            CreateMap<InitResponse, Order>();
        }
    }
}
