using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vegetable.API.Services
{
    public interface IEmailService
    {
        void SendConfirmationEmail();

        void SendVerificationCode(string email, string code);
    }
}
