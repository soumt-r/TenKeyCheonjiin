using System;
using System.Collections.Generic;

namespace HanSon
{
    public class AutomataResult
    {
        public int DeleteCount { get; set; } = 0;
        public string InsertText { get; set; } = "";
    }

    public class HangeulAutomata
    {
        private char _cho = ' ', _jung = ' ';
        private string _jong = "";
        private string _lastOutput = "";

        // 자음 순환 규칙
        private static readonly Dictionary<char, char> CycleRules = new Dictionary<char, char> {
            {'ㅂ','ㅍ'}, {'ㅍ','ㅃ'}, {'ㅃ','ㅂ'},
            {'ㅅ','ㅎ'}, {'ㅎ','ㅆ'}, {'ㅆ','ㅅ'},
            {'ㅈ','ㅊ'}, {'ㅊ','ㅉ'}, {'ㅉ','ㅈ'},
            {'ㄱ','ㅋ'}, {'ㅋ','ㄲ'}, {'ㄲ','ㄱ'},
            {'ㄴ','ㄹ'}, {'ㄹ','ㄴ'},
            {'ㄷ','ㅌ'}, {'ㅌ','ㄸ'}, {'ㄸ','ㄷ'},
            {'ㅇ','ㅁ'}, {'ㅁ','ㅇ'}
        };

        // 키 그룹 매핑
        private static readonly Dictionary<char, char> KeyBaseMap = new Dictionary<char, char> {
            {'ㅂ','ㅂ'}, {'ㅍ','ㅂ'}, {'ㅃ','ㅂ'},
            {'ㅅ','ㅅ'}, {'ㅎ','ㅅ'}, {'ㅆ','ㅅ'},
            {'ㅈ','ㅈ'}, {'ㅊ','ㅈ'}, {'ㅉ','ㅈ'},
            {'ㄱ','ㄱ'}, {'ㅋ','ㄱ'}, {'ㄲ','ㄱ'},
            {'ㄴ','ㄴ'}, {'ㄹ','ㄴ'},
            {'ㄷ','ㄷ'}, {'ㅌ','ㄷ'}, {'ㄸ','ㄷ'},
            {'ㅇ','ㅇ'}, {'ㅁ','ㅇ'}
        };

        // 천지인 모음 결합 규칙
        private static readonly Dictionary<string, char> VowelMergeRules = new Dictionary<string, char> {
            {"ㅣㆍ", 'ㅏ'}, {"ㅏㆍ", 'ㅑ'}, {"ㆍㅣ", 'ㅓ'}, {"ㆍㅡ", 'ㅗ'},
            {"ㅡㆍ", 'ㅜ'}, {"ㅜㆍ", 'ㅠ'}, {"ㆍㆍ", 'ᆢ'}, {"ᆢㅣ", 'ㅕ'},
            {"ᆢㅡ", 'ㅛ'}, {"ㅚㆍ", 'ㅘ'}, {"ㅟㆍ", 'ㅝ'}, {"ㅠㅣ", 'ㅝ'},
            {"ㅓㅣ", 'ㅔ'}, {"ㅕㅣ", 'ㅖ'}, {"ㅏㅣ", 'ㅐ'}, {"ㅑㅣ", 'ㅒ'},
            {"ㅗㅣ", 'ㅚ'}, {"ㅜㅣ", 'ㅟ'}, {"ㅗㅏ", 'ㅘ'}, {"ㅜㅓ", 'ㅝ'},
            {"ㅘㅣ", 'ㅙ'}, {"ㅝㅣ", 'ㅞ'}, {"ㅡㅣ", 'ㅢ'}
        };

