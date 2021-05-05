using System;
using System.Windows;
using System.Windows.Controls;

namespace Narumikazuchi.Hiragana_And_Katakana_SelfTester
{
    /// <summary>
    /// Interaction logic for SymbolChecker.xaml
    /// </summary>
    public partial class SymbolChecker : UserControl
    {
        #region Constructor

        public SymbolChecker(ICharacter character)
        {
            this.InitializeComponent();
            this.Margin = new(3);
            this.DataContext = character;
            this._checkbox.IsChecked = QuizController.Data.GetIsActive(character);
        }

        #endregion

        #region Event Handlers

        private void CheckedChanged(Object sender, RoutedEventArgs args)
        {
            if (this._checkbox.IsChecked.HasValue)
            {
                QuizController.Data.SetIsActive(this.Character, this._checkbox.IsChecked.Value);
            }
            else
            {
                QuizController.Data.SetIsActive(this.Character, false);
            }
        }

        #endregion

        #region Properties

        public ICharacter Character => this.DataContext as ICharacter;

        #endregion
    }
}
