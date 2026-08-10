using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Vegetable.Core.Extensions;
using Vegetable.API.ViewModels;

namespace Vegetable.UnitTests.API
{
    [TestClass]
    public class ImageTests
    {
        private readonly HttpClient _client;
        private readonly IHost _host;

        public ImageTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var hostBuilder = new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.UseConfiguration(config).UseStartup<TestStartup>();
            });

            // Build and start the IHost
            _host = hostBuilder.Start();

            // Create an HttpClient to send requests to the TestServer
            _client = _host.GetTestClient();

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            TestStartup.CurrentOwner = Guid.Parse("dc56f0d9-d152-4b6e-8044-c7c62a1a4216");
        }

        [TestCategory("Vegetable.API"), TestMethod]
        public async Task CanGetImages()
        {
            Byte[] bytes = File.ReadAllBytes(@"C:\Users\Sensenario\Pictures\massage2.jpeg");
            String file = Convert.ToBase64String(bytes);
            string name = $"massage{DateTime.UtcNow.Ticks.ToString()}.jpg";

            var image = new ImageInfo
            {
                ImageBase64 = file,
                Name = name
            };

            var jsonImageInfo = new StringContent(JsonConvert.SerializeObject(image), Encoding.UTF8, "application/json");
            var addResp = await _client.PostAsync("/images", jsonImageInfo);
            var addedImage = GetObjectFromResponse<string>(addResp).Result;
            addResp.EnsureSuccessStatusCode();

            var uploadedImage = GetObjectFromUrl<string>(String.Format("/images/{0}", name)).Result;


            Assert.AreEqual(uploadedImage, file);
        }

        private async Task<T> GetObjectFromUrl<T>(string url)
        {
            var response = await _client.GetAsync(url);
            return await GetObjectFromResponse<T>(response);
        }

        private async Task<T> GetObjectFromResponse<T>(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsAsync<string>();
            return JsonHelper.ToObject<T>(content);
        }
    }
}
