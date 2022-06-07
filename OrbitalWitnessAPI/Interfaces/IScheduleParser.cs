using EyeExamApi.DTOs;
using System.Collections.Generic;

namespace OrbitalWitnessAPI.Interfaces
{
    public interface IScheduleParser
    {
        public IEnumerable<ParsedScheduleNoticeOfLease> Parse(string data);
    }
}
