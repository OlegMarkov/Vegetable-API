using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Vegetable.API.ViewModels
{
    public class CaptchaServiceRequest
    {
        [JsonPropertyName("secret")]
        public string Secret { get; set; }

        [JsonPropertyName("response")]
        public string Token { get; set; }
    }
}
