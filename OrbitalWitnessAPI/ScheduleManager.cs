using EyeExamApi.DTOs;
using Microsoft.Extensions.Configuration;
using OrbitalWitnessAPI.Interfaces;
using Rest;
using System.Collections.Generic;


namespace OrbitalWitnessAPI
{
    public class ScheduleManager : IScheduleManager
    {
        private IScheduleParser _scheduleParser;
        private IConfigurationRoot _configuration;

        public ScheduleManager(IScheduleParser scheduleParser, IConfiguration configuration)
        {
            _scheduleParser = scheduleParser;
            _configuration = (IConfigurationRoot)configuration;
        }

        public IEnumerable<ParsedScheduleNoticeOfLease> GetSchedules()
        {
            var restClient = new RestClient();
            var outcomes = restClient.GetFromRestService(_configuration.GetSection("MicroservicesConnections").GetSection("RawDataScheduleService").Value);

            return _scheduleParser.Parse(outcomes);
        }
    }
}
