using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vegetable.Entities
{
    public class User : BaseEntity
    {
        public string Auth0UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool AllowNotifications { get; set; }
        public bool OnboardingCompleted { get; set; }
        public Hints ShownHintsFlag { get; set; }
        public string Language { get; set; }
        public string PhoneNumber { get; set; }
        public TimeSpan DailyNotificationTime { get; set; }
        public ICollection<UserData> UserData { get; set;}
    }
}

[Flags]
public enum Hints
{
    None = 0,
    DashboardSwipe = 1
}
