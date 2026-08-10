using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Vegetable.API.ViewModels.Payment;
using Vegetable.Entities;

namespace Vegetable.API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly string _terminalKey;
        private readonly string _password;
        private readonly string[] _filterKeys;
        private readonly string _baseUrl;
        private readonly string _initUrl;
        private readonly string _retryCount;
        private bool _bypassSignCheck;

        private readonly IHttpClientFactory _clientFactory;
        private readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            IgnoreNullValues = true
        };

        public PaymentService(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _terminalKey = configuration["Payment:TerminalKey"];
            _password = configuration["Payment:TerminalPassword"];
            _filterKeys = configuration["Payment:TokenFilterKeys"].Split(',');
            _baseUrl = configuration["Payment:BaseUrl"];
            _initUrl = configuration["Payment:InitUrl"];
            _retryCount = configuration["Payment:RetryCount"];

            _bypassSignCheck = configuration.GetValue("Payment:BypassSignCheck", false);
            _clientFactory = clientFactory;
        }

        public async Task<InitResponse> InitPaymentRequest(Order order, Owner owner, SubscriptionType subscriptionType)
        {
            InitRequest initRequest = new InitRequest() {
                TerminalKey = _terminalKey,
                Amount = order.Amount,
                OrderId = order.Id.ToString(),
                Description = subscriptionType.Name + (order.Quantity > 1 ? " x " + order.Quantity : string.Empty),
                CustomerKey = owner.Id.ToString(),
                Receipt = new Receipt()
                {
                    Email = owner.Email,
                    Phone = "+" + owner.UserId,
                    Taxation = "usn_income",
                    Items = new List<Item>()
                    {
                        new Item
                        {
                            Name = subscriptionType.Name,
                            Quantity = order.Quantity,
                            Amount = order.Amount,
                            Price = order.Amount / order.Quantity,
                            Tax = "none"
                        }
                    }
                },
                DATA = new Dictionary<string, string>()
                {
                    { "Email", owner.Email },
                    { "Phone", "+" + owner.UserId }
                }
            };
            initRequest.Token = GetToken(initRequest);

            var requestDataJson = new StringContent(JsonSerializer.Serialize(initRequest), Encoding.UTF8, "application/json");
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(_baseUrl);
            var retryCount = int.Parse(_retryCount);
            var i = 0;
            while (i < retryCount)
            {
                try
                {
                    var response = await client.PostAsync(_initUrl, requestDataJson);
                    using var responseStream = await response.Content.ReadAsStreamAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        return await JsonSerializer.DeserializeAsync<InitResponse>(responseStream, options);
                    }
                    else
                    {
                        i++;
                        continue;
                    }
                }
                catch (Exception e)
                {
                    if (i++ < retryCount) continue;
                    else throw e;
                }
                
            }
            throw new Exception("Payment Service connection error.");
        }

        public bool CheckSign(PaymentNotificationMessage paymentNotification)
        {
            if (_bypassSignCheck) return true;
            var token = GetToken(paymentNotification);
            return paymentNotification.Token == token;
        }

        private string GetToken<T>(T obj) {

            var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { IgnoreNullValues = true });
            JObject o = JObject.Parse(json);
            foreach (var item in _filterKeys)
            {
                if (o.ContainsKey(item)) o.Property(item).Remove();
            }
            var str = o.ToString();
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(str);
            dictionary = dictionary
                .Where(i => !_filterKeys.Any(x => x.Equals(i.Key, System.StringComparison.OrdinalIgnoreCase)))
                .ToDictionary(i => i.Key, i => i.Value);
            dictionary.Add("Password", _password);
            var concatValues = string.Join(string.Empty, dictionary.OrderBy(x => x.Key).Select(x => x.Value));
            return ComputeSha256Hash(concatValues);
        }

        static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
