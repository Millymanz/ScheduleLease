using EyeExamApi.DTOs;
using Microsoft.Extensions.Configuration;
using OrbitalWitnessAPI.Interfaces;
using System.Collections.Generic;


namespace OrbitalWitnessAPI
{
    public class ScheduleManager : IScheduleManager
    {
        private IScheduleParser _scheduleParser;
        private IConfigurationRoot _configuration;
        private IRestClient _restClient;

        public ScheduleManager(IScheduleParser scheduleParser, IConfiguration configuration, IRestClient restClient)
        {
            _scheduleParser = scheduleParser;
            _configuration = (IConfigurationRoot)configuration;
            _restClient = restClient;
        }

        public IEnumerable<ParsedScheduleNoticeOfLease> GetSchedules()
        {
            var outcomes = _restClient.GetFromRestService(_configuration.GetSection("MicroservicesConnections").GetSection("RawDataScheduleService").Value);

            return _scheduleParser.Parse(outcomes.Result);
        }
    }
}
