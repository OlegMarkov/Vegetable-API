using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Vegetable.Core.Database;

namespace Vegetable.UnitTests
{
    public class TestJwtMiddleware 
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public TestJwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context, IOwnerRepo ownerRepo)
        {
            context.Items["OwnerId"] = TestStartup.CurrentOwner.ToString();
            await _next(context);
        }


    }
}
