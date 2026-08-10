using System.Threading.Tasks;
using Vegetable.Entities;

namespace Vegetable.Core.Database
{
    public interface IUserRepo
    {
        void UpdateMetadata(Owner owner);
        Task<string> GetOwnerId(string token);
    }
}
