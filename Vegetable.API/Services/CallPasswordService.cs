using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Vegetable.API.Services
{
    public class CallPasswordService : ICallPasswordService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };


        public CallPasswordService(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        public bool RequestError { get; private set; }

        public async Task<string> SendCallVerification(string phoneNumber)
        {
            var requestDataJson = new StringContent(
                JsonSerializer.Serialize(new { 
                    user = _configuration["GreenSms:User"], 
                    pass = _configuration["GreenSms:Pass"], 
                    to = phoneNumber }),
                Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["GreenSms:BaseUrl"]);
            CallPasswordFailResponse failResponse = null;
            var retryCount = int.Parse(_configuration["GreenSms:RetryCount"]);
            var i = 0;
            while (i < retryCount)
            {
                var response = await client.PostAsync(_configuration["GreenSms:SendCallUrl"], requestDataJson); 
                using var responseStream = await response.Content.ReadAsStreamAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = await JsonSerializer.DeserializeAsync<CallPasswordSuccessResponse>(responseStream, options);
                    return result.Code;
                }
                else
                {
                    failResponse = await JsonSerializer.DeserializeAsync<CallPasswordFailResponse>(responseStream, options);
                    i++;
                    continue;
                }
            }
            throw new Exception (failResponse != null ? $"Error:{failResponse.Error}| Code:{failResponse.Code}" : "Connection error to GreenSMS");
        }

        class CallPasswordSuccessResponse
        {
            public Guid Request_id { get; set; }
            public string Code { get; set; }
        }

        class CallPasswordFailResponse
        {
            public string Error { get; set; }
            public int Code { get; set; }
        }
    }
}