        // 겹받침 형성 규칙
        private static readonly Dictionary<string, char> BatchimCombineRules = new Dictionary<string, char> {
            {"ㄴㅈ", 'ㄵ'}, {"ㄴㅎ", 'ㄶ'},
            {"ㄱㅅ", 'ㄳ'}, 
            {"ㄹㄱ", 'ㄺ'}, {"ㄹㅁ", 'ㄻ'},
            {"ㄹㅂ", 'ㄼ'}, {"ㄹㅅ", 'ㄽ'},
            {"ㄹㅌ", 'ㄾ'}, {"ㄹㅍ", 'ㄿ'}, {"ㄹㅎ", 'ㅀ'}, {"ㅂㅅ", 'ㅄ'}
        };

        // 겹받침 분할 규칙
        private static readonly Dictionary<char, (char, char)> SplitRules = new Dictionary<char, (char, char)> {
            {'ㄵ', ('ㄴ', 'ㅈ')}, {'ㄶ', ('ㄴ', 'ㅎ')}, {'ㄳ', ('ㄱ', 'ㅅ')}, {'ㄺ', ('ㄹ', 'ㄱ')},
            {'ㄻ', ('ㄹ', 'ㅁ')}, {'ㄼ', ('ㄹ', 'ㅂ')}, {'ㄽ', ('ㄹ', 'ㅅ')}, {'ㄾ', ('ㄹ', 'ㅌ')},
            {'ㄿ', ('ㄹ', 'ㅍ')}, {'ㅀ', ('ㄹ', 'ㅎ')}, {'ㅄ', ('ㅂ', 'ㅅ')}
        };

        public AutomataResult ProcessConsonant(char input)
        {
            string prevDisplay = _lastOutput;

            // 1. 초성 단계
            if (_jung == ' ')
            {
                if (_cho != ' ' && KeyBaseMap.ContainsKey(input) && KeyBaseMap.ContainsKey(_cho) && KeyBaseMap[input] == KeyBaseMap[_cho])
                {
                    _cho = CycleRules[_cho];
                    _lastOutput = _cho.ToString();
                    return new AutomataResult { DeleteCount = prevDisplay.Length, InsertText = _lastOutput };
                }
                else
                {
                    return StartNewSyllable(input);
                }
            }
            // 2. 종성 단계
            else
            {
                if (string.IsNullOrEmpty(_jong))
                {
                    _jong = input.ToString();
                }
                else if (_jong.Length == 1)
                {
                    string combo = _jong + input;
                    if (BatchimCombineRules.ContainsKey(combo))
                    {
                        _jong = BatchimCombineRules[combo].ToString();
                    }
                    else if (KeyBaseMap.ContainsKey(input) && KeyBaseMap.ContainsKey(_jong[0]) && KeyBaseMap[input] == KeyBaseMap[_jong[0]])
                    {
                        _jong = CycleRules[_jong[0]].ToString();
                    }
                    else if (SplitRules.ContainsKey(_jong[0]))
                    {
                        var split = SplitRules[_jong[0]];
                        if (KeyBaseMap.ContainsKey(input) && KeyBaseMap[input] == KeyBaseMap[split.Item2])
                        {
                            char nextSec = CycleRules[split.Item2];
                            string newC = split.Item1.ToString() + nextSec;
                            if (BatchimCombineRules.ContainsKey(newC)) _jong = BatchimCombineRules[newC].ToString();
                            else
                            {
                                string s1 = Compose(_cho, _jung, split.Item1.ToString());
                                _cho = nextSec; _jung = ' '; _jong = "";
                                _lastOutput = _cho.ToString();
                                return new AutomataResult { DeleteCount = prevDisplay.Length, InsertText = s1 + _lastOutput };
                            }
                        }
                        else return StartNewSyllable(input);
                    }
                    else if ("ㄱㄴㄹㅂㅅ".Contains(_jong))
                    {
                        _jong += input;
                    }
                    else
                    {
                        return StartNewSyllable(input);
                    }
                }
                else // _jong.Length == 2 (가상 겹받침 상태)
                {
                    char first = _jong[0];
                    char second = _jong[1];
                    if (KeyBaseMap.ContainsKey(input) && KeyBaseMap.ContainsKey(second) && KeyBaseMap[input] == KeyBaseMap[second])
                    {
                        char nextSec = CycleRules[second];
                        string newC = first.ToString() + nextSec;
                        if (BatchimCombineRules.ContainsKey(newC))
                        {
                            _jong = BatchimCombineRules[newC].ToString();
                        }
                        else
                        {
                            _jong = first.ToString() + nextSec;
                        }
                    }
                    else
                    {
                        return StartNewSyllable(input);
                    }
                }
                _lastOutput = Compose(_cho, _jung, _jong);
                return new AutomataResult { DeleteCount = prevDisplay.Length, InsertText = _lastOutput };
            }
        }

