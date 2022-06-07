using EyeExamApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrbitalWitnessAPI.Interfaces;
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

        private readonly ISingletonOperationCache _singletonCache;

        public LeaseController(ILogger<LeaseController> logger, ISingletonOperationCache singletonCache)
        {
            _logger = logger;
            _singletonCache = singletonCache;
        }

        [HttpGet]
        public IEnumerable<ParsedScheduleNoticeOfLease> Get()
        {
            var results = new List<ParsedScheduleNoticeOfLease>();

            if (_singletonCache.GetCount() == 0)
            {
                var scheduleMgr = new ScheduleManager(new ScheduleParser());

                var schedules = scheduleMgr.GetSchedules();

                foreach (var schedule in schedules)
                {
                    _singletonCache.GetOrAdd(schedule.EntryNumber, () => schedule);
                }
                return _singletonCache.GetAll();
            }            

            return _singletonCache.GetAll();
        }
    }
}
