using System.Threading.Tasks;
using Vegetable.API.Controllers;

namespace Vegetable.API.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationService.AuthenticateResponse> Authenticate(AuthenticateRequest model);
    }
}
