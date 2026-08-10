using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vegetable.Core.Database;
using Vegetable.Entities;

namespace Vegetable.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {

        private readonly ILogRepo _logRepo;

        public LogController(ILogRepo logRepo)
        {
            _logRepo = logRepo;
        }
        
        [HttpPost]
        public void Add([FromBody] Log log)
        {
            _logRepo.AddLog(log);
        }
    }
}