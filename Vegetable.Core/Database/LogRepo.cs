using System;
using Vegetable.Entities;

namespace Vegetable.Core.Database
{
    public class LogRepo : ILogRepo
    {

        private PostgreDbContext _context { get; }

        public LogRepo(PostgreDbContext context)
        {
            _context = context;
        }

        public void AddLog(Log log)
        {
            try
            {              
                log.Date = DateTime.UtcNow;
                _context.Logs.Add(log);
                _context.SaveChanges();
            }
            catch (Exception exc) 
            {
                throw;
            }
        }
    }
}
