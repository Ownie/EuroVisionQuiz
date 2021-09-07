using EuroVisionQuiz.Helpers;
using System;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;

namespace EuroVisionQuiz.ViewModels
{
    internal class QuickQuizViewModel : INotifyPropertyChanged
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

        public string PossibleAnswer0
        {
            get => _currentQuestion.GetPossibleAnswer(0);
        }

        public string PossibleAnswer1
        {
            get => _currentQuestion.GetPossibleAnswer(1);
        }

        public string PossibleAnswer2
        {
            get => _currentQuestion.GetPossibleAnswer(2);
        }

        public string PossibleAnswer3
        {
            get => _currentQuestion.GetPossibleAnswer(3);
        }

        private Color[] _buttonColors = new Color[4];

        public Color ButtonColor0
        {
            get => _buttonColors[0];
        }

        public Color ButtonColor1
        {
            get => _buttonColors[1];
        }

        public Color ButtonColor2
        {
            get => _buttonColors[2];
        }

        public Color ButtonColor3
        {
            get => _buttonColors[3];
        }

        private int _correctCount = 0;
        private int _questionCount = 0;

        public string TopText
        {
            get => "Question " + _questionCount + "/" + Globals.QuizSettings.AmountOfQuestions;
        }

        private bool _ready = true;

        public bool Ready
        {
            get => _ready;
            set
            {
                _ready = value;
                var args = new PropertyChangedEventArgs(nameof(Ready));
                PropertyChanged?.Invoke(this, args);
            }
        }

        private bool _quizDone = false;

        public bool QuizDone
        {
            get => _quizDone;
            set
            {
                _quizDone = value;
                var args = new PropertyChangedEventArgs(nameof(QuizDone));
                PropertyChanged?.Invoke(this, args);
            }
        }

        //
        public Command BackCommand { get; }

        public Command TryAgainCommand { get; }

        public Command Option1Command { get; }
        public Command Option2Command { get; }
        public Command Option3Command { get; }
        public Command Option4Command { get; }

        //
        private QuestionGenerator _generator = null;

        public QuickQuizViewModel()
        {
            ResetButtonColors();

            if (DesignMode.IsDesignModeEnabled) return;

            //
            BackCommand = new Command(() =>
            {
                Application.Current.MainPage.Navigation.PopAsync();
            })
            ;

            //
            Option1Command = new Command(() =>
            {
                if (PossibleAnswer0.Length == 0)
                {
                    return;
                }

                CheckAnswer(0);
            })
            ;
            Option2Command = new Command(() =>
            {
                if (PossibleAnswer1.Length == 0)
                {
                    return;
                }

                CheckAnswer(1);
            })
            ;
            Option3Command = new Command(() =>
            {
                if (PossibleAnswer2.Length == 0)
                {
                    return;
                }

                CheckAnswer(2);
            })
            ;
            Option4Command = new Command(() =>
            {
                if (PossibleAnswer3.Length == 0)
                {
                    return;
                }

                CheckAnswer(3);
            })
            ;

            TryAgainCommand = new Command(() =>
            {
                _correctCount = 0;
                _questionCount = 0;
                _generator = new QuestionGenerator();
                NewQuestion();
            })
            ;

            //
            _generator = new QuestionGenerator();

            //
            NewQuestion();
        }

        private void CheckAnswer(int buttonId)
        {
            if (!_ready)
            {
                return;
            }

            _ready = false;

            if (CurrentQuestion.CorrectAnswers.SequenceEqual(CurrentQuestion.PossibleAnswers[buttonId]))
            {
                ColorCorrect(buttonId);
                ++_correctCount;
            }
            else
            {
                ColorIncorrect(buttonId);
                ColorCorrect();
            }

            TimerNextQuestion();
            UpdateOptions();
        }

        private void ColorCorrect()
        {
            for (int i = 0; i < 4; i++)
            {
                if (CurrentQuestion.CorrectAnswers.SequenceEqual(CurrentQuestion.PossibleAnswers[i]))
                {
                    ColorCorrect(i);
                }
            }
        }

        private void ColorCorrect(int buttonId)
        {
            _buttonColors[buttonId] = new Color(110.0 / 256.0, 230.0 / 250.0, 110.0 / 256.0);
        }

        private void ColorIncorrect(int buttonId)
        {
            _buttonColors[buttonId] = new Color(230.0 / 256.0, 110.0 / 256.0, 110.0 / 256.0);
        }

        private void ResetButtonColors()
        {
            for (int i = 0; i < 4; i++)
            {
                _buttonColors[i] = new Color(255.0 / 256.0, 253.0 / 256.0, 253.0 / 256.0);
            }
        }

        private void UpdateOptions()
        {
            var args = new PropertyChangedEventArgs(nameof(PossibleAnswer0));
            PropertyChanged?.Invoke(this, args);

            args = new PropertyChangedEventArgs(nameof(PossibleAnswer1));
            PropertyChanged?.Invoke(this, args);

            args = new PropertyChangedEventArgs(nameof(PossibleAnswer2));
            PropertyChanged?.Invoke(this, args);

            args = new PropertyChangedEventArgs(nameof(PossibleAnswer3));
            PropertyChanged?.Invoke(this, args);

            args = new PropertyChangedEventArgs(nameof(ButtonColor0));
            PropertyChanged?.Invoke(this, args);

            args = new PropertyChangedEventArgs(nameof(ButtonColor1));
            PropertyChanged?.Invoke(this, args);

            args = new PropertyChangedEventArgs(nameof(ButtonColor2));
            PropertyChanged?.Invoke(this, args);

            args = new PropertyChangedEventArgs(nameof(ButtonColor3));
            PropertyChanged?.Invoke(this, args);

            args = new PropertyChangedEventArgs(nameof(TopText));
            PropertyChanged?.Invoke(this, args);

            args = new PropertyChangedEventArgs(nameof(QuizDone));
            PropertyChanged?.Invoke(this, args);
        }

        private void NewQuestion()
        {
            CurrentQuestion = _generator.Generate();
            FlipCardText = CurrentQuestion.QuestionText;

            ++_questionCount;
            _ready = true;
            QuizDone = false;

            ResetButtonColors();
            UpdateOptions();
        }

        private void TimerNextQuestion()
        {
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (_questionCount == Globals.QuizSettings.AmountOfQuestions)
                {
                    QuizDone = true;
                    FlipCardText = "You got " + _correctCount + " question(s) right from a total of " + _questionCount + " questions!";
                    CurrentQuestion.PossibleAnswers.Clear();
                    ResetButtonColors();
                    UpdateOptions();
                }
                else
                {
                    NewQuestion();
                }

                return false; // True = Repeat again, False = Stop the timer
            });
        }
    }
}