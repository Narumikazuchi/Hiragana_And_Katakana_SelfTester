using System;
using System.Collections.Generic;

namespace Narumikazuchi.Hiragana_And_Katakana_SelfTester
{
    public struct HiraganaCharacter : IEquatable<HiraganaCharacter>, ICharacter
    {
        #region Constructor

        private HiraganaCharacter(Char letter, String romanji)
        {
            this.Letter = letter;
            this.Romaji = romanji;
        }

        #endregion

        #region ToString

        public override String ToString() => this;

        #endregion

        #region IEquatable

        public Boolean Equals(HiraganaCharacter other) => this.Letter.Equals(other.Letter);

        public override Boolean Equals(Object obj) => obj is HiraganaCharacter other && this.Equals(other);

        public override Int32 GetHashCode() => this.Letter.GetHashCode();

        #endregion

        #region Operators

        public static implicit operator String(HiraganaCharacter @this) => @this.Letter.ToString();

        public static HiraganaCharacter FromData(Char letter, String romanized) => String.IsNullOrWhiteSpace(romanized) ?
                                                                                    throw new ArgumentException("'romanized' can't be empty!", nameof(romanized)) :
                                                                                    new(letter, romanized);

        #endregion

        #region Properties

        public Char Letter { get; }
        public String Romaji { get; }

        #endregion

        #region Syllabary

        public static IReadOnlyList<HiraganaCharacter> Syllabary { get; } = new List<HiraganaCharacter>
        {
            new('あ', "a"),
            new('い', "i"),
            new('う', "u"),
            new('え', "e"),
            new('お', "o"),
            new('か', "ka"),
            new('き', "ki"),
            new('く', "ku"),
            new('け', "ke"),
            new('こ', "ko"),
            new('が', "ga"),
            new('ぎ', "gi"),
            new('ぐ', "gu"),
            new('げ', "ge"),
            new('ご', "go"),
            new('さ', "sa"),
            new('し', "shi"),
            new('す', "su"),
            new('せ', "se"),
            new('そ', "so"),
            new('ざ', "za"),
            new('じ', "ji"),
            new('ず', "zu"),
            new('ぜ', "ze"),
            new('ぞ', "zo"),
            new('た', "ta"),
            new('ち', "chi"),
            new('つ', "tsu"),
            new('て', "te"),
            new('と', "to"),
            new('だ', "da"),
            new('ぢ', "ji"),
            new('づ', "zu"),
            new('で', "de"),
            new('ど', "do"),
            new('な', "na"),
            new('に', "ni"),
            new('ぬ', "nu"),
            new('ね', "ne"),
            new('の', "no"),
            new('は', "ha"),
            new('ひ', "hi"),
            new('ふ', "fu"),
            new('へ', "he"),
            new('ほ', "ho"),
            new('ば', "ba"),
            new('び', "bi"),
            new('ぶ', "bu"),
            new('べ', "be"),
            new('ぼ', "bo"),
            new('ぱ', "pa"),
            new('ぴ', "pi"),
            new('ぷ', "pu"),
            new('ぺ', "pe"),
            new('ぽ', "po"),
            new('ま', "ma"),
            new('み', "mi"),
            new('む', "mu"),
            new('め', "me"),
            new('も', "mo"),
            new('や', "ya"),
            new('ゆ', "yu"),
            new('よ', "yo"),
            new('ら', "ra"),
            new('り', "ri"),
            new('る', "ru"),
            new('れ', "re"),
            new('ろ', "ro"),
            new('わ', "wa"),
            new('を', "wo"),
            new('ん', "n"),
        };

        #endregion
    }
}
