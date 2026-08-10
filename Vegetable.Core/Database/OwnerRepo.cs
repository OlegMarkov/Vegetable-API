using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vegetable.Core.Extensions;
using Vegetable.Entities;
using Vegetable.Entities.DTO;

namespace Vegetable.Core.Database
{
    public class OwnerRepo : RepoBase, IOwnerRepo
    {

        public OwnerRepo(PostgreDbContext postgreDbContext) : base(postgreDbContext){}


        public async Task<Address> CreateAddress(Guid ownerId, Address address)
        {
            try
            {
                address.OwnerId = ownerId;
                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();
                return address;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Employee> CreateEmployee(Guid ownerId, Employee employee)
        {
            try
            {
                FillOwnerIdFields(employee, ownerId);
                _context.Attach(employee);
                await _context.SaveChangesAsync();
                return await GetEmployee(ownerId, employee.Id);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task ImportCustomers(Guid ownerId, Customer[] customers)
        {
            foreach (var customer in customers)
            {
                await CreateOrUpdateCustomer(ownerId, customer);
            }
        }

        public async Task DeleteCustomer(Guid ownerId, Guid customerId)
        {
            try
            {
                var existingCustomer = _context.Customers.FirstOrDefault(c => c.Id == customerId);
                if (existingCustomer == null)
                {
                    return;
                }
                existingCustomer.IsDeleted = true;
                _context.Update(existingCustomer);

                var reservations = _context.Reservations.Include(r => r.Owner).Where(r => r.CustomerId == customerId).ToList().Where(r => r.StartTime.DateTimeToLocal(r.Owner.TimeZone) > DateTime.UtcNow.DateTimeToLocal(r.Owner.TimeZone)).ToList();

                if (reservations.Any())
                {
                    _context.RemoveRange(reservations);
                }

                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<Customer> GetCustomer(Guid ownerId, Guid customerId)
        {
            try
            {
                return await GetExistingObjectAsNoTracking<Customer>(ownerId, customerId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Customer> GetCustomerByPhoneNumber(Guid ownerId, string phoneNumber)
        {
            try
            {
                var customer = _context.Customers
                     .AsNoTracking().Where(x => x.OwnerId == ownerId && x.IsDeleted == false && x.Phone == phoneNumber).FirstOrDefaultAsync();

                if (customer != null)
                {
                    return await customer;
                }

                else
                {
                    return await _context.Customers
                    .AsNoTracking().Where(x => x.OwnerId == ownerId && x.IsDeleted == false && x.Phone.EndsWith(phoneNumber)).FirstOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Customer[]> GetCustomers(Guid ownerId)
        {
            var customers = await _context.Customers
                .Where(x => x.OwnerId == ownerId)
                .ToArrayAsync();

            return customers;
        }

        public async Task<bool> UpdateCustomerChatId(Guid customerId, long? chatId, string chatLanguage)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == customerId);
            if (customer == null) return false;
            customer.ChatId = chatId;
            customer.ChatLanguage = chatLanguage;
            _context.Entry(customer).Property(x => x.ChatId).IsModified = true;
            _context.Entry(customer).Property(x => x.ChatLanguage).IsModified = true;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<long?> GetChatIdByCustomerId(Guid customerId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == customerId);
            if (customer == null) return null;
            return customer.ChatId;
        }

        public async Task<bool> RemoveChatIdFromCustomers(long? chatId)
        {
            var customers = await _context.Customers.Where(x => x.ChatId == chatId.Value).ToListAsync();
            if (customers == null || !customers.Any()) return false;
            foreach (var customer in customers)
            {
                customer.ChatId = null;
                customer.ChatLanguage = String.Empty;
                _context.Entry(customer).Property(x => x.ChatId).IsModified = true;
                _context.Entry(customer).Property(x => x.ChatLanguage).IsModified = true;
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateCustomerSendConfirmationSms(Guid customerId, bool sendConfirmationSms)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == customerId);
            if (customer == null) return false;
            customer.SendConfirmationSms = sendConfirmationSms;            
            _context.Entry(customer).Property(x => x.SendConfirmationSms).IsModified = true;            
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Owner> CreateOwner(Owner owner)
        {
            try
            {
                //TODO: for mvp version need to create employee with owner
                owner.Employees = new List<Employee>();
                var schedule = new Schedule(ScheduleType.Week) { Owner = owner };
                foreach (var v in schedule.ScheduleOnDays) v.Owner = owner;
                owner.Employees.Add(new Employee() { Schedules = new List<Schedule>() { schedule } });
                owner.Services = new List<Service>();
                owner.Customers = new List<Customer>();
                owner.Addresses = new List<Address>();
                owner.Addresses.Add(new Address());
                owner.PhoneNumbers = new List<PhoneNumber>();
                owner.PhoneNumbers.Add(new PhoneNumber());
                owner.SocialNetworks = new List<SocialNetwork>();
                owner.SocialNetworks.Add(new SocialNetwork { Type = SocialNetworkTypes.Facebook });
                owner.SocialNetworks.Add(new SocialNetwork { Type = SocialNetworkTypes.VK });
                owner.SocialNetworks.Add(new SocialNetwork { Type = SocialNetworkTypes.Instagram });
                //TODO: get currency based on locale
                owner.Currency = _context.Currencies.ToArray()[0];
                _context.Owners.Add(owner);
                await _context.SaveChangesAsync();
                return owner;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<PhoneNumber[]> GetPhoneNumbers(Guid ownerId)
        {
            try
            {
                return await _context.PhoneNumbers
                    .AsNoTracking()
                    .Where(x => x.OwnerId == ownerId).ToArrayAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<PhoneNumber> GetPhoneNumbersById(Guid ownerId, Guid phoneNumberId)
        {
            try
            {
                return await _context.PhoneNumbers
                    .AsNoTracking()
                    .Where(x => x.OwnerId == ownerId && x.Id == phoneNumberId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<PhoneNumber> CreatePhoneNumber(Guid ownerId, PhoneNumber phoneNumber)
        {
            try
            {
                phoneNumber.OwnerId = ownerId;
                _context.PhoneNumbers.Add(phoneNumber);
                await _context.SaveChangesAsync();
                return phoneNumber;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Reservation> GetReservation(Guid ownerId, Guid reservationId)
        {
            try
            {
                return await GetExistingObjectAsNoTracking<Reservation>(ownerId, reservationId);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Reservation> GetReservationById(Guid ownerId, Guid reservationId)
        {
            try
            {
                return await _context.Reservations.AsNoTracking().Include(t => t.Customer).Include(t => t.ReservationServices).ThenInclude(t => t.Service).Where(x => x.OwnerId == ownerId && x.Id == reservationId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Reservation> GetReservationInfo(Guid ownerId, Guid reservationId)
        {
            try
            {
                return await _context.Reservations.AsNoTracking().Include(t => t.Customer).Include(t=>t.Employee).ThenInclude(emp => emp.Address).Include(t=>t.ReservationServices).ThenInclude(service => service.Service).FirstOrDefaultAsync(x => x.OwnerId == ownerId && x.Id == reservationId);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Reservation[]> GetReservations(Guid ownerId)
        {
            try
            {
                return await _context.Reservations.AsNoTracking().Include(t => t.Customer).Where(x => x.OwnerId == ownerId).ToArrayAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Reservation[]> GetReservations(Guid ownerId, DateTime dateTime, string timeZone)
        {
            try
            {                
                var startDateUTC = dateTime.StartDateUTC(timeZone).AddDays(-3);
                var endDateUTC = dateTime.EndDateUTC(timeZone).AddDays(3);
                return await _context.Reservations.AsNoTracking().Include(t => t.Customer).Include(t => t.ReservationServices).Where(x => x.OwnerId == ownerId &&
                x.StartTime >= startDateUTC && x.EndTime <= endDateUTC).ToArrayAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Reservation[]> GetReservationsByDate(Guid ownerId, DateTime dateTime, string timeZone)
        {
            try
            {
                var startDateUTC = dateTime.StartDateUTC(timeZone);
                var endDateUTC = dateTime.EndDateUTC(timeZone);
                return await _context.Reservations.AsNoTracking().Include(t => t.Customer).Where(x => x.OwnerId == ownerId &&
                x.StartTime >= startDateUTC && x.EndTime <= endDateUTC).ToArrayAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Reservation[]> GetReservationsByTimeRange(Guid ownerId, DateTime startDateTime, DateTime endDateTime, string timeZone)
        {
            try
            {
                var startDateUTC = startDateTime.DateTimeToUtc(timeZone);
                var endDateUTC = endDateTime.DateTimeToUtc(timeZone);
                return await _context.Reservations.AsNoTracking().Include(t => t.Customer).Where(x => x.OwnerId == ownerId &&
                ((startDateUTC <= x.StartTime && x.EndTime <= endDateUTC) || (x.StartTime <= startDateUTC && endDateUTC <= x.EndTime)
                    || (startDateUTC <= x.StartTime && x.StartTime < endDateUTC) || (startDateUTC < x.EndTime && x.EndTime <= endDateUTC))).ToArrayAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Dictionary<string,int>> GetReservationsCountByDays(Guid ownerId)
        {
            try
            {
                return await _context.Set<ReservationsCountByDay>()
                    .FromSqlRaw(string.Format(@"
                        SELECT to_char(r.""StartTime"" at time zone 'utc' at time zone o.""TimeZone"", 'YYYY-MM-DD') AS Date, SUM(1) AS Count 
                        FROM public.""Reservations"" r
                        JOIN public.""Owners"" o on r.""OwnerId"" = o.""Id""
                        where ""OwnerId""='{0}' Group By 1;
                        ", ownerId))
                    .AsNoTracking().ToDictionaryAsync(k=>k.Date,v=>v.Count);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Dictionary<int, int>> GetReservationsTotalCostByMonth(Guid ownerId)
        {
            try
            {
                return await _context.Set<ReservationsTotalCostByMonth>()
                    .FromSqlRaw(string.Format(@"
                        SELECT EXTRACT(MONTH FROM r.""StartTime"") as Month, SUM(r.""Cost"") AS TotalCost 
                        FROM public.""Reservations"" r
                        JOIN public.""Owners"" o on r.""OwnerId"" = o.""Id""
                        where ""OwnerId""='{0}' 
                        Group By EXTRACT(MONTH FROM r.""StartTime"");
                        ", ownerId))
                    .AsNoTracking().ToDictionaryAsync(k => (int)k.Month - 1, v => v.TotalCost);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Reservation[]> GetReservationsForSendingReminder(DateTime time)
        {
            try
            {
                return await _context.Reservations.AsNoTracking().Include(x => x.Customer).Include(x=>x.ReservationServices).ThenInclude(x=>x.Service)
                    .Where(x => x.RemindInMin != 0 && time <= x.StartTime.AddMinutes(-x.RemindInMin) && x.StartTime.AddMinutes(-x.RemindInMin) < time.AddMinutes(1)).ToArrayAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Reservation> CreateReservation(Guid ownerId, Reservation reservation)
        {
            try
            {
                reservation.OwnerId = ownerId;
                if (reservation.StartTime.Kind != DateTimeKind.Utc)
                    reservation.StartTime = reservation.StartTime.ToUniversalTime();

                if (reservation.EndTime.Kind != DateTimeKind.Utc)
                    reservation.EndTime = reservation.EndTime.ToUniversalTime();

                if (reservation.Customer.Id == Guid.Empty)
                    reservation.Customer = await CreateOrUpdateCustomer(ownerId, reservation.Customer);
                _context.Attach(reservation);
                await _context.SaveChangesAsync();
                return reservation;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<Reservation> FillReservationCalculatedFields(Guid ownerId, Reservation reservation)
        {
            try
            {
                if (reservation == null) return reservation;
                reservation.OwnerId = ownerId;
                reservation.EndTime = reservation.StartTime;
                reservation.Cost = 0;
                if (reservation.ReservationServices == null || !reservation.ReservationServices.Any()) return reservation;
                
                var servicesIds = reservation.ReservationServices.Select(x => x.ServiceId);
                var services = await _context.Services.Where(x => servicesIds.Contains(x.Id) && x.OwnerId == ownerId).AsNoTracking().ToListAsync();
                if (services.Any())
                    foreach (var service in services)
                    {
                        reservation.ReservationServices.FirstOrDefault(s => s.ServiceId == service.Id).Service = service;
                        reservation.EndTime = reservation.EndTime.AddMinutes(service.DurationInMinutes);
                        reservation.Cost += service.Cost;
                    }
                return reservation;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<Customer> CreateCustomer(Guid ownerId, Customer customer)
        {
            try
            {
                customer.OwnerId = ownerId;
                customer.Phone = !string.IsNullOrEmpty(customer.Phone) ? FormatePhone(customer.Phone) : string.Empty;
                FillOwnerIdFields(customer, ownerId);
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return customer;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task UpdateCustomer(Guid ownerId, Customer customer)
        {
            try
            {
                await GetExistingObjectAsNoTracking(ownerId, customer);
                customer.OwnerId = ownerId;
                customer.Phone = !string.IsNullOrEmpty(customer.Phone) ? FormatePhone(customer.Phone) : string.Empty;
                FillOwnerIdFields(customer, ownerId);
                _context.Update(customer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<Customer> CreateOrUpdateCustomer(Guid ownerId, Customer customer)
        {
            var existingCustomer = await _context.Customers
                .Where(x => !string.IsNullOrEmpty(x.Phone) && x.Phone == customer.Phone && x.OwnerId == ownerId).FirstOrDefaultAsync();
            if (existingCustomer != null)
            {
                existingCustomer.FirstName = string.IsNullOrEmpty(customer.FirstName) ? existingCustomer.FirstName : customer.FirstName;
                existingCustomer.LastName = string.IsNullOrEmpty(customer.LastName) ? existingCustomer.LastName : customer.LastName;
                existingCustomer.Email = string.IsNullOrEmpty(customer.Email) ? existingCustomer.Email : customer.Email;
                if (customer.ChatId != null && customer.ChatId.Value != 0) existingCustomer.ChatId = customer.ChatId;
                existingCustomer.IsDeleted = false;
                FillOwnerIdFields(customer, ownerId);
                _context.Update(existingCustomer);
                await _context.SaveChangesAsync();
                return existingCustomer;
            }
            else
            {
                existingCustomer = await _context.Customers
                    .Where(x => !string.IsNullOrEmpty(x.Email) && x.Email == customer.Email).FirstOrDefaultAsync();
                if (existingCustomer != null)
                {
                    existingCustomer.FirstName = string.IsNullOrEmpty(customer.FirstName) ? existingCustomer.FirstName : customer.FirstName;
                    existingCustomer.LastName = string.IsNullOrEmpty(customer.LastName) ? existingCustomer.LastName : customer.LastName;
                    if (existingCustomer.Phone != null)
                        existingCustomer.Phone = customer.Phone == null || string.IsNullOrEmpty(customer.Phone) ? existingCustomer.Phone : FormatePhone(customer.Phone);
                    else
                        existingCustomer.Phone = FormatePhone(customer.Phone);
                    existingCustomer.IsDeleted = false;
                    if (customer.ChatId != null && customer.ChatId.Value != 0) existingCustomer.ChatId = customer.ChatId;
                    FillOwnerIdFields(customer, ownerId);
                    _context.Update(existingCustomer);
                    await _context.SaveChangesAsync();
                    return existingCustomer;
                }
                else
                {
                    customer.Phone = !string.IsNullOrEmpty(customer.Phone) ? FormatePhone(customer.Phone) : "";
                    FillOwnerIdFields(customer, ownerId);
                    _context.Add(customer);
                    await _context.SaveChangesAsync();
                    return customer;
                }
            }

        }


        public async Task<Schedule> GetSchedule(Guid ownerId, Guid scheduleId)
        {
            try
            {
                return await _context.Schedules
                    .AsNoTracking()
                    .Include(x => x.ScheduleOnDays.OrderBy(so => so.Sequence))
                    .FirstOrDefaultAsync(s => s.Id == scheduleId && s.OwnerId == ownerId); 
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ScheduleOnDay> GetScheduleOnDay(Guid ownerId, Guid scheduleOnDayId)
        {
            try
            {
                var existingObject = await _context.SchedulesOnDays
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == scheduleOnDayId && s.OwnerId == ownerId);

                return existingObject;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Schedule[]> GetSchedules(Guid ownerId, Guid employeeId)
        {
            try
            {
                return await _context.Schedules
                    .AsNoTracking()
                    .Include(s => s.ScheduleOnDays.OrderBy(so => so.Sequence))
                    .Where(s => s.OwnerId == ownerId && s.EmployeeId == employeeId)
                    .ToArrayAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Schedule> CreateSchedule(Guid ownerId, Schedule schedule)
        {
            try
            {
                schedule.OwnerId = ownerId;

                if (schedule.ScheduleOnDays != null && schedule.ScheduleOnDays.Any())
                {
                    foreach (var onDays in schedule.ScheduleOnDays)
                    {
                        onDays.OwnerId = ownerId;
                    }
                }

                _context.Schedules.Add(schedule);
                await _context.SaveChangesAsync();
                return schedule;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<Service> CreateService(Guid ownerId, Service service)
        {
            try
            {
                service.OwnerId = ownerId;
                _context.Attach(service);
                await _context.SaveChangesAsync();
                return service;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<SocialNetwork[]> GetSocialNetworks(Guid ownerId)
        {
            try
            {
                return await _context.SocialNetworks
                    .AsNoTracking()
                    .Where(x => x.OwnerId == ownerId).ToArrayAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<SocialNetwork> GetSocialNetworkById(Guid ownerId, Guid socialNetworkId)
        {
            try
            {
                return await _context.SocialNetworks
                    .AsNoTracking()
                    .Where(x => x.OwnerId == ownerId && x.Id == socialNetworkId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<SocialNetwork> CreateSocialNetwork(Guid ownerId, SocialNetwork socialNetwork)
        {
            try
            {
                socialNetwork.OwnerId = ownerId;
                _context.SocialNetworks.Add(socialNetwork);
                await _context.SaveChangesAsync();
                return socialNetwork;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task DeleteAddress(Guid ownerId, Guid addressId)
        {
            try
            {
                var address = await GetExistingObjectAsNoTracking<Address>(ownerId, addressId);
                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task DeleteEmployee(Guid ownerId, Guid employeeId)
        {
            try
            {
                var employee = await GetExistingObjectAsNoTracking<Employee>(ownerId, employeeId);
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> DeleteOwner(Guid ownerId)
        {
            try
            {
                _context.Owners.Remove(await _context.Owners.FindAsync(ownerId));
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task DeletePhoneNumber(Guid ownerId, Guid phoneNumberId)
        {
            try
            {
                var phoneNumber = await _context.PhoneNumbers
                    .AsNoTracking().FirstOrDefaultAsync(x => x.Id == phoneNumberId && x.OwnerId == ownerId);
                if (phoneNumber == null)
                    throw new UnauthorizedAccessException(string.Format("There is no {0} with Id={1} for Owner with OwnerId={2}.", typeof(PhoneNumber).Name, phoneNumberId, ownerId));
                _context.PhoneNumbers.Remove(phoneNumber);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task DeleteReservation(Guid ownerId, Guid reservationId)
        {
            try
            {

                var notifications = _context.Notifications.AsNoTracking().Where(x => x.OwnerId == ownerId && x.ReservationId == reservationId).ToList();                
               _context.Notifications.RemoveRange(notifications);

                var reservations = await GetExistingObjectAsNoTracking<Reservation>(ownerId, reservationId);
                _context.Reservations.Remove(reservations);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task DeleteSchedule(Guid ownerId, Guid scheduleId)
        {
            try
            {
                _context.SchedulesOnDays.RemoveRange(await _context.SchedulesOnDays.Where(x => x.ScheduleId == scheduleId).ToArrayAsync());
                var schedule = await GetExistingObjectAsNoTracking<Schedule>(ownerId, scheduleId);
                _context.Schedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task DeleteService(Guid ownerId, Guid serviceId)
        {
            try
            {
                var service = await GetExistingObjectAsNoTracking<Service>(ownerId, serviceId);
                if(service == null)
                {
                    return;
                }

                service.IsDeleted = true;
                _context.Update(service);

                var reservations = _context.Reservations.Include(r => r.Owner).Include(r=>r.ReservationServices).Where(r => r.ReservationServices.Any(x=>x.ServiceId == serviceId)).ToList().Where(r => r.StartTime.DateTimeToLocal(r.Owner.TimeZone) > DateTime.UtcNow.DateTimeToLocal(r.Owner.TimeZone)).ToList();

                if (reservations.Any())
                {
                    _context.RemoveRange(reservations);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task DeleteSocialNetwork(Guid ownerId, Guid socialNetworkId)
        {
            try
            {
                var socialNetwork = await GetExistingObjectAsNoTracking<SocialNetwork>(ownerId, socialNetworkId);
                _context.SocialNetworks.Remove(socialNetwork);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }



        public async Task<Address> GetAdress(Guid ownerId, Guid addressId)
        {
            try
            {
                return await GetExistingObjectAsNoTracking<Address>(ownerId, addressId);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Address[]> GetAddresses(Guid ownerId)
        {
            try
            {
                return await _context.Addresses.Where(x => x.OwnerId == ownerId).ToArrayAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Employee> GetEmployee(Guid ownerId, Guid employeeId)
        {
            try
            {
                var existingObject = await _context.Employees
                    .AsNoTracking()
                    .Include(x => x.Reservations)
                    .Include(x => x.Schedules).ThenInclude(x => x.ScheduleOnDays.OrderBy(so => so.Sequence))
                    .Include(x => x.EmployeeServices).ThenInclude(x => x.Service)
                    .FirstOrDefaultAsync(x => x.Id == employeeId && x.OwnerId == ownerId);

                return existingObject;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Employee[]> GetEmployees(Guid ownerId)
        {
            try
            {
                return await _context.Employees
                    .AsNoTracking()
                    .Include(x => x.Reservations)
                    .Include(x => x.Schedules)
                    .Include(x => x.EmployeeServices).ThenInclude(x => x.Service)
                    .Where(x => x.OwnerId == ownerId).ToArrayAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Owner> GetOwner(Guid ownerId, bool onlyOwnerFields = false)
        {
            try
            {
                if (onlyOwnerFields)
                {
                    return await _context.Owners
                        .AsNoTracking().FirstOrDefaultAsync(x => x.Id == ownerId);
                }
                else
                {
                    return await _context.Owners
                        .AsNoTracking()
                        .Include(x => x.Services)
                        .Include(x => x.Employees)
                        .Include(x => x.Addresses)
                        .Include(x => x.PhoneNumbers)
                        .Include(x => x.SocialNetworks)
                        .Include(x => x.Customers)
                        .Include(x => x.Currency)
                        .Include(x => x.Images)
                        .Include(x => x.Schedules)
                            .ThenInclude(z => z.ScheduleOnDays.OrderBy(so => so.Sequence)).AsSplitQuery()
                        .FirstOrDefaultAsync(x => x.Id == ownerId);
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<IEnumerable<Owner>> GetAllOwners()
        {
            try
            {
                return await _context.Owners
                    .AsNoTracking()
                    .Include(x => x.Services)
                    .Include(x => x.Reservations)
                    .Include(x => x.Employees)
                    .Include(x => x.Addresses)
                    .Include(x => x.PhoneNumbers)
                    .Include(x => x.SocialNetworks)
                    .Include(x => x.Currency).ToListAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Owner> GetOwnerInformation(Guid ownerId)
        {
            try
            {
                var owner = await _context.Owners
                     .AsNoTracking()
                     .Include(x => x.Addresses)
                     .Include(x => x.PhoneNumbers)
                     .Include(x => x.SocialNetworks)
                     .Include(x => x.Currency)
                     .Include(x => x.Images)
                     .FirstOrDefaultAsync(x => x.Id == ownerId);

                if (owner.Addresses.Count == 0)
                {
                    owner.Addresses.Add(new Address());
                }

                if (owner.PhoneNumbers.Count == 0)
                {
                    owner.PhoneNumbers.Add(new PhoneNumber());
                }

                if (owner.SocialNetworks.Count == 0)
                {
                    owner.SocialNetworks.Add(new SocialNetwork { Type = SocialNetworkTypes.Facebook });
                    owner.SocialNetworks.Add(new SocialNetwork { Type = SocialNetworkTypes.VK });
                    owner.SocialNetworks.Add(new SocialNetwork { Type = SocialNetworkTypes.Instagram });
                }

                return owner;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Owner> GetOwnerByAlias(string alias)
        {
            try
            {
                return await _context.Owners
                    .AsNoTracking()
                    // Deleted services must not travel. This is what the public
                    // booking page renders, so an unfiltered include let a
                    // customer pick a service the owner had removed — and then
                    // book it, since nothing downstream re-checks. Service is
                    // the only soft-deletable entity in this graph.
                    //
                    // Safe for the reservation lookup that also uses this
                    // method: GetReservationInfo includes ReservationServices
                    // .Service itself, so a past booking still shows the
                    // service it was made for even once that service is gone.
                    .Include(x => x.Services.Where(service => !service.IsDeleted))
                    //.Include(x => x.Customers)
                    //.Include(x => x.Reservations)
                    .Include(x => x.Employees)
                    .Include(x => x.Addresses)
                    .Include(x => x.PhoneNumbers)
                    .Include(x => x.SocialNetworks)
                    .Include(x => x.Images)
                    .Include(x => x.Currency).FirstOrDefaultAsync(x => x.Alias == alias);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Service> GetService(Guid ownerId, Guid serviceId)
        {
            try
            {
                return await _context.Services.AsNoTracking().FirstOrDefaultAsync(x => x.OwnerId == ownerId && x.Id == serviceId);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Service[]> GetServices(Guid ownerId)
        {
            try
            {
                return await _context.Services.AsNoTracking().Where(x => x.OwnerId == ownerId).OrderBy(x => x.Title).ToArrayAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task UpdateAddress(Guid ownerId, Address address)
        {
            try
            {
                await GetExistingObjectAsNoTracking(ownerId, address);
                address.OwnerId = ownerId;
                _context.Update(address);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task UpdateEmployee(Guid ownerId, Employee employee)
        {
            try
            {
                await GetExistingObjectAsNoTracking(ownerId, employee);
                FillOwnerIdFields(employee, ownerId);
                _context.Schedules.RemoveRange(await _context.Schedules.Where(x => x.EmployeeId == employee.Id && !employee.Schedules.Select(xp => xp.Id).Contains(x.Id)).ToArrayAsync());
                _context.EmployeeService.RemoveRange(await _context.EmployeeService.Where(x => x.EmployeeId == employee.Id).ToArrayAsync());
                _context.Update(employee);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task UpdateOwner(Guid ownerId, Owner owner)
        {
            try
            {
                owner.Id = ownerId;
                _context.Addresses.RemoveRange(await _context.Addresses.Where(x => x.OwnerId == owner.Id && !owner.Addresses.Select(xp => xp.Id).Contains(x.Id)).ToArrayAsync());
                _context.Update(owner);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task UpdateOwnerInformation(Guid ownerId, Owner owner)
        {
            try
            {
                owner.Id = ownerId;
                _context.Owners.Attach(owner);
                _context.Entry(owner).Property(o => o.Title).IsModified = true;
                _context.Entry(owner).Property(o => o.Description).IsModified = true;
                _context.Entry(owner).Property(o => o.Alias).IsModified = true;
                _context.Entry(owner).Property(o => o.AllowSite).IsModified = true;
                _context.Entry(owner).Property(o => o.DisableReservationAtSameDay).IsModified = true;
                _context.Entry(owner).Reference(o => o.Currency).IsModified = true;

                foreach (var address in owner.Addresses)
                {
                    _context.Update(address);
                }

                foreach (var phoneNumber in owner.PhoneNumbers)
                {
                    _context.Update(phoneNumber);
                }

                foreach (var socialNetwork in owner.SocialNetworks)
                {
                    _context.Update(socialNetwork);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task UpdateOwnerSubscription(Guid ownerId, Guid orderId)
        {
            try
            {
                var owner = await _context.Owners.FirstOrDefaultAsync(x => x.Id == ownerId);
                var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
                if (owner == null || order == null) return;
                owner.SubscriptionStartDate = owner.SubscriptionStartDate ?? DateTime.UtcNow;
                owner.SubscriptionEndDate = owner.SubscriptionEndDate == null ? DateTime.UtcNow.AddMonths(order.Quantity) : owner.SubscriptionEndDate.Value.AddMonths(order.Quantity);
                owner.SubscriptionTypeId = order.SubscriptionTypeId;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task UpdatePhoneNumber(Guid ownerId, PhoneNumber phoneNumber)
        {
            try
            {
                var existingObject = await _context.PhoneNumbers
                    .AsNoTracking().FirstOrDefaultAsync(x => x.Id == phoneNumber.Id && x.OwnerId == ownerId);
                if (existingObject == null)
                    throw new UnauthorizedAccessException(string.Format("There is no {0} with Id={1} for Owner with OwnerId={2}.", typeof(PhoneNumber).Name, phoneNumber.Id, ownerId));
                phoneNumber.OwnerId = ownerId;
                _context.Update(phoneNumber);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Reservation> UpdateReservation(Guid ownerId, Reservation reservation)
        {
            try
            {
                await GetExistingObjectAsNoTracking(ownerId, reservation);
                reservation.OwnerId = ownerId;
                if (reservation.StartTime.Kind != DateTimeKind.Utc)
                    reservation.StartTime = reservation.StartTime.ToUniversalTime();

                if (reservation.EndTime.Kind != DateTimeKind.Utc)
                    reservation.EndTime = reservation.EndTime.ToUniversalTime();

                _context.ReservationService.RemoveRange(await _context.ReservationService.Where(x => x.ReservationId == reservation.Id).ToArrayAsync());

                _context.Update(reservation);
                await _context.SaveChangesAsync();
                return reservation;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task UpdateSchedule(Guid ownerId, Schedule schedule)
        {
            try
            {
                await GetExistingObjectAsNoTracking(ownerId, schedule);
                schedule.OwnerId = ownerId;
                if (schedule.ScheduleOnDays != null && schedule.ScheduleOnDays.Any())
                {
                    foreach (var onDays in schedule.ScheduleOnDays)
                    {
                        onDays.OwnerId = ownerId;
                    }
                }
                _context.SchedulesOnDays.RemoveRange(await _context.SchedulesOnDays.Where(x => x.ScheduleId == schedule.Id).ToArrayAsync());
                _context.Update(schedule);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task UpdateService(Guid ownerId, Service service)
        {
            try
            {
                await GetExistingObjectAsNoTracking(ownerId, service);
                service.OwnerId = ownerId;
                _context.Update(service);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task UpdateSocialNetwork(Guid ownerId, SocialNetwork socialNetwork)
        {
            try
            {
                await GetExistingObjectAsNoTracking(ownerId, socialNetwork);
                socialNetwork.OwnerId = ownerId;
                _context.Update(socialNetwork);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> VerifyDuplicateAlias(Guid ownerId, string alias)
        {
            try
            {
                return await _context.Owners.AsNoTracking().AnyAsync(x => x.Id != ownerId && x.Alias == alias);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> VerifyReservationTime(Guid ownerId, Reservation reservation, Guid? excludeReservation)
        {
            var existing = await _context.Reservations.AsNoTracking().Where(r => (reservation.StartTime >= r.StartTime && reservation.StartTime <= r.EndTime || reservation.EndTime >= r.StartTime && reservation.EndTime <= r.EndTime) && excludeReservation.HasValue ? r.Id != excludeReservation.Value : true).ToListAsync();
            return !existing.Any();
        }

        private void FillOwnerIdFields(Employee employee, Guid ownerId)
        {
            if (employee.Schedules != null && employee.Schedules.Any())
                foreach (var schedule in employee.Schedules)
                    schedule.OwnerId = ownerId;
            if (employee.Reservations != null && employee.Reservations.Any())
                foreach (var reservation in employee.Reservations)
                    reservation.OwnerId = ownerId;
            employee.OwnerId = ownerId;
        }

        private void FillOwnerIdFields(Customer customer, Guid ownerId)
        {
            if (customer.Reservations != null && customer.Reservations.Any())
                foreach (var reservation in customer.Reservations)
                    reservation.OwnerId = ownerId;

            customer.OwnerId = ownerId;
        }

        public string FormatePhone(string phone)
        {
            if (!string.IsNullOrWhiteSpace(phone))
            {
                return new string(phone.Where(c => char.IsDigit(c) || c == '+').ToArray());
            }

            return string.Empty;
        }

        public async Task<User> GetUser(string auth0User)
        {
            return await _context.User.Include(u => u.UserData).Where(u => u.Auth0UserId == auth0User).FirstOrDefaultAsync();
        }

        public async Task<User> GetUser(Guid ownerId)
        {
            return await _context.User.Include(u => u.UserData).Where(u => u.OwnerId == ownerId).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByPhoneNumber(string phoneNUmber)
        {
            return await _context.User.AsNoTracking().Include(u => u.UserData).Where(u => u.PhoneNumber == phoneNUmber).FirstOrDefaultAsync();
        }

        public async Task<User> AddUser(Guid ownerId, User user)
        {
            try
            {
                var userDb = await _context.User.Include(u => u.UserData).Where(u => u.PhoneNumber == user.PhoneNumber).FirstOrDefaultAsync();
                if (userDb == null)
                {
                    user.AllowNotifications = true;
                    user.ShownHintsFlag = Hints.None;
                    user.OnboardingCompleted = false; 
                    user.DailyNotificationTime = new TimeSpan(9, 0, 0);
                    _context.User.Add(user);
                    await _context.SaveChangesAsync();
                }
                else if (user.UserData != null && user.UserData.Count() > 0)
                {
                    var cid = user.UserData.ToList()[0].CID;
                    var platform = user.UserData.ToList()[0].Platform;
                    var userMetadataDb = await _context.UserData.Include(x=>x.User).Where(ud => ud.CID == cid && ud.User.OwnerId == user.OwnerId).FirstOrDefaultAsync();

                    if (userMetadataDb == null)
                    {
                        userDb.UserData.Add(new UserData { CID = cid, Platform = platform });
                        await _context.SaveChangesAsync();
                    }
                }
                return await _context.User.Include(u => u.UserData).Where(u => u.PhoneNumber == user.PhoneNumber).FirstOrDefaultAsync();
            }
            catch (Exception exc)
            {
                return user;
            }
        }

        public async Task UpdateUser(Guid ownerId, User user)
        {
            try
            {
                await GetExistingObjectAsNoTracking(ownerId, user);
                user.OwnerId = ownerId;
                _context.User.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception exc)
            {
                throw exc;
            }

        }

        public async Task<bool> UpsertUserData(Guid ownerId, string phoneNumber, UserData userData)
        {
            try
            {
                if (string.IsNullOrEmpty(userData.CID)) return false;
                var user = await _context.User.Include(u => u.UserData).Where(u => u.PhoneNumber == phoneNumber && u.OwnerId == ownerId).FirstOrDefaultAsync();
                if (user == null) 
                    throw new UnauthorizedAccessException(string.Format("There is no {0} with Number={1} for Owner with OwnerId={2}.", typeof(User).Name, phoneNumber, ownerId));
                if (user.UserData != null && !user.UserData.Any(x=>x.CID == userData.CID))
                {
                    user.UserData.Add(userData);
                    return await _context.SaveChangesAsync() > 0;
                }
                if(userData != null && userData.Platform != null)
                {
                    var userDataToUpdate = user.UserData.Where(ud => ud.CID == userData.CID).FirstOrDefault();
                    if(userDataToUpdate != null && string.IsNullOrEmpty(userDataToUpdate.Platform))
                    {
                        userDataToUpdate.Platform = userData.Platform;
                        _context.UserData.Update(userDataToUpdate);
                        await _context.SaveChangesAsync();
                    }
                }
                return false;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public async Task<bool> DeleteUserDataFromOtherOwnerIds(Guid ownerId, UserData userData)
        {
            try
            {
                if (string.IsNullOrEmpty(userData.CID)) return false;
                var userDataForRemove = await _context.UserData.Include(x => x.User).Where(x => x.CID == userData.CID && x.User.OwnerId != ownerId).ToArrayAsync();
                if (userDataForRemove != null && userDataForRemove.Length > 0)
                {
                    _context.UserData.RemoveRange(userDataForRemove);
                    return await _context.SaveChangesAsync() > 0;
                }
                return true;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public async Task<bool> DeleteUserDataByCid(Guid ownerId, string cid)
        {
            try
            {
                if (string.IsNullOrEmpty(cid)) return false;
                var userDataForRemove = await _context.UserData.Where(x => x.CID == cid).ToArrayAsync();
                if (userDataForRemove != null && userDataForRemove.Length > 0)
                {
                    _context.UserData.RemoveRange(userDataForRemove);
                    return await _context.SaveChangesAsync() > 0;
                }
                return true;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public async Task<string[]> GetCIDs(Guid ownerId)
        {
            var cids = await _context.UserData
                .Where(u => u.User.OwnerId == ownerId && u.CID != null).Select(u => u.CID)
                .ToArrayAsync();

            return cids;
        }

        public async Task<IEnumerable<UserToPush>> GetUsersToPush(DateTime time)
        {
            // job running every minute, getting all records for current minute

            try
            {
                var searchParam = new NpgsqlParameter("time", time);
                return await _context.Set<UserToPush>()
                   .FromSqlRaw(@"
                        select ud.""CID"", ud.""Platform"", u.""Id"" as UserId, u.""OwnerId"", u.""Language"",
		                        (select count(*) from public.""Reservations"" r
		                         where r.""OwnerId"" = o.""Id"" and
                                 to_char(r.""StartTime"" at time zone 'utc' at time zone o.""TimeZone"", 'YYYY-MM-DD')::date = 
                                 to_char(@time at time zone 'utc' at time zone o.""TimeZone"", 'YYYY-MM-DD')::date) as Count
                        from public.""User"" u
                        JOIN public.""UserData"" ud on u.""Id"" = ud.""UserId""
                        JOIN public.""Owners"" o on u.""OwnerId"" = o.""Id""
                        where u.""AllowNotifications"" = true 
                        and
                        (
	                        u.""DailyNotificationTime"" >= (to_char(@time at time zone o.""TimeZone"", 'HH24:MI')::interval)
	                        and u.""DailyNotificationTime"" < (to_char(@time at time zone o.""TimeZone"", 'HH24:MI')::interval + interval '1 minutes')
                        )
                        or
                        ((u.""DailyNotificationTime"" + '24:00:00' >= (to_char(@time at time zone o.""TimeZone"", 'HH24:MI')::interval)
	                        and u.""DailyNotificationTime"" + '24:00:00' < (to_char(@time at time zone o.""TimeZone"", 'HH24:MI')::interval + interval '1 minutes'))
                        )
                        ", searchParam)
                   .AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }


        public async Task<Image> CreateImage(Guid ownerId, Image image)
        {
            try
            {
                if (image.IsPrimary) _context.Database.ExecuteSqlRaw($"update \"Images\" set \"IsPrimary\" = false where \"OwnerId\"= '{ownerId}'");
                image.OwnerId = ownerId;
                _context.Attach(image);
                await _context.SaveChangesAsync();
                return image;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Image> GetImage(Guid ownerId, Guid imageId)
        {
            try
            {
                return await GetExistingObjectAsNoTracking<Image>(ownerId, imageId);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Image[]> GetImages(Guid ownerId)
        {
            try
            {
                return await _context.Images
                    .AsNoTracking()
                    .Where(x => x.OwnerId == ownerId).ToArrayAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> DeleteImage(Guid ownerId, Guid imageId)
        {
            try
            {
                var image = await GetExistingObjectAsNoTracking<Image>(ownerId, imageId);
                _context.Images.Remove(image);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {

                throw;
            }
        }       

        public async Task<Notification> CreateNotification(Guid ownerId, Notification notification)
        {
            try
            {               
                notification.OwnerId = ownerId;
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();
                return notification;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task UpdateNotification(Guid ownerId, Notification notification)
        {
            try
            {
                await GetExistingObjectAsNoTracking(ownerId, notification);
                notification.OwnerId = ownerId;
                _context.Update(notification);
                await _context.SaveChangesAsync();                
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Notification> GetNotification(Guid ownerId, Guid notificationId)
        {
            try
            {
                return await GetExistingObjectAsNoTracking<Notification>(ownerId, notificationId);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Notification> GetNotificationReminder(Guid ownerId, Guid reservationId)
        {
            try
            {
                return await _context.Notifications
                    .AsNoTracking()
                    .Where(x => x.OwnerId == ownerId && x.ReservationId == reservationId && x.NotificationType == NotificationType.ReminderReservation).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Notification> GetReservationNotification(Guid ownerId, Guid reservationId)
        {
            try
            {
                return await _context.Notifications
                    .AsNoTracking()
                    .Where(x => x.OwnerId == ownerId && x.ReservationId == reservationId && (x.NotificationType == NotificationType.NewReservationClient || x.NotificationType == NotificationType.ChangeReservationClient)).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Notification[]> GetNotifications(Guid ownerId)
        {
            try
            {
                return await _context.Notifications
                    .AsNoTracking()
                    .Where(x => x.OwnerId == ownerId && x.NotificationDateUTC.Date <= DateTime.UtcNow.Date && x.NotificationDateUTC.Date >= DateTime.UtcNow.AddDays(-14).Date).ToArrayAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> DeleteNotification(Guid ownerId, Guid notificationId)
        {
            try
            {
                var notification = await GetExistingObjectAsNoTracking<Notification>(ownerId, notificationId);
                _context.Notifications.Remove(notification);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Notification[]> GetNotificationsForSending(int batchSize = 50)
        {
            try
            {
                return await _context.Notifications
                    .AsNoTracking().Where(x =>  x.NotificationDateUTC.Date <= DateTime.UtcNow.Date && 
                                                x.NotificationDateUTC.Date >= DateTime.UtcNow.AddDays(-14).Date && 
                                                x.SentDateUTC == null && 
                                                x.SendAttempts < 100).OrderBy(x=>x.NotificationDateUTC).Take(batchSize).ToArrayAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
