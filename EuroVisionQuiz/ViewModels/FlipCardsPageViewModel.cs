using EuroVisionQuiz.Helpers;
using System.ComponentModel;
using Xamarin.Forms;

namespace EuroVisionQuiz.ViewModels
{
    internal class FlipCardsPageViewModel : INotifyPropertyChanged
    {
        //
        public event PropertyChangedEventHandler PropertyChanged;

        //
        private string _flipCardText;

        public string FlipCardText
        {
            get => _flipCardText;
            set
            {
                _flipCardText = value;
                var args = new PropertyChangedEventArgs(nameof(FlipCardText));
                PropertyChanged?.Invoke(this, args);
            }
        }

        //
        private Question _currentQuestion;

        public Question CurrentQuestion
        {
            get => _currentQuestion;
            set
            {
                _currentQuestion = value;
                var args = new PropertyChangedEventArgs(nameof(CurrentQuestion));
                PropertyChanged?.Invoke(this, args);
            }
        }

        //
        public Command FlipCommand { get; }

        public Command BackCommand { get; }

        //
        private QuestionGenerator _generator = null;

        private bool _question = true;

        public FlipCardsPageViewModel()
        {
            if (DesignMode.IsDesignModeEnabled) return;

            //
            FlipCommand = new Command(() =>
            {
                if (_question)
                {
                    ShowAnswer();
                }
                else
                {
                    NewQuestion();
                }
            })
            ;
            //
            BackCommand = new Command(() =>
            {
                Application.Current.MainPage.Navigation.PopAsync();
            })
            ;

            //
            _generator = new QuestionGenerator();

            //
            NewQuestion();
        }

        private void NewQuestion()
        {
            _question = true;
            CurrentQuestion = _generator.Generate();
            FlipCardText = CurrentQuestion.QuestionText;
        }

        private void ShowAnswer()
        {
            _question = false;
            FlipCardText = CurrentQuestion.GetCorrectAnswers();
        }
    }
}