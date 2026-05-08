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
        private char _cho = ' ', _jung = ' ', _jong = ' ';
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
                if (_jong == ' ')
                {
                    _jong = input;
                }
                else
                {
                    // A. 겹받침 형성 시도 (ㄹ + ㅅ = ㄽ)
                    string combo = _jong.ToString() + input;
                    if (BatchimCombineRules.ContainsKey(combo))
                    {
                        _jong = BatchimCombineRules[combo];
                    }
                    // B. 단일 종성 사이클 (ㄱ -> ㅋ)
                    else if (KeyBaseMap.ContainsKey(input) && KeyBaseMap.ContainsKey(_jong) && KeyBaseMap[input] == KeyBaseMap[_jong])
                    {
                        _jong = CycleRules[_jong];
                    }
                    // C. 겹받침 내부 사이클 (ㄽ -> ㅀ)
                    else if (SplitRules.ContainsKey(_jong))
                    {
                        var split = SplitRules[_jong];
                        if (KeyBaseMap.ContainsKey(input) && KeyBaseMap.ContainsKey(split.Item2) && KeyBaseMap[input] == KeyBaseMap[split.Item2])
                        {
                            char newSecond = CycleRules[split.Item2];
                            string newCombo = split.Item1.ToString() + newSecond;
                            
                            if (BatchimCombineRules.ContainsKey(newCombo))
                            {
                                _jong = BatchimCombineRules[newCombo];
                            }
                            else
                            {
                                // 더 이상 결합 불가 시 분리 (일 + ㅆ)
                                string s1 = Compose(_cho, _jung, split.Item1);
                                _cho = newSecond; _jung = ' '; _jong = ' ';
                                _lastOutput = _cho.ToString();
                                return new AutomataResult { DeleteCount = prevDisplay.Length, InsertText = s1 + _lastOutput };
                            }
                        }
                        else
                        {
                            return StartNewSyllable(input);
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
            _cho = input; _jung = ' '; _jong = ' ';
            _lastOutput = _cho.ToString();
            return new AutomataResult { DeleteCount = 0, InsertText = _lastOutput };
        }

        public AutomataResult ProcessVowel(char input)
        {
            string prevDisplay = _lastOutput;

            if (_jong != ' ')
            {
                char firstPart, secondPart;
                if (SplitRules.ContainsKey(_jong)) { (firstPart, secondPart) = SplitRules[_jong]; }
                else { firstPart = ' '; secondPart = _jong; }

                string s1 = Compose(_cho, _jung, firstPart);
                _cho = secondPart; _jung = input; _jong = ' ';
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
                    // 결합 불가 시 새 글자 시작
                    string prevSyllable = Compose(_cho, _jung, _jong);
                    _cho = ' '; _jung = input; _jong = ' ';
                    string newOut = prevSyllable + _jung;
                    var res = new AutomataResult { DeleteCount = prevDisplay.Length, InsertText = newOut };
                    _lastOutput = newOut;
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

        public void Reset() { _cho = _jung = _jong = ' '; _lastOutput = ""; }

        private string Compose(char cho, char jung, char jong)
        {
            if (cho == ' ' && jung == ' ' && jong == ' ') return "";
            string choSungList = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";
            string jungSungList = "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ";
            string jongSungList = " ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ";

            int choIdx = choSungList.IndexOf(cho);
            int jungIdx = jungSungList.IndexOf(jung);
            int jongIdx = jongSungList.IndexOf(jong);

            if (choIdx < 0 || jungIdx < 0)
            {
                return $"{cho}{jung}{jong}".Replace(" ", "").Replace("\0", "");
            }

            int uniValue = (choIdx * 21 * 28) + (jungIdx * 28) + jongIdx + 0xAC00;
            return ((char)uniValue).ToString();
        }
    }
}
