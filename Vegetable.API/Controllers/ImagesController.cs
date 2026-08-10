using System;
using System.Threading.Tasks;
using AspNetCore.Yandex.ObjectStorage;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Vegetable.API.Attributes;
using Vegetable.Core.Database;
using Vegetable.API.ViewModels;
using Vegetable.Entities;

namespace Vegetable.API.Controllers
{
    [AuthorizeOwner]
    [Route("[controller]")]
    public class ImagesController : Controller
    {
        private readonly YandexStorageService _yaService;
        private readonly IOwnerRepo _repo;
        private readonly IMapper _mapper;

        public ImagesController(YandexStorageService yaService, IOwnerRepo repo, IMapper mapper)
        {
            _yaService = yaService;
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet("{imageName}")]
        public async Task<string> GetImage(string imageName)
        {
            if (!string.IsNullOrWhiteSpace(imageName))
            {
                var byteImage = await _yaService.GetAsByteArrayAsync(imageName);
                return SerializeObject<string>(Convert.ToBase64String(byteImage));
            }

            return null;
        }

        [HttpGet("all")]
        public async Task<string> GetImages()
        {
            var ownerId = Guid.Parse((string)HttpContext.Items["OwnerId"]);
            return SerializeObject(await _repo.GetImages(ownerId));
        }

        [HttpPost]
        public async Task<string> AddImage([FromBody] ImageInfo image)
        {
            if (image != null && !string.IsNullOrWhiteSpace(image.ImageBase64))
            {
                var ownerId = Guid.Parse((string)HttpContext.Items["OwnerId"]);
                var imageByte = ConvertImageToBytyArray(image.ImageBase64);
                image.Name = $"{ownerId}{DateTime.UtcNow.Ticks}";
                image.Url = (await _yaService.PutObjectAsync(imageByte, image.Name)).Result;
                return SerializeObject(await _repo.CreateImage(ownerId, _mapper.Map<Image>(image)));
            }

            return null;
        }

        [HttpDelete("{imageId}")]
        public async Task<bool> DeleteImage(Guid imageId)
        {
            if (imageId != Guid.Empty)
            {
                var ownerId = Guid.Parse((string)HttpContext.Items["OwnerId"]);
                var img = await _repo.GetImage(ownerId, imageId);
                if (img != null)
                    if((await _yaService.DeleteObjectAsync(img.Name)).IsSuccess)
                        return await _repo.DeleteImage(ownerId,imageId);
            }

            return false;
        }

        private string SerializeObject<T>(T bsonObject)
        {
            return JsonConvert.SerializeObject(bsonObject, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        private byte[] ConvertImageToBytyArray(string base64Encoded)
        {
            return Convert.FromBase64String(base64Encoded.Substring(base64Encoded.IndexOf(',') + 1));
        }
    }
}