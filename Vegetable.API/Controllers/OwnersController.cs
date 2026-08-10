using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Vegetable.API.Attributes;
using Vegetable.Core.Database;
using Vegetable.API.Services;
using Vegetable.Entities;
using Vegetable.Core.Services;
using Microsoft.AspNetCore.Http;
using Vegetable.Core.Storage;
using Vegetable.Core.Storage.Models;
using static Vegetable.Core.Storage.Models.BotCommand;

namespace Vegetable.API.Controllers
{
    [Route("[controller]")]
    [AuthorizeOwner]
    public class OwnerController : Controller
    {
        private readonly IOwnerRepo _ownerRepo;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _cache;
        private readonly IUserRepo _userRepo;
        private readonly ILogRepo _logRepo;
        private readonly IConfiguration _configuration;
        private readonly IBotCommandRepository _botCommandRepository;
        private readonly INotificationsService _notificationsService;
        

        private Guid OwnerId => Guid.Parse((string)HttpContext.Items["OwnerId"]);
        private string UserId => (string)HttpContext.Items["UserId"];


        public OwnerController(IOwnerRepo repo, IEmailService emailService, IMemoryCache memoryCache, IUserRepo userRepo,
            IConfiguration configuration, ILogRepo logRepo, IBotCommandRepository botCommandRepository, INotificationsService notificationsService)
        {
            _ownerRepo = repo;
            _emailService = emailService;
            _cache = memoryCache;
            _userRepo = userRepo;
            _configuration = configuration;
            _logRepo = logRepo;
            _botCommandRepository = botCommandRepository;
            _notificationsService = notificationsService;
        }

        [HttpGet("")]
        public async Task<string> GetById()
        {
            var owner = await _ownerRepo.GetOwner(OwnerId);
            return owner == null ? "{}" : SerializeObject(owner);
        }

        [HttpGet("information")]
        public async Task<string> GetOwnerInformation()
        {
            var owner = await _ownerRepo.GetOwnerInformation(OwnerId);
            return owner == null ? "{}" : SerializeObject(owner);
        }

        [HttpGet("search")]
        public async Task<string> GetByAlias([FromQuery]string alias)
        {
            var owner = await _ownerRepo.GetOwnerByAlias(alias);
            return SerializeObject(owner);
        }

        [HttpGet("employee")]
        public async Task<string> GetAllEmployees()
        {
            var employees = await _ownerRepo.GetEmployees(OwnerId);
            return SerializeObject(employees);
        }        

        [HttpGet("employee/{employeeId}")]
        public async Task<string> GetEmployeeById(Guid employeeId)
        {
            var employee = await _ownerRepo.GetEmployee(OwnerId, employeeId) ?? new Employee();
            return SerializeObject(employee);
        }

        //[HttpGet("{id}/employee/{employeeId}/schedule")]
        //public async Task<string> GetEmpoyeeSchedules(string id, Guid employeeId)
        //{
        //    var owner = await _ownerRepo.GetOwner(id);
        //    var employee = owner.Employees.FirstOrDefault(e => e.Id == employeeId);

        //    if (employee != null && employee.Schedules != null)
        //    {
        //        return SerializeObject(employee.Schedules);
        //    }
        //    return null;
        //}

        //[HttpGet("{id}/employee/{employeeId}/schedule/{scheduleId}")]
        //public async Task<string> GetEmployeeScheduleById(string id, Guid employeeId, Guid scheduleId)
        //{
        //    var owner = await _ownerRepo.GetOwner(id);

        //    var employee = owner.Employees.FirstOrDefault(e => e.Id == employeeId);

        //    if (employee != null && employee.Schedules != null)
        //    {
        //        return SerializeObject(employee.Schedules.FirstOrDefault(e => e.Id == scheduleId));
        //    }
        //    return null;
        //}

        [HttpGet("phonenumber")]
        public async Task<string> GetAllPhoneNumbers()
        {
            return SerializeObject(await _ownerRepo.GetPhoneNumbers(OwnerId));
        }

        [HttpGet("phonenumber/{phoneNumberId}")]
        public async Task<string> GetPhoneNumberById(Guid phoneNumberId)
        {
            return SerializeObject(await _ownerRepo.GetPhoneNumbersById(OwnerId, phoneNumberId));
        }

        [HttpGet("socialnetwork")]
        public async Task<string> GetAllSocialNetworks()
        {
            return SerializeObject(await _ownerRepo.GetSocialNetworks(OwnerId));
        }

