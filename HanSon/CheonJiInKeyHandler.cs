using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace HanSon
{
    class CheonJiInKeyHandler : IKeyHandler
    {
        private const short CHO_SUNG = 0;
        private const short JUNG_SUNG = 1;
        private const short JONG_SUNG = 2;

        // delegate declarations part
        private delegate void KeyDownFunc(ref KeyEventArgs e);

        public delegate void HancheckFunc();
        public delegate void ShowFunc();
        public delegate bool IsHangeulNowFunc();
        public delegate void HideFunc();

        private char tuk = ' ';
        private bool n0 = false;
        private bool et = false;

        private bool isInstanceActivated = true;

        private char[] buffer = { ' ', ' ', ' ' };

        private HancheckFunc Hancheck;

        private ShowFunc Show;
        private HideFunc Hide;

        private Dictionary<Keys, KeyDownFunc> KeyDownDictionary;
        private void InitializeKeyDictionary()
        {
            KeyDownDictionary = new Dictionary<Keys, KeyDownFunc>
            {
                { Keys.NumPad1, KeyNumPad1Down },
                { Keys.NumPad2, KeyNumPad2Down },
                { Keys.NumPad3, KeyNumPad3Down },
                { Keys.NumPad4, KeyNumPad4Down },
                { Keys.NumPad5, KeyNumPad5Down },
                { Keys.NumPad6, KeyNumPad6Down },
                { Keys.NumPad7, KeyNumPad7Down },
                { Keys.NumPad8, KeyNumPad8Down },
                { Keys.NumPad9, KeyNumPad9Down },
                { Keys.NumPad0, KeyNumPad0Down },
                { Keys.Add, KeyAddDown },
                { Keys.Subtract, KeySubtractDown },
                { Keys.Decimal, KeyDecimalDown },
                { Keys.Enter, KeyEnterDown},
            };
        }
        private static int GetIndexFromSyllables(char[] syllables, char targetCharacter)
        {
            for (int index = 0; index < syllables.Length; index++)
            {
                if (syllables[index] == targetCharacter)
                {
                    return index; // Find
                }
            }
            return -1; // Cannot Find
        }

        private static bool GBCCheck(char Jong, ref char F, ref char L)
        {
            if(Jong == 'ㄳ')
            {
                F = 'ㄱ';
                L = 'ㅅ';
                return true;
            }
            else if(Jong == 'ㄵ')
            {
                F = 'ㄴ';
                L = 'ㅈ';
                return true;
            }
            else if (Jong == 'ㄶ')
            {
                F = 'ㄴ';
                L = 'ㅎ';
                return true;
            }
            else if (Jong == 'ㄺ')
            {
                F = 'ㄹ';
                L = 'ㄱ';
                return true;
            }
            else if (Jong == 'ㄻ')
            {
                F = 'ㄹ';
                L = 'ㅁ';
                return true;
            }
            else if (Jong == 'ㄼ')
            {
                F = 'ㄹ';
                L = 'ㅂ';
                return true;
            }
            else if (Jong == 'ㄽ')
            {
                F = 'ㄹ';
                L = 'ㅅ';
                return true;
            }
            else if (Jong == 'ㄾ')
            {
                F = 'ㄹ';
                L = 'ㅌ';
                return true;
            }
            else if (Jong == 'ㄿ')
            {
                F = 'ㄹ';
                L = 'ㅍ';
                return true;
            }
            else if (Jong == 'ㅀ')
            {
                F = 'ㄹ';
                L = 'ㅎ';
                return true;
            }
            else if (Jong == 'ㅄ')
            {
                F = 'ㅂ';
                L = 'ㅅ';
                return true;
            }
            return false;
        }

        private static string HangleHap(char ch1, char ch2, char ch3)
        {
            char[] choSung = { 'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };
            char[] jungSung = { 'ㅏ', 'ㅐ', 'ㅑ', 'ㅒ', 'ㅓ', 'ㅔ', 'ㅕ', 'ㅖ', 'ㅗ', 'ㅘ', 'ㅙ', 'ㅚ', 'ㅛ', 'ㅜ', 'ㅝ',
                            'ㅞ', 'ㅟ', 'ㅠ', 'ㅡ', 'ㅢ', 'ㅣ' };
            char[] jongSung = { ' ', 'ㄱ', 'ㄲ', 'ㄳ', 'ㄴ', 'ㄵ', 'ㄶ', 'ㄷ', 'ㄹ', 'ㄺ', 'ㄻ', 'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ',
                            'ㅀ', 'ㅁ', 'ㅂ', 'ㅄ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };
            int choSungIndex, jungSungIndex, jongSungIndex;
            bool isValidIndex;
            
            choSungIndex = GetIndexFromSyllables(choSung, ch1);
            jungSungIndex = GetIndexFromSyllables(jungSung, ch2);
            jongSungIndex = GetIndexFromSyllables(jongSung, ch3);

            isValidIndex = (-1 != choSungIndex) && (-1 != jungSungIndex) && (-1 != jongSungIndex);
            if (!isValidIndex)
            {
                return string.Format("{0}{1}{2}", ch1, ch2, ch3) ;
            }

            int uniValue = (choSungIndex * 21 * 28) + (jungSungIndex * 28) + (jongSungIndex) + 0xAC00;

            return string.Format("{0}",(char)uniValue);
        }

        private void KeyNumPad1Down(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;

            if (buffer[JUNG_SUNG] == ' ')
            {
                if (buffer[CHO_SUNG] == ' ')
                {
                    buffer[CHO_SUNG] = 'ㅂ';
                    SendKeys.SendWait("ㅂ");
                }
                else if (buffer[CHO_SUNG] == 'ㅂ')
                {
                    buffer[CHO_SUNG] = 'ㅍ';
                    SendKeys.SendWait("{BS}ㅍ");
                }
                else if (buffer[CHO_SUNG] == 'ㅍ')
                {
                    buffer[CHO_SUNG] = 'ㅃ';
                    SendKeys.SendWait("{BS}ㅃ");
                }
                else if (buffer[CHO_SUNG] == 'ㅃ')
                {
                    buffer[CHO_SUNG] = 'ㅂ';
                    SendKeys.SendWait("{BS}ㅂ");
                }
                else
                {
                    buffer[CHO_SUNG] = 'ㅂ';
                    SendKeys.SendWait("ㅂ");
                }
            }
            else
            {
                if (buffer[JONG_SUNG] == ' ')
                {
                    buffer[JONG_SUNG] = 'ㅂ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㅂ')
                {
                    buffer[JONG_SUNG] = 'ㅍ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㅍ')
                {
                    buffer[JONG_SUNG] = 'ㅂ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㄹ')
                {
                    buffer[JONG_SUNG] = 'ㄼ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㄼ')
                {
                    buffer[JONG_SUNG] = 'ㄿ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else
                {
                    buffer[CHO_SUNG] = 'ㅂ';
                    buffer[JUNG_SUNG] = ' ';
                    buffer[JONG_SUNG] = ' ';

                    SendKeys.SendWait("ㅂ");
                }
            }
        }
        private void KeyNumPad2Down(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;

            if (buffer[JUNG_SUNG] == ' ')
            {
                if (buffer[CHO_SUNG] == ' ')
                {
                    buffer[CHO_SUNG] = 'ㅅ';
                    SendKeys.SendWait("ㅅ");
                }
                else if (buffer[CHO_SUNG] == 'ㅅ')
                {
                    buffer[CHO_SUNG] = 'ㅎ';
                    SendKeys.SendWait("{BS}ㅎ");
                }
                else if (buffer[CHO_SUNG] == 'ㅎ')
                {
                    buffer[CHO_SUNG] = 'ㅆ';
                    SendKeys.SendWait("{BS}ㅆ");
                }
                else if (buffer[CHO_SUNG] == 'ㅆ')
                {
                    buffer[CHO_SUNG] = 'ㅅ';
                    SendKeys.SendWait("{BS}ㅅ");
                }
                else
                {
                    buffer[CHO_SUNG] = 'ㅅ';
                    SendKeys.SendWait("ㅅ");
                }
            }
            else
            {
                if (buffer[JONG_SUNG] == ' ')
                {
                    buffer[JONG_SUNG] = 'ㅅ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㅅ')
                {
                    buffer[JONG_SUNG] = 'ㅎ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㅎ')
                {
                    buffer[JONG_SUNG] = 'ㅆ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㅆ')
                {
                    buffer[JONG_SUNG] = 'ㅅ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㄴ')
                {
                    buffer[JONG_SUNG] = 'ㄶ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㄹ')
                {
                    buffer[JONG_SUNG] = 'ㄽ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㄱ')
                {
                    buffer[JONG_SUNG] = 'ㄳ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㄽ')
                {
                    buffer[JONG_SUNG] = 'ㅀ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㅀ')
                {
                    buffer[JONG_SUNG] = 'ㄽ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }

                else if (buffer[JONG_SUNG] == 'ㅂ')
                {
                    buffer[JONG_SUNG] = 'ㅄ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }

                else if (buffer[JONG_SUNG] == 'ㅄ')
                {
                    buffer[JONG_SUNG] = 'ㅂ';
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                    buffer[CHO_SUNG] = 'ㅆ';
                    buffer[JUNG_SUNG] = ' ';
                    buffer[JONG_SUNG] = ' ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait("ㅆ");
                }

                else if (buffer[JONG_SUNG] == 'ㄶ')
                {
                    buffer[JONG_SUNG] = 'ㄴ';
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                    buffer[CHO_SUNG] = 'ㅅ';
                    buffer[JUNG_SUNG] = ' ';
                    buffer[JONG_SUNG] = ' ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait("ㅅ");
                }
                else
                {
                    buffer[CHO_SUNG] = 'ㅅ';
                    buffer[JUNG_SUNG] = ' ';
                    buffer[JONG_SUNG] = ' ';
                    SendKeys.SendWait("ㅅ");
                }
            }
        }
        private void KeyNumPad3Down(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;

            if (buffer[JUNG_SUNG] == ' ')
            {
                if (buffer[CHO_SUNG] == ' ')
                {
                    buffer[CHO_SUNG] = 'ㅈ';
                    SendKeys.SendWait("ㅈ");
                }
                else if (buffer[CHO_SUNG] == 'ㅈ')
                {
                    buffer[CHO_SUNG] = 'ㅊ';
                    SendKeys.SendWait("{BS}ㅊ");
                }
                else if (buffer[CHO_SUNG] == 'ㅊ')
                {
                    buffer[CHO_SUNG] = 'ㅉ';
                    SendKeys.SendWait("{BS}ㅉ");
                }
                else if (buffer[CHO_SUNG] == 'ㅉ')
                {
                    buffer[CHO_SUNG] = 'ㅈ';
                    SendKeys.SendWait("{BS}ㅈ");
                }
                else
                {
                    buffer[CHO_SUNG] = 'ㅈ';
                    SendKeys.SendWait("ㅈ");
                }
            }
            else
            {
                if (buffer[JONG_SUNG] == ' ')
                {
                    buffer[JONG_SUNG] = 'ㅈ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㅈ')
                {
                    buffer[JONG_SUNG] = 'ㅊ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㅊ')
                {
                    buffer[JONG_SUNG] = 'ㅈ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㄴ')
                {
                    buffer[JONG_SUNG] = 'ㄵ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }

                else if (buffer[JONG_SUNG] == 'ㄵ')
                {
                    buffer[JONG_SUNG] = 'ㄴ';
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                    buffer[CHO_SUNG] = 'ㅉ';
                    buffer[JUNG_SUNG] = ' ';
                    buffer[JONG_SUNG] = ' ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait("ㅉ");
                }

                else
                {
                    buffer[CHO_SUNG] = 'ㅈ';
                    buffer[JUNG_SUNG] = ' ';
                    buffer[JONG_SUNG] = ' ';
                    SendKeys.SendWait("ㅈ");
                }
            }
        }
        private void KeyNumPad4Down(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;

            if (buffer[JUNG_SUNG] == ' ')
            {
                if (buffer[CHO_SUNG] == ' ')
                {
                    buffer[CHO_SUNG] = 'ㄱ';
                    SendKeys.SendWait("ㄱ");
                }
                else if (buffer[CHO_SUNG] == 'ㄱ')
                {
                    buffer[CHO_SUNG] = 'ㅋ';
                    SendKeys.SendWait("{BS}ㅋ");
                }
                else if (buffer[CHO_SUNG] == 'ㅋ')
                {
                    buffer[CHO_SUNG] = 'ㄲ';
                    SendKeys.SendWait("{BS}ㄲ");
                }
                else if (buffer[CHO_SUNG] == 'ㄲ')
                {
                    buffer[CHO_SUNG] = 'ㄱ';
                    SendKeys.SendWait("{BS}ㄱ");
                }
                else
                {
                    buffer[CHO_SUNG] = 'ㄱ';
                    SendKeys.SendWait("ㄱ");
                }
            }
            else
            {
                if (buffer[JONG_SUNG] == ' ')
                {
                    buffer[JONG_SUNG] = 'ㄱ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㄱ')
                {
                    buffer[JONG_SUNG] = 'ㅋ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㅋ')
                {
                    buffer[JONG_SUNG] = 'ㄲ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㄲ')
                {
                    buffer[JONG_SUNG] = 'ㄱ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㄹ')
                {
                    buffer[JONG_SUNG] = 'ㄺ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else
                {
                    buffer[CHO_SUNG] = 'ㄱ';
                    buffer[JUNG_SUNG] = ' ';
                    buffer[JONG_SUNG] = ' ';
                    SendKeys.SendWait("ㄱ");
                }
            }

        }
        private void KeyNumPad5Down(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;

            if (buffer[JUNG_SUNG] == ' ')
            {
                if (buffer[CHO_SUNG] == ' ')
                {
                    buffer[CHO_SUNG] = 'ㄴ';
                    SendKeys.SendWait("ㄴ");
                }
                else if (buffer[CHO_SUNG] == 'ㄴ')
                {
                    buffer[CHO_SUNG] = 'ㄹ';
                    SendKeys.SendWait("{BS}ㄹ");
                }
                else if (buffer[CHO_SUNG] == 'ㄹ')
                {
                    buffer[CHO_SUNG] = 'ㄴ';
                    SendKeys.SendWait("{BS}ㄴ");
                }
                else
                {
                    buffer[CHO_SUNG] = 'ㄴ';
                    SendKeys.SendWait("ㄴ");
                }
            }
            else
            {
                if (buffer[JONG_SUNG] == ' ')
                {
                    buffer[JONG_SUNG] = 'ㄴ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㄴ')
                {
                    buffer[JONG_SUNG] = 'ㄹ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㄹ')
                {
                    buffer[JONG_SUNG] = 'ㄴ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else
                {
                    buffer[CHO_SUNG] = 'ㄴ';
                    buffer[JUNG_SUNG] = ' ';
                    buffer[JONG_SUNG] = ' ';
                    SendKeys.SendWait("ㄴ");
                }
            }
        }
        private void KeyNumPad6Down(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;
            if (buffer[JUNG_SUNG] == ' ')
            {
                if (buffer[CHO_SUNG] == ' ')
                {
                    buffer[CHO_SUNG] = 'ㄷ';
                    SendKeys.SendWait("ㄷ");
                }
                else if (buffer[CHO_SUNG] == 'ㄷ')
                {
                    buffer[CHO_SUNG] = 'ㅌ';
                    SendKeys.SendWait("{BS}ㅌ");
                }
                else if (buffer[CHO_SUNG] == 'ㅌ')
                {
                    buffer[CHO_SUNG] = 'ㄸ';
                    SendKeys.SendWait("{BS}ㄸ");
                }
                else if (buffer[CHO_SUNG] == 'ㄸ')
                {
                    buffer[CHO_SUNG] = 'ㄷ';
                    SendKeys.SendWait("{BS}ㄷ");
                }
                else
                {
                    buffer[CHO_SUNG] = 'ㄷ';
                    SendKeys.SendWait("ㄷ");
                }
            }
            else
            {
                if (buffer[JONG_SUNG] == ' ')
                {
                    buffer[JONG_SUNG] = 'ㄷ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㄷ')
                {
                    buffer[JONG_SUNG] = 'ㅌ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㅌ')
                {
                    buffer[JONG_SUNG] = 'ㄷ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㄹ')
                {
                    buffer[JONG_SUNG] = 'ㄾ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JONG_SUNG] == 'ㄾ')
                {
                    buffer[JONG_SUNG] = 'ㄹ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                    buffer[CHO_SUNG] = 'ㄷ';
                    buffer[JUNG_SUNG] = ' ';
                    buffer[JONG_SUNG] = ' ';
                    SendKeys.SendWait("ㄷ");
                }
                else
                {
                    buffer[CHO_SUNG] = 'ㄷ';
                    buffer[JUNG_SUNG] = ' ';
                    buffer[JONG_SUNG] = ' ';
                    SendKeys.SendWait("ㄷ");
                }
            }
        }
        private void KeyNumPad7Down(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;
            if (buffer[JONG_SUNG] == ' ')
            {
                if (buffer[CHO_SUNG] == ' ' && buffer[JUNG_SUNG] == ' ')
                {
                    buffer[JUNG_SUNG] = 'ㅣ';
                    SendKeys.SendWait("ㅣ");
                }
                else if (buffer[JUNG_SUNG] == 'ㅓ')
                {
                    buffer[JUNG_SUNG] = 'ㅔ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JUNG_SUNG] == 'ㆍ')
                {
                    buffer[JUNG_SUNG] = 'ㅓ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JUNG_SUNG] == 'ᆢ')
                {
                    buffer[JUNG_SUNG] = 'ㅕ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JUNG_SUNG] == 'ㅡ')
                {
                    buffer[JUNG_SUNG] = 'ㅢ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JUNG_SUNG] == 'ㅗ')
                {
                    buffer[JUNG_SUNG] = 'ㅚ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JUNG_SUNG] == 'ㅜ')
                {
                    buffer[JUNG_SUNG] = 'ㅟ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JUNG_SUNG] == 'ㅏ')
                {
                    buffer[JUNG_SUNG] = 'ㅐ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JUNG_SUNG] == 'ㅑ')
                {
                    buffer[JUNG_SUNG] = 'ㅒ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }

                else if (buffer[JUNG_SUNG] == 'ㅕ')
                {
                    buffer[JUNG_SUNG] = 'ㅖ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JUNG_SUNG] == 'ㅠ')
                {
                    buffer[JUNG_SUNG] = 'ㅝ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JUNG_SUNG] == 'ㅝ')
                {
                    buffer[JUNG_SUNG] = 'ㅞ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JUNG_SUNG] == 'ㅘ')
                {
                    buffer[JUNG_SUNG] = 'ㅙ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JUNG_SUNG] == 'ㅣ')
                {
                    if (buffer[CHO_SUNG] == ' ')
                        buffer[JONG_SUNG] = ' ';
                    buffer[CHO_SUNG] = ' ';
                    buffer[JUNG_SUNG] = 'ㅣ';


                    SendKeys.SendWait(buffer[CHO_SUNG].ToString());
                    SendKeys.SendWait("ㅣ");
                }
                else if (buffer[JUNG_SUNG] == ' ')
                {
                    buffer[JUNG_SUNG] = 'ㅣ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }

            }
            else if (buffer[JONG_SUNG] != ' ')
            {
                char firstJong = new char();
                char secondJong = new char();
                SendKeys.SendWait("{BS}");
                if (GBCCheck(buffer[JONG_SUNG], ref firstJong, ref secondJong))
                {
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], firstJong).ToString());

                    buffer[JUNG_SUNG] = 'ㅣ';
                    buffer[CHO_SUNG] = secondJong;
                    buffer[JONG_SUNG] = ' ';
                }
                else
                {
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], ' ').ToString());

                    buffer[JUNG_SUNG] = 'ㅣ';
                    buffer[CHO_SUNG] = buffer[JONG_SUNG];
                    buffer[JONG_SUNG] = ' ';
                }
                SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
            }

        }
        private void KeyNumPad8Down(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;

            if (buffer[JONG_SUNG] == ' ')
            {
                if (buffer[CHO_SUNG] == ' ' && buffer[JUNG_SUNG] == ' ')
                {
                    buffer[JUNG_SUNG] = 'ㆍ';
                    SendKeys.SendWait("ㆍ");
                }
                else if (buffer[JUNG_SUNG] == 'ㆍ')
                {
                    buffer[JUNG_SUNG] = 'ᆢ';
                    if (buffer[CHO_SUNG] == ' ')
                    {
                        SendKeys.SendWait("{BS}");
                        SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                    }

                }
                else if (buffer[JUNG_SUNG] == ' ')
                {
                    buffer[JUNG_SUNG] = 'ㆍ';
                    if (buffer[CHO_SUNG] == ' ')
                    {
                        SendKeys.SendWait("{BS}");
                        SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                    }

                }
                else if (buffer[JUNG_SUNG] == 'ㅣ')
                {
                    buffer[JUNG_SUNG] = 'ㅏ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JUNG_SUNG] == 'ㅏ')
                {
                    buffer[JUNG_SUNG] = 'ㅑ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JUNG_SUNG] == 'ㅡ')
                {
                    buffer[JUNG_SUNG] = 'ㅜ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JUNG_SUNG] == 'ㅜ')
                {
                    buffer[JUNG_SUNG] = 'ㅠ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JUNG_SUNG] == 'ㅚ')
                {
                    buffer[JUNG_SUNG] = 'ㅘ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JUNG_SUNG] == 'ㅟ')
                {
                    buffer[JUNG_SUNG] = 'ㅝ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
            }
            else if (buffer[JONG_SUNG] != ' ')
            {
                char firstJong = new char();
                char secondJong = new char();
                SendKeys.SendWait("{BS}");
                if (GBCCheck(buffer[JONG_SUNG], ref firstJong, ref secondJong))
                {
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], firstJong).ToString());

                    buffer[JUNG_SUNG] = 'ㆍ';
                    buffer[CHO_SUNG] = secondJong;
                    buffer[JONG_SUNG] = ' ';
                }
                else
                {
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], ' ').ToString());

                    buffer[JUNG_SUNG] = 'ㆍ';
                    buffer[CHO_SUNG] = buffer[JONG_SUNG];
                    buffer[JONG_SUNG] = ' ';
                }
                SendKeys.SendWait(buffer[CHO_SUNG].ToString());
            }
        }
        private void KeyNumPad9Down(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;
            if (buffer[JONG_SUNG] == ' ')
            {
                if (buffer[CHO_SUNG] == ' ' && buffer[JUNG_SUNG] == ' ')
                {
                    buffer[JUNG_SUNG] = 'ㅡ';
                    SendKeys.SendWait("ㅡ");
                }
                else if (buffer[JUNG_SUNG] == 'ㅡ')
                {
                    if (buffer[CHO_SUNG] == ' ')
                        buffer[JONG_SUNG] = ' ';
                    buffer[CHO_SUNG] = ' ';
                    buffer[JUNG_SUNG] = 'ㅡ';


                    SendKeys.SendWait(buffer[CHO_SUNG].ToString());
                    SendKeys.SendWait("ㅡ");
                }
                else if (buffer[JUNG_SUNG] == 'ㆍ')
                {
                    buffer[JUNG_SUNG] = 'ㅗ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JUNG_SUNG] == 'ᆢ')
                {
                    buffer[JUNG_SUNG] = 'ㅛ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                }
                else if (buffer[JUNG_SUNG] == ' ')
                {
                    buffer[JUNG_SUNG] = 'ㅡ';
                    SendKeys.SendWait("{BS}");
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());

                }
            }
            else if (buffer[JONG_SUNG] != ' ')
            {
                char firstJong = new char();
                char secondJong = new char();
                SendKeys.SendWait("{BS}");
                if (GBCCheck(buffer[JONG_SUNG], ref firstJong, ref secondJong))
                {
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], firstJong).ToString());

                    buffer[JUNG_SUNG] = 'ㅡ';
                    buffer[CHO_SUNG] = secondJong;
                    buffer[JONG_SUNG] = ' ';
                }
                else
                {
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], ' ').ToString());

                    buffer[JUNG_SUNG] = 'ㅡ';
                    buffer[CHO_SUNG] = buffer[JONG_SUNG];
                    buffer[JONG_SUNG] = ' ';
                }
                SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
            }
        }
        private void KeyNumPad0Down(ref KeyEventArgs e)
        {
            if (isInstanceActivated)
            {
                e.Handled = true;

                if (buffer[JUNG_SUNG] == ' ')
                {
                    if (buffer[CHO_SUNG] == ' ')
                    {
                        buffer[CHO_SUNG] = 'ㅇ';
                        SendKeys.SendWait("ㅇ");
                    }
                    else if (buffer[CHO_SUNG] == 'ㅇ')
                    {
                        buffer[CHO_SUNG] = 'ㅁ';
                        SendKeys.SendWait("{BS}ㅁ");
                    }
                    else if (buffer[CHO_SUNG] == 'ㅁ')
                    {
                        buffer[CHO_SUNG] = 'ㅇ';
                        SendKeys.SendWait("{BS}ㅇ");
                    }
                    else
                    {
                        buffer[CHO_SUNG] = 'ㅇ';
                        SendKeys.SendWait("ㅇ");
                    }
                }
                else
                {
                    if (buffer[JONG_SUNG] == ' ')
                    {
                        buffer[JONG_SUNG] = 'ㅇ';
                        SendKeys.SendWait("{BS}");
                        SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                    }
                    else if (buffer[JONG_SUNG] == 'ㅇ')
                    {
                        buffer[JONG_SUNG] = 'ㅁ';
                        SendKeys.SendWait("{BS}");
                        SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                    }
                    else if (buffer[JONG_SUNG] == 'ㅁ')
                    {
                        buffer[JONG_SUNG] = 'ㅇ';
                        SendKeys.SendWait("{BS}");
                        SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                    }
                    else if (buffer[JONG_SUNG] == 'ㄹ')
                    {
                        buffer[JONG_SUNG] = 'ㄻ';
                        SendKeys.SendWait("{BS}");
                        SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]).ToString());
                    }
                    else
                    {
                        buffer[CHO_SUNG] = 'ㅇ';
                        buffer[JUNG_SUNG] = ' ';
                        buffer[JONG_SUNG] = ' ';
                        SendKeys.SendWait("ㅇ");
                    }
                }

            }
            n0 = true;
            if (et)
            {
                if (!isInstanceActivated)
                {
                    SendKeys.SendWait("{BS}");
                    Hancheck();
                    isInstanceActivated = true;
                    e.Handled = true;
                    Show();
                }
                else
                {
                    isInstanceActivated = false;
                    e.Handled = true;
                    Hide();
                }
            }
        }
        private void KeySubtractDown(ref KeyEventArgs e)
        {
            e.Handled = true;
            buffer[CHO_SUNG] = ' ';
            buffer[JUNG_SUNG] = ' ';
            buffer[JONG_SUNG] = ' ';
            tuk = ' ';
            SendKeys.SendWait("{BS}");

        }
        private void KeyAddDown(ref KeyEventArgs e)
        {
            e.Handled = true;
            if ((buffer[CHO_SUNG] != ' ') || (buffer[JUNG_SUNG] != ' ') || (buffer[JONG_SUNG] != ' '))
            {
                buffer[CHO_SUNG] = ' ';
                buffer[JUNG_SUNG] = ' ';
                buffer[JONG_SUNG] = ' ';
                tuk = ' ';
            }
            else
                SendKeys.SendWait(" ");
        }
        private void KeyDecimalDown(ref KeyEventArgs e)
        {
            e.Handled = true;
            buffer[CHO_SUNG] = ' ';
            buffer[JUNG_SUNG] = ' ';
            buffer[JONG_SUNG] = ' ';
            if (tuk == ' ')
            {
                tuk = '.';
                SendKeys.SendWait(".");
            }
            else if (tuk == '.')
            {
                tuk = ',';
                SendKeys.SendWait("{BS}");
                SendKeys.SendWait(",");
            }
            else if (tuk == ',')
            {
                tuk = '?';
                SendKeys.SendWait("{BS}");
                SendKeys.SendWait("?");
            }
            else if (tuk == '?')
            {
                tuk = '!';
                SendKeys.SendWait("{BS}");
                SendKeys.SendWait("!");
            }
            else if (tuk == '!')
            {
                tuk = '.';
                SendKeys.SendWait("{BS}");
                SendKeys.SendWait(".");
            }

        }
        private void KeyEnterDown(ref KeyEventArgs e)
        {
            et = true;
            if (n0)
            {
                if (!isInstanceActivated)
                {
                    SendKeys.SendWait("{BS}");
                    Hancheck();
                    isInstanceActivated = true;
                    e.Handled = true;
                    Show();
                }
                else
                {
                    isInstanceActivated = false;
                    e.Handled = true;
                    Hide();
                }
            }
            if (isInstanceActivated)
            {
                buffer[CHO_SUNG] = ' ';
                buffer[JONG_SUNG] = ' ';
                buffer[JUNG_SUNG] = ' ';
                tuk = ' ';
            }
        }

        public CheonJiInKeyHandler(HancheckFunc hancheck, ShowFunc showFunc, HideFunc hideFunc)
        {
            Hancheck = hancheck;
            Show = showFunc;
            Hide = hideFunc;

            InitializeKeyDictionary();
        }


        public void KeyDown(ref KeyEventArgs e)
        {
            if (KeyDownDictionary.ContainsKey(e.KeyData))
            {
                KeyDownDictionary[e.KeyData](ref e);
            }
            else
            {
                tuk = ' ';
            }
        }

        public void KeyUp(ref KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.NumPad0:
                    n0 = false;
                    break;
                case Keys.Enter:
                    et = false;
                    break;
            }
        }
    }
}
