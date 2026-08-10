using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vegetable.API.ViewModels;
using Vegetable.Core.Database;
using Vegetable.Core.Extensions;
using Vegetable.Core.Services;
using Vegetable.Entities;
using Vegetable.API.Filters;
using Microsoft.Extensions.Caching.Memory;
using Vegetable.Core.Storage;
using Vegetable.Core.Storage.Models;
using static Vegetable.Core.Storage.Models.BotCommand;
using Vegetable.Core.Models;
using PhoneNumbers;

namespace Vegetable.API.Controllers
{
    [Route("[controller]")]
    public class PublicOwnerController : Controller
    {
        private readonly IOwnerRepo _ownerRepo;
        private readonly IBotCommandRepository _botCommandRepository;
        private readonly INotificationsService _notificationsService;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        ILogRepo _log;

        public PublicOwnerController(IOwnerRepo repo, INotificationsService notificationsService, IConfiguration configuration, IBotCommandRepository botCommandRepository, IMemoryCache cache, ILogRepo logRepo)
        {
            _ownerRepo = repo;
            _notificationsService = notificationsService;
            _configuration = configuration;
            _botCommandRepository = botCommandRepository;
            _cache = cache;
            _log = logRepo;
            _notificationsService = notificationsService;
        }

        [HttpGet("search/{alias}")]
        public async Task<string> GetByAlias(string alias)
        {
            var owner = await _ownerRepo.GetOwnerByAlias(alias);
            return SerializeObject(owner);
        }

        //[HttpPut("reservation/{alias}")]
        //public async Task<string> CreateReservation(string alias, [FromBody] Reservation reservation)
        //{
        //    var owner = await _ownerRepo.GetOwnerByAlias(alias);

        //    var reservationInRange = await _ownerRepo.GetReservationsByTimeRange(owner.Id, reservation.StartTime, reservation.EndTime, owner.TimeZone);
        //    if (reservationInRange != null && reservationInRange.Any())
        //        return "{}";

        //    if (!IsReservationInSchedule(owner.Id, reservation, owner.TimeZone))
        //        return "{}";

        //    var newReservation = await _ownerRepo.CreateReservation(owner.Id, reservation);

        //    return SerializeObject(newReservation);
        //}

        [ServiceFilter(typeof(QueryTokenFilter))]
        [HttpPut("reservation/{alias}")]
        public async Task<IActionResult> CreateReservation(string alias, [FromBody] Reservation reservation)
        {
            var owner = await _ownerRepo.GetOwnerByAlias(alias);
            if (owner == null) return BadRequest(new { ErrorMessage = "Incorrect org!" });

            reservation = await _ownerRepo.FillReservationCalculatedFields(owner.Id, reservation);

            var reservationInRange = await _ownerRepo.GetReservationsByTimeRange(owner.Id, reservation.StartTime, reservation.EndTime, owner.TimeZone);
            if (reservationInRange != null && reservationInRange.Any())
                return BadRequest(new { ErrorMessage = "This time not avalible!" });

            if (!IsReservationInSchedule(owner.Id, reservation, owner.TimeZone))
                return BadRequest(new { ErrorMessage = "Out of schedule!" });

            var customer = await GetCustomerByPhoneNumber(owner, reservation.Customer.Phone);
            if (customer != null) reservation.Customer = customer;

            if (customer != null && reservation.Customer.ChatId != null && reservation.Customer.ChatId.Value != 0)
            {
                var key = _botCommandRepository.SaveCommand(new BotCommand(CommandType.ConfirmReservation, reservation), TimeSpan.FromMinutes(10));
                await _notificationsService.SendReservationConfirmation(reservation, key);
                return Json(new
                {
                    Type = "CustomerWithTlg",
                    CommandKey = key
                });
            }
            else
            {
                var key = _botCommandRepository.SaveCommand(new BotCommand(CommandType.SubscribeWithReservation, reservation), TimeSpan.FromMinutes(10));
                var botName = _configuration["BotConfiguration:BotName"];
                return Json(new
                {
                    Type = "NoTlg",
                    TlgUrl = $"https://t.me/{botName}?start={key}",
                    CommandKey = key
                });
            }
        }