        [HttpGet("socialnetwork/{socialNerworkId}")]
        public async Task<string> GetSocialNetworkById(Guid socialNerworkId)
        {
            return SerializeObject(await _ownerRepo.GetSocialNetworkById(OwnerId, socialNerworkId));
        }

        [HttpGet("address")]
        public async Task<string> GetAllAddresses()
        {
            var addresses = await _ownerRepo.GetAddresses(OwnerId);
            return SerializeObject(addresses);
        }

        [HttpGet("address/{addressId}")]
        public async Task<string> GetAddressById(Guid addressId)
        {
            var address = await _ownerRepo.GetAdress(OwnerId, addressId);
            return SerializeObject(address);
        }

        [HttpPost]
        public async Task<string> Create([FromBody] Owner owner)
        {
            var createdOwner  = await _ownerRepo.CreateOwner(owner);
            // Update Auth0 user metadata
            _userRepo.UpdateMetadata(createdOwner);
            return SerializeObject(createdOwner);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Owner owner)
        {
            await _ownerRepo.UpdateOwner(OwnerId, owner);
            return new OkResult();
        }

        [HttpPut("information")]
        public async Task<IActionResult> UpdateOwnerInformation([FromBody] Owner owner)
        {
            if (owner?.Addresses?.Count > 0)
            {                
                owner.Addresses.FirstOrDefault().Points = await GetAddressPoints(owner.Addresses.FirstOrDefault());
            }
            await _ownerRepo.UpdateOwnerInformation(OwnerId, owner);
            return new OkResult();
        }

