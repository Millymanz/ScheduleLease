using EyeExamApi.Implementations;
using OrbitalWitnessAPI;
using OrbitalWitnessAPI.Interfaces;
using Xunit;
using System.Linq;
using FluentAssertions;
using System;
using Microsoft.Extensions.Logging;
using Moq;

namespace OrbitalWitness.Tests
{
    public class ScheduleParserTest
    {
        [Fact]
        public void Can_Parse_RawScheduleData()
        {
            var mock = new Mock<ILogger<ScheduleParser>>();
            ILogger<ScheduleParser> logger = mock.Object;

            var scheduleParser = new ScheduleParser(logger);

            var results = scheduleParser.Parse(RequestedData());

            var parsedData = new ParsedScheduleDataService();

            var expectedData = parsedData.GetParsedScheduleNoticeOfLeases();                       

            for (int i = 1; i < 6; i++)
            {
                var expected = expectedData.Where(mk => mk.EntryNumber == i).FirstOrDefault();
                var actual = results.Where(mk => mk.EntryNumber == i).FirstOrDefault();

                actual.Should().BeEquivalentTo(expected);
            }
        }

        [Fact]
        public void Can_Manage_Corrupt_RawScheduleData()
        {
            var mock = new Mock<ILogger<ScheduleParser>>();
            ILogger<ScheduleParser> logger = mock.Object;

            var scheduleParser = new ScheduleParser(logger);

            var results = scheduleParser.Parse("x lo p 12586 @ p");
            results.Count().Should().Be(0);

            results = scheduleParser.Parse("pwgfkwognjoewgnwegowwemgowejg54563634omvwomowmojtt[][][r3t3345335");
            results.Count().Should().Be(0);
        }
        
        [Fact]
        public void Can_Parse_Single_RawScheduleData()
        {
            var mock = new Mock<ILogger<ScheduleParser>>();
            ILogger<ScheduleParser> logger = mock.Object;

            var scheduleParser = new ScheduleParser(logger);

            var results = scheduleParser.Parse("[{\"entryNumber\":\"3\",\"entryDate\":\"\",\"entryType\":\"Schedule of Notices of Leases\",\"entryText\":[\"16.08.2013      21 Sheen Road (Ground floor   06.08.2013      TGL383606  \",\"shop)                         Beginning on               \",\"and including              \",\"6.8.2013 and               \",\"ending on and              \",\"including                  \",\"6.8.2023\"]}]");
            results.Count().Should().Be(1);

            var parsedData = new ParsedScheduleDataService();
            var expectedData = parsedData.GetParsedScheduleNoticeOfLeases();
            var expected = expectedData.Where(mk => mk.EntryNumber == 3).FirstOrDefault();
            var actual = results.Where(mk => mk.EntryNumber == 3).FirstOrDefault();

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Can_Manage_Empty_RawScheduleData()
        {
            var mock = new Mock<ILogger<ScheduleParser>>();
            ILogger<ScheduleParser> logger = mock.Object;

            var scheduleParser = new ScheduleParser(logger);

            var results = scheduleParser.Parse(" ");
            results.Count().Should().Be(0);

            results = scheduleParser.Parse(String.Empty);
            results.Count().Should().Be(0);
        }

        private string RequestedData()
        {
            var data = "[{\"entryNumber\":\"3\",\"entryDate\":\"\",\"entryType\":\"Schedule of Notices of Leases\",\"entryText\":[\"16.08.2013      21 Sheen Road (Ground floor   06.08.2013      TGL383606  \",\"shop)                         Beginning on               \",\"and including              \",\"6.8.2013 and               \",\"ending on and              \",\"including                  \",\"6.8.2023\"]},{\"entryNumber\":\"1\",\"entryDate\":\"\",\"entryType\":\"Schedule of Notices of Leases\",\"entryText\":[\"09.07.2009      Endeavour House, 47 Cuba      06.07.2009      EGL557357  \",\"Edged and       Street, London                125 years from             \",\"numbered 2 in                                 1.1.2009                   \",\"blue (part of)\"]},{\"entryNumber\":\"2\",\"entryDate\":\"\",\"entryType\":\"Schedule of Notices of Leases\",\"entryText\":[\"15.11.2018      Ground Floor Premises         10.10.2018      TGL513556  \",\"Edged and                                     from 10                    \",\"numbered 2 in                                 October 2018               \",\"blue (part of)                                to and                     \",\"including 19               \",\"April 2028\"]},{\"entryNumber\":\"4\",\"entryDate\":\"\",\"entryType\":\"Schedule of Notices of Leases\",\"entryText\":[\"24.07.1989      17 Ashworth Close (Ground     01.06.1989      TGL24029   \",\"Edged and       and First Floor Flat)         125 years from             \",\"numbered 19                                   1.6.1989                   \",\"(Part of) in                                                             \",\"brown                                                                    \",\"NOTE 1: A Deed of Rectification dated 7 September 1992 made between (1) Orbit Housing Association and (2) John Joseph McMahon Nellie Helen McMahon and John George McMahon is supplemental to the Lease dated 1 June 1989 of 17 Ashworth Close referred to above. The lease actually comprises the second floor flat numbered 24 (Part of) on the filed plan. (Copy Deed filed under TGL24029)\",\"NOTE 2: By a Deed dated 23 May 1996 made between (1) Orbit Housing Association (2) John Joseph McMahon Nellie Helen McMahon and John George McMahon and (3) Britannia Building Society the terms of the lease were varied. (Copy Deed filed under TGL24029).\",\"NOTE 3: A Deed dated 13 February 1997 made between (1) Orbit Housing Association (2) John Joseph McMahon and others and (3) Britannia Building Society is supplemental to the lease. It substitutes a new plan for the original lease plan. (Copy Deed filed under TGL24029)\"]},{\"entryNumber\":\"5\",\"entryDate\":\"\",\"entryType\":\"Schedule of Notices of Leases\",\"entryText\":[\"19.09.1989      12 Harbord Close (Ground      01.09.1989      TGL27196   \",\"Edged and       and First Floor Flat)         125 years from             \",\"numbered 25                                   1.9.1989                   \",\"(Part of) in                                                             \",\"brown                                                                    \",\"NOTE: By a Deed dated 20 July 1995 made between (1) Orbit Housing Association and (2) Clifford Ronald Mitchell the terms of the Lease were varied.  (Copy Deed filed under TGL27169)\"]}]";

            return data;
        }
    }
}