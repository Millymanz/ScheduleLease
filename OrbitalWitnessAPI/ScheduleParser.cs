using EyeExamApi.DTOs;
using Microsoft.Extensions.Logging;
using OrbitalWitnessAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrbitalWitnessAPI
{
    public class ScheduleParser : IScheduleParser
    {
        private ILogger<ScheduleParser> _logger;
        public ScheduleParser(ILogger<ScheduleParser> logger) 
        {
            _logger = logger;
        }

        public IEnumerable<ParsedScheduleNoticeOfLease> Parse(string data)
        {
            var result = new List<ParsedScheduleNoticeOfLease>();

            try
            {
                var json = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RawScheduleNoticeOfLease>>(data);

                if (json != null)
                {
                    foreach (var item in json)
                    {
                        var parsedSchedule = ConvertRawSchedule(item);
                        result.Add(parsedSchedule);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return result;
        }

        private ParsedScheduleNoticeOfLease ConvertRawSchedule(RawScheduleNoticeOfLease rawScheduleNoticeOfLease)
        {
            var parsedScheduleNotice = new ParsedScheduleNoticeOfLease();

            if (rawScheduleNoticeOfLease != null)
            {
                var spaceSplits = SpaceSplits("   ");

                var registrationDateAndPlanRefData = new List<string>();
                var propertyDescriptionData = new List<string>();
                var dateOfLeaseAndTermData = new List<string>();
                var lesseesTitleData = new List<string>();
                var notesData = new List<string>();

                for (int row = 0; row < rawScheduleNoticeOfLease.EntryText.Count; row++)
                {
                        InitialiseNotes(notesData, rawScheduleNoticeOfLease.EntryText[row]);

                        var columnData = rawScheduleNoticeOfLease.EntryText[row].Split(spaceSplits, StringSplitOptions.None);

                        columnData = OrganiseData(columnData, row);

                        PopulateColumnData(columnData, registrationDateAndPlanRefData, propertyDescriptionData, dateOfLeaseAndTermData, lesseesTitleData, notesData);
                }

                parsedScheduleNotice.EntryNumber = Convert.ToInt32(rawScheduleNoticeOfLease.EntryNumber);

                if (DateTime.TryParse(rawScheduleNoticeOfLease.EntryDate, out DateTime dateRes))
                {
                    parsedScheduleNotice.EntryDate = DateOnly.FromDateTime(dateRes);
                }

                parsedScheduleNotice.LesseesTitle = ConvertToSentence(lesseesTitleData);
                parsedScheduleNotice.RegistrationDateAndPlanRef = ConvertToSentence(registrationDateAndPlanRefData);
                parsedScheduleNotice.DateOfLeaseAndTerm = ConvertToSentence(dateOfLeaseAndTermData);
                parsedScheduleNotice.PropertyDescription = ConvertToSentence(propertyDescriptionData);
                
                parsedScheduleNotice.Notes = notesData;
            }
            return parsedScheduleNotice;
        }

        private void PopulateColumnData(string[] columnData, List<string> registrationDateAndPlanRefData, List<string> propertyDescriptionData, List<string> dateOfLeaseAndTermData, List<string> lesseesTitleData, List<string> notesData)
        {
            if (columnData.Length > 0)
            {
                for (int i = 3; i >= 0; i--)
                {
                    if (i < columnData.Length)
                    {
                        if (columnData[i].ToLower().Contains("note") == false && string.IsNullOrEmpty(columnData[i]) == false
                            && columnData[i] != " ")
                        {
                            switch (i)
                            {
                                case 0:
                                    {
                                        registrationDateAndPlanRefData.Add(columnData[i].Trim());
                                    }
                                    break;
                                case 1:
                                    {
                                        propertyDescriptionData.Add(columnData[i].Trim());
                                    }
                                    break;
                                case 2:
                                    {
                                        dateOfLeaseAndTermData.Add(columnData[i].Trim());
                                    }
                                    break;
                                case 3:
                                    {
                                        lesseesTitleData.Add(columnData[i].Trim());
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }
        private void InitialiseNotes(List<string> notesData, string entry)
        {
            bool previousNotesFlag = false;
            var extractedNotes = string.Empty;

            if (ExtractNotes(entry.ToString(), out extractedNotes) || previousNotesFlag)
            {
                if (previousNotesFlag)
                {
                    notesData.Add(entry.ToString());
                }
                else
                {
                    notesData.Add(extractedNotes);
                    previousNotesFlag = true;
                }
            }
        }

        private bool ExtractNotes(string text, out string outputString)
        {
            outputString = string.Empty;

            if (text.ToLower().Contains("note"))
            {
                var noteIndex = text.ToLower().IndexOf("note");
                var lastCharacterPositon = text.Length;

                outputString = text.Substring(noteIndex, lastCharacterPositon);
                return true;
            }
            return false;
        }

        private string ConvertToSentence(List<string> columnData)
        {
            var sentence = "";
            foreach (var item in columnData)
            {
                sentence += item + " ";
            }

            return sentence.Trim();
        }

        private bool RegisterationRelatedKeywords(string keyword)
        {
            var regRelatedKeywords = new string[] { "edged", "numbered", "part of" };

            foreach (var item in regRelatedKeywords)
            {
                if (keyword.ToLower().Contains(item))
                {
                    return true;
                }

                var spaceSplits = SpaceSplits(" ");
                var dateTextSplit = keyword.Split(spaceSplits, StringSplitOptions.None);

                if (regRelatedKeywords.Any(ol => dateTextSplit.Any(mm => mm.ToLower().Contains(ol))))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsColorKeywords(string keyword)
        {
            var colours = new string[] { "blue", "brown", "yellow", "green", "purple", 
                "black", "orange", "white", "pink", "red", "green", "rose" };

            return colours.Any(ol => keyword.ToLower().Contains(ol));
        }

        private bool IsLeaseTitleKeywords(string keyword)
        {
            var titleRelatedKeywords = new string[] { "TGL", "EGL" };

            return titleRelatedKeywords.Any(ol => keyword.ToLower().Contains(ol));
        }

        private bool DateRelatedKeywords(string keyword)
        {
            var dateRelatedKeywords = new string[] { "beginning", "ending", "including", "on", "from", "years", "to" };

            foreach (var item in dateRelatedKeywords)
            {
                if (keyword.ToLower().Contains(item) || DateTime.TryParse(keyword, out _))
                {
                    return true;
                }

                var spaceSplits = SpaceSplits(" ");
                var dateTextSplit = keyword.Split(spaceSplits, StringSplitOptions.None);

                if (dateRelatedKeywords.Any(ol => dateTextSplit.Any(mm => mm.ToLower().Contains(ol)
                || DateTime.TryParse(mm, out _))))
                {
                    return true;
                }
            }

            return false;
        }

        private string[] OrganiseData(string[] columnData, int row)
        {
            var columnDataFiltered = new List<string>();
            int countSpaces = 0;

            var countRealData = columnData.Count(uk => uk != string.Empty);

            if (countRealData == 4)
            {
                for (int i = 0; i < columnData.Length; i++)
                {
                    if (columnData[i] != string.Empty)
                    {
                        columnDataFiltered.Add(columnData[i]);
                    }
                }
            }
            else if (countRealData == 1)
            {
                columnDataFiltered.Add(columnData[0]);

                for (int i = 0; i < 3; i++)
                {
                    columnDataFiltered.Add(" ");
                }
            }
            else
            {
                for (int i = 0; i < columnData.Length; i++)
                {
                    if (columnData[i] == string.Empty)
                    {
                        countSpaces++;
                    }
                    else
                    {
                        if (countSpaces > 0)
                        {
                            columnDataFiltered.Add(" ");
                            countSpaces = 0;
                        }

                        if (columnDataFiltered.Count < 4)
                        {
                            columnDataFiltered.Add(columnData[i]);
                        }
                    }
                }
            }

            if (row > 0)
            {
                var dateDataMoved = RemapDateRelatedData(columnDataFiltered);

                var regDataMoved = RemapRegisterationData(columnDataFiltered);
                
                RemapPropertyDescription(columnDataFiltered);
            }

            return columnDataFiltered.ToArray();
        }

        private bool RemapPropertyDescription(List<string> columnDataFiltered)
        {
            var proprtyRelatedIndex = -1;

            for (int j = 0; j < columnDataFiltered.Count; j++)
            {
                if (string.IsNullOrEmpty(columnDataFiltered[j].Trim()) == false
                    && RegisterationRelatedKeywords(columnDataFiltered[j]) == false
                    && DateRelatedKeywords(columnDataFiltered[j]) == false
                    && IsLeaseTitleKeywords(columnDataFiltered[j]) == false
                    && columnDataFiltered[j].ToLower().Contains("note") == false
                    && IsColorKeywords(columnDataFiltered[j]) == false)
                {
                    proprtyRelatedIndex = j;
                }
            }

            if (proprtyRelatedIndex > -1 && proprtyRelatedIndex != 1)
            {
                columnDataFiltered[1] = columnDataFiltered[proprtyRelatedIndex];
                columnDataFiltered[proprtyRelatedIndex] = " ";

                return true;
            }

            return false;
        }

        private bool RemapDateRelatedData(List<string> columnDataFiltered)
        {
            var dateRelatedIndex = -1;
            for (int j = 0; j < columnDataFiltered.Count; j++)
            {
                if (DateRelatedKeywords(columnDataFiltered[j])
                    && columnDataFiltered[j].ToLower().Contains("note") == false)
                {
                    dateRelatedIndex = j;
                }
            }

            if (dateRelatedIndex > -1 && dateRelatedIndex != 2)
            {
                columnDataFiltered[2] = columnDataFiltered[dateRelatedIndex];
                columnDataFiltered[dateRelatedIndex] = " ";

                return true;
            }
            return false;
        }

        private bool RemapRegisterationData(List<string> columnDataFiltered)
        {
            var regRelatedIndex = -1;

            for (int j = 0; j < columnDataFiltered.Count; j++)
            {

                if (RegisterationRelatedKeywords(columnDataFiltered[j])
                    && columnDataFiltered[j].ToLower().Contains("note") == false)
                {
                    regRelatedIndex = j;
                }
            }

            if (regRelatedIndex > -1 && regRelatedIndex != 0)
            {
                columnDataFiltered[0] = columnDataFiltered[regRelatedIndex];
                columnDataFiltered[regRelatedIndex] = " ";
            }

            return false;
        }

        private string[] SpaceSplits(string startSpace)
        {
            var spacesArray = new string[20];

            var space = startSpace;
            for (int i = 0; i < 20; i++)
            {
                spacesArray[i] = space;
                space += " ";
            }
            return spacesArray;
        }
    }
}
