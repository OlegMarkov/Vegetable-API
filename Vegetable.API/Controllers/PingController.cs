using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Vegetable.Core.Database;
using Vegetable.Core.Services;

namespace Vegetable.API.Controllers
{
    [Route("[controller]")]
    public class PingController : Controller
    {
        private readonly IPushService _pushService;
        private readonly IOwnerRepo _ownerRepo;
        private readonly IConfiguration _configuration;

        public PingController(IPushService pushService, IOwnerRepo ownerRepo, IConfiguration configuration)
        {
            _pushService = pushService;
            _ownerRepo = ownerRepo;
            _configuration = configuration;
        }

        // GET api/values
        [HttpGet]       
        public IEnumerable<string> Get()
        {
            return new string[] { "hello", "world" };
        }

        [HttpGet("testpush")]
        public async Task<string> TestPush()
        {

            var redirectUrl = $"#/pages/reservation/edit?id=3c359b86-ad22-4753-9bea-ae32ac2b6b19";
            //_pushService.PushMessageToApp();           
            await _pushService.PushMessageToSingleAsync("aa5145398a1ac70c1237990709e99142", "test" + DateTime.Now.Ticks, "test push " + DateTime.Now.ToString(), redirectUrl, "android"); //android german
           // return await _pushService.PushMessageToSingleAsync("6dd6c6f83b62ce96c55118de4e3d9a69", "test" + DateTime.Now.Ticks, "test push", redirectUrl); //milykhd iphone
           //_pushService.PushMessageToSingleAsync("b745c439608d808bb4f610d5d3eb867a", "test" + DateTime.Now.Ticks, "test push"); //ios
           return await _pushService.PushMessageToSingleAsync("ec1ec90c1b824d08ed585848e52a97c8", "test" + DateTime.Now.Ticks, "test push", redirectUrl); // ios german
        }


        //TODO: Move to the PublicController 
        //[HttpGet("push")]
        //public async Task SendPush(DateTime time)
        //{
        //    var users = await _ownerRepo.GetUsersToPush(time);

        //    foreach (var user in users)
        //    {
        //        if (user.Count > 0)
        //        {
        //            await _pushService.PushMessageToSingleAsync(user.CID, _configuration["Translations:PushTitle." + user.Language], string.Format(_configuration["Translations:PushContent." + user.Language], user.Count));
        //        }
        //    }
        //}

        //[HttpGet("TestReminders")]
        //public async Task TestReminders(DateTime time)
        //{
        //    var users = await _ownerRepo.GetReservationsForSendingReminder(time);
        //}

        

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "hello world";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpPost("customerMessage/{id}")]
        public async Task<JsonResult> GetWebhookInfo([FromServices] ITelegramBotClient bot, Guid id, [FromBody] string message)
        {
            var chatId = await _ownerRepo.GetChatIdByCustomerId(id);
            if (chatId == null) return new JsonResult(string.Empty);
            await bot.SendTextMessageAsync(chatId: chatId, text: message);
            return new JsonResult(new OkResult());
        }
    }
}
