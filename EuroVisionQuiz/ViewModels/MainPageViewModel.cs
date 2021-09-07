using EuroVisionQuiz.Helpers;
using EuroVisionQuiz.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;

namespace EuroVisionQuiz.ViewModels
{
    internal class MainPageViewModel : INotifyPropertyChanged
    {
        //
        public event PropertyChangedEventHandler PropertyChanged;

        // Create itemsource binding for year pickers
        private ObservableCollection<int> _years;

        public ObservableCollection<int> Years
        {
            get
            {
                if (_years == null)
                {
                    _years = new ObservableCollection<int>();

                    for (int i = 0; i < SQLData.LstYears.Count; i++)
                    {
                        _years.Add(SQLData.LstYears[i].Year);
                    }
                }

                return _years;
            }
        }

        // selected min year index binding
        private int _minYearsIndex = 0;

        private int _maxYearsIndex = -1;

        public int MinYearsIndex
        {
            get
            {
                return _minYearsIndex;
            }
            set
            {
                _minYearsIndex = value;
                var args = new PropertyChangedEventArgs(nameof(MinYearsIndex));
                PropertyChanged?.Invoke(this, args);
                args = new PropertyChangedEventArgs(nameof(SettingsBackgroundColor));
                PropertyChanged?.Invoke(this, args);
                args = new PropertyChangedEventArgs(nameof(SettingsCheck));
                PropertyChanged?.Invoke(this, args);
                Globals.QuizSettings.StartYear = _minYearsIndex;
            }
        }

        // selected max year index binding

        public int MaxYearsIndex
        {
            get
            {
                if (_maxYearsIndex == -1)
                {
                    _maxYearsIndex = SQLData.LstYears.Count - 1;
                }

                return _maxYearsIndex;
            }
            set
            {
                _maxYearsIndex = value;
                var args = new PropertyChangedEventArgs(nameof(MaxYearsIndex));
                PropertyChanged?.Invoke(this, args);
                args = new PropertyChangedEventArgs(nameof(SettingsBackgroundColor));
                PropertyChanged?.Invoke(this, args);
                args = new PropertyChangedEventArgs(nameof(SettingsCheck));
                PropertyChanged?.Invoke(this, args);
                Globals.QuizSettings.EndYear = _maxYearsIndex;
            }
        }

        public Color SettingsBackgroundColor
        {
            get
            {
                if (MinYearsIndex > MaxYearsIndex)
                {
                    return Color.Red;
                }
                return Color.Transparent;
            }
        }

        public bool SettingsCheck
        {
            get
            {
                if (MinYearsIndex > MaxYearsIndex)
                {
                    return false;
                }
                return true;
            }
        }

        // Create itemsource binding for top picker
        private ObservableCollection<int> _topList;

        public ObservableCollection<int> TopList
        {
            get
            {
                if (_topList == null)
                {
                    _topList = new ObservableCollection<int>();

                    int most = 0;

                    for (int i = 1; i < SQLData.LstEuroVision.Count; i++)
                    {
                        int entryCount = SQLData.LstEuroVision[i].Entries.Count;
                        if (entryCount > most)
                        {
                            most = entryCount;
                        }
                    }

                    for (int i = 1; i <= most; i++)
                    {
                        _topList.Add(i);
                    }
                }

                return _topList;
            }
        }

        // selected index for top entries
        private int _topIndex = 5 - 1;

        public int Topindex
        {
            get
            {
                Globals.QuizSettings.TopAmount = _topIndex + 1;
                return _topIndex;
            }
            set
            {
                _topIndex = value;
                var args = new PropertyChangedEventArgs(nameof(Topindex));
                PropertyChanged?.Invoke(this, args);
                Globals.QuizSettings.TopAmount = _topIndex + 1;
            }
        }

        //
        public string StepperText
        {
            get => _questionCount.ToString() + " Questions";
        }

        private int _questionCount = 20;

        public int QuestionCount
        {
            get
            {
                Globals.QuizSettings.AmountOfQuestions = _questionCount;
                return _questionCount;
            }
            set
            {
                _questionCount = value;
                var args = new PropertyChangedEventArgs(nameof(QuestionCount));
                PropertyChanged?.Invoke(this, args);
                args = new PropertyChangedEventArgs(nameof(StepperText));
                PropertyChanged?.Invoke(this, args);
                Globals.QuizSettings.AmountOfQuestions = _questionCount;
            }
        }

        // Commands
        public Command FlipCardsCommand { get; }

        public Command QuickQuizCommand { get; }

        // Constructor
        public MainPageViewModel()
        {
            if (DesignMode.IsDesignModeEnabled) return;

            // Set Commands
            FlipCardsCommand = new Command(async () =>
            {
                Globals.QuizSettings.QuizMode = QuizMode.FlipCards;
                await Application.Current.MainPage.Navigation.PushAsync(new FlipCardsPageView());
            });
            QuickQuizCommand = new Command(async () =>
            {
                Globals.QuizSettings.QuizMode = QuizMode.MultipleChoice;
                await Application.Current.MainPage.Navigation.PushAsync(new QuickQuizPageView());
            });
        }
    }
}