using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Threading.Tasks;
using Vegetable.Entities;

namespace Vegetable.Core.Database
{
    public class UserRepo : IUserRepo
    {
        private readonly RestClient _client;
        private readonly Auth0Configuration _configuration;

        public UserRepo(Auth0Configuration configuration)
        {
            _client = new RestClient();
            _configuration = configuration;
        }

        public async void UpdateMetadata(Owner owner)
        {       
            var token = GetToken();
            var userMetadataJson = JsonConvert.SerializeObject(new { user_metadata = new { company_id = owner.Id } });
            var requestUpdateMetadata = new RestRequest(new Uri($"{_configuration.UserUrl}{owner.UserId}"), Method.Patch);
            requestUpdateMetadata.AddHeader("content-type", "application/json");
            requestUpdateMetadata.AddHeader("authorization", $"Bearer {token}");
            requestUpdateMetadata.AddParameter("application/json", userMetadataJson, ParameterType.RequestBody);
            var  responseMetadata = await _client.ExecuteAsync(requestUpdateMetadata);
        }

        /// <summary>
        /// This method returns OwnerId
        /// <param name="token">You cant take token from _httpContextAccessor.HttpContext.User.FindFirst("access_token").Value </param>
        /// </summary>
        public async Task<string> GetOwnerId(string token)
        { 
            var request = new RestRequest(new Uri($"{_configuration.UserInfo}"), Method.Get);
            request.AddHeader("authorization", $"Bearer {token}");
            var response = await _client.ExecuteAsync(request);
            JObject responseUser = JObject.Parse(response.Content);
            return responseUser["https://vegetableproj:eu:auth0:com/company_id"].ToString();
        }

        //TODO:  Investigate if it's possible to use existing token instead of additional request to get token.
        private async Task<string> GetToken()
        {
            var tokenJson = JsonConvert.SerializeObject(new
            {
                client_id = _configuration.UserClientId,
                client_secret = _configuration.UserClientSecret,
                audience = _configuration.UserAudience,
                grant_type = "client_credentials"
            });
            var requestToken = new RestRequest(new Uri(_configuration.UserTokenUrl), Method.Post);
            requestToken.AddHeader("content-type", "application/json");
            requestToken.AddParameter("application/json", tokenJson, ParameterType.RequestBody);
            var response = await _client.ExecuteAsync(requestToken);
            JObject responseToken = JObject.Parse(response.Content);
            return responseToken["access_token"].ToString();
        }
    }

    public class Auth0Configuration
    {
        public string Domain { get; set; }
        public string ApiIdentifier { get; set; }
        public string ClaimOwnerId { get; set; }
        public string UserClientId { get; set; }
        public string UserClientSecret { get; set; }
        public string UserInfo { get; set; }
        public string UserAudience { get; set; }
        public string UserTokenUrl { get; set; }
        public string UserUrl { get; set; }
    }
}
