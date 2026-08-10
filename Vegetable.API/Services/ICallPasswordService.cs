using System.Threading.Tasks;

namespace Vegetable.API.Services
{
    public interface ICallPasswordService
    {
        Task<string> SendCallVerification(string phoneNumber);
    }
}
