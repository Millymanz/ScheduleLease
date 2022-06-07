using EyeExamApi.DTOs;
using System;
using System.Collections.Generic;

namespace OrbitalWitnessAPI.Interfaces
{
    public interface ISingletonOperationCache
    {
        public List<ParsedScheduleNoticeOfLease> GetAll();
        public void Add(int entry, ParsedScheduleNoticeOfLease parsedScheduleNoticeOfLease);
        public ParsedScheduleNoticeOfLease GetOrAdd(int key, Func<ParsedScheduleNoticeOfLease> factory);
        public ParsedScheduleNoticeOfLease GetOrNull(int key);
        public void Clear();
        public bool Remove(int key);
        public int GetCount();

    }
}
