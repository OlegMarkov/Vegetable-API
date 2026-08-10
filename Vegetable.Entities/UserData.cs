using System;

namespace Vegetable.Entities
{
    public class UserData 
    {
        public Guid Id { get; set; }
        public string CID { get; set; } 
        public string Platform { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