        [ServiceFilter(typeof(QueryTokenFilter))]
        [HttpGet("verifycode/{phone}")]
        public async Task<IActionResult> VerifyCode(string phone, string code, string commandKey)
        {
            string originalCode;
            if (!_cache.TryGetValue(phone, out originalCode))
            {
                return BadRequest(new { ErrorMessage = "Incorrect code!" });
            }
            if(originalCode != code) return BadRequest(new { ErrorMessage = "Incorrect code!" });

            var command = _botCommandRepository.GetCommand(commandKey);
            if (command == null) return BadRequest(new { ErrorMessage = "Incorrect command!" });
            var reservation = command.GetPayload<Reservation>();
            var owner = await _ownerRepo.GetOwner(reservation.OwnerId, true);
            var reservationInRange = await _ownerRepo.GetReservationsByTimeRange(owner.Id, reservation.StartTime, reservation.EndTime, owner.TimeZone);
            if (reservationInRange != null && reservationInRange.Any())
                return BadRequest(new { ErrorMessage = "Time unavalible!" });

            var newReservation = await _ownerRepo.CreateReservation(reservation.OwnerId, reservation);
            await _notificationsService.ReservationCreated(owner.Id, newReservation);
            _botCommandRepository.RemoveCommand(command.Key);
            return new JsonResult(SerializeObject(newReservation));
        }

        [HttpGet("reservation/{alias}/{id}")]
        public async Task<string> GetReservation(string alias, Guid id)
        {
            var owner = await _ownerRepo.GetOwnerByAlias(alias);
            var reservation = await _ownerRepo.GetReservationInfo(owner.Id, id);

            return SerializeObject(reservation);
        }

        /// <summary>
        /// Whether a reservation falls inside the working hours it is being
        /// booked against. This is the gate on the public booking write, so it
        /// has to agree with GetAvailableSlots: never reject a time that
        /// endpoint offered, and never accept one it did not.
        ///
        /// It disagreed in three ways, all fixed here.
        /// </summary>
        private bool IsReservationInSchedule(Guid ownerId, Reservation reservation, string timeZone)
        {
            var date = reservation.StartTime.Date;

            var startTime = reservation.StartTime.ToUniversalTime();
            var endTime = reservation.EndTime.ToUniversalTime();

            var schedule = GetScheduleByDate(ownerId, reservation.EmployeeId.Value, reservation.StartTime);
            if (schedule == null) return false;

            // Ordered explicitly. The rows arrive from Postgres in no
            // particular order and only the query happens to sort them; if that
            // stopped, this method and GetAvailableSlots would silently read
            // different days and validation would reject valid bookings.
            var days = schedule.ScheduleOnDays.OrderBy(day => day.Sequence).ToArray();
            if (days.Length == 0) return false;

            ScheduleOnDay scheduleOnDay;

            if (schedule.ScheduleType == ScheduleType.Custom)
            {
                scheduleOnDay = days[0];
            }
            else if (schedule.ScheduleType == ScheduleType.Week)
            {
                // (DayOfWeek + 6) % 7, not (int)DayOfWeek. ScheduleOnDays is
                // Monday-first — GetAvailableSlots and PublicOwnerController
                // both index it that way — while DayOfWeek is Sunday-first. The
                // bare cast read the wrong day for all seven: a Monday booking
                // was validated against Tuesday's hours, and a Sunday booking
                // against Monday's.
                scheduleOnDay = days[((int)date.DayOfWeek + 6) % 7];
            }
            else
            {
                // Switch was not handled at all. `scheduleOnDay` stayed a fresh
                // ScheduleOnDay whose times are all TimeSpan.Zero, so work
                // start and end were both midnight and the range test could
                // never pass — every booking against a rotating schedule was
                // refused with "Out of schedule!". Same arithmetic as
                // GetAvailableSlots.
                var elapsed = (date - schedule.ScheduleStartDate).TotalDays;
                var cycle = schedule.OnDays + schedule.OffDays;
                var index = cycle > elapsed ? (int)elapsed : (int)elapsed % cycle;

                if (index < 0 || index >= days.Length) return false;
                scheduleOnDay = days[index];
            }

            // Not checked before, so a booking landed on a day the owner had
            // marked closed as long as the (wrong) day's hours happened to
            // contain it. GetAvailableSlots offers nothing on a disabled day.
            if (!scheduleOnDay.IsEnabled) return false;

            var workStartTime = (date + scheduleOnDay.WorkStartTime).DateTimeToUtc(timeZone);
            var workEndTime = (date + scheduleOnDay.WorkEndTime).DateTimeToUtc(timeZone);
            var breakStartTime = (date + scheduleOnDay.BreakStartTime).DateTimeToUtc(timeZone);
            var breakEndTime = (date + scheduleOnDay.BreakEndTime).DateTimeToUtc(timeZone);

            var reservationInWorkHours = startTime >= workStartTime && endTime <= workEndTime;
            var reservationOurOfBreakTime = scheduleOnDay.EnableBreakTime ? (startTime < breakStartTime && endTime <= breakStartTime)
                                                                            || (startTime >= breakEndTime && endTime > breakEndTime)
                                                                          : true;

            return reservationInWorkHours && reservationOurOfBreakTime;
        }

