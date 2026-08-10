using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Vegetable.Core.Database;
using Vegetable.API.Services;
using Vegetable.Entities;
using Microsoft.Extensions.Configuration;
using Vegetable.API.Filters;

namespace Vegetable.API.Controllers
{
    // [Authorize]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserRepo _userRepo;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICallPasswordService _callPasswordService;
        private readonly IMemoryCache _cache;
        private readonly ILogRepo _logRepo;
        private readonly IConfiguration _configuration;

        public UsersController(IUserRepo userRepo, IMemoryCache cache, IAuthenticationService authenticationService, ICallPasswordService callPasswordService, ILogRepo logRepo, IConfiguration configuration)
        {
            _userRepo = userRepo;
            _cache = cache;
            _authenticationService = authenticationService;
            _callPasswordService = callPasswordService;
            _logRepo = logRepo;
            _configuration = configuration;
        }

        // POST: /users/updatemetadata/
        [HttpPost("updatemetadata")]
        public IActionResult UpdateMetadata([FromBody] Owner owner)
        {
            _userRepo.UpdateMetadata(owner);
            return new OkResult();
        }


        //[HttpGet("sendverification/{phoneNumber}")]
        //public bool SendVerificationCode(string phoneNumber)
        //{
        //    try
        //    {
        //        var code = "0";
                
        //        if (phoneNumber.Contains("123456"))
        //        {
        //            code = "123456";
        //            var cacheEntryOptions = new MemoryCacheEntryOptions()
        //                .SetAbsoluteExpiration(TimeSpan.FromSeconds(120));

        //            _cache.Set(phoneNumber, code, cacheEntryOptions);

        //            return true;
        //        }

        //        Random generator = new Random();
        //        code = generator.Next(0, 999999).ToString("D6");

        //        var result = _smsService.SendVerificationCode(phoneNumber, code);

        //        if (result != null)
        //        {
        //            var cacheEntryOptions = new MemoryCacheEntryOptions()
        //                .SetAbsoluteExpiration(TimeSpan.FromSeconds(120));

        //            _cache.Set(phoneNumber, code, cacheEntryOptions);

        //            return true;
        //        }

        //        return false;
        //    }
        //    catch (Exception exc)
        //    {
        //        return false;
        //    }
        //}

        [HttpGet("verifycode/{phone}")]
        public bool VerifyCode(string phone, string code)
        {
            string originalCode;
            if (!_cache.TryGetValue(phone, out originalCode))
            {
                return false;
            }
            return originalCode == code;
        }


        [HttpGet("GetCaptcha/{key}")]
        public string GetCaptcha(string key)
        {
            try
            {
                return Captcha.GenerateCaptchaCode(key, 120, 40, _cache);
            }
            catch (Exception exc)
            {
                var error = new Log
                {
                    Date = DateTime.UtcNow,
                    Level = "Error",
                    Text = $"GetCaptcha error: {exc}"
                };

                _logRepo.AddLog(error);
                return string.Empty;
            }
        }

        [HttpGet("SendVerificationCall/{phoneNumber}")]
        public async Task<IActionResult> SendVerificationCall(string phoneNumber, string key, string captcha)
        {
            try
            {
                // Test back door, and it has to be guarded on the setting being
                // present rather than compared straight through. `captcha` is a
                // query parameter, so it is null when omitted; with SecretCaptcha
                // unset this read null too, null == null matched, and captcha
                // validation was skipped entirely for any request that simply
                // left the parameter off. Leaving the key out must disable the
                // door, not remove the lock.
                var secretCaptcha = _configuration["SecretCaptcha"];
                if (!string.IsNullOrEmpty(secretCaptcha) && captcha == secretCaptcha)
                {
                    var result = await _callPasswordService.SendCallVerification(_configuration["SecretPhone"]);
                    if (result != null)
                    {
                        _cache.Set(phoneNumber, result, TimeSpan.FromSeconds(120));
                        return Ok(true);
                    }
                }
                else if (!Captcha.ValidateCaptchaCode(key, captcha, _cache))
                    return BadRequest(false);

                // The other test back door: any number containing 123456 gets
                // the verification code 123456. This was unconditional, so it
                // was live in production too. Now it needs asking for, and only
                // appsettings.Local/Development ask.
                if (_configuration.GetValue<bool>("AllowTestVerificationCode") && phoneNumber.Contains("123456"))
                {
                    _cache.Set(phoneNumber, "123456", TimeSpan.FromSeconds(120));
                    return Ok(true);
                } else
                {
                    var result = await _callPasswordService.SendCallVerification(phoneNumber);
                    if (result != null)
                    {
                        _cache.Set(phoneNumber, result, TimeSpan.FromSeconds(120));
                        return Ok(true);
                    }
                }
                return BadRequest(false);
            }
            catch (Exception exc)
            {
                return BadRequest(false);
            }
        }

        [ServiceFilter(typeof(QueryTokenFilter))]
        [HttpGet("SendVerificationCallWithToken/{phoneNumber}")]
        public async Task<IActionResult> SendVerificationCall(string phoneNumber)
        {
            try
            {
                if (phoneNumber.Contains("123456"))
                {
                    _cache.Set(phoneNumber, "123456", TimeSpan.FromSeconds(120));
                    return Ok(true);
                }
                else
                {
                    var result = await _callPasswordService.SendCallVerification(phoneNumber);
                    if (result != null)
                    {
                        _cache.Set(phoneNumber, result, TimeSpan.FromSeconds(120));
                        return Ok(true);
                    }
                }
                return BadRequest(false);
            }
            catch (Exception exc)
            {
                return BadRequest(false);
            }
        }

        //[HttpGet("VerifyCallCode/{phoneNumber}")]
        //public bool VerifyCallCode(string phoneNumber, string code)
        //{
        //    string originalCode;
        //    if (!_cache.TryGetValue(phoneNumber, out originalCode))
        //    {
        //        return false;
        //    }
        //    return originalCode == code;
        //}



        // POST: /users/authenticate/
        [HttpPost("authenticate")]
        public async Task<string> Authenticate([FromBody] AuthenticateRequest model)
        {
            if ((string)_cache.Get(model.User.PhoneNumber) != model.Code)
            {
                return null;
            }
            var response = await _authenticationService.Authenticate(model);
            return SerializeObject(response);
        }

        private string SerializeObject<T>(T bsonObject)
        {
            return JsonConvert.SerializeObject(bsonObject, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

    }

    public class AuthenticateRequest
    {
        public string Code { get; set; }

        public string TimeZone { get; set; }

        public string Country { get; set; }

        public User User { get; set; }
    }
}
