using Pangolin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core
{
    public static class CodePage
    {
        private static IReadOnlyList<CodePoint> _codePoints = null;
        private static IReadOnlyDictionary<char, int> _codePointIndexByToken = null;
        private static IReadOnlyDictionary<string, int> _codePointIndexByCombination = null;
        
        public static bool CharacterExistsInCodePage(char c)
        {
            CreateCodePoints();

            return _codePointIndexByToken.ContainsKey(c);
        }

        public static int GetIndexFromCharacter(char c)
        {
            CreateCodePoints();

            if (_codePointIndexByToken.TryGetValue(c, out var index))
            {
                return index;
            }
            else
            {
                throw new PangolinInvalidTokenException($"Unrecognised token {c}");
            }
        }

        public static char GetCharacterFromIndex(int i)
        {
            return GetCodePointFromIndex(i).HexValue;
        }

        public static char GetCharacterFromCombination(string s)
        {
            CreateCodePoints();

            if (_codePointIndexByCombination.TryGetValue(s, out var index))
            {
                return _codePoints[index].HexValue;
            }
            else
            {
                throw new PangolinInvalidTokenException($"Unrecognised combination {s}");
            }
        }

        public static CodePoint GetCodePointFromIndex(int index)
        {
            CreateCodePoints();

            if (index >= _codePoints.Count)
            {
                throw new PangolinException($"Invalid code point index {index}");
            }

            return _codePoints[index];
        }

        private static void CreateCodePoints()
        {
            if (_codePoints == null)
            {
                var codePointsJson = Properties.Resources.codePage;
                var deserialised = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(codePointsJson, new { characters = new CodePoint[0] });
                ProvideCodePoints(deserialised.characters);
            }
        }

        public static void ProvideCodePoints(CodePoint[] rawCodePoints)
        {
            var codePoints = new CodePoint[rawCodePoints.Length].ToList();
            var codePointIndexByToken = new Dictionary<char, int>();

            // Pass 1, indexed
            for (int i = 0; i < codePoints.Count; i++)
            {
                var current = rawCodePoints[i];
                if (current.Id.HasValue)
                {
                    if (codePoints[current.Id.Value] != null)
                    {
                        throw new Exception($"Duplicate index {current.Id}");
                    }
                    else
                    {
                        codePoints[current.Id.Value] = current;
                        codePointIndexByToken.Add(current.HexValue, current.Id.Value);
                    }
                }
            }

            // Pass 2, unindexed
            for (int i = 0; i < codePoints.Count; i++)
            {
                var current = rawCodePoints[i];
                if (!current.Id.HasValue)
                {
                    int index = codePoints.IndexOf(null);
                    current.Id = index;
                    codePoints[index] = current;
                    codePointIndexByToken.Add(current.HexValue, index);
                }
            }

            _codePoints = codePoints;
            _codePointIndexByToken = codePointIndexByToken;
            _codePointIndexByCombination = _codePoints
                .Where(c => c.Combination != "")
                .ToDictionary(c => c.Combination, c => c.Id.Value);
        }

        public static CodePoint GetCodePointFromHex(string hex)
        {
            var result = _codePoints.FirstOrDefault(c => c.Hex == hex);

            if (result == null)
            {
                throw new PangolinException($"Could not find code point with hex {hex}");
            }

            return result;
        }

        public class CodePoint
        {
            public string Hex { get; private set; }
            public string Combination { get; private set; }
            public int? Id { get; set; }
            public string DisplayHex { get; private set; }

            public char HexValue { get; private set; }
            public char DisplayHexValue { get; private set; }

            public string SimpleCombination { get; private set; }

            public CodePoint(string hex, string combination, int? id, string displayHex)
            {
                Hex = hex;
                Combination = combination;
                Id = id;
                DisplayHex = displayHex;

                HexValue = Convert.ToChar(Convert.ToInt32(hex, 16));
                DisplayHexValue = displayHex == null ? HexValue : Convert.ToChar(Convert.ToInt32(displayHex, 16));

                SimpleCombination = combination == "" ? HexValue.ToString() : $"`{combination}";
            }
        }
    }
}
