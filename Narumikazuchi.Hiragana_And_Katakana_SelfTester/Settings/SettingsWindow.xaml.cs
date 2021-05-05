using System;
using System.Windows;

namespace Narumikazuchi.Hiragana_And_Katakana_SelfTester
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        #region Constructor

        public SettingsWindow()
        {
            this.InitializeComponent();
            this.FillWithData();
        }

        #endregion

        #region Fill Data

        private void FillWithData()
        {
            foreach (ICharacter character in QuizController.Data)
            {
                SymbolChecker symbol = new(character);
                if (character is HiraganaCharacter)
                {
                    this._hiraganaBox.Children.Add(symbol);
                }
                else if (character is KatakanaCharacter)
                {
                    this._katakanaBox.Children.Add(symbol);
                }
            }
        }

        #endregion

        #region Save Data

        private void CloseClick(Object sender, RoutedEventArgs e) => this.Close();

        private void SaveClick(Object sender, RoutedEventArgs args)
        {
            QuizController.Save();
            this.Close();
        }

        #endregion
    }
}
