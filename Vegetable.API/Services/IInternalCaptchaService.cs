using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vegetable.API.ViewModels;

namespace Vegetable.API.Services
{
    public interface IInternalCaptchaService
    {
        Task<BaseCaptchaServiceResponse> SendCallVerification(string token);
    }
}
