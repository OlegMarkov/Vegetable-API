using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Vegetable.API.Attributes;
using Vegetable.Core.Database;

namespace Vegetable.API.Controllers
{
    [AuthorizeOwner]
    [Route("[controller]")]
    public class SettingsController : Controller
    {
        private readonly ISettingsRepo _repository;

        public SettingsController(ISettingsRepo repository)
        {
            _repository = repository;
        }

        [HttpGet("currency")]
        public async Task<string> GetAllCurrencies()
        {
            var currencies = await _repository.GetCurrencies();
            return SerializeObject(currencies);
        }

        [HttpGet("subscriptionTypes")]
        public async Task<string> GetAllSubscriptionTypes()
        {
            var subscriptionTypes = await _repository.GetSubscriptionTypes();
            return SerializeObject(subscriptionTypes);
        }

        [HttpGet("discounts")]
        public async Task<string> GetDiscounts()
        {
            var discounts = await _repository.GetDiscounts();
            return SerializeObject(discounts);
        }

        [HttpGet("applicationSettings")]
        public async Task<string> GetApplicationSettings()
        {
            var applicationSettings = await _repository.GetApplicationSettings();
            return SerializeObject(applicationSettings);
        }

        private string SerializeObject<T>(T bsonObject)
        {
            return JsonConvert.SerializeObject(bsonObject, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        [HttpGet("hints")]
        public async Task<string> GetHints()
        {
            var hints = new Dictionary<string, Hints>();
            foreach (Hints hint in Enum.GetValues(typeof(Hints)))
            {
                hints.Add(Enum.GetName(typeof(Hints), hint), hint);
            }
            return SerializeObject(hints);
        }
    }
}