using System;

namespace Narumikazuchi.Hiragana_And_Katakana_SelfTester
{
    public interface ICharacter
    {
        #region Properties

        Char Letter { get; }
        String Romaji { get; }

        #endregion
    }
}
