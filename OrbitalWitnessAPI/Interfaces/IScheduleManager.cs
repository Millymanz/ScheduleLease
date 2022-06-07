using EyeExamApi.DTOs;
using System.Collections.Generic;

namespace OrbitalWitnessAPI.Interfaces
{
    public interface IScheduleManager
    {
        IEnumerable<ParsedScheduleNoticeOfLease> GetSchedules();
    }
}