        [HttpGet("duplicate/alias/{alias}")]
        public async Task<bool> VerifyDuplicateAlias(string alias)
        {
            return await _ownerRepo.VerifyDuplicateAlias(OwnerId, alias);
           
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            return await _ownerRepo.DeleteOwner(OwnerId) ? new OkResult() : new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        [HttpGet("service/{serviceId}")]
        public async Task<string> GetServiceById(Guid serviceId)
        {
            var service = serviceId == Guid.Empty ? new Service() : await _ownerRepo.GetService(OwnerId, serviceId);
            return SerializeObject(service);
        }

        [HttpPost("employee")]
        public async Task<string> CreateEmployee([FromBody] Employee employee)
        {
            var newEmployee = await _ownerRepo.CreateEmployee(OwnerId, employee);
            return SerializeObject(newEmployee);
        }

        [HttpPut("employee/{employeeId}")]
        public async Task<IActionResult> UpdateEmployee(string employeeId, [FromBody] Employee employee)
        {
            await _ownerRepo.UpdateEmployee(OwnerId, employee);
            return new OkResult();
        }

        [HttpDelete("employee/{employeeId}")]
        public async Task<IActionResult> DeleteEmployee(Guid employeeId)
        {
            await _ownerRepo.DeleteEmployee(OwnerId, employeeId);
            return new NoContentResult();
        }

        [HttpPost("phonenumber")]
        public async Task<string> CreatePhoneNumber([FromBody] PhoneNumber phoneNumber)
        {
            var result = await _ownerRepo.CreatePhoneNumber(OwnerId, phoneNumber);
            return SerializeObject(result);
        }

        [HttpPut("phonenumber/{phoneNumberId}")]
        public async Task<IActionResult> UpdatePhoneNumber(Guid phoneNumberId, [FromBody] PhoneNumber phoneNumber)
        {
            await _ownerRepo.UpdatePhoneNumber(OwnerId, phoneNumber);
            return new OkResult();
        }

        [HttpDelete("phonenumber")]
        public async Task<IActionResult> DeletePhoneNumber([FromQuery] Guid phoneNumberId)
        {
            await _ownerRepo.DeletePhoneNumber(OwnerId, phoneNumberId);
            return new NoContentResult();
        }

        [HttpPost("address")]
        public async Task<string> CreateAddress([FromBody] Address address)
        {
            var result = await _ownerRepo.CreateAddress(OwnerId, address);
            return SerializeObject(result);
        }

        [HttpPut("address/{addressId}")]
        public async Task<IActionResult> UpdateAddress(Guid addressId, [FromBody] Address address)
        {
            await _ownerRepo.UpdateAddress(OwnerId, address);
            return new OkResult();
        }

        [HttpDelete("address/{addressId}")]
        public async Task<IActionResult> DeleteAddress(Guid addressId)
        {
            await _ownerRepo.DeleteAddress(OwnerId, addressId);
            return new NoContentResult();
        }

        [HttpPost("socialnetwork")]
        public async Task<string> CreateSocialNetwork([FromBody] SocialNetwork socialNetwork)
        {
            var result = await _ownerRepo.CreateSocialNetwork(OwnerId, socialNetwork);
            return SerializeObject(result);
        }

        [HttpPut("socialnetwork/{socialNetworkId}")]
        public async Task<IActionResult> UpdateSocialNetwork(Guid socialNetworkId, [FromBody] SocialNetwork socialNetwork)
        {
            await _ownerRepo.UpdateSocialNetwork(OwnerId, socialNetwork);
            return new OkResult();
        }

        [HttpDelete("socialnetwork")]
        public async Task<IActionResult> DeleteSocialNetwork([FromQuery] Guid socialNetworkId)
        {
            await _ownerRepo.DeleteSocialNetwork(OwnerId, socialNetworkId);
            return new NoContentResult();
        }

        [HttpGet("service")]
        public async Task<string> GetAllServices()
        {
            var services = await _ownerRepo.GetServices(OwnerId);
            return SerializeObject(services);
        }

        [HttpPost("service")]
        public async Task<string> CreateService([FromBody] Service service)
        {
            var newService = await _ownerRepo.CreateService(OwnerId, service);
            return SerializeObject(newService);
        }

        [HttpPut("service/{serviceId}")]
        public async Task<IActionResult> UpdateService(Guid serviceId, [FromBody] Service service)
        {
            service.Id = serviceId;
            await _ownerRepo.UpdateService(OwnerId, service);
            return new OkResult(); 
        }

        [HttpDelete("service/{serviceId}")]
        public async Task<IActionResult> DeleteService(Guid serviceId)
        {
            await _ownerRepo.DeleteService(OwnerId, serviceId);
            return new NoContentResult();
        }

        [HttpGet("reservation")]
        public async Task<string> GetReservations()
        {
            var reservations = await _ownerRepo.GetReservations(OwnerId);
            return SerializeObject(reservations);
        }


        [HttpGet("reservation/{dateTime}")]
        public async Task<string> GetReservations(DateTime? dateTime = null, string timezone = null)
        {
            if (dateTime == null)
            {
                dateTime = DateTime.Now;
            }
            var reservations = await _ownerRepo.GetReservations(OwnerId, dateTime.Value, timezone);
            return SerializeObject(reservations);
        }

        [HttpGet("reservation/countbydays")]
        public async Task<string> GetReservationsCountByDays()
        {
            var reservationsCountByDays = await _ownerRepo.GetReservationsCountByDays(OwnerId);
            return SerializeObject(reservationsCountByDays);
        }

        [HttpGet("reservation/costbymonth")]
        public async Task<string> GetReservationsTotalCostByMonth()
        {
            var reservationsTotalCostByMonth = await _ownerRepo.GetReservationsTotalCostByMonth(OwnerId);
            return SerializeObject(reservationsTotalCostByMonth);
        }

        [HttpGet("reservation/r/{reservationId}")]
        public async Task<string> GetReservationById(Guid reservationId)
        {
            var reservation = reservationId == Guid.Empty ? new Reservation() : await _ownerRepo.GetReservationById(OwnerId, reservationId);
            return SerializeObject(reservation);
        }

        [HttpPost("reservation")]
        public async Task<string> CreateReservation([FromBody] Reservation reservation)
        {
            var newReservation = await _ownerRepo.CreateReservation(OwnerId, reservation);

            try
            {
                await _notificationsService.ReservationCreated(OwnerId, newReservation);
            }
            catch (Exception exc)
            {
                var error = new Log
                {
                    Date = DateTime.UtcNow,
                    Level = "Error",
                    Text = $"New Reservation Notificartion error: {exc}"
                };

                _logRepo.AddLog(error);
            }

            return SerializeObject(newReservation);
        }

        [HttpPut("reservation/{reservationId}")]
        public async Task<IActionResult> UpdateReservation(Guid reservationId,  [FromBody] Reservation reservation)
        {
            var oldReservation = await _ownerRepo.GetReservationById(OwnerId, reservation.Id);
            var newReservation = await _ownerRepo.UpdateReservation(OwnerId, reservation);

            try
            {
                await _notificationsService.ReservationUpdated(OwnerId, oldReservation, newReservation);
            }
            catch (Exception exc)
            {
                var error = new Log
                {
                    Date = DateTime.UtcNow,
                    Level = "Error",
                    Text = $"Update Reservation Notificartion error: {exc}"
                };

                _logRepo.AddLog(error);
            }

            return new OkResult();
        }


        [HttpDelete("reservation/{reservationId}")]
        public async Task<IActionResult> DeleteReservation(Guid reservationId)
        {
            var reservation = await _ownerRepo.GetReservationById(OwnerId, reservationId);
            await _ownerRepo.DeleteReservation(OwnerId, reservationId);

            try
            {
                await _notificationsService.ReservationDeleted(OwnerId, reservation);
            }
            catch (Exception exc)
            {
                var error = new Log
                {
                    Date = DateTime.UtcNow,
                    Level = "Error",
                    Text = $"Cancel Reservation Notificartion error: {exc}"
                };

                _logRepo.AddLog(error);
            }

            return new NoContentResult();
        }

        [HttpPost("customer")]
        public async Task<string> CreateCustomer([FromBody] Customer customer)
        {
            var newCustomer = await _ownerRepo.CreateCustomer(OwnerId, customer);
            return SerializeObject(newCustomer);
        }

        [HttpPut("customer/{customerId}")]
        public async Task<IActionResult> UpdateCustomer([FromBody] Customer customer)
        {
            await _ownerRepo.UpdateCustomer(OwnerId, customer);
            return new OkResult();
        }

        [HttpPost("customer/import")]
        public async Task<IActionResult> Import([FromBody] Customer[] customers)
        {
            await _ownerRepo.ImportCustomers(OwnerId, customers);
            return new OkResult();
        }


        [HttpDelete("customer/{customerId}")]
        public async Task<IActionResult> DeleteCustomer(Guid customerId)
        {
            await _ownerRepo.DeleteCustomer(OwnerId, customerId);
            return new NoContentResult();
        }

        [HttpGet("customer/{customerId}")]
        public async Task<string> GetById(Guid customerId)
        {
            var customer = customerId == Guid.Empty ? new Customer() : await _ownerRepo.GetCustomer(OwnerId, customerId) ?? new Customer();
            return SerializeObject(customer);
        }

        [HttpGet("customer/sharelink/{customerId}")]
        public async Task<string> GetShareLink(Guid customerId)
        {
            var customer = await _ownerRepo.GetCustomer(OwnerId, customerId);
            if (customer == null) return string.Empty;
            var key = _botCommandRepository.SaveCommand(new BotCommand(CommandType.Subscribe, customerId), TimeSpan.FromHours(48));
            var botName = _configuration["BotConfiguration:BotName"];
            return $"https://t.me/{botName}?start={key}";
        }

        [HttpGet("customer/all")]
        public async Task<string> GetAllCustomers()
        {
            var customer = await _ownerRepo.GetCustomers(OwnerId);
            return customer == null ? "{}" : SerializeObject(customer);
        }

        [HttpGet("user/{phoneNumber}")]
        public async Task<string> GetUser(string phoneNumber)
        {
            var userDb = await _ownerRepo.GetUserByPhoneNumber(phoneNumber);
            return userDb == null ? "{}" : SerializeObject(userDb);
        }

        [HttpPost("user")]
        public async Task<string> AddUser([FromBody] User user)
        {
            var userDb = await _ownerRepo.AddUser(OwnerId, user);
            return userDb == null ? "{}" : SerializeObject(userDb);
        }

        [HttpPut("user")]
        public async Task UpdateUser([FromBody] User user)
        {
            await _ownerRepo.UpdateUser(OwnerId, user);
        }

        [HttpPut("user/userdata/{phoneNumber}")]
        public async Task<IActionResult> UpsertUserData(string phoneNumber, [FromBody] UserData userData)
        {
            await _ownerRepo.DeleteUserDataFromOtherOwnerIds(OwnerId, userData);
            return (await _ownerRepo.UpsertUserData(OwnerId, phoneNumber, userData)) ?  new OkResult() : new BadRequestResult();
        }

        [HttpDelete("user/userdata/{cid}")]
        public async Task<IActionResult> DeleteUserData(string cid)
        {
            return (await _ownerRepo.DeleteUserDataByCid(OwnerId, cid)) ? new OkResult() : new BadRequestResult();
        }

        #region Schedules

        [HttpGet("schedule/all/{employeeId}")]
        public async Task<string> GetAllSchedules(Guid employeeId)
        {
            var schedules = await _ownerRepo.GetSchedules(OwnerId, employeeId);

            var scheduleSortedList = new List<Schedule>();
            var generalSchedule = schedules.FirstOrDefault(s => s.ScheduleType == ScheduleType.Week || s.ScheduleType == ScheduleType.Switch);
            if(generalSchedule != null)
                scheduleSortedList.Add(generalSchedule);
            var upcomingSchedules = schedules.Where(s => s.ScheduleType == ScheduleType.Custom && s.ScheduleStartDate >= DateTime.Now.Date);
            var pastSchedules = schedules.Where(s => s.ScheduleType == ScheduleType.Custom && s.ScheduleStartDate < DateTime.Now.Date).OrderByDescending(o => o.ScheduleStartDate);
            
            scheduleSortedList.AddRange(upcomingSchedules);
            scheduleSortedList.AddRange(pastSchedules);

            return SerializeObject(scheduleSortedList);
        }

        [HttpGet("schedule/{scheduleId}")]
        public async Task<string> GetScheduleById(Guid scheduleId)
        {
            var schedule = await _ownerRepo.GetSchedule(OwnerId, scheduleId) ?? new Schedule(ScheduleType.Week);
            return SerializeObject(schedule);
        }

        [HttpGet("schedule/scheduleOnDay/{scheduleOnDayId}")]
        public async Task<string> GetScheduleOnDayById(Guid scheduleOnDayId)
        {
            var schedule = await _ownerRepo.GetScheduleOnDay(OwnerId, scheduleOnDayId) ?? new ScheduleOnDay();
            return SerializeObject(schedule);
        }

        [HttpPost("schedule")]
        public async Task<string> CreateSchedule([FromBody] Schedule schedule)
        {
            var result = await _ownerRepo.CreateSchedule(OwnerId, schedule);
            return SerializeObject(result);
        }

        [HttpPut("schedule")]
        public async Task<IActionResult> UpdateSchedule([FromBody] Schedule schedule)
        {
            await _ownerRepo.UpdateSchedule(OwnerId, schedule);
            return new OkResult();
        }

        [HttpDelete("schedule/{scheduleId}")]
        public async Task<IActionResult> DeleteSchedule(Guid scheduleId)
        {
            await _ownerRepo.DeleteSchedule(OwnerId, scheduleId);
            return new NoContentResult();
        }

        #endregion

        #region Notifications

        [HttpGet("notification")]
        public async Task<string> GetAllNotifications()
        {
            var notifications = await _ownerRepo.GetNotifications(OwnerId);
            return SerializeObject(notifications);
        }

        [HttpGet("notification/{notificationId}")]
        public async Task<string> GetNotification(Guid notificationId)
        {
            var notification = await _ownerRepo.GetNotification(OwnerId, notificationId);
            return SerializeObject(notification);
        }

        [HttpPost("notification")]
        public async Task<string> CreateNotification([FromBody] Notification notification)
        {
            var newNotification = await _ownerRepo.CreateNotification(OwnerId, notification);
            return SerializeObject(newNotification);
        }

        [HttpPut("notification/{notificationId}")]
        public async Task<IActionResult> UpdateNotification(Guid notificationId, [FromBody] Notification notification)
        {
            notification.Id = notificationId;
            await _ownerRepo.UpdateNotification(OwnerId, notification);
            return new OkResult();
        }

        [HttpDelete("notification/{notificationId}")]
        public async Task<IActionResult> DeleteNotification(Guid notificationId)
        {
            await _ownerRepo.DeleteNotification(OwnerId, notificationId);
            return new NoContentResult();
        }

        #endregion

        [HttpGet("sendverification/{email}")]
        public bool SendVerificationCode(string email)
        {
            try
            {
                Random generator = new Random();
                var code = generator.Next(0, 999999).ToString("D6");
                _emailService.SendVerificationCode(email, code);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(70));

                _cache.Set(email, code, cacheEntryOptions);

                return true;
            }
            catch (Exception exc)
            {
                return false;
            }
        }

        [HttpGet("verifycode/{email}/{code}")]
        public bool VerifyCode(string email, string code)
        {
            string originalCode;
            if (!_cache.TryGetValue(email, out originalCode))
            {
                return false;
            }
            return originalCode == code;
        }

        private string SerializeObject<T>(T bsonObject)
        {
            return JsonConvert.SerializeObject(bsonObject, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        private async Task<string> GetAddressPoints(Address address)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var url = $"https://geocode-maps.yandex.ru/1.x/?apikey=7c1e9c67-7998-46c6-ac8c-160761574e72&format=json&geocode={address.City}+{address.Street}+{address.Unit}";
                    using (HttpResponseMessage response = await client.GetAsync(url))
                    {
                        response.EnsureSuccessStatusCode();
                        using (HttpContent content = response.Content)
                        {
                            string result = await content.ReadAsStringAsync();
                            var resultObject = (JObject)JsonConvert.DeserializeObject(result);
                            return resultObject["response"]["GeoObjectCollection"]["featureMember"][0]["GeoObject"]["Point"]["pos"].ToString();
                        }
                    }
                }
                catch (Exception exc)
                {
                    return "";
                }

            }
        }
    }
}