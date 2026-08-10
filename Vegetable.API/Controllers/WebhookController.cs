using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Vegetable.API.Services;

namespace Vegetable.API.Controllers
{
    public class WebhookController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromServices] HandleUpdateService handleUpdateService, [FromBody] Update update)
        {
            if (update == null) return Ok();
            await handleUpdateService.EchoAsync(update);
            return Ok();
        }
    }
}