        [HttpGet("monthslots/{alias}")]
        public async Task<string> GetAvailableMonthSlots(string alias, Guid employeeId, int duration, DateTime? startDate = null, DateTime? endDate = null)
        {
            var owner = await _ownerRepo.GetOwnerByAlias(alias);

            startDate = startDate == null ? DateTime.Now.DateTimeToLocal(owner.TimeZone) : startDate.Value.Date;
            endDate = endDate == null ? DateTime.Now.DateTimeToLocal(owner.TimeZone) : endDate.Value.Date;

            var availableMonthSlots = new List<(DateTime Date, bool Available)>();

            while (startDate <= endDate)
            {
                var date = startDate;
                startDate = startDate.Value.AddDays(1);
                var availableMonthSlot = (Date: date.Value, Available: false);

                if (date == DateTime.Now.DateTimeToLocal(owner.TimeZone).Date && owner.DisableReservationAtSameDay)
                {
                    availableMonthSlots.Add(availableMonthSlot);
                    continue;
                }

                var reservations = await _ownerRepo.GetReservationsByDate(owner.Id, date.Value, owner.TimeZone);
                var schedule = GetScheduleByDate(owner.Id, employeeId, date.Value);
                if (schedule == null) continue;

                var scheduleOnDay = new ScheduleOnDay();

                if (schedule.ScheduleType == ScheduleType.Custom)
                {
                    scheduleOnDay = schedule.ScheduleOnDays.OrderBy(d => d.Sequence).ToArray()[0];
                    if (scheduleOnDay.Schedule.OnDays == 0)
                    {
                        availableMonthSlots.Add(availableMonthSlot);
                        continue;
                    }
                }

                if (schedule.ScheduleType == ScheduleType.Week)
                {
                    scheduleOnDay = schedule.ScheduleOnDays.OrderBy(d => d.Sequence).ToArray()[(int)(date.Value.DayOfWeek + 6) % 7];
                    if (!scheduleOnDay.IsEnabled)
                    {
                        availableMonthSlots.Add(availableMonthSlot);
                        continue;
                    }
                }

                if (schedule.ScheduleType == ScheduleType.Switch)
                {
                    var diffDays = (date.Value - schedule.ScheduleStartDate).TotalDays;
                    var switchDays = schedule.OnDays + schedule.OffDays;
                    var dayIndex = 0;

                    if (switchDays > diffDays)
                    {
                        dayIndex = (int)diffDays;
                    }
                    else
                    {
                        dayIndex = (int)diffDays % switchDays;
                    }

                    if (schedule.ScheduleOnDays.Count() > dayIndex)
                    {
                        scheduleOnDay = schedule.ScheduleOnDays.OrderBy(d => d.Sequence).ToArray()[dayIndex];
                    }
                    else
                    {
                        availableMonthSlots.Add(availableMonthSlot);
                        continue;
                    }
                }

                var workStartTime = (date.Value + scheduleOnDay.WorkStartTime).DateTimeToUtc(owner.TimeZone);
                var workEndTime = (date.Value + scheduleOnDay.WorkEndTime).DateTimeToUtc(owner.TimeZone);
                var breakStartTime = (date.Value + scheduleOnDay.BreakStartTime).DateTimeToUtc(owner.TimeZone);
                var breakEndTime = (date.Value + scheduleOnDay.BreakEndTime).DateTimeToUtc(owner.TimeZone);

                var reservationsFilteredInit = reservations.Where(r => (r.StartTime >= workStartTime && r.StartTime < workEndTime)
                                                       || (r.EndTime > workStartTime && r.EndTime <= workEndTime)).ToList();

                if (scheduleOnDay.EnableBreakTime)
                {
                    reservationsFilteredInit.Add(new Reservation { StartTime = breakStartTime, EndTime = breakEndTime });
                }

                var reservationsFiltered = reservationsFilteredInit.OrderBy(r => r.StartTime).ToArray();

                foreach (var res in reservationsFiltered)
                {
                    if (res.StartTime < workStartTime && res.EndTime > workStartTime)
                        res.StartTime = workStartTime;

                    if (res.EndTime > workEndTime && res.StartTime < workEndTime)
                        res.EndTime = workEndTime;

                }

                var availableTimeRanges = new List<(DateTime Start, DateTime End)>();

                if (reservationsFiltered.Any())
                {
                    for (int i = 0; i < reservationsFiltered.Length + 1; i++)
                    {
                        if (i == 0)
                        {
                            availableTimeRanges.Add((workStartTime, reservationsFiltered[i].StartTime));
                            continue;
                        }

                        if (i == reservationsFiltered.Length)
                        {
                            availableTimeRanges.Add((reservationsFiltered[i - 1].EndTime, workEndTime));
                            continue;
                        }

                        availableTimeRanges.Add((reservationsFiltered[i - 1].EndTime, reservationsFiltered[i].StartTime));
                    }
                }
                else if (scheduleOnDay.EnableBreakTime)
                {
                    availableTimeRanges.Add((workStartTime, breakStartTime));
                    availableTimeRanges.Add((breakEndTime, workEndTime));
                }
                else
                {
                    availableTimeRanges.Add((workStartTime, workEndTime));
                }


                foreach (var (Start, End) in availableTimeRanges)
                {
                    if (Start.AddMinutes(duration) <= End)
                    {
                        availableMonthSlot.Available = true;
                        break;
                    }
                }

                availableMonthSlots.Add(availableMonthSlot);
            }

            return SerializeObject(availableMonthSlots);
        }

