using EyeExamApi.DTOs;
using OrbitalWitnessAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrbitalWitnessAPI
{
    public class SingletonOperationCache : ISingletonOperationCache
    {
        private Dictionary<int, ParsedScheduleNoticeOfLease> _cache;

        public SingletonOperationCache()
        {
            _cache = new Dictionary<int, ParsedScheduleNoticeOfLease>();
        }

        public List<ParsedScheduleNoticeOfLease> GetAll()
        {
            if (_cache.Values.Any())
            {
                return _cache.Values.ToList();
            }
            return null;
        }

        public void Add(int entry, ParsedScheduleNoticeOfLease parsedScheduleNoticeOfLease)
        {
            lock (_cache)
            {
                if (_cache.ContainsKey(entry) == false)
                {
                    _cache.Add(entry, parsedScheduleNoticeOfLease);
                }
            }
        }

        public ParsedScheduleNoticeOfLease GetOrAdd(int key, Func<ParsedScheduleNoticeOfLease> factory)
        {
            var value = GetOrNull(key);
            if (value != null)
            {
                return value;
            }

            value = factory();

            Add(key, value);

            return value;
        }

        public ParsedScheduleNoticeOfLease GetOrNull(int key)
        {
            lock (_cache)
            {
                if (_cache.TryGetValue(key, out ParsedScheduleNoticeOfLease value))
                {
                    return value;
                }
            }

            return null;
        }

        public void Clear()
        {
            lock (_cache)
            {
                _cache.Clear();
            }
        }

        public bool Remove(int key)
        {
            lock (_cache)
            {
                return _cache.Remove(key);
            }
        }

        public int GetCount()
        {
            lock (_cache)
            {
                return _cache.Count;
            }
        }
    }
}
