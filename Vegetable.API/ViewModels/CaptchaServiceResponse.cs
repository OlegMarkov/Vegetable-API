using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Vegetable.API.ViewModels
{
    public class BaseCaptchaServiceResponse
    {
        public virtual bool IsSuccess { get; set; }
    }

    public class GoogleCaptchaServiceResponse : BaseCaptchaServiceResponse
    {
        [JsonPropertyName("success")]
        public override bool IsSuccess { get => base.IsSuccess; set => base.IsSuccess = value; }

        [JsonPropertyName("hostname")]
        public string HostName { get; set; }

        [JsonPropertyName("error-codes")]
        public string[] Errors { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }
    }
}
