using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;
using Vegetable.API.Services;

namespace Vegetable.API.Filters
{
    public class QueryTokenFilter : ActionFilterAttribute, IAsyncActionFilter
    {
        private readonly IInternalCaptchaService _intCaptchaService;

        public QueryTokenFilter(IInternalCaptchaService intCaptchaService) => _intCaptchaService = intCaptchaService;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            if (context.HttpContext.Request.Query!= null && 
                context.HttpContext.Request.Query.Any() &&
                !string.IsNullOrEmpty(context.HttpContext.Request.Query["token"]) &&
                (await _intCaptchaService.SendCallVerification(context.HttpContext.Request.Query["token"])).IsSuccess)
            {
                await next();
            }
            else
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
