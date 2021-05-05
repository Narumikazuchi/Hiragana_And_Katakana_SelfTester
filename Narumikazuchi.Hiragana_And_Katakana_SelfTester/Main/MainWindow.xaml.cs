using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Narumikazuchi.Hiragana_And_Katakana_SelfTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructor

        public MainWindow()
        {
            this.InitializeComponent();
            QuizController.Register(this);
            QuizController.Load();
        }

        #endregion

        #region Page Control

        private void StartQuizClick(Object sender, RoutedEventArgs e)
        {
            Int32 sets = Int32.Parse(this._setCountTextBox.Text);
            Int32 difficult = Int32.Parse(this._longCountTextBox.Text);
            Int32 length = Int32.Parse(this._longLengthTextBox.Text);
            if (QuizController.Setup(sets, difficult, length))
            {
                QuizController.StartFadeOut(this._mainMenu);
            }
        }

        private void ConfirmAnswerClick(Object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this._quizAnswerTextBox.Text))
            {
                this._quizAnswerTextBlock.Text = "Please enter an answer first!";
                this._quizAnswerTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                return;
            }
            QuizController.ConfirmAnswer();
            this._quizAnswerTextBox.IsEnabled = false;
            this._quizAnswerButton.IsEnabled = false;
            this._timer = new()
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            this._timer.Tick += QuizController.ProceedToNextStage;
            this._timer.Start();
        }

        private void ReturnMainMenuClick(Object sender, RoutedEventArgs e)
        {
            QuizController.Save();
            QuizController.Stage = QuizStage.ResultBlendOut;
            QuizController.StartFadeOut(this._quizResult);
        }

        private void AcceptAnswer(Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.ConfirmAnswerClick(this._quizAnswerButton, e);
            }
        }

        public void SetProgressbar(Double value)
        {
            Double width = this.Width;
            value = value < 0 ? 0 : value > 1 ? 1 : value;
            this._progressBorder.Width = value * width;
        }

        #endregion

        #region Input Check

        private void CheckTextInput(Object sender, TextCompositionEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(e.Text))
            {
                for (Int32 i = 0; i < e.Text.Length; i++)
                {
                    if (!Char.IsDigit(e.Text[i]))
                    {
                        e.Handled = true;
                        return;
                    }
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        #endregion

        #region Window Control

        private void CloseClick(Object sender, RoutedEventArgs e) => this.Close();

        protected override void OnClosing(CancelEventArgs e)
        {
            QuizController.Save();
            base.OnClosing(e);
        }

        private void SettingsClick(Object sender, RoutedEventArgs args)
        {
            SettingsWindow window = new SettingsWindow();
            window.ShowDialog();
        }

        private void StatisticsClick(Object sender, RoutedEventArgs args)
        {
            StatisticsWindow window = new StatisticsWindow();
            window.ShowDialog();
        }

        #endregion

        #region Fields

        internal UIElement _fadeTarget = null;
        internal DispatcherTimer _timer = null;

        #endregion
    }
}
