using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Narumikazuchi.Hiragana_And_Katakana_SelfTester
{
    public static class QuizController
    {
        #region Constants

        private const Int32 SETSIZE = 20;

        #endregion

        #region Quiz Setup

        public static Boolean Setup(Int32 sets, Int32 difficultChars, Int32 difficultyMaxLength)
        {
            _totalSets = sets;
            _totalWords = difficultChars;
            _maxWordlength = difficultyMaxLength;
            _currentSet = 0;
            _currentQuestion = 0;
            _pool.Clear();
            foreach (ICharacter character in _data.Where(c => _data.GetIsActive(c)))
            {
                UInt16 priority = _data.GetPriority(character);
                for (UInt16 i = 0; i < priority; i++)
                {
                    _pool.Add(character);
                }
            }
            if (_pool.Count < 3)
            {
                MessageBox.Show("Can't setup with less than 3 selected characters!");
                return false;
            }
            Stage = QuizStage.MenuBlendOut;
            return true;
        }

        #endregion

        #region Quiz Stages

        public static void SetupNextFadeOrStage()
        {
            switch (Stage)
            {
                case QuizStage.MenuBlendOut:
                    Stage = QuizStage.QuestionBlendIn;
                    if (_window._progressBorder.Visibility == Visibility.Collapsed)
                    {
                        _window._progressBorder.Visibility = Visibility.Visible;
                    }
                    SetupNextQuestion();
                    StartFadeIn(_window._quizPage);
                    return;
                case QuizStage.QuestionBlendIn:
                    Stage = QuizStage.QuestionStatic;
                    _window._quizAnswerTextBox.IsEnabled = true;
                    _window._quizAnswerButton.IsEnabled = true;
                    _window._quizAnswerTextBox.Focus();
                    return;
                case QuizStage.QuestionBlendOut:
                    if (_currentQuestion >= SETSIZE &&
                        _currentSet >= _totalSets - 1)
                    {
                        _window._correctResultTextBlock.Text = (100d * _correctAnswers / (SETSIZE * _totalSets)).ToString("0.##") + "%";
                        _window._statisticWorstCharacters.Text = GetWorstStatistic();
                        Stage = QuizStage.ResultBlendIn;
                        _window._progressBorder.Visibility = Visibility.Collapsed;
                        StartFadeIn(_window._quizResult);
                        return;
                    }
                    else if (_currentQuestion < SETSIZE)
                    {
                        Stage = QuizStage.QuestionBlendIn;
                        SetupNextQuestion();
                        StartFadeIn(_window._quizPage);
                    }
                    else
                    {
                        _currentSet++;
                        _currentQuestion = 0;
                        _currentWords = 0;
                        Stage = QuizStage.QuestionBlendIn;
                        SetupNextQuestion();
                        StartFadeIn(_window._quizPage);
                    }
                    return;
                case QuizStage.ResultBlendIn:
                    Stage = QuizStage.ResultStatic;
                    return;
                case QuizStage.ResultBlendOut:
                    _window._totalRunsTextBlock.Text = (_data.TotalQuestions / SETSIZE).ToString();
                    _window._correctRateTextBlock.Text = _data.SuccessRate.ToString("0.##") + "%";
                    Stage = QuizStage.MenuBlendIn;
                    StartFadeIn(_window._mainMenu);
                    return;
                case QuizStage.MenuBlendIn:
                    Stage = QuizStage.MenuBlendStatic;
                    return;
            }
        }

        private static void SetupNextQuestion()
        {
            _window._quizAnswerTextBox.Text = "";
            _data.TotalQuestions++;
            _currentQuestion++;
            _window.SetProgressbar((Double)(_currentQuestion + (SETSIZE * _currentSet)) / (SETSIZE * _totalSets));
            if (_currentWords == _totalWords)
            {
                SetupSingleCharacter();
                return;
            }
            if (_totalWords - _currentWords >= SETSIZE - _currentQuestion)
            {
                SetupDifficultCharacter();
                return;
            }
            Int32 setting = _randomizer.Next(10);
            if (setting < 8)
            {
                SetupSingleCharacter();
            }
            else
            {
                SetupDifficultCharacter();
            }

        }

        private static void SetupSingleCharacter()
        {
            Int32 choice = _randomizer.Next(_pool.Count);
            ICharacter character = _pool[choice];
            _data.ChangeAppearances(character, 1);
            _window._quizTokenTextBlock.Text = character.Letter.ToString();
            _currentAnswer.Push(character);
        }

        private static void SetupDifficultCharacter()
        {
            Int32 count = _randomizer.Next(2, _maxWordlength + 1);
            _window._quizTokenTextBlock.Text = "";
            for (Int32 i = 0; i < count; i++)
            {
                Int32 choice = _randomizer.Next(_pool.Count);
                ICharacter character = _pool[choice];
                _data.ChangeAppearances(character, 1);
                _window._quizTokenTextBlock.Text += character.Letter.ToString();
                _currentAnswer.Push(character);
            }
            _currentWords++;
        }

        public static void ConfirmAnswer()
        {
            String answer = _window._quizAnswerTextBox.Text.ToLower();
            if (answer == GetAnswerString())
            {
                _window._quizAnswerTextBlock.Text = "Correct!";
                _window._quizAnswerTextBlock.Foreground = new SolidColorBrush(Colors.LimeGreen);
                while (_currentAnswer.Count > 0)
                {
                    ICharacter character = _currentAnswer.Pop();
                    _data.ChangePriority(character, -1);
                    _data.ChangeCorrectAnswered(character, 1);
                }
                _data.TotalCorrectAnswers++;
                _correctAnswers++;
            }
            else
            {
                _window._quizAnswerTextBlock.Text = "Wrong! Correct answer: " + GetAnswerString();
                _window._quizAnswerTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                while (_currentAnswer.Count > 0)
                {
                    ICharacter character = _currentAnswer.Pop();
                    _data.ChangePriority(character, 3);
                }
            }
        }

        public static void ProceedToNextStage(Object sender = null, EventArgs e = null)
        {
            if (sender is DispatcherTimer timer)
            {
                timer.Stop();
            }
            else
            {
                return;
            }
            Stage = QuizStage.QuestionBlendOut;
            _window._quizAnswerTextBlock.Text = "";
            StartFadeOut(_window._quizPage);
        }

        #endregion

        #region Page Flow

        public static void StartFadeIn(UIElement element)
        {
            _timer?.Stop();
            _timer = new()
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };
            _timer.Tick += OnFadeIn;
            _fadeTarget = element;
            element.Visibility = Visibility.Visible;
            _timer.Start();
        }

        public static void StartFadeOut(UIElement element)
        {
            _timer?.Stop();
            _timer = new()
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };
            _timer.Tick += OnFadeOut;
            _fadeTarget = element;
            _timer.Start();
        }

        private static void OnFadeOut(Object sender, EventArgs e)
        {
            if (_fadeTarget is null)
            {
                _timer.Stop();
                _timer = null;
                return;
            }

            _fadeTarget.Opacity -= 0.05d;
            if (_fadeTarget.Opacity <= 0d)
            {
                _timer.Stop();
                _fadeTarget.Visibility = Visibility.Collapsed;
                _timer = null;
                _fadeTarget = null;
                SetupNextFadeOrStage();
            }
        }

        private static void OnFadeIn(Object sender, EventArgs e)
        {
            if (_fadeTarget is null)
            {
                _timer.Stop();
                _timer = null;
                return;
            }

            _fadeTarget.Opacity += 0.05d;
            if (_fadeTarget.Opacity >= 1d)
            {
                _timer.Stop();
                _timer = null;
                _fadeTarget = null;
                SetupNextFadeOrStage();
            }
        }

        #endregion

        #region Load Save Data

        public static void Load()
        {
            String path = Path.Combine(Environment.CurrentDirectory, "savedata.xml");
            _data = !File.Exists(path) ? new() : SaveData.Deserialize(path);
            _window._totalRunsTextBlock.Text = (_data.TotalQuestions / SETSIZE).ToString();
            _window._correctRateTextBlock.Text = _data.SuccessRate.ToString("0.##") + "%";
            _window._statisticWorstCharacters.Text = GetWorstStatistic();
        }

        #endregion

        #region Save Data

        public static void Save()
        {
            String path = Path.Combine(Environment.CurrentDirectory, "savedata.xml");
            String pathBackup = Path.Combine(Environment.CurrentDirectory, "savedata.xml.bak");
            if (File.Exists(path))
            {
                File.Move(path, pathBackup);
                File.Delete(path);
            }
            try
            {
                SaveData.Serialize(path, _data);
            }
            catch (Exception ex)
            {
                File.Delete(path);
                if (File.Exists(pathBackup))
                {
                    File.Move(pathBackup, path);
                }
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (File.Exists(pathBackup))
                {
                    File.Delete(pathBackup);
                }
            }
        }

        #endregion

        #region Register Window

        public static void Register(MainWindow window) => _window = window;

        #endregion

        #region Check Answer

        private static String GetAnswerString()
        {
            String result = "";
            foreach (ICharacter hiragana in _currentAnswer)
            {
                result = hiragana.Romaji + result;
            }
            return result;
        }

        private static String GetWorstStatistic()
        {
            String result = "";
            UInt16 max = 0;
            foreach (ICharacter character in _data)
            {
                UInt16 priority = _data.GetPriority(character);
                if (priority > max)
                {
                    max = priority;
                }
            }

            foreach (ICharacter character in _data)
            {
                UInt16 priority = _data.GetPriority(character);
                if (priority == max)
                {
                    if (String.IsNullOrWhiteSpace(result))
                    {
                        result = character.ToString();
                    }
                    else
                    {
                        result += ", " + character.ToString();
                    }
                }
            }
            return result;
        }

        #endregion

        #region Properties

        public static QuizStage Stage { get; set; } = QuizStage.Disabled;

        public static SaveData Data
        {
            get => _data;
            set => _data = value;
        }

        #endregion

        #region Fields

        private static readonly Random _randomizer = new(DateTime.Now.Millisecond);
        private static readonly Stack<ICharacter> _currentAnswer = new();
        private static readonly List<ICharacter> _pool = new();

        private static MainWindow _window = null;
        private static DispatcherTimer _timer = null;
        private static UIElement _fadeTarget = null;
        private static Int32 _currentQuestion = 0;
        private static Int32 _totalSets = 3;
        private static Int32 _currentSet = 0;
        private static Int32 _totalWords = 3;
        private static Int32 _currentWords = 0;
        private static Int32 _maxWordlength = 3;
        private static Int32 _correctAnswers = 0;
        private static SaveData _data = null;

        #endregion
    }
}
