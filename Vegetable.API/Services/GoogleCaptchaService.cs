using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Vegetable.API.ViewModels;

namespace Vegetable.API.Services
{
    public class GoogleCaptchaService : IInternalCaptchaService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _apiUrl;
        private readonly string _secretKey;

        public GoogleCaptchaService(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _apiUrl = configuration["Google:CaptchaApiUrl"];
            _secretKey = configuration["Google:Secret"];
            _clientFactory = clientFactory;
        }

        public async Task<BaseCaptchaServiceResponse> SendCallVerification(string token)
        {
            var client = _clientFactory.CreateClient();
            var content = new FormUrlEncodedContent(new[]
             {
                new KeyValuePair<string, string>("secret", _secretKey),
                new KeyValuePair<string, string>("response", token)

            });
            var response = await client.PostAsync(_apiUrl, content);
            using var responseStream = await response.Content.ReadAsStreamAsync();
            if (response.IsSuccessStatusCode)
            {
                return await JsonSerializer.DeserializeAsync<GoogleCaptchaServiceResponse>(responseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    IgnoreNullValues = true
                });
            }

            throw new Exception("Google Captcha Service connection error.");
        }
    }
}