        [HttpGet("slots/{alias}")]
        public async Task<string> GetAvailableSlots(string alias, Guid employeeId, int duration, DateTime? date = null, Guid? excludeReservationId = null)
        {
            var owner = await _ownerRepo.GetOwnerByAlias(alias);
            var now = DateTime.UtcNow;

            date = date == null ? now : date.Value.Date;

            var avaiableSlots = new List<DateTime>();
            if (date == now.DateTimeToLocal(owner.TimeZone).Date && owner.DisableReservationAtSameDay)
                return SerializeObject(avaiableSlots);

            var schedule = GetScheduleByDate(owner.Id, employeeId, date.Value);

            var scheduleOnDay = new ScheduleOnDay();

            if (schedule.ScheduleType == ScheduleType.Custom)
            {
                scheduleOnDay = schedule.ScheduleOnDays.OrderBy(d => d.Sequence).ToArray()[0];
                if (scheduleOnDay.Schedule.OnDays == 0)
                {
                    return SerializeObject(avaiableSlots);
                }
            }

            if (schedule.ScheduleType == ScheduleType.Week)
            {
                scheduleOnDay = schedule.ScheduleOnDays.OrderBy(d => d.Sequence).ToArray()[(int)(date.Value.DayOfWeek + 6) % 7];
                if (!scheduleOnDay.IsEnabled)
                {
                    return SerializeObject(avaiableSlots);
                }
            }

            if (schedule.ScheduleType == ScheduleType.Switch)
            {
                var diffDays = (date.Value - schedule.ScheduleStartDate).TotalDays;
                var switchDays = schedule.OnDays + schedule.OffDays;
                var dayIndex = 0;

                if (switchDays > diffDays)
                {
                    dayIndex = (int)diffDays;
                }
                else
                {
                    dayIndex = (int)diffDays % switchDays;
                }

                if (schedule.ScheduleOnDays.Count() > dayIndex)
                {
                    scheduleOnDay = schedule.ScheduleOnDays.OrderBy(d => d.Sequence).ToArray()[dayIndex];
                }
                else
                {
                    return SerializeObject(avaiableSlots);
                }
            }

            var workStartTime = (date.Value + scheduleOnDay.WorkStartTime).DateTimeToUtc(owner.TimeZone);
            workStartTime = now >= workStartTime ? now.TimeRoundUp() : workStartTime;
            var workEndTime = (date.Value + scheduleOnDay.WorkEndTime).DateTimeToUtc(owner.TimeZone);
            workEndTime = now >= workEndTime ? now.TimeRoundUp() : workEndTime;
            var breakStartTime = (date.Value + scheduleOnDay.BreakStartTime).DateTimeToUtc(owner.TimeZone);
            var breakEndTime = (date.Value + scheduleOnDay.BreakEndTime).DateTimeToUtc(owner.TimeZone);

            var reservations = await _ownerRepo.GetReservationsByDate(owner.Id, date.Value, owner.TimeZone);

            var reservationsFilteredInit = reservations.Where(r => ((r.StartTime >= workStartTime && r.StartTime < workEndTime)
                                                        || (r.EndTime > workStartTime && r.EndTime <= workEndTime)) && excludeReservationId.HasValue ? r.Id != excludeReservationId.Value : true).ToList();

            if (scheduleOnDay.EnableBreakTime)
            {
                reservationsFilteredInit.Add(new Reservation { StartTime = breakStartTime, EndTime = breakEndTime });
            }

            var timeline = new TimeLine(workStartTime, workEndTime);

            timeline.Substruct(reservationsFilteredInit.OrderByDescending(r => r.StartTime).Select(x => new DateTimeRange(DateTime.SpecifyKind(x.StartTime, DateTimeKind.Utc), DateTime.SpecifyKind(x.EndTime, DateTimeKind.Utc))));

            avaiableSlots = timeline.GetAvalibleSlots(15, duration);

            return SerializeObject(avaiableSlots);
        }

