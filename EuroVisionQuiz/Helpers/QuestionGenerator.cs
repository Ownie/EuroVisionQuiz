using EuroVisionQuiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EuroVisionQuiz.Helpers
{
    public class Question
    {
        public Question()
        {
            Id = "";
            Type = QuestionType.HowManyCountriesCompetedInEuroVisionYearX;
            QuestionText = "";
            CorrectAnswers = new List<string>();
            PossibleAnswers = new List<List<string>>();
        }

        public string Id { get; set; }
        public QuestionType Type { get; set; }
        public string QuestionText { get; set; }
        public List<string> CorrectAnswers { get; set; }
        public List<List<string>> PossibleAnswers { get; set; }

        public string GetCorrectAnswers()
        {
            string answer = "";

            for (int i = 0; i < CorrectAnswers.Count; i++)
            {
                answer += CorrectAnswers[i];
                if (i != CorrectAnswers.Count - 1)
                {
                    answer += ", ";
                }
            }

            return answer;
        }

        public string GetPossibleAnswer(int index)
        {
            //
            if (index >= PossibleAnswers.Count)
            {
                return "";
            }

            //
            string answer = "";

            for (int i = 0; i < PossibleAnswers[index].Count; i++)
            {
                answer += (PossibleAnswers[index])[i];
                if (i != (PossibleAnswers[index]).Count - 1)
                {
                    answer += ", ";
                }
            }

            return answer;
        }
    }

    public enum QuestionType
    {
        WhichPerformerPlacedXInTheYearX,
        WhichPerformerPlacedXInTheYearX1,
        WhoRepresentedCountryXAtEuroVisionYearX,
        WhoRepresentedCountryXAtEuroVisionYearX1,
        WhatSongRepresentedCountryXAtEuroVisionYearX,
        WhatSongRepresentedCountryXAtEuroVisionYearX1,
        HowManyCountriesCompetedInEuroVisionYearX,
        HowManyTimesWasTheEuroVisionContestHeldInCountryX,
        HowManyPointsDidPerformerXGetInYearX,
        WhichCityHostedEuroVisionContextInYearX,
        WhatWasTheSloganForTheEuroVisionContestInYearX,
        HowManyTimesHasCountryXWonTheEuroVisionContest,
        WhichCountryDidPerformerXRepresentInYearX,
        WhichCountryDidPerformerXRepresentInYearX1,
        WhichSongWasNotInTheEurovisionSongFinalsInYearX
    }

    internal class QuestionGenerator
    {
        // Shortcut
        private List<EuroVisionModel> _data { get { return SQLData.LstEuroVision; } }

        private bool _isMPC => Globals.QuizSettings.QuizMode == QuizMode.MultipleChoice;

        private Random r = null;

        // Keep track of asked questions
        private List<Question> _askedQuestions = new List<Question>();

        private int _mpCCount = 3;
        private int _mpcDeviation = 6;
        private QuestionType _previousQType = QuestionType.HowManyCountriesCompetedInEuroVisionYearX;

        public void ClearAskedQuestions()
        {
            _askedQuestions.Clear();
        }

        // Constructor
        public QuestionGenerator(int multipleChoiceDeviation = 6)
        {
            _mpcDeviation = multipleChoiceDeviation;

            r = new Random();
        }

        //
        public Question Generate()
        {
            Question q = null;

            int tries = 0;
            int maxTries = 1000;
            int max = Enum.GetNames(typeof(QuestionType)).Length;

            do
            {
                q = new Question();

                ++tries;

                // choose random q type
                q.Type = (QuestionType)(r.Next(0, max));
                q.Id = ((int)q.Type).ToString();

                //
                Generate(ref q);
            } while (
            q == null ||
            (q.Type == _previousQType && tries < maxTries / 10) ||
            (maxTries > tries && _askedQuestions.Exists(u => u.Id == q.Id)));

            _previousQType = q.Type;

            //
            _askedQuestions.Add(q);

            return q;
        }

        private Question Generate(ref Question q)
        {
            switch (q.Type)
            {
                case QuestionType.WhichPerformerPlacedXInTheYearX:
                case QuestionType.WhichPerformerPlacedXInTheYearX1:
                    WhichPerformerPlacedXInTheYearX(ref q);
                    break;

                case QuestionType.WhoRepresentedCountryXAtEuroVisionYearX:
                case QuestionType.WhoRepresentedCountryXAtEuroVisionYearX1:
                    WhoRepresentedCountryXAtEuroVisionYearX(ref q);
                    break;

                case QuestionType.HowManyCountriesCompetedInEuroVisionYearX:
                    HowManyCountriesCompetedInEuroVisionYearX(ref q);
                    break;

                case QuestionType.HowManyTimesWasTheEuroVisionContestHeldInCountryX:
                    HowManyTimesWasTheEuroVisionContestHeldInCountryX(ref q);
                    break;

                case QuestionType.HowManyPointsDidPerformerXGetInYearX:
                    HowManyPointsDidPerformerXGetInYearX(ref q);
                    break;

                case QuestionType.WhichCityHostedEuroVisionContextInYearX:
                    WhichCityHostedEuroVisionContextInYearX(ref q);
                    break;

                case QuestionType.WhatWasTheSloganForTheEuroVisionContestInYearX:
                    WhatWasTheSloganForTheEuroVisionContestInYearX(ref q);
                    break;

                case QuestionType.HowManyTimesHasCountryXWonTheEuroVisionContest:
                    HowManyTimesHasCountryXWonTheEuroVisionContest(ref q);
                    break;

                case QuestionType.WhichCountryDidPerformerXRepresentInYearX:
                case QuestionType.WhichCountryDidPerformerXRepresentInYearX1:
                    WhichCountryDidPerformerXRepresentInYearX(ref q);
                    break;

                case QuestionType.WhichSongWasNotInTheEurovisionSongFinalsInYearX:
                    WhichSongWasNotInTheEurovisionSongFinalsInYearX(ref q);
                    break;

                case QuestionType.WhatSongRepresentedCountryXAtEuroVisionYearX:
                case QuestionType.WhatSongRepresentedCountryXAtEuroVisionYearX1:
                    WhatSongRepresentedCountryXAtEuroVisionYearX(ref q);
                    break;

                default:
                    break;
            }

            return q;
        }

        //
        private void WhichPerformerPlacedXInTheYearX(ref Question q)
        {
            int year = RandomYear();
            // EXCEPTION YEAR 1956
            if (year == 1956)
            {
                WhichPerformeWonInYear1956(ref q);
                return;
            }
            int place = GetRandomPlace(year);
            var entries = CreateStringList(GetEntryPlaced(year, place), "Performer");

            q.Id += year.ToString();
            q.Id += place.ToString();
            q.QuestionText = "Which performer(s) placed " + Format.AddOrdinal(place) + " in the year " + year.ToString() + "?";
            q.CorrectAnswers = entries;
            q.PossibleAnswers.Add(entries);

            //
            if (_isMPC)
            {
                while (q.PossibleAnswers.Count <= _mpCCount)
                {
                    do
                    {
                        entries = CreateStringList(GetEntryPlaced(year, GetRandomPlace(year, false)), "Performer");
                    } while (entries.Count == 0);

                    if (!DoesAnswerExist(q, entries))
                        q.PossibleAnswers.Add(entries);
                }

                q.PossibleAnswers.Shuffle(r);
            }
        }

        private void WhichPerformeWonInYear1956(ref Question q)
        {
            int year = 1956;
            int place = 1;
            var entries = CreateStringList(GetEntryPlaced(year, place), "Performer");

            q.Id += year.ToString();
            q.Id += place.ToString();
            q.QuestionText = "Which performer(s) placed " + Format.AddOrdinal(place) + " in the year " + year.ToString() + "?";
            q.CorrectAnswers = entries;
            q.PossibleAnswers.Add(entries);

            //
            if (_isMPC)
            {
                while (q.PossibleAnswers.Count <= _mpCCount)
                {
                    do
                    {
                        entries = CreateStringList(GetRandomEntries(year), "Performer");
                    } while (entries.Count == 0);

                    if (!DoesAnswerExist(q, entries))
                        q.PossibleAnswers.Add(entries);
                }

                q.PossibleAnswers.Shuffle(r);
            }
        }

        private void WhoRepresentedCountryXAtEuroVisionYearX(ref Question q)
        {
            int year = RandomYear(false);
            var entry = GetRandomEntry(year);
            var performer = CreateStringList(entry.Performer);

            q.Id += year.ToString();
            q.Id += entry.Draw.ToString();
            q.QuestionText = "Who Represented " + entry.Country + " in the year " + year.ToString() + "?";
            q.CorrectAnswers = performer;
            q.PossibleAnswers.Add(performer);

            //
            if (_isMPC)
            {
                while (q.PossibleAnswers.Count <= _mpCCount)
                {
                    do
                    {
                        performer = CreateStringList(GetRandomEntry(year).Performer);
                    } while (performer.Count == 0);

                    if (!DoesAnswerExist(q, performer))
                        q.PossibleAnswers.Add(performer);
                }

                q.PossibleAnswers.Shuffle(r);
            }
        }

        private void HowManyCountriesCompetedInEuroVisionYearX(ref Question q)
        {
            int year = RandomYear(false);
            int entryCount = GetEntryCount(year);
            var count = CreateStringList(entryCount);

            q.Id += year.ToString();
            q.QuestionText = "How many countries competed in the Finals of Eurovision " + year.ToString() + "?";
            q.CorrectAnswers = count;
            q.PossibleAnswers.Add(count);

            //
            if (_isMPC)
            {
                int random1 = r.Next(-4, 1);
                while (q.PossibleAnswers.Count <= _mpCCount)
                {
                    do
                    {
                        count = CreateStringList(entryCount + r.Next(random1, random1 + 5));
                    } while (count.Count == 0);

                    if (!DoesAnswerExist(q, count))
                        q.PossibleAnswers.Add(count);
                }

                q.PossibleAnswers.Shuffle(r);
            }
        }

        private void HowManyTimesWasTheEuroVisionContestHeldInCountryX(ref Question q)
        {
            var country = GetRandomCountry();
            int count = GetCountryHostCount(country.Country);
            if (count == 0 && r.Next() % 3 == 0)
            {
                q = null;
                return;
            }
            var countryList = CreateStringList(count + " time(s)");

            q.Id += country.Country.ToString();
            q.QuestionText = "How many times was the Eurovision Contest held in " + country.Country + "?";
            q.CorrectAnswers = countryList;
            q.PossibleAnswers.Add(countryList);

            //
            if (_isMPC)
            {
                int random1 = r.Next(-4, 1);
                while (q.PossibleAnswers.Count <= _mpCCount)
                {
                    do
                    {
                        countryList = CreateStringList(Math.Max(count + r.Next(random1, 5), 0));
                        countryList[0] += " time(s)";
                    } while (countryList.Count == 0);

                    if (!DoesAnswerExist(q, countryList))
                        q.PossibleAnswers.Add(countryList);
                }

                q.PossibleAnswers.Shuffle(r);
            }
        }

        private void HowManyPointsDidPerformerXGetInYearX(ref Question q)
        {
            int year = RandomYear(false);
            var entry = GetRandomEntry(year, 1, Math.Min(3, Globals.QuizSettings.TopAmount));
            var winnerList = CreateStringList(entry.Points);

            q.Id += year.ToString();
            q.Id += entry.Performer;
            q.QuestionText = "How many points did " + entry.Performer + " (" + Format.AddOrdinal(entry.Place) + " place) get in " + entry.Year.ToString() + "?";
            q.CorrectAnswers = winnerList;
            q.PossibleAnswers.Add(winnerList);

            //
            if (_isMPC)
            {
                while (q.PossibleAnswers.Count <= _mpCCount)
                {
                    do
                    {
                        winnerList = CreateStringList(entry.Points + r.Next(-(entry.Points * 2 / 5), (entry.Points * 2 / 5)));
                    } while (winnerList.Count == 0);

                    if (!DoesAnswerExist(q, winnerList))
                        q.PossibleAnswers.Add(winnerList);
                }

                q.PossibleAnswers.Shuffle(r);
            }
        }

        private void WhichCityHostedEuroVisionContextInYearX(ref Question q)
        {
            int year = RandomYear();
            var yearInfo = GetYearInfo(year);
            var cityList = CreateStringList(yearInfo.City);

            q.Id += year.ToString();
            q.QuestionText = "Which city hosted the Eurovision Contest in " + year.ToString() + "?";
            q.CorrectAnswers = cityList;
            q.PossibleAnswers.Add(cityList);

            //
            if (_isMPC)
            {
                // Look for cities which hosted ESC in host city before
                var cities = GetCitiesInCountry(yearInfo.Country);
                cities.Remove(cityList[0]);

                for (int i = 0; i < cities.Count; ++i)
                {
                    if (q.PossibleAnswers.Count <= _mpCCount)
                    {
                        q.PossibleAnswers.Add(CreateStringList(cities[i]));
                    }
                }

                // Fill up with other cities
                while (q.PossibleAnswers.Count <= _mpCCount)
                {
                    do
                    {
                        cityList = CreateStringList(GetRandomYearInfo().City);
                    } while (cityList.Count == 0);

                    if (!DoesAnswerExist(q, cityList))
                        q.PossibleAnswers.Add(cityList);
                }

                q.PossibleAnswers.Shuffle(r);
            }
        }

        private List<string> _slogans = new List<string>()
        {
            "A Classic Fairytale",
            "A Fairytale",
            "Under the Sky",
            "Feel the Night!",
            "Real Fantasy",
            "Symphony of Sound",
            "Share the Future",
            "Feel the Fire!",
            "#Together",
            "Dare to Live",
            "Dare to Sing",
            "#Together",
            "Orchestra of Sound",
            "Living Fantasy",
            "Magical"
        };

        private void WhatWasTheSloganForTheEuroVisionContestInYearX(ref Question q)
        {
            var yearInfo = GetRandomYearInfoWithSlogan();
            if (yearInfo == null)
            {
                q = null;
                return;
            }
            var sloganList = CreateStringList(yearInfo.Slogan);

            q.Id += yearInfo.Year.ToString();
            q.QuestionText = "What was the slogan for the Eurovision Contest in the year " + yearInfo.Year.ToString() + "?";
            q.CorrectAnswers = sloganList;
            q.PossibleAnswers.Add(sloganList);

            //
            if (_isMPC)
            {
                // add a none of the above
                List<string> correct = q.CorrectAnswers;
                if (r.Next() % 3 == 0)
                {
                    q.PossibleAnswers.Add(CreateStringList("None of these"));

                    // delete correct
                    if (r.Next() % 2 == 0)
                    {
                        q.PossibleAnswers.RemoveAt(0);
                        q.CorrectAnswers = new List<string>(q.PossibleAnswers[0]);
                    }
                }

                while (q.PossibleAnswers.Count <= _mpCCount)
                {
                    do
                    {
                        // add non existant
                        if (r.Next() % 4 == 0)
                            sloganList = CreateStringList(_slogans[r.Next(0, _slogans.Count)]);
                        else
                            sloganList = CreateStringList(GetRandomYearInfoWithSlogan(false).Slogan);
                    } while (sloganList.Count == 0);

                    bool exists = false;
                    for (int i = 0; i < q.PossibleAnswers.Count; i++)
                    {
                        if (DoesAnswerExist(q, sloganList))
                        {
                            exists = true;
                        }
                        if (sloganList[0] == correct[0])
                        {
                            exists = true;
                        }
                    }

                    if (!exists)
                    {
                        q.PossibleAnswers.Add(sloganList);
                    }
                }

                q.PossibleAnswers.Shuffle(r);
            }
        }

        private void HowManyTimesHasCountryXWonTheEuroVisionContest(ref Question q)
        {
            var country = GetRandomCountry();
            int count = GetCountryWinCount(country.Country);
            if (count == 0 && r.Next() % 3 == 0)
            {
                q = null;
                return;
            }
            var countryList = CreateStringList(count + " time(s)");

            q.Id += country.Country.ToString();
            q.QuestionText = "How many times has " + country.Country + " won the Eurovision Contest ?";
            q.CorrectAnswers = countryList;
            q.PossibleAnswers.Add(countryList);

            //
            if (_isMPC)
            {
                int random1 = r.Next(-4, 1);
                while (q.PossibleAnswers.Count <= _mpCCount)
                {
                    do
                    {
                        countryList = CreateStringList(Math.Max(count + r.Next(random1, 5), 0));
                        countryList[0] += " time(s)";
                    } while (countryList.Count == 0);

                    if (!DoesAnswerExist(q, countryList))
                        q.PossibleAnswers.Add(countryList);
                }

                q.PossibleAnswers.Shuffle(r);
            }
        }

        private void WhichCountryDidPerformerXRepresentInYearX(ref Question q)
        {
            int year = RandomYear(false);
            var entry = GetRandomEntry(year);
            var country = CreateStringList(entry.Country);

            q.Id += year.ToString();
            q.Id += entry.Draw.ToString();
            q.QuestionText = "Which country did " + entry.Performer + " represent in " + entry.Year + "?";
            q.CorrectAnswers = country;
            q.PossibleAnswers.Add(country);

            //
            if (_isMPC)
            {
                while (q.PossibleAnswers.Count <= _mpCCount)
                {
                    do
                    {
                        country = CreateStringList(GetRandomCountry().Country);
                    } while (country.Count == 0);

                    if (!DoesAnswerExist(q, country))
                        q.PossibleAnswers.Add(country);
                }

                q.PossibleAnswers.Shuffle(r);
            }
        }

        private void WhichSongWasNotInTheEurovisionSongFinalsInYearX(ref Question q)
        {
            if (!_isMPC)
            {
                q = null;
                return;
            }

            int year = RandomYear();
            var entry = GetRandomEntry(year);
            if (r.Next() % 2 == 0 && year != SQLData.LstYears.Last().Year)
                entry = GetRandomEntry(year + 1);
            else
                entry = GetRandomEntry(year - 1);
            var song = CreateStringList(entry.Song);

            q.Id += year.ToString();
            q.QuestionText = "Which song was not in the Eurovision Song Finals in " + year.ToString() + " ?";
            q.CorrectAnswers = song;
            q.PossibleAnswers.Add(song);

            //
            if (_isMPC)
            {
                while (q.PossibleAnswers.Count <= _mpCCount)
                {
                    do
                    {
                        song = CreateStringList(GetRandomEntry(year).Song);
                    } while (song.Count == 0);

                    if (!DoesAnswerExist(q, song))
                        q.PossibleAnswers.Add(song);
                }

                q.PossibleAnswers.Shuffle(r);
            }
        }

        private void WhatSongRepresentedCountryXAtEuroVisionYearX(ref Question q)
        {
            int year = RandomYear(false);
            var entry = GetRandomEntry(year);
            var song = CreateStringList(entry.Song);

            q.Id += year.ToString();
            q.Id += entry.Draw.ToString();
            q.QuestionText = "What song represented " + entry.Country + " at Eurovision in " + entry.Year + " ?";
            q.CorrectAnswers = song;
            q.PossibleAnswers.Add(song);

            //
            if (_isMPC)
            {
                while (q.PossibleAnswers.Count <= _mpCCount)
                {
                    do
                    {
                        song = CreateStringList(GetRandomEntry(year).Song);
                    } while (song.Count == 0);

                    if (!DoesAnswerExist(q, song))
                        q.PossibleAnswers.Add(song);
                }

                q.PossibleAnswers.Shuffle(r);
            }
        }

        //
        private int BoolToInt(bool b)
        {
            if (b)
                return 1;
            return 0;
        }

        private bool DoesAnswerExist(Question q, List<string> answer)
        {
            bool exists = false;
            for (int i = 0; i < q.PossibleAnswers.Count; i++)
            {
                if (q.PossibleAnswers[i].SequenceEqual(answer))
                {
                    exists = true;
                    break;
                }
            }

            if (exists)
            {
                return true;
            }

            return false;
        }

        private List<string> CreateStringList(int lst)
        {
            List<string> stringList = new List<string>
            {
                lst.ToString()
            };

            return stringList;
        }

        private List<string> CreateStringList(string lst)
        {
            List<string> stringList = new List<string>
            {
                lst
            };

            return stringList;
        }

        //private List<string> CreateStringList(EuroVisionEntryModel lst, string property)
        //{
        //    List<string> stringList = new List<string>();
        //    PropertyInfo pinfo = typeof(EuroVisionEntryModel).GetProperty(property);

        //    stringList.Add(pinfo.GetValue(lst, null).ToString());

        //    return stringList;
        //}

        private List<string> CreateStringList(List<EuroVisionEntryModel> lst, string property)
        {
            List<string> stringList = new List<string>();
            PropertyInfo pinfo = typeof(EuroVisionEntryModel).GetProperty(property);

            for (int i = 0; i < lst.Count; ++i)
            {
                stringList.Add(pinfo.GetValue(lst[i], null).ToString());
            }

            return stringList;
        }

        private EuroVisionYearModel GetRandomYearInfoWithSlogan(bool inrange = true)
        {
            List<EuroVisionYearModel> years = new List<EuroVisionYearModel>();

            if (inrange)
            {
                int min = SQLData.LstYears[0].Year + Globals.QuizSettings.StartYear;
                int max = SQLData.LstYears[0].Year + Globals.QuizSettings.EndYear;
                years = SQLData.LstYears.FindAll(u => u.Slogan != null && u.Year >= min && u.Year <= max);
            }
            else
            {
                years = SQLData.LstYears.FindAll(u => u.Slogan != null);
            }

            // no years in range with slogan
            if (years.Count == 0)
            {
                return null;
            }

            return years[r.Next(0, years.Count)];
        }

        private int RandomYear(bool include1956 = true)
        {
            int year = 0;

            //
            bool yearExists = false;
            while (!yearExists)
            {
                // Generate random year
                //year = r.Next(_data[0].Year, _data[_data.Count - 1].Year + 1);
                year = r.Next(_data[0].Year + Globals.QuizSettings.StartYear, _data[0].Year + Globals.QuizSettings.EndYear + 1);
                // Check if year is allowed to be 1956
                if (year == 1956 && !include1956)
                    continue;
                // Check if year exists
                if (_data.Exists(u => u.Year == year))
                    yearExists = true;
            }

            return year;
        }

        //
        private int GetCountryHostCount(string country)
        {
            var years = _data.FindAll(u => u.Info.Country == country);

            return years.Count;
        }

        private int GetCountryWinCount(string country)
        {
            var wins = SQLData.LstEntries.FindAll(u => u.Country == country && u.Place == 1);

            return wins.Count;
        }

        private List<EuroVisionEntryModel> GetWinners(int year)
        {
            return _data.Find(u => u.Year == year).Entries.FindAll(j => j.Place == 1);
        }

        private EuroVisionEntryModel GetRandomWinner(int year)
        {
            var winners = _data.Find(u => u.Year == year).Entries.FindAll(j => j.Place == 1);
            int randomIndex = r.Next(0, winners.Count);

            return winners[randomIndex];
        }

        private EuroVisionEntryModel GetRandomEntry(int year, int minPlace, int maxPlace)
        {
            var winners = _data.Find(u => u.Year == year).Entries.FindAll(j => j.Place >= minPlace && j.Place <= maxPlace);
            int randomIndex = r.Next(0, winners.Count);

            return winners[randomIndex];
        }

        private List<EuroVisionEntryModel> GetEntryPlaced(int year, int place)
        {
            return _data.Find(u => u.Year == year).Entries.FindAll(j => j.Place == place);
        }

        private int GetEntryCount(int year)
        {
            return _data.Find(u => u.Year == year).Entries.Count;
        }

        private EuroVisionCountryModel GetRandomCountry()
        {
            var country = SQLData.LstCountries[r.Next(0, SQLData.LstCountries.Count)];

            return country;
        }

        private List<string> GetCitiesInCountry(string country)
        {
            List<string> lst = new List<string>();
            var cities = SQLData.LstYears.FindAll(u => u.Country == country).GroupBy(u => u.City).Select(v => v.First()).ToList();

            for (int i = 0; i < cities.Count; i++)
            {
                lst.Add(cities[i].City);
            }

            lst.Shuffle(r);

            return lst;
        }

        private EuroVisionYearModel GetRandomYearInfo()
        {
            var year = SQLData.LstYears[r.Next(0, SQLData.LstYears.Count)];

            return year;
        }

        private EuroVisionEntryModel GetRandomEntry(int year)
        {
            var entriesYear = _data.Find(u => u.Year == year).Entries;
            var randomEntry = entriesYear[r.Next(1, entriesYear.Count)];

            return randomEntry;
        }

        private List<EuroVisionEntryModel> GetRandomEntries(int year)
        {
            var entriesYear = _data.Find(u => u.Year == year).Entries;
            int randomDraw = r.Next(1, entriesYear.Count + 1);

            return entriesYear.FindAll(j => j.Draw == randomDraw);
        }

        private EuroVisionYearModel GetYearInfo(int year)
        {
            return SQLData.LstYears.Find(u => u.Year == year);
        }

        private int GetRandomPlace(int year, bool inTopRange = true)
        {
            bool exists = false;
            int randomPlace = 0;

            while (!exists)
            {
                // Generate random place
                //randomPlace = Math.Min(r.Next(1, Globals.QuizSettings.TopAmount + 1), _data.Find(u => u.Year == year).Entries.Count);
                if (inTopRange)
                    randomPlace = Math.Min(r.Next(1, Globals.QuizSettings.TopAmount + 1), _data.Find(u => u.Year == year).Entries.Aggregate((i1, i2) => i1.Place > i2.Place ? i1 : i2).Place);
                else
                    randomPlace = Math.Min(r.Next(1, Globals.QuizSettings.TopAmount + 1 + _mpcDeviation), _data.Find(u => u.Year == year).Entries.Aggregate((i1, i2) => i1.Place > i2.Place ? i1 : i2).Place);

                // Check if place exists
                exists = _data.Find(u => u.Year == year).Entries.Exists(j => j.Place == randomPlace);
            }

            return randomPlace;
        }
    }
}