using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Narumikazuchi.Hiragana_And_Katakana_SelfTester
{
    public sealed class SaveData : IEnumerable<ICharacter>
    {
        #region Constructor

        static SaveData()
        {
            for (Int32 i = 0; i < HiraganaCharacter.Syllabary.Count; i++)
            {
                _characters.Add(HiraganaCharacter.Syllabary[i]);
            }
            for (Int32 i = 0; i < KatakanaCharacter.Syllabary.Count; i++)
            {
                _characters.Add(KatakanaCharacter.Syllabary[i]);
            }
        }

        public SaveData()
        {
            for (Int32 i = 0; i < _characters.Count; i++)
            {
                this._priorities.Add(_characters[i], 10);
                this._used.Add(_characters[i], true);
                this._appearances.Add(_characters[i], 0);
                this._correctAnswers.Add(_characters[i], 0);
            }
        }

        #endregion

        #region Serialization

        public static void Serialize(String filepath, SaveData data)
        {
            using FileStream stream = File.Create(filepath);
            using XmlTextWriter writer = new(stream, Encoding.UTF8)
            {
                Formatting = Formatting.Indented,
                Indentation = 4
            };

            writer.WriteStartDocument();
            writer.WriteStartElement("Data");
            writer.WriteAttributeString("Questions", data.TotalQuestions.ToString());
            writer.WriteAttributeString("Answers", data.TotalCorrectAnswers.ToString());
            writer.WriteStartElement("Priorities");
            foreach (ICharacter character in data)
            {
                writer.WriteStartElement(character.GetType() == typeof(HiraganaCharacter) ? "Hiragana" : "Katakana");
                writer.WriteAttributeString("Char", character.Letter.ToString());
                writer.WriteAttributeString("Romanji", character.Romaji);
                writer.WriteAttributeString("Active", data.GetIsActive(character).ToString());
                writer.WriteAttributeString("Correct", data.GetCorrectAnswered(character).ToString());
                writer.WriteAttributeString("Appearances", data.GetAppearances(character).ToString());
                writer.WriteAttributeString("Priority", data.GetPriority(character).ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        public static SaveData Deserialize(String filepath)
        {
            SaveData result = new();

            using FileStream stream = File.OpenRead(filepath);
            using XmlReader reader = XmlReader.Create(stream);

            while (reader.Read())
            {
                if (reader.Name == "Data" &&
                    reader.NodeType == XmlNodeType.Element)
                {
                    if (!UInt16.TryParse(reader.GetAttribute("Questions"), out UInt16 questions))
                    {
                        throw new InvalidCastException("TotalQuestions couldn't be cast to correct type!");
                    }
                    if (!UInt16.TryParse(reader.GetAttribute("Answers"), out UInt16 answers))
                    {
                        throw new InvalidCastException("TotalCorrectAnswers couldn't be cast to correct type!");
                    }
                    result.TotalQuestions = questions;
                    result.TotalCorrectAnswers = answers;
                }
                if (reader.Name == "Hiragana" &&
                    reader.NodeType == XmlNodeType.Element)
                {
                    Char letter = reader.GetAttribute("Char")[0];
                    String romanized = reader.GetAttribute("Romaji");
                    HiraganaCharacter hiragana = HiraganaCharacter.FromData(letter, romanized);
                    if (!_characters.Contains(hiragana))
                    {
                        throw new ArgumentException("Passed hiragana is not in the official syllabary!");
                    }

                    if (!UInt16.TryParse(reader.GetAttribute("Priority"), out UInt16 priority))
                    {
                        throw new InvalidCastException("Priority couldn't be cast to correct type!");
                    }
                    result._priorities[hiragana] = priority;
                    if (!Boolean.TryParse(reader.GetAttribute("Active"), out Boolean active))
                    {
                        throw new InvalidCastException("Active couldn't be cast to correct type!");
                    }
                    result._used[hiragana] = active;
                    if (!UInt32.TryParse(reader.GetAttribute("Appearances"), out UInt32 appearances))
                    {
                        throw new InvalidCastException("Appearances couldn't be cast to correct type!");
                    }
                    result._appearances[hiragana] = appearances;
                    if (!UInt32.TryParse(reader.GetAttribute("Correct"), out UInt32 answers))
                    {
                        throw new InvalidCastException("Correct couldn't be cast to correct type!");
                    }
                    result._correctAnswers[hiragana] = answers;
                }
                if (reader.Name == "Katakana" &&
                    reader.NodeType == XmlNodeType.Element)
                {
                    Char letter = reader.GetAttribute("Char")[0];
                    String romanized = reader.GetAttribute("Romaji");
                    KatakanaCharacter katakana = KatakanaCharacter.FromData(letter, romanized);
                    if (!_characters.Contains(katakana))
                    {
                        throw new ArgumentException("Passed katakana is not in the official syllabary!");
                    }

                    if (!UInt16.TryParse(reader.GetAttribute("Priority"), out UInt16 priority))
                    {
                        throw new InvalidCastException("Priority couldn't be cast to correct type!");
                    }
                    result._priorities[katakana] = priority;
                    if (!Boolean.TryParse(reader.GetAttribute("Active"), out Boolean active))
                    {
                        throw new InvalidCastException("Active couldn't be cast to correct type!");
                    }
                    result._used[katakana] = active;
                    if (!UInt32.TryParse(reader.GetAttribute("Appearances"), out UInt32 appearances))
                    {
                        throw new InvalidCastException("Appearances couldn't be cast to correct type!");
                    }
                    result._appearances[katakana] = appearances;
                    if (!UInt32.TryParse(reader.GetAttribute("Correct"), out UInt32 answers))
                    {
                        throw new InvalidCastException("Correct couldn't be cast to correct type!");
                    }
                    result._correctAnswers[katakana] = answers;
                }
            }

            return result;
        }

        #endregion

        #region Access Data

        public UInt16 GetPriority(ICharacter character) => _characters.Contains(character) ? this._priorities[character] : throw new KeyNotFoundException();

        public void SetPriority(ICharacter character, UInt16 priority)
        {
            if (_characters.Contains(character))
            {
                this._priorities[character] = priority > 0 ? priority : (UInt16)1;
                return;
            }
            throw new KeyNotFoundException();
        }

        public void ChangePriority(ICharacter character, Int16 byValue)
        {
            if (_characters.Contains(character))
            {
                Int64 newValue = this._priorities[character] + byValue;
                this._priorities[character] = newValue < 1 ? (UInt16)1 : (UInt16)newValue;
                return;
            }
            throw new KeyNotFoundException();
        }

        public Boolean GetIsActive(ICharacter character) => _characters.Contains(character) ? this._used[character] : throw new KeyNotFoundException();

        public void SetIsActive(ICharacter character, Boolean active)
        {
            if (_characters.Contains(character))
            {
                this._used[character] = active;
                return;
            }
            throw new KeyNotFoundException();
        }

        public UInt32 GetAppearances(ICharacter character) => _characters.Contains(character) ? this._appearances[character] : throw new KeyNotFoundException();

        public void SetAppearances(ICharacter character, UInt32 appearances)
        {
            if (_characters.Contains(character))
            {
                this._appearances[character] = appearances;
                return;
            }
            throw new KeyNotFoundException();
        }

        public void ChangeAppearances(ICharacter character, Int32 byValue)
        {
            if (_characters.Contains(character))
            {
                Int64 newValue = this._appearances[character] + byValue;
                this._appearances[character] = newValue < 0 ? 0 : (UInt32)newValue;
                return;
            }
            throw new KeyNotFoundException();
        }

        public UInt32 GetCorrectAnswered(ICharacter character) => _characters.Contains(character) ? this._correctAnswers[character] : throw new KeyNotFoundException();

        public void SetCorrectAnswered(ICharacter character, UInt32 answers)
        {
            if (_characters.Contains(character))
            {
                this._correctAnswers[character] = answers;
                return;
            }
            throw new KeyNotFoundException();
        }

        public void ChangeCorrectAnswered(ICharacter character, Int32 byValue)
        {
            if (_characters.Contains(character))
            {
                Int64 newValue = this._correctAnswers[character] + byValue;
                this._correctAnswers[character] = newValue < 0 ? 0 : (UInt32)newValue;
                return;
            }
            throw new KeyNotFoundException();
        }

        #endregion

        #region IEnumerable

        public IEnumerator<ICharacter> GetEnumerator()
        {
            for (Int32 i = 0; i < _characters.Count; i++)
            {
                yield return _characters[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        #endregion

        #region Properties

        public UInt32 TotalQuestions { get; set; } = 0;
        public UInt32 TotalCorrectAnswers { get; set; } = 0;
        public Double SuccessRate => 100d * this.TotalCorrectAnswers / this.TotalQuestions;

        #endregion

        #region Fields

        private static readonly List<ICharacter> _characters = new();
        private readonly Dictionary<ICharacter, UInt16> _priorities = new();
        private readonly Dictionary<ICharacter, Boolean> _used = new();
        private readonly Dictionary<ICharacter, UInt32> _appearances = new();
        private readonly Dictionary<ICharacter, UInt32> _correctAnswers = new();

        #endregion
    }
}
