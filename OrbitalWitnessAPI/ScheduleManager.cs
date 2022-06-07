using EyeExamApi.DTOs;
using OrbitalWitnessAPI.Interfaces;
using Rest;
using System.Collections.Generic;


namespace OrbitalWitnessAPI
{
    public class ScheduleManager 
    {
        IScheduleParser _scheduleParser;

        public ScheduleManager(IScheduleParser scheduleParser)
        {
            _scheduleParser = scheduleParser;
        }

        public IEnumerable<ParsedScheduleNoticeOfLease> GetSchedules()
        {
            var restClient = new RestClient();
            var outcomes = restClient.GetFromRestService("https://localhost:7203/schedules");

            return _scheduleParser.Parse(outcomes);
        }
    }
}
