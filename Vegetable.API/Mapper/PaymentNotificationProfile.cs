using AutoMapper;
using Newtonsoft.Json;
using System;
using Vegetable.API.ViewModels.Payment;
using Vegetable.Entities;

namespace Vegetable.API.Mapper
{
    public class PaymentNotificationProfile : Profile
    {
        public PaymentNotificationProfile()
        {
            CreateMap<PaymentNotificationMessage, PaymentNotification>()
                .ForMember(dst => dst.Data, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.Data)));
        }
    }
}
