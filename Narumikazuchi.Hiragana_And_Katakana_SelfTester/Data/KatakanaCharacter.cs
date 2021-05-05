using System;
using System.Collections.Generic;

namespace Narumikazuchi.Hiragana_And_Katakana_SelfTester
{
    public struct KatakanaCharacter : IEquatable<KatakanaCharacter>, ICharacter
    {
        #region Constructor

        private KatakanaCharacter(Char letter, String romanji)
        {
            this.Letter = letter;
            this.Romaji = romanji;
        }

        #endregion

        #region ToString

        public override String ToString() => this;

        #endregion

        #region IEquatable

        public Boolean Equals(KatakanaCharacter other) => this.Letter.Equals(other.Letter);

        public override Boolean Equals(Object obj) => obj is KatakanaCharacter other && this.Equals(other);

        public override Int32 GetHashCode() => this.Letter.GetHashCode();

        #endregion

        #region Operators

        public static implicit operator String(KatakanaCharacter @this) => @this.Letter.ToString();

        public static KatakanaCharacter FromData(Char letter, String romanized) => String.IsNullOrWhiteSpace(romanized) ?
                                                                                    throw new ArgumentException("'romanized' can't be empty!", nameof(romanized)) :
                                                                                    new(letter, romanized);

        #endregion

        #region Properties

        public Char Letter { get; }
        public String Romaji { get; }

        #endregion

        #region Syllabary

        public static IReadOnlyList<KatakanaCharacter> Syllabary { get; } = new List<KatakanaCharacter>
        {
            new('ア', "a"),
            new('イ', "i"),
            new('ウ', "u"),
            new('エ', "e"),
            new('オ', "o"),
            new('カ', "ka"),
            new('キ', "ki"),
            new('ク', "ku"),
            new('ケ', "ke"),
            new('コ', "ko"),
            new('ガ', "ga"),
            new('ギ', "gi"),
            new('グ', "gu"),
            new('ゲ', "ge"),
            new('ゴ', "go"),
            new('サ', "sa"),
            new('シ', "shi"),
            new('ス', "su"),
            new('セ', "se"),
            new('ソ', "so"),
            new('ザ', "za"),
            new('ジ', "ji"),
            new('ズ', "zu"),
            new('ゼ', "ze"),
            new('ゾ', "zo"),
            new('タ', "ta"),
            new('チ', "chi"),
            new('ツ', "tsu"),
            new('テ', "te"),
            new('ト', "to"),
            new('ダ', "da"),
            new('ヂ', "ji"),
            new('ヅ', "zu"),
            new('デ', "de"),
            new('ド', "do"),
            new('ナ', "na"),
            new('ニ', "ni"),
            new('ヌ', "nu"),
            new('ネ', "ne"),
            new('ノ', "no"),
            new('ハ', "ha"),
            new('ヒ', "hi"),
            new('フ', "fu"),
            new('ヘ', "he"),
            new('ホ', "ho"),
            new('バ', "ba"),
            new('ビ', "bi"),
            new('ブ', "bu"),
            new('ベ', "be"),
            new('ボ', "bo"),
            new('パ', "pa"),
            new('ピ', "pi"),
            new('プ', "pu"),
            new('ペ', "pe"),
            new('ポ', "po"),
            new('マ', "ma"),
            new('ミ', "mi"),
            new('ム', "mu"),
            new('メ', "me"),
            new('モ', "mo"),
            new('ヤ', "ya"),
            new('ユ', "yu"),
            new('ヨ', "yo"),
            new('ラ', "ra"),
            new('リ', "ri"),
            new('ル', "ru"),
            new('レ', "re"),
            new('ロ', "ro"),
            new('ワ', "wa"),
            new('ヲ', "wo")
        };

        #endregion
    }
}
