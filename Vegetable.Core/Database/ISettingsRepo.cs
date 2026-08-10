using System.Threading.Tasks;
using Vegetable.Entities;

namespace Vegetable.Core.Database
{
    public interface ISettingsRepo
    {
        Task<Currency[]> GetCurrencies();
        Task<SubscriptionType[]> GetSubscriptionTypes();
        Task<SubscriptionType> GetSubscriptionTypesById(int typeId);
        Task<Discount> GetDiscountByQuantity(int quantity);
        Task<Discount[]> GetDiscounts();
        Task<ApplicationSettings> GetApplicationSettings();
    }
}
