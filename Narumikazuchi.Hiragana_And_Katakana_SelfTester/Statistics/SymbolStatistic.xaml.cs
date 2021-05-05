using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Narumikazuchi.Hiragana_And_Katakana_SelfTester
{
    /// <summary>
    /// Interaction logic for SymbolStatistic.xaml
    /// </summary>
    public partial class SymbolStatistic : UserControl
    {
        #region Constructor

        public SymbolStatistic(ICharacter character, Boolean bad, Double rate)
        {
            this.InitializeComponent();
            this.DataContext = character;
            this._color = bad ? Brushes.Red : Brushes.Green;
            this._rate = rate;
        }

        #endregion

        #region Apply Data

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            this.ApplyCharacter();
        }

        private void ApplyCharacter()
        {
            this._percentageText.Text = (100d * this._rate).ToString("0.00") + "%";
            this._progressBorder.Width = this.ActualWidth * this._rate;
            this._progressBorder.Background = this._color;
        }

        #endregion

        #region Properties

        public ICharacter Character => this.DataContext as ICharacter;

        #endregion

        #region Fields

        private readonly Brush _color;
        private readonly Double _rate;

        #endregion
    }
}