        [HttpGet("privacypolicy")]
        public ActionResult PrivacyPolicy()
        {
            return View("~/Views/Public/PrivacyPolicy.cshtml");
        }

        [HttpPut("reservation/{alias}/{reservationId}")]
        public async Task<string> UpdateReservation(string alias, Guid reservationId, [FromBody] Reservation reservation)
        {
            Owner owner;
            try
            {
                owner = await _ownerRepo.GetOwnerByAlias(alias);
            }
            catch (Exception ex)
            {
                var error = new Log
                {
                    Date = DateTime.UtcNow,
                    Level = "Error",
                    Text = $"[UpdateReservation]: Error during get owner by alias ({alias}). Exeption: {ex.Message}"
                };

                _log.AddLog(error);            

                return null;
            }

            if (owner == null)
            {
                var error = new Log
                {
                    Date = DateTime.UtcNow,
                    Level = "Error",
                    Text = $"[UpdateReservation]: Could not find owner by alias ({alias})."
                };

                _log.AddLog(error);

                return null;
            }

            try
            {
                if (!_ownerRepo.VerifyReservationTime(owner.Id, reservation, reservation.Id).Result)
                {
                    var error = new Log
                    {
                        Date = DateTime.UtcNow,
                        Level = "Error",
                        Text = $"[UpdateReservation]: Reservation time is already used ({alias})."
                    };

                    _log.AddLog(error);

                    return SerializeObject(new ReservationJsonResult { HasErrors = true, ErrorMessage = "Time_Range_Used" });
                }

                var oldReservation = await _ownerRepo.GetReservationById(owner.Id, reservation.Id);
                var newReservation = await _ownerRepo.UpdateReservation(owner.Id, reservation);

               

                try
                {
                    await _notificationsService.ReservationUpdated(owner.Id, oldReservation, newReservation);
                }
                catch (Exception exc)
                {
                    var error = new Log
                    {
                        Date = DateTime.UtcNow,
                        Level = "Error",
                        Text = $"[UpdateReservation]: Update Reservation Notificartion error: {exc.Message}"
                    };

                    _log.AddLog(error);
                }

                //return SerializeObject(newReservation);

                return SerializeObject(new ReservationJsonResult {Reservation = newReservation });
            }
            catch(Exception ex)
            {
                var error = new Log
                {
                    Date = DateTime.UtcNow,
                    Level = "Error",
                    Text = $"[UpdateReservation]: Update Reservation for {alias} with id {reservationId} error: {ex.Message}"
                };

                _log.AddLog(error);
                return null;
            }            
        }