        private AutomataResult StartNewSyllable(char input)
        {
            _cho = input; _jung = ' '; _jong = "";
            _lastOutput = _cho.ToString();
            return new AutomataResult { DeleteCount = 0, InsertText = _lastOutput };
        }

        public AutomataResult ProcessVowel(char input)
        {
            string prevDisplay = _lastOutput;

            if (!string.IsNullOrEmpty(_jong))
            {
                char firstPart, secondPart;
                if (_jong.Length == 2)
                {
                    firstPart = _jong[0];
                    secondPart = _jong[1];
                }
                else if (SplitRules.ContainsKey(_jong[0]))
                {
                    (firstPart, secondPart) = SplitRules[_jong[0]];
                }
                else
                {
                    firstPart = ' ';
                    secondPart = _jong[0];
                }

                string s1 = Compose(_cho, _jung, firstPart == ' ' ? "" : firstPart.ToString());
                _cho = secondPart; _jung = input; _jong = "";
                string s2 = Compose(_cho, _jung, _jong);
                
                _lastOutput = s2;
                return new AutomataResult { DeleteCount = prevDisplay.Length, InsertText = s1 + s2 };
            }

            if (_jung == ' ')
            {
                _jung = input;
            }
            else
            {
                string key = _jung.ToString() + input;
                if (VowelMergeRules.ContainsKey(key))
                {
                    _jung = VowelMergeRules[key];
                }
                else
                {
                    string prevS = Compose(_cho, _jung, _jong);
                    _cho = ' '; _jung = input; _jong = "";
                    string newOut = prevS + _jung;
                    var res = new AutomataResult { DeleteCount = prevDisplay.Length, InsertText = newOut };
                    _lastOutput = _jung.ToString();
                    return res;
                }
            }

            _lastOutput = Compose(_cho, _jung, _jong);
            return new AutomataResult { DeleteCount = prevDisplay.Length, InsertText = _lastOutput };
        }

        public AutomataResult ProcessConsonantOrVowel(char input, bool isVowel)
        {
            return isVowel ? ProcessVowel(input) : ProcessConsonant(input);
        }

        public void Reset() { _cho = _jung = ' '; _jong = ""; _lastOutput = ""; }

        private string Compose(char cho, char jung, string jong)
        {
            if (cho == ' ' && jung == ' ' && string.IsNullOrEmpty(jong)) return "";
            if (jong.Length == 2) return Compose(cho, jung, jong[0].ToString()) + jong[1];

            char j = (jong.Length > 0) ? jong[0] : ' ';
            string choSungList = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";
            string jungSungList = "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ";
            string jongSungList = " ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ";

            int choIdx = choSungList.IndexOf(cho);
            int jungIdx = jungSungList.IndexOf(jung);
            int jongIdx = jongSungList.IndexOf(j);

            if (choIdx < 0 || jungIdx < 0)
            {
                return $"{cho}{jung}{j}".Replace(" ", "").Replace("\0", "");
            }

            int uniValue = (choIdx * 21 * 28) + (jungIdx * 28) + jongIdx + 0xAC00;
            return ((char)uniValue).ToString();
        }
    }
}
