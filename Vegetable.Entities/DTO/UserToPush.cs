using System;

namespace Vegetable.Entities.DTO
{
    public class UserToPush
    {
        public string CID { get; set; }
        public string Platform { get; set; }
        public Guid UserId { get; set; }
        public Guid OwnerId { get; set; }
        public string Language { get; set; }
        public int Count { get; set; }
    }
}
