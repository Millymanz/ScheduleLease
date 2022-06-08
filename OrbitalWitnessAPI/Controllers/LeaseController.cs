using EyeExamApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrbitalWitnessAPI.Interfaces;
using System.Collections.Generic;


namespace OrbitalWitnessAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LeaseController : ControllerBase
    {
        private readonly ILogger<LeaseController> _logger;

        private readonly ISingletonOperationCache _singletonCache;
        private IConfigurationRoot _configuration;

        private IScheduleManager _scheduleMgr;

        public LeaseController(ILogger<LeaseController> logger, ISingletonOperationCache singletonCache, IScheduleManager scheduleMgr, IConfiguration configuration)
        {
            _logger = logger;
            _singletonCache = singletonCache;
            _configuration = (IConfigurationRoot)configuration;

            _scheduleMgr = scheduleMgr;
        }


        /// <summary>
        /// Fetches all available Schedule Lease Notices.
        /// Requested data is cached after initial call
        /// </summary>
        /// <remarks>
        /// Get request
        /// </remarks>
        [HttpGet]
        public IEnumerable<ParsedScheduleNoticeOfLease> Get()
        {
            var results = new List<ParsedScheduleNoticeOfLease>();

            if (_singletonCache.GetCount() == 0)
            {
                var schedules = _scheduleMgr.GetSchedules();

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
