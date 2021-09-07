using EuroVisionQuiz.Models;
using SQLite;
using System.Collections.Generic;

namespace EuroVisionQuiz.Helpers
{
    public class SQLiteManager
    {
        public SQLiteConnection DBInstance { get; set; }

        public SQLiteManager()
        {
            // Create Connection
            DBInstance = new SQLiteConnection(Constants.DatabasePath, Constants.Flags);
        }

        public void LoadData()
        {
            SQLData.LstYears = GetList<EuroVisionYearModel>(
                "SELECT Year, Date, Countries.CountryName as [Country], City, Slogan " +
                "FROM Years " +
                "INNER JOIN Countries " +
                "ON Years.CountryId = Countries.CountryId");

            SQLData.LstEntries = GetList<EuroVisionEntryModel>(
                "SELECT Year, Draw, Countries.CountryName as Country, Performer, Song, Language, Place, Points " +
                "FROM Results " +
                "INNER JOIN Countries " +
                "ON Results.CountryId = Countries.CountryId");
            SQLData.LstCountries = GetList<EuroVisionCountryModel>(
                "SELECT * FROM Countries");

            SQLData.CompileData();
        }

        public List<T> GetList<T>(string query) where T : new()
        {
            return DBInstance.Query<T>(query);
        }
    }
}