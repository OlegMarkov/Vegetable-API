using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Vegetable.Core.Storage.Models
{
    public class BotCommand
    {
        string _payload;

        public string Key { get; set; }
        public CommandType Type { get; set; }
        public BotCommand() { }
        public BotCommand(CommandType type, object payload)
        {
            Type = type;
            SetPayload(payload);
        }

        public T GetPayload<T>()
        {
            return JsonConvert.DeserializeObject<T>(_payload);
        }

        public void SetPayload(object payload)
        {
            _payload = JsonConvert.SerializeObject(payload, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
        public enum CommandType
        {
            Subscribe,
            ConfirmReservation,
            SubscribeWithReservation
        }
    }
}
