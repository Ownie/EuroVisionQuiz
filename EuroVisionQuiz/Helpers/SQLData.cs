using EuroVisionQuiz.Models;
using System;
using System.Collections.Generic;

namespace EuroVisionQuiz.Helpers
{
    public class SQLData
    {
        public static List<EuroVisionYearModel> LstYears { get; set; }
        public static List<EuroVisionEntryModel> LstEntries { get; set; }
        public static List<EuroVisionModel> LstEuroVision { get; set; }
        public static List<EuroVisionCountryModel> LstCountries { get; set; }

        public static void CompileData()
        {
            LstEuroVision = new List<EuroVisionModel>();

            // Convert date strings to datetime
            for (int i = 0; i < LstYears.Count; i++)
            {
                LstYears[i].Date = DateTime.Parse(LstYears[i].FDate);
            }

            for (int i = 0; i < LstYears.Count; ++i)
            {
                //
                LstEuroVision.Add(new EuroVisionModel
                {
                    // Info
                    Info = LstYears[i],
                    Entries = LstEntries.FindAll(u => u.Year == LstYears[i].Year)
                });
            }
        }
    }
}