using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Vegetable.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();

                // The bind address is deliberately not hardcoded here.
                //
                // It used to be UseUrls("http://localhost:5002/"), which is
                // fatal in a container: `localhost` binds the loopback
                // interface only, so the process starts, looks healthy, and is
                // unreachable from outside the container with no error to
                // explain it.
                //
                // ASPNETCORE_URLS now decides. launchSettings.json supplies it
                // for local runs, the Dockerfile sets http://0.0.0.0:5002, and
                // IIS ignores it entirely and uses its own binding.
            });
    }
}
