using EyeExamApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrbitalWitnessAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LeaseController : ControllerBase
    {
        private readonly ILogger<LeaseController> _logger;

        public LeaseController(ILogger<LeaseController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<ParsedScheduleNoticeOfLease> Get()
        {
            var scheduleMgr = new ScheduleManager(new ScheduleParser());

            var schedules = scheduleMgr.GetSchedules();

            return schedules.ToArray();
        }
    }
}
