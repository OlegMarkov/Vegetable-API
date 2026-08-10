using System.Threading.Tasks;

namespace Vegetable.Core.Services
{
    public interface IPushService
    {
        Task<string> PushMessageToSingleAsync(string cid, string title, string content, string urlId = "", string platform = "ios");

        void PushMessageToApp();
    }
}
