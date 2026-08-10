using Microsoft.Extensions.Caching.Memory;
using System;
using Vegetable.Core.Storage.Models;

namespace Vegetable.Core.Storage
{
    public class BotCommandRepository : IBotCommandRepository
    {
        private readonly IMemoryCache _cache;
        public BotCommandRepository(IMemoryCache cache)
        {
            _cache = cache;
        }
        public string SaveCommand(BotCommand command, TimeSpan expiration)
        {
            var key = Guid.NewGuid().ToString("n").Substring(0, 8);
            command.Key = key;
            _cache.Set(key, command, expiration);
            return key;
        }

        public BotCommand GetCommand(string key)
        {
            if (_cache.TryGetValue(key, out BotCommand botCommand))
            {
                return botCommand;
            }
            return null;
        }

        public void RemoveCommand(string key)
        {
            _cache.Remove(key);
        }
    }
}
