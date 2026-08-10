using System;
using Vegetable.Core.Storage.Models;

namespace Vegetable.Core.Storage
{
    public interface IBotCommandRepository
    {
        public string SaveCommand(BotCommand command, TimeSpan expiration);
        public BotCommand GetCommand(string key);
        public void RemoveCommand(string key);

    }
}
