using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vegetable.Entities;
using Vegetable.Entities.DTO;

namespace Vegetable.Core.Database
{
    public interface IOwnerRepo
    {
        Task<IEnumerable<Owner>> GetAllOwners();
        Task<Owner> GetOwner(Guid ownerId, bool onlyOwnerFields = false);
        Task<Owner> GetOwnerInformation(Guid ownerId);
        Task<Owner> GetOwnerByAlias(string alias);
        Task<Owner> CreateOwner(Owner owner);
        Task UpdateOwner(Guid ownerId, Owner owner);
        Task UpdateOwnerInformation(Guid ownerId, Owner owner);
        Task UpdateOwnerSubscription(Guid ownerId, Guid orderId);
        Task<bool> DeleteOwner(Guid ownerId);

        Task<Employee> GetEmployee(Guid ownerId, Guid employeeId);
        Task<Employee[]> GetEmployees(Guid ownerId);
        Task<Employee> CreateEmployee(Guid ownerId, Employee employee);
        Task UpdateEmployee(Guid ownerId, Employee employee);
        Task DeleteEmployee(Guid ownerId, Guid employeeId);

        Task<Address> GetAdress(Guid ownerId, Guid addressId);
        Task<Address[]> GetAddresses(Guid ownerId);
        Task<Address> CreateAddress(Guid ownerId, Address address);
        Task UpdateAddress(Guid ownerId, Address address);
        Task DeleteAddress(Guid ownerId, Guid addressId);


        Task<Schedule> GetSchedule(Guid ownerId, Guid scheduleId);
        Task<ScheduleOnDay> GetScheduleOnDay(Guid ownerId, Guid scheduleOnDayId);
        Task<Schedule[]> GetSchedules(Guid ownerId, Guid employeeId);
        Task<Schedule> CreateSchedule(Guid ownerId, Schedule schedule);
        Task UpdateSchedule(Guid ownerId, Schedule schedule);
        Task DeleteSchedule(Guid ownerId, Guid scheduleId);

        Task<PhoneNumber[]> GetPhoneNumbers(Guid ownerId);
        Task<PhoneNumber> GetPhoneNumbersById(Guid ownerId, Guid phoneNumberId);
        Task<PhoneNumber> CreatePhoneNumber(Guid ownerId, PhoneNumber phoneNumber);
        Task UpdatePhoneNumber(Guid ownerId, PhoneNumber phoneNumber);
        Task DeletePhoneNumber(Guid ownerId, Guid phoneNumberId);

        Task<SocialNetwork[]> GetSocialNetworks(Guid ownerId);
        Task<SocialNetwork> GetSocialNetworkById(Guid ownerId, Guid socialNetworkId);
        Task<SocialNetwork> CreateSocialNetwork(Guid ownerId, SocialNetwork socialNetwork);
        Task UpdateSocialNetwork(Guid ownerId, SocialNetwork socialNetwork);
        Task DeleteSocialNetwork(Guid ownerId, Guid socialNetworkId);

        Task<Service> CreateService(Guid ownerId, Service service);
        Task UpdateService(Guid ownerId, Service service);
        Task DeleteService(Guid ownerId, Guid serviceId);

        Task<Service> GetService(Guid ownerId, Guid serviceId);
        Task<Service[]> GetServices(Guid ownerId);

        Task<Reservation> GetReservation(Guid ownerId, Guid reservationId);
        Task<Reservation> GetReservationById(Guid ownerId, Guid reservationId);
        Task<Reservation> GetReservationInfo(Guid ownerId, Guid reservationId);
        Task<Reservation[]> GetReservations(Guid ownerId);
        Task<Reservation> FillReservationCalculatedFields(Guid ownerId, Reservation reservation);

        Task<Reservation[]> GetReservations(Guid ownerId, DateTime dateTime, string timeZone);
        Task<Reservation[]> GetReservationsByDate(Guid ownerId, DateTime dateTime, string timeZone);
        Task<Reservation[]> GetReservationsByTimeRange(Guid ownerId, DateTime startDateTime, DateTime endDateTime, string timeZone);
        Task<Dictionary<string, int>> GetReservationsCountByDays(Guid ownerId);
        Task<Dictionary<int, int>> GetReservationsTotalCostByMonth(Guid ownerId);
        Task<Reservation[]> GetReservationsForSendingReminder(DateTime time);
        Task<Reservation> CreateReservation(Guid ownerId, Reservation reservation);
        Task<Reservation> UpdateReservation(Guid ownerId, Reservation reservation);
        Task DeleteReservation(Guid ownerId, Guid reservationId);

        Task<Customer> CreateCustomer(Guid ownerId, Customer customer);
        Task UpdateCustomer(Guid ownerId, Customer customer);
        Task DeleteCustomer(Guid ownerId, Guid customerId);
        Task<Customer> GetCustomer(Guid ownerId, Guid customerId);
        Task<Customer> GetCustomerByPhoneNumber(Guid ownerId, string phoneNumber);
        Task<Customer[]> GetCustomers(Guid ownerId);
        Task ImportCustomers(Guid ownerId, Customer[] customers);
        Task<bool> UpdateCustomerChatId(Guid customerId, long? chatId, string chatLanguage);
        Task<long?> GetChatIdByCustomerId(Guid customerId);
        Task<bool> RemoveChatIdFromCustomers(long? chatId);
        Task<bool> UpdateCustomerSendConfirmationSms(Guid customerId, bool sendConfirmationSms);


        Task<User> GetUser(string auth0User);
        Task<User> GetUser(Guid ownerId);
        Task<User> GetUserByPhoneNumber(string phoneNumber);
        Task<User> AddUser(Guid ownerId, User user);
        Task UpdateUser(Guid ownerId, User user);
        Task<bool> UpsertUserData(Guid ownerId, string phoneNumber, UserData userData);
        Task<bool> DeleteUserDataFromOtherOwnerIds(Guid ownerId, UserData userData);
        Task<bool> DeleteUserDataByCid(Guid ownerId, string cid);
        Task<string[]> GetCIDs(Guid ownerId);
        Task<IEnumerable<UserToPush>> GetUsersToPush(DateTime time);

        Task<bool> VerifyDuplicateAlias(Guid ownerId, string alias);
        Task<bool> VerifyReservationTime(Guid ownerId, Reservation reservation, Guid? excludeReservation);

        Task<Image> CreateImage(Guid ownerId, Image image);
        Task<Image> GetImage(Guid ownerId, Guid imageId);
        Task<Image[]> GetImages(Guid ownerId);
        Task<bool> DeleteImage(Guid ownerId, Guid imageId);


        Task<Notification> CreateNotification(Guid ownerId, Notification notification);
        Task<Notification> GetNotification(Guid ownerId, Guid notificationId);
        Task<Notification[]> GetNotifications(Guid ownerId);
        Task UpdateNotification(Guid ownerId, Notification notification);
        Task<bool> DeleteNotification(Guid ownerId, Guid notificationId);
        Task<Notification> GetNotificationReminder(Guid ownerId, Guid reservationId);
        Task<Notification> GetReservationNotification(Guid ownerId, Guid reservationId);
        Task<Notification[]> GetNotificationsForSending(int batchSize = 50);
    }
}