        [HttpDelete("reservation/{alias}/{reservationId}")]
        public async Task<JsonResult> DeleteReservation(string alias, Guid reservationId)
        {
            Owner owner;
            try
            {
                owner = await _ownerRepo.GetOwnerByAlias(alias);
            }
            catch (Exception ex)
            {
                var error = new Log
                {
                    Date = DateTime.UtcNow,
                    Level = "Error",
                    Text = $"[DeleteReservation]: Error during get owner by alias ({alias}). Exeption: {ex.Message}"
                };

                _log.AddLog(error);

                return Json(new BaseJsonResult() { HasErrors = true });
            }

            if (owner == null)
            {
                var error = new Log
                {
                    Date = DateTime.UtcNow,
                    Level = "Error",
                    Text = $"[DeleteReservation]: Could not find owner by alias ({alias})."
                };

                _log.AddLog(error);

                return Json(new BaseJsonResult() { HasErrors = true });
            }            

            try
            {
                var reservation = await _ownerRepo.GetReservationById(owner.Id, reservationId);                

                await _ownerRepo.DeleteReservation(owner.Id, reservationId);

                try
                {
                    await _notificationsService.ReservationDeleted(owner.Id, reservation);
                }
                catch (Exception exc)
                {
                    var error = new Log
                    {
                        Date = DateTime.UtcNow,
                        Level = "Error",
                        Text = $"Cancel Reservation Notificartion error: {exc}"
                    };

                    _log.AddLog(error);
                }

                return Json(new BaseJsonResult() { HasErrors = false });
            }
            catch(Exception ex)
            {
                var error = new Log
                {
                    Date = DateTime.UtcNow,
                    Level = "Error",
                    Text = $"[UpdateReservation]: Update Reservation for {alias} with id {reservationId} error: {ex.Message}"
                };

                _log.AddLog(error);

                return Json(new BaseJsonResult() { HasErrors = true });
            }
        }

        private async Task<Customer> GetCustomerByPhoneNumber(Owner owner, string phoneNumber)
        {
            if (owner == null)
                return null;

            var phoneNumberUtil = PhoneNumberUtil.GetInstance();
            var metaData = phoneNumberUtil.GetMetadataForRegion(owner.Country ?? "RU").Mobile;


            if(metaData.PossibleLengthCount > 1)
            {
                foreach(var mobileLength in metaData.PossibleLengthList.OrderByDescending(x => x))
                {
                    if (phoneNumber.Length >= mobileLength)
                    {
                        var localPhoneNumber = phoneNumber.Substring(phoneNumber.Length - mobileLength);
                        var customer = await _ownerRepo.GetCustomerByPhoneNumber(owner.Id, localPhoneNumber);
                        if (customer != null)
                        {
                            return customer;
                        }
                    }
                }
            }
            else
            {
                var mobileLength = metaData.ExampleNumber.Length;
                if (phoneNumber.Length >= mobileLength)
                {                    
                    var localPhoneNumber = phoneNumber.Substring(phoneNumber.Length - mobileLength);
                    var customer = await _ownerRepo.GetCustomerByPhoneNumber(owner.Id, localPhoneNumber);
                    if (customer != null)
                    {
                        return customer;
                    }                    
                }
            }

            return null;

        }

        private string SerializeObject<T>(T bsonObject)
        {
            return JsonConvert.SerializeObject(bsonObject, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        private Schedule GetScheduleByDate(Guid ownerId, Guid employeeId, DateTime dateTime)
        {
            var schedules = _ownerRepo.GetSchedules(ownerId, employeeId).Result;

            var scheduleAtDay = schedules.Where(s => s.ScheduleStartDate <= dateTime && dateTime <= s.ScheduleEndDate)
                                            .OrderByDescending(s => s.ScheduleType)
                                            .FirstOrDefault();
            return scheduleAtDay;
        }
    }
}
