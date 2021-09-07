using SQLite;
using System;
using System.Collections.Generic;

namespace EuroVisionQuiz.Models
{
    public class EuroVisionModel
    {
        public EuroVisionModel()
        {
        }

        public EuroVisionModel(EuroVisionYearModel info, List<EuroVisionEntryModel> entries)
        {
            Info = info;
            Entries = entries;
        }

        public int Year
        {
            get
            {
                return Info.Year;
            }
        }

        public EuroVisionYearModel Info { get; set; }
        public List<EuroVisionEntryModel> Entries { get; set; }
    }

    [Table("EuroVisionCountryModel")]
    public class EuroVisionCountryModel
    {
        [Column("CountryId"), PrimaryKey, Unique, NotNull]
        public int CountryId { get; set; }

        [Column("CountryName"), Unique]
        public string Country { get; set; }

        [Column("DebutYear")]
        public int DebutYear { get; set; }

        [Column("LatestEntryYear")]
        public int LatestEntry { get; set; }

        [Column("Entries")]
        public int Entries { get; set; }

        [Column("Finals")]
        public int Finals { get; set; }

        [Column("LatestFinalYear")]
        public int LatestFinal { get; set; }

        [Column("Wins")]
        public int Wins { get; set; }
    }

    [Table("Years")]
    public class EuroVisionYearModel
    {
        [Column("Year"), PrimaryKey, Unique, NotNull]
        public int Year { get; set; }

        [Column("Date")]
        public string FDate { get; set; }

        [Column("Country")]
        public string Country { get; set; }

        [Column("City")]
        public string City { get; set; }

        [Column("Slogan")]
        public string Slogan { get; set; }

        public DateTime Date { get; set; }
    }

    [Table("Results")]
    public class EuroVisionEntryModel
    {
        [Column("Year")]
        public int Year { get; set; }

        [Column("Draw")]
        public int Draw { get; set; }

        [Column("Country")]
        public string Country { get; set; }

        [Column("Performer")]
        public string Performer { get; set; }

        [Column("Song")]
        public string Song { get; set; }

        [Column("Language")]
        public string Language { get; set; }

        [Column("Place")]
        public int Place { get; set; }

        [Column("Points")]
        public int Points { get; set; }
    }
}