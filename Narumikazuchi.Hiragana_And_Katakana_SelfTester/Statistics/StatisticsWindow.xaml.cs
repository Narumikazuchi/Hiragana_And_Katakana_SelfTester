using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Narumikazuchi.Hiragana_And_Katakana_SelfTester
{
    /// <summary>
    /// Interaction logic for StatisticsWindow.xaml
    /// </summary>
    public partial class StatisticsWindow : Window
    {
        #region Constructor

        public StatisticsWindow()
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
                UInt32 total = QuizController.Data.GetAppearances(character);
                if (total == 0)
                {
                    continue;
                }

                // Best
                if (this.RateCheckForBest(character))
                {
                    continue;
                }

                // Worst
                this.RateCheckForWorst(character);
            }

            foreach (KeyValuePair<ICharacter, Double> kv in this._best.OrderByDescending(kv => kv.Value))
            {
                SymbolStatistic statistic = new(kv.Key, false, kv.Value);
                this._bestBox.Children.Add(statistic);
            }

            foreach (KeyValuePair<ICharacter, Double> kv in this._worst.OrderBy(kv => kv.Value))
            {
                SymbolStatistic statistic = new(kv.Key, true, kv.Value);
                this._worstBox.Children.Add(statistic);
            }
        }

        private static Double GetRate(ICharacter character)
        {
            UInt32 total = QuizController.Data.GetAppearances(character);
            if (total == 0)
            {
                return 0d;
            }

            UInt32 correct = QuizController.Data.GetCorrectAnswered(character);
            return 1d * correct / total;
        }

        private Boolean RateCheckForBest(ICharacter character)
        {
            Double rate = GetRate(character);
            if (this._best.Count < MAXENTRIES)
            {
                this._best[character] = rate;
                this._best = this._best.OrderBy(kv => kv.Value).ToDictionary(kv => kv.Key, kv => kv.Value);
                return true;
            }
            ICharacter toReplace = null;
            foreach (KeyValuePair<ICharacter, Double> kv in this._best)
            {
                if (rate > kv.Value)
                {
                    toReplace = kv.Key;
                    break;
                }
            }
            if (toReplace is not null)
            {
                this._best.Remove(toReplace);
                this._best[character] = rate;
                this._best = this._best.OrderBy(kv => kv.Value).ToDictionary(kv => kv.Key, kv => kv.Value);
                this.RateCheckForWorst(toReplace);
                return true;
            }
            return false;
        }

        private void RateCheckForWorst(ICharacter character)
        {
            Double rate = GetRate(character);
            if (this._worst.Count < MAXENTRIES)
            {
                this._worst[character] = rate;
                this._worst = this._worst.OrderByDescending(kv => kv.Value).ToDictionary(kv => kv.Key, kv => kv.Value);
                return;
            }
            ICharacter toReplace = null;
            foreach (KeyValuePair<ICharacter, Double> kv in this._worst)
            {
                if (rate < kv.Value)
                {
                    toReplace = kv.Key;
                    break;
                }
            }
            if (toReplace is not null)
            {
                this._worst.Remove(toReplace);
                this._worst[character] = rate;
                this._worst = this._worst.OrderByDescending(kv => kv.Value).ToDictionary(kv => kv.Key, kv => kv.Value);
            }
        }

        #endregion

        #region Close Window

        private void CloseClick(Object sender, RoutedEventArgs e) => this.Close();

        #endregion

        #region Fields

        private Dictionary<ICharacter, Double> _best = new();
        private Dictionary<ICharacter, Double> _worst = new();

        #endregion

        #region Constants

        public const Int32 MAXENTRIES = 8;

        #endregion
    }
}
