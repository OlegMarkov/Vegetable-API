using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vegetable.Entities;

namespace Vegetable.API.ViewModels
{
    public class BaseJsonResult
    {
        public bool HasErrors { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class ReservationJsonResult : BaseJsonResult
    {
        public Reservation Reservation { get; set; }
    }
}
