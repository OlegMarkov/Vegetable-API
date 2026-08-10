using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Vegetable.Entities;

namespace Vegetable.Core.Database
{
    public class SettingsRepo : RepoBase, ISettingsRepo
    {

        public SettingsRepo(PostgreDbContext postgreDbContext):base (postgreDbContext) {}

        public async Task<Currency[]> GetCurrencies()
        {
            try
            {
                return await _context.Currencies.AsNoTracking().ToArrayAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<SubscriptionType[]> GetSubscriptionTypes()
        {
            try
            {
                return await _context.SubscriptionTypes.AsNoTracking().Where(x=>x.IsEnabled).ToArrayAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<SubscriptionType> GetSubscriptionTypesById(int typeId)
        {
            try
            {
                return await _context.SubscriptionTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == typeId && x.IsEnabled);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Discount> GetDiscountByQuantity(int quantity)
        {
            try
            {
                return await _context.Discounts.AsNoTracking().FirstOrDefaultAsync(x=>x.IsEnabled && x.Quantity == quantity);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Discount[]> GetDiscounts()
        {
            try
            {
                return await _context.Discounts.AsNoTracking().Where(x => x.IsEnabled).ToArrayAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApplicationSettings> GetApplicationSettings()
        {
            try
            {
                return await _context.ApplicationSettings.AsNoTracking().FirstAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
