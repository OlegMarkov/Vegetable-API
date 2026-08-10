//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Caching.Memory;
//using Microsoft.Extensions.Configuration;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Vegetable.Core.Database;
//using Vegetable.API.Services;
//using Vegetable.Entities;

//namespace Vegetable.API.Controllers
//{
//    [Route("[controller]")]
//    [Authorize]
//    public class CustomerController : Controller
//    {
//        private readonly ICustomerRepo _customerRepo;
//        private readonly IEmailService _emailService;
//        private readonly IMemoryCache _cache;
//        private readonly IConfiguration _configuration;

//        public CustomerController(ICustomerRepo repo, IEmailService emailService, IMemoryCache memoryCache, IConfiguration configuration)
//        {
//            _customerRepo = repo;
//            _emailService = emailService;
//            _cache = memoryCache;
//            _configuration = configuration;
//        }

//        [HttpGet("{id}")]
//        public async Task<string> GetById(Guid id)
//        {
//            var customer = await _customerRepo.GetCustomer(id) ?? new Customer();
//            return SerializeObject(customer);
//        }

//        [HttpGet("GetAllCustomers")]
//        public async Task<string> GetAllCustomers()
//        {
//            var ownerId = Guid.Parse(User.FindFirstValue(_configuration.GetEnvironmentValue("Auth0:ClaimOwnerId")));
//            var customer = await _customerRepo.GetCustomers(ownerId);
//            return customer == null ? "{}" : SerializeObject(customer);
//        }

      

//        [HttpPost("Import")]
//        public async Task<IActionResult> Import([FromBody] Customer[] customers)
//        {
//            var ownerId = Guid.Parse(User.FindFirstValue(_configuration.GetEnvironmentValue("Auth0:ClaimOwnerId")));
//            if (customers.Any())
//            {
//                foreach (var customer in customers)
//                {
//                    customer.CustomerOwners = new List<OwnerCustomer> { new OwnerCustomer
//                {
//                    Customer = (Customer)customer.Clone(),
//                    OwnerId = ownerId
//                }
//                };
//                }


//            }

//            await _customerRepo.ImportCustomers(customers);
//            return new OkResult();
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> Update(Guid id, [FromBody] Customer customer)
//        {
//            await _customerRepo.UpdateCustomer(id, customer);
//            return new OkResult();
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(Guid id)
//        {
//            await _customerRepo.DeleteCustomer(id);
//            return new NoContentResult();
//        }

//        private string SerializeObject<T>(T bsonObject)
//        {
//            return JsonConvert.SerializeObject(bsonObject, new JsonSerializerSettings
//            {
//                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
//                ContractResolver = new CamelCasePropertyNamesContractResolver()
//            });
//        }

//    }
//}