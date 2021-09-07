using EuroVisionQuiz.Helpers;

namespace EuroVisionQuiz
{
    public enum QuizMode
    {
        FlipCards,
        MultipleChoice
    }

    public class QuizSettings
    {
        public QuizMode QuizMode { get; set; }
        public int TopAmount { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public int AmountOfQuestions { get; set; }
    }

    public static class Globals
    {
        private static SQLiteManager _database;

        public static SQLiteManager Database
        {
            get
            {
                if (_database == null)
                {
                    _database = new SQLiteManager();
                }
                return _database;
            }
        }

        private static QuizSettings _settings;

        public static QuizSettings QuizSettings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new QuizSettings
                    {
                        QuizMode = QuizMode.MultipleChoice,
                        AmountOfQuestions = 20
                    };
                }
                return _settings;
            }
        }
    }
}