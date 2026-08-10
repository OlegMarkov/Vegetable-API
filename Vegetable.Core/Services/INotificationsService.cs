using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vegetable.Entities;

namespace Vegetable.Core.Services
{
    public interface INotificationsService
    {
        Task ReservationCreated(Guid ownerId, Reservation reservation);

        Task ReservationUpdated(Guid ownerId, Reservation oldReservation, Reservation newReservation);

        Task ReservationDeleted(Guid ownerId, Reservation reservation);

        Task CreateReservationReminder(Reservation reservation);

        Task SendReservationConfirmation(Reservation reservation, string commandKey);
    }
}
