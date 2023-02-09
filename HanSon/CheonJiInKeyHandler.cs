using System.Collections.Generic;
using System.Windows.Forms;

namespace HanSon
{
    class CheonJiInKeyHandler : IKeyHandler
    {
        private const short CHO_SUNG = 0;
        private const short JUNG_SUNG = 1;
        private const short JONG_SUNG = 2;

        // delegate declarations part
        public delegate void ChangeUILangFunc(int langMode);
        private delegate void KeyDownFunc(ref KeyEventArgs e);

        public delegate void HancheckFunc(bool toHangeul);
        public delegate void ShowFunc();
        public delegate bool IsHangeulNowFunc();
        public delegate void HideFunc();

        private char tuk = ' ';
        private bool n0;
        private bool et;

        private int langMode = 0;       // 0: Korean 1: English
        private bool isInstanceActivated = true;

        private char[] buffer = { ' ', ' ', ' ' };
        private char engbuffer = ' ';

        private readonly HancheckFunc Hancheck;

        private readonly ShowFunc Show;
        private readonly HideFunc Hide;
        private readonly ChangeUILangFunc ChangeUILang;

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
                { Keys.Multiply, KeyMultiplyDown },
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
            bool result = true;
            switch (Jong)
            {
                case 'ㄳ':
                    F = 'ㄱ';
                    L = 'ㅅ';
                    break;
                case 'ㄵ':
                    F = 'ㄴ';
                    L = 'ㅈ';
                    break;
                case 'ㄶ':
                    F = 'ㄴ';
                    L = 'ㅎ';
                    break;
                case 'ㄺ':
                    F = 'ㄹ';
                    L = 'ㄱ';
                    break;
                case 'ㄻ':
                    F = 'ㄹ';
                    L = 'ㅁ';
                    break;
                case 'ㄼ':
                    F = 'ㄹ';
                    L = 'ㅂ';
                    break;
                case 'ㄽ':
                    F = 'ㄹ';
                    L = 'ㅅ';
                    break;
                case 'ㄾ':
                    F = 'ㄹ';
                    L = 'ㅌ';
                    break;
                case 'ㄿ':
                    F = 'ㄹ';
                    L = 'ㅍ';
                    break;
                case 'ㅀ':
                    F = 'ㄹ';
                    L = 'ㅎ';
                    break;
                case 'ㅄ':
                    F = 'ㅂ';
                    L = 'ㅅ';
                    break;
                default:
                    result = false;
                    break;
            }
            return result;
        }
        private void SendBuffer()
        {
            SendKeys.SendWait("{BS}");
            SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]));
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
                return string.Format("{0}{1}{2}", ch1.ToString().Replace(' ', '\0'), ch2.ToString().Replace(' ', '\0'), ch3.ToString().Replace(' ', '\0'));
            }

            int uniValue = (choSungIndex * 21 * 28) + (jungSungIndex * 28) + (jongSungIndex) + 0xAC00;

            
            return string.Format("{0}", (char)uniValue);
        }

        private void KeyNumPad1Down(ref KeyEventArgs e)
        {
            if(langMode == 0)
            {
                KeyNumPad1DownHangul(ref e);
            }
            else if (langMode == 1)
            {
                KeyNumPad1DownEnglish(ref e);
            }

        }

        private void KeyNumPad2Down(ref KeyEventArgs e)
        {
            if(langMode == 0)
            {
                KeyNumPad2DownHangul(ref e);
            }
            else if (langMode == 1)
            {
                KeyNumPad2DownEnglish(ref e);
            }

        }

        private void KeyNumPad3Down(ref KeyEventArgs e)
        {
            if(langMode == 0)
            {
                KeyNumPad3DownHangul(ref e);
            }
            else if (langMode == 1)
            {
                KeyNumPad3DownEnglish(ref e);
            }
        }
        
        private void KeyNumPad4Down(ref KeyEventArgs e)
        {
            if(langMode == 0)
            {
                KeyNumPad4DownHangul(ref e);
            }
            else if (langMode == 1)
            {
                KeyNumPad4DownEnglish(ref e);
            }
        }

        private void KeyNumPad5Down(ref KeyEventArgs e)
        {
            if(langMode == 0)
            {
                KeyNumPad5DownHangul(ref e);
            }
            else if (langMode == 1)
            {
                KeyNumPad5DownEnglish(ref e);
            }
        }
        private void KeyNumPad6Down(ref KeyEventArgs e)
        {
            if (langMode == 0)
            {
                KeyNumPad6DownHangul(ref e);
            }
            else if (langMode == 1)
            {
                KeyNumPad6DownEnglish(ref e);
            }
        }
        private void KeyNumPad7Down(ref KeyEventArgs e)
        {
            if (langMode == 0)
            {
                KeyNumPad7DownHangul(ref e);
            }
            else if (langMode == 1)
            {
                e.Handled = true;    
            }
        }
        private void KeyNumPad8Down(ref KeyEventArgs e)
        {
            if (langMode == 0)
            {
                KeyNumPad8DownHangul(ref e);
            }
            else if (langMode == 1)
            {
                KeyNumPad8DownEnglish(ref e);
            }
        }
        private void KeyNumPad9Down(ref KeyEventArgs e)
        {
            if (langMode == 0)
            {
                KeyNumPad9DownHangul(ref e);
            }
            else if (langMode == 1)
            {
                KeyNumPad9DownEnglish(ref e);
            }
        }

        private void KeyAddDown(ref KeyEventArgs e)
        {
            if (langMode == 0)
            {
                KeyAddDownHangul(ref e);
            }
            else if (langMode == 1)
            {
                KeyAddDownEnglish(ref e);
            }
        }

        private void KeyMultiplyDown(ref KeyEventArgs e)
        {
            if (langMode == 0)
            {
                langMode = 1;
            }
            else if (langMode == 1)
            {
                langMode = 0;
            }
            ChangeUILang(langMode);
            Hancheck(langMode == 0);
            e.Handled = true;
        }

        private void KeyNumPad1DownEnglish(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;

            bool noChangeChar = false;
            switch (engbuffer)
            {
                case 'p':
                    engbuffer = 'q';
                    break;
                case 'q':
                    engbuffer = 'r';
                    break;
                case 'r':
                    engbuffer = 's';
                    break;
                case 's':
                    engbuffer = 'P';
                    break;
                case 'P':
                    engbuffer = 'Q';
                    break;
                case 'Q':
                    engbuffer = 'R';
                    break;
                case 'R':
                    engbuffer = 'S';
                    break;
                case 'S':
                    engbuffer = 'p';
                    break;
                default:
                    engbuffer = 'p';
                    noChangeChar = true;
                    break;

            }
            SendKeys.SendWait((noChangeChar ? "" : "{BS}") + engbuffer.ToString());
        }

        private void KeyNumPad2DownEnglish(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;

            bool noChangeChar = false;
            switch (engbuffer)
            {
                case 't':
                    engbuffer = 'u';
                    break;
                case 'u':
                    engbuffer = 'v';
                    break;
                case 'v':
                    engbuffer = 'T';
                    break;
                case 'T':
                    engbuffer = 'U';
                    break;
                case 'U':
                    engbuffer = 'V';
                    break;
                case 'V':
                    engbuffer = 't';
                    break;
                default:
                    engbuffer = 't';
                    noChangeChar = true;
                    break;

            }
            SendKeys.SendWait((noChangeChar ? "" : "{BS}") + engbuffer.ToString());
        }

        private void KeyNumPad3DownEnglish(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;

            bool noChangeChar = false;
            switch (engbuffer)
            {
                case 'w':
                    engbuffer = 'x';
                    break;
                case 'x':
                    engbuffer = 'y';
                    break;
                case 'y':
                    engbuffer = 'z';
                    break;
                case 'z':
                    engbuffer = 'W';
                    break;
                case 'W':
                    engbuffer = 'X';
                    break;
                case 'X':
                    engbuffer = 'Y';
                    break;
                case 'Y':
                    engbuffer = 'Z';
                    break;
                case 'Z':
                    engbuffer = 'w';
                    break;
                default:
                    engbuffer = 'w';
                    noChangeChar = true;
                    break;

            }
            SendKeys.SendWait((noChangeChar ? "" : "{BS}") + engbuffer.ToString());
        }

        private void KeyNumPad4DownEnglish(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;

            bool noChangeChar = false;
            switch (engbuffer)
            {
                case 'g':
                    engbuffer = 'h';
                    break;
                case 'h':
                    engbuffer = 'i';
                    break;
                case 'i':
                    engbuffer = 'G';
                    break;
                case 'G':
                    engbuffer = 'H';
                    break;
                case 'H':
                    engbuffer = 'I';
                    break;
                case 'I':
                    engbuffer = 'g';
                    break;
                default:
                    engbuffer = 'g';
                    noChangeChar = true;
                    break;

            }
            SendKeys.SendWait((noChangeChar ? "" : "{BS}") + engbuffer.ToString());
        }

        private void KeyNumPad5DownEnglish(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;

            bool noChangeChar = false;
            switch (engbuffer)
            {
                case 'j':
                    engbuffer = 'k';
                    break;
                case 'k':
                    engbuffer = 'l';
                    break;
                case 'l':
                    engbuffer = 'J';
                    break;
                case 'J':
                    engbuffer = 'K';
                    break;
                case 'K':
                    engbuffer = 'L';
                    break;
                case 'L':
                    engbuffer = 'j';
                    break;
                default:
                    engbuffer = 'j';
                    noChangeChar = true;
                    break;

            }
            SendKeys.SendWait((noChangeChar ? "" : "{BS}") + engbuffer.ToString());
        }

        private void KeyNumPad6DownEnglish(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;

            bool noChangeChar = false;
            switch (engbuffer)
            {
                case 'm':
                    engbuffer = 'n';
                    break;
                case 'n':
                    engbuffer = 'o';
                    break;
                case 'o':
                    engbuffer = 'M';
                    break;
                case 'M':
                    engbuffer = 'N';
                    break;
                case 'N':
                    engbuffer = 'O';
                    break;
                case 'O':
                    engbuffer = 'm';
                    break;
                default:
                    engbuffer = 'm';
                    noChangeChar = true;
                    break;

            }
            SendKeys.SendWait((noChangeChar ? "" : "{BS}") + engbuffer.ToString());
        }

        private void KeyNumPad8DownEnglish(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;
            
            bool noChangeChar = false;
            switch (engbuffer)
            {
                case 'a':
                    engbuffer = 'b';
                    break;
                case 'b':
                    engbuffer = 'c';
                    break;
                case 'c':
                    engbuffer = 'A';
                    break;
                case 'A':
                    engbuffer = 'B';
                    break;
                case 'B':
                    engbuffer = 'C';
                    break;
                case 'C':
                    engbuffer = 'a';
                    break;
                default:
                    engbuffer = 'a';
                    noChangeChar = true;
                    break;

            }
            SendKeys.SendWait((noChangeChar ? "" : "{BS}") + engbuffer.ToString());
        }

        private void KeyNumPad9DownEnglish(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;

            bool noChangeChar = false;
            switch (engbuffer)
            {
                case 'd':
                    engbuffer = 'e';
                    break;
                case 'e':
                    engbuffer = 'f';
                    break;
                case 'f':
                    engbuffer = 'D';
                    break;
                case 'D':
                    engbuffer = 'E';
                    break;
                case 'E':
                    engbuffer = 'F';
                    break;
                case 'F':
                    engbuffer = 'd';
                    break;
                default:
                    engbuffer = 'd';
                    noChangeChar = true;
                    break;

            }
            SendKeys.SendWait((noChangeChar ? "" : "{BS}") + engbuffer.ToString());
        }

        private void KeyNumPad1DownHangul(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;

            if (buffer[JUNG_SUNG] == ' ')
            {
                bool noChangeChar = false;
                switch(buffer[CHO_SUNG])
                {
                    case 'ㅂ':
                        buffer[CHO_SUNG] = 'ㅍ';
                        break;
                    case 'ㅍ':
                        buffer[CHO_SUNG] = 'ㅃ';
                        break;
                    case 'ㅃ':
                        buffer[CHO_SUNG] = 'ㅂ';
                        break;
                    default:
                        buffer[CHO_SUNG] = 'ㅂ';
                        noChangeChar = true;
                        break;
                }
                SendKeys.SendWait((noChangeChar ? "" : "{BS}") + buffer[CHO_SUNG].ToString());
            }
            else
            {
                bool noBufferSend = false;
                switch (buffer[JONG_SUNG])
                {
                    case ' ':
                    case 'ㅍ':
                        buffer[JONG_SUNG] = 'ㅂ';
                        break;
                    case 'ㅂ':
                        buffer[JONG_SUNG] = 'ㅍ';
                        break;
                    case 'ㄹ':
                        buffer[JONG_SUNG] = 'ㄼ';
                        break;
                    case 'ㄼ':
                        buffer[JONG_SUNG] = 'ㄿ';
                        break;
                    case 'ㄿ':
                        buffer[JONG_SUNG] = 'ㄼ';
                        break;
                    default:
                        buffer[CHO_SUNG] = 'ㅂ';
                        buffer[JUNG_SUNG] = ' ';
                        buffer[JONG_SUNG] = ' ';
                        SendKeys.SendWait("ㅂ");
                        noBufferSend = true;
                        break;
                }
                if (!noBufferSend)
                {
                    SendBuffer();
                }
            }
        }
        private void KeyNumPad2DownHangul(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;

            if (buffer[JUNG_SUNG] == ' ')
            {
                bool noChangeChar = false;
                switch(buffer[CHO_SUNG])
                {
                    case 'ㅅ':
                        buffer[CHO_SUNG] = 'ㅎ';
                        break;
                    case 'ㅎ':
                        buffer[CHO_SUNG] = 'ㅆ';
                        break;
                    case 'ㅆ':
                        buffer[CHO_SUNG] = 'ㅅ';
                        break;
                    default:
                        buffer[CHO_SUNG] = 'ㅅ';
                        noChangeChar = true;
                        break;
                }
                SendKeys.SendWait((noChangeChar ? "" : "{BS}") + buffer[CHO_SUNG].ToString());
            }
            else
            {
                bool noBufferSend = false;
                switch (buffer[JONG_SUNG])
                {
                    case ' ':
                    case 'ㅆ':
                        buffer[JONG_SUNG] = 'ㅅ';
                        break;
                    case 'ㅅ':
                        buffer[JONG_SUNG] = 'ㅎ';
                        break;
                    case 'ㅎ':
                        buffer[JONG_SUNG] = 'ㅆ';
                        break;
                    case 'ㄴ':
                        buffer[JONG_SUNG] = 'ㄶ';
                        break;
                    case 'ㄱ':
                        buffer[JONG_SUNG] = 'ㄳ';
                        break;
                    case 'ㄹ':
                        buffer[JONG_SUNG] = 'ㄽ';
                        break;
                    case 'ㄽ':
                        buffer[JONG_SUNG] = 'ㅀ';
                        break;
                    case 'ㅀ':
                        buffer[JONG_SUNG] = 'ㄽ';
                        break;
                    case 'ㅂ':
                        buffer[JONG_SUNG] = 'ㅄ';
                        break;
                    case 'ㅄ':
                        buffer[JONG_SUNG] = 'ㅂ';
                        SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]));
                        buffer[CHO_SUNG] = 'ㅆ';
                        buffer[JUNG_SUNG] = ' ';
                        buffer[JONG_SUNG] = ' ';
                        SendKeys.SendWait("{BS}");
                        SendKeys.SendWait("ㅆ");
                        noBufferSend = true;
                        break;
                    default:
                        buffer[CHO_SUNG] = 'ㅅ';
                        buffer[JUNG_SUNG] = ' ';
                        buffer[JONG_SUNG] = ' ';
                        SendKeys.SendWait("ㅅ");
                        noBufferSend = true;
                        break;
                }
                if (!noBufferSend)
                {
                    SendBuffer();
                }
            }
        }
        private void KeyNumPad3DownHangul(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;

            if (buffer[JUNG_SUNG] == ' ')
            {
                bool noChangeChar = false;
                switch(buffer[CHO_SUNG])
                {
                    case 'ㅈ':
                        buffer[CHO_SUNG] = 'ㅊ';
                        break;
                    case 'ㅊ':
                        buffer[CHO_SUNG] = 'ㅉ';
                        break;
                    case 'ㅉ':
                        buffer[CHO_SUNG] = 'ㅈ';
                        break;
                    default:
                        buffer[CHO_SUNG] = 'ㅈ';
                        noChangeChar = true;
                        break;
                }
                SendKeys.SendWait((noChangeChar ? "" : "{BS}") + buffer[CHO_SUNG].ToString());
            }
            else
            {
                bool noBufferSend = false;
                switch(buffer[JONG_SUNG])
                {
                    case ' ':
                    case 'ㅊ':
                        buffer[JONG_SUNG] = 'ㅈ';
                        break;
                    case 'ㅈ':
                        buffer[JONG_SUNG] = 'ㅊ';
                        break;
                    case 'ㄴ':
                        buffer[JONG_SUNG] = 'ㄵ';
                        break;
                    case 'ㄵ':
                        buffer[JONG_SUNG] = 'ㄴ';
                        SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]));
                        buffer[CHO_SUNG] = 'ㅉ';
                        buffer[JUNG_SUNG] = ' ';
                        buffer[JONG_SUNG] = ' ';
                        SendKeys.SendWait("{BS}");
                        SendKeys.SendWait("ㅉ");
                        noBufferSend = true;
                        break;
                    default:
                        buffer[CHO_SUNG] = 'ㅈ';
                        buffer[JUNG_SUNG] = ' ';
                        buffer[JONG_SUNG] = ' ';
                        SendKeys.SendWait("ㅈ");
                        noBufferSend = true;
                        break;
                }
                if (!noBufferSend)
                {
                    SendBuffer();
                }
            }
        }
        private void KeyNumPad4DownHangul(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;
            if (buffer[JUNG_SUNG] == ' ')
            {
                bool noChangeChar = false;
                switch(buffer[CHO_SUNG])
                {
                    case 'ㄱ':
                        buffer[CHO_SUNG] = 'ㅋ';
                        break;
                    case 'ㅋ':
                        buffer[CHO_SUNG] = 'ㄲ';
                        break;
                    case 'ㄲ':
                        buffer[CHO_SUNG] = 'ㄱ';
                        break;
                    default:
                        buffer[CHO_SUNG] = 'ㄱ';
                        noChangeChar = true;
                        break;
                }
                SendKeys.SendWait((noChangeChar ? "" : "{BS}") + buffer[CHO_SUNG].ToString());
            }
            else
            {
                bool noBufferSend = false;
                switch(buffer[JONG_SUNG])
                {
                    case ' ':
                    case 'ㄲ':
                        buffer[JONG_SUNG] = 'ㄱ';
                        break;
                    case 'ㄱ':
                        buffer[JONG_SUNG] = 'ㅋ';
                        break;
                    case 'ㅋ':
                        buffer[JONG_SUNG] = 'ㄲ';
                        break;
                    case 'ㄹ':
                        buffer[JONG_SUNG] = 'ㄺ';
                        break;
                    default:
                        buffer[CHO_SUNG] = 'ㄱ';
                        buffer[JUNG_SUNG] = ' ';
                        buffer[JONG_SUNG] = ' ';
                        SendKeys.SendWait("ㄱ");
                        noBufferSend = true;
                        break;
                }
                if(!noBufferSend)
                {
                    SendBuffer();
                }
            }
        }
        private void KeyNumPad5DownHangul(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;
            if (buffer[JUNG_SUNG] == ' ')
            {
                bool noChangeChar = false;
                switch(buffer[CHO_SUNG])
                {
                    case 'ㄴ':
                        buffer[CHO_SUNG] = 'ㄹ';
                        break;
                    case 'ㄹ':
                        buffer[CHO_SUNG] = 'ㄴ';
                        break;
                    default:
                        buffer[CHO_SUNG] = 'ㄴ';
                        noChangeChar = true;
                        break;
                }
                SendKeys.SendWait((noChangeChar ? "" : "{BS}") + buffer[CHO_SUNG].ToString());
            }
            else
            {
                bool noBufferSend = false;
                switch(buffer[JONG_SUNG])
                {
                    case ' ':
                    case 'ㄹ':
                        buffer[JONG_SUNG] = 'ㄴ';
                        break;
                    case 'ㄴ':
                        buffer[JONG_SUNG] = 'ㄹ';
                        break;
                    default:
                        buffer[CHO_SUNG] = 'ㄴ';
                        buffer[JUNG_SUNG] = ' ';
                        buffer[JONG_SUNG] = ' ';
                        SendKeys.SendWait("ㄴ");
                        noBufferSend = true;
                        break;
                }
                if(!noBufferSend)
                {
                    SendBuffer();
                }
            }
        }
        private void KeyNumPad6DownHangul(ref KeyEventArgs e)
        {
            if (!isInstanceActivated)
            {
                return;
            }
            e.Handled = true;
            if (buffer[JUNG_SUNG] == ' ')
            {
                bool noChangeChar = false;
                switch(buffer[CHO_SUNG])
                {
                    case 'ㄷ':
                        buffer[CHO_SUNG] = 'ㅌ';
                        break;
                    case 'ㅌ':
                        buffer[CHO_SUNG] = 'ㄸ';
                        break;
                    case 'ㄸ':
                        buffer[CHO_SUNG] = 'ㄷ';
                        break;
                    default:
                        buffer[CHO_SUNG] = 'ㄷ';
                        noChangeChar = true;
                        break;
                }
                SendKeys.SendWait((noChangeChar ? "" : "{BS}") + buffer[CHO_SUNG].ToString());
            }
            else
            {
                bool noBufferSend = false;
                switch(buffer[JONG_SUNG])
                {
                    case ' ':
                    case 'ㅌ':
                        buffer[JONG_SUNG] = 'ㄷ';
                        break;
                    case 'ㄷ':
                        buffer[JONG_SUNG] = 'ㅌ';
                        break;
                    case 'ㄹ':
                        buffer[JONG_SUNG] = 'ㄾ';
                        break;
                    default:
                        buffer[CHO_SUNG] = 'ㄷ';
                        buffer[JUNG_SUNG] = ' ';
                        buffer[JONG_SUNG] = ' ';
                        SendKeys.SendWait("ㄷ");
                        noBufferSend = true;
                        break;
                }
                if(!noBufferSend)
                {
                    SendBuffer();
                }
            }
        }
        private void KeyNumPad7DownHangul(ref KeyEventArgs e)
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
                else
                {
                    bool noBufferSend = false;
                    switch (buffer[JUNG_SUNG])
                    {
                        case ' ':
                            buffer[JUNG_SUNG] = 'ㅣ';
                            break;
                        case 'ㅓ':
                            buffer[JUNG_SUNG] = 'ㅔ';
                            break;
                        case 'ㆍ':
                            buffer[JUNG_SUNG] = 'ㅓ';
                            break;
                        case 'ᆢ':
                            buffer[JUNG_SUNG] = 'ㅕ';
                            break;
                        case 'ㅡ':
                            buffer[JUNG_SUNG] = 'ㅢ';
                            break;
                        case 'ㅗ':
                            buffer[JUNG_SUNG] = 'ㅚ';
                            break;
                        case 'ㅜ':
                            buffer[JUNG_SUNG] = 'ㅟ';
                            break;
                        case 'ㅏ':
                            buffer[JUNG_SUNG] = 'ㅐ';
                            break;
                        case 'ㅑ':
                            buffer[JUNG_SUNG] = 'ㅒ';
                            break;
                        case 'ㅕ':
                            buffer[JUNG_SUNG] = 'ㅖ';
                            break;
                        case 'ㅠ':
                            buffer[JUNG_SUNG] = 'ㅝ';
                            break;
                        case 'ㅝ':
                            buffer[JUNG_SUNG] = 'ㅞ';
                            break;
                        case 'ㅘ':
                            buffer[JUNG_SUNG] = 'ㅙ';
                            break;
                        case 'ㅣ':
                            if (buffer[CHO_SUNG] == ' ')
                            {
                                buffer[JONG_SUNG] = ' ';
                            }
                            buffer[CHO_SUNG] = ' ';
                            buffer[JUNG_SUNG] = 'ㅣ';
                            noBufferSend = true;
                            SendKeys.SendWait((buffer[CHO_SUNG] == ' ' ? "" : buffer[CHO_SUNG].ToString()));
                            SendKeys.SendWait("ㅣ");
                            break;
                        default:
                            noBufferSend = true;
                            break;
                    }
                    if (!noBufferSend)
                    {
                        SendBuffer();
                    }

                }
            }
            else if (buffer[JONG_SUNG] != ' ')
            {
                char firstJong = new char();
                char secondJong = new char();
                SendKeys.SendWait("{BS}");
                if (GBCCheck(buffer[JONG_SUNG], ref firstJong, ref secondJong))
                {
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], firstJong));

                    buffer[JUNG_SUNG] = 'ㅣ';
                    buffer[CHO_SUNG] = secondJong;
                    buffer[JONG_SUNG] = ' ';
                }
                else
                {
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], ' '));

                    buffer[JUNG_SUNG] = 'ㅣ';
                    buffer[CHO_SUNG] = buffer[JONG_SUNG];
                    buffer[JONG_SUNG] = ' ';
                }
                SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]));
            }

        }
        private void KeyNumPad8DownHangul(ref KeyEventArgs e)
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
                else
                {
                    bool noBufferSend = false;
                    switch (buffer[JUNG_SUNG])
                    {
                        case 'ㆍ':
                            buffer[JUNG_SUNG] = 'ᆢ';
                            noBufferSend = (buffer[CHO_SUNG] != ' ');
                            break;
                        case ' ':
                            buffer[JUNG_SUNG] = 'ㆍ';
                            noBufferSend = (buffer[CHO_SUNG] != ' ');
                            break;
                        case 'ㅣ':
                            buffer[JUNG_SUNG] = 'ㅏ';
                            break;
                        case 'ㅏ':
                            buffer[JUNG_SUNG] = 'ㅑ';
                            break;
                        case 'ㅡ':
                            buffer[JUNG_SUNG] = 'ㅜ';
                            break;
                        case 'ㅜ':
                            buffer[JUNG_SUNG] = 'ㅠ';
                            break;
                        case 'ㅚ':
                            buffer[JUNG_SUNG] = 'ㅘ';
                            break;
                        case 'ㅟ':
                            buffer[JUNG_SUNG] = 'ㅝ';
                            break;
                        default:
                            noBufferSend = true;
                            break;
                    }
                    if (!noBufferSend)
                    {
                        SendBuffer();
                    }
                }
            }
            else if (buffer[JONG_SUNG] != ' ')
            {
                char firstJong = new char();
                char secondJong = new char();


                SendKeys.SendWait("{BS}");
                if (GBCCheck(buffer[JONG_SUNG], ref firstJong, ref secondJong))
                {
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], firstJong));
                    buffer[JUNG_SUNG] = 'ㆍ';
                    buffer[CHO_SUNG] = secondJong;
                    buffer[JONG_SUNG] = ' ';
                }
                else
                {
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], ' '));
                    buffer[JUNG_SUNG] = 'ㆍ';
                    buffer[CHO_SUNG] = buffer[JONG_SUNG];
                    buffer[JONG_SUNG] = ' ';
                }
                SendKeys.SendWait(buffer[CHO_SUNG].ToString());
            }
        }
        private void KeyNumPad9DownHangul(ref KeyEventArgs e)
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
                else
                {
                    bool noBufferSend = false;
                    switch (buffer[JUNG_SUNG])
                    {
                        case 'ㅡ':
                            if (buffer[CHO_SUNG] == ' ')
                            {
                                buffer[JONG_SUNG] = ' ';
                            }
                            buffer[CHO_SUNG] = ' ';
                            buffer[JUNG_SUNG] = 'ㅡ';

                            SendKeys.SendWait((buffer[CHO_SUNG] == ' ' ? "" : buffer[CHO_SUNG].ToString()));
                            SendKeys.SendWait("ㅡ");
                            noBufferSend = true;
                            break;

                        case 'ㆍ':
                            buffer[JUNG_SUNG] = 'ㅗ';
                            break;

                        case 'ᆢ':
                            buffer[JUNG_SUNG] = 'ㅛ';
                            break;

                        case ' ':
                            buffer[JUNG_SUNG] = 'ㅡ';
                            break;

                        default:
                            noBufferSend = true;
                            break;

                    }
                    if (!noBufferSend)
                    {
                        SendBuffer();
                    }
                }
            }
            else if (buffer[JONG_SUNG] != ' ')
            {
                char firstJong = new char();
                char secondJong = new char();

                SendKeys.SendWait("{BS}");
                if (GBCCheck(buffer[JONG_SUNG], ref firstJong, ref secondJong))
                {
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], firstJong));
                    buffer[JUNG_SUNG] = 'ㅡ';
                    buffer[CHO_SUNG] = secondJong;
                    buffer[JONG_SUNG] = ' ';
                }
                else
                {
                    SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], ' '));
                    buffer[JUNG_SUNG] = 'ㅡ';
                    buffer[CHO_SUNG] = buffer[JONG_SUNG];
                    buffer[JONG_SUNG] = ' ';
                }
                SendKeys.SendWait(HangleHap(buffer[CHO_SUNG], buffer[JUNG_SUNG], buffer[JONG_SUNG]));
            }
        }
        private void KeyNumPad0Down(ref KeyEventArgs e)
        {
            if (isInstanceActivated)
            {
                e.Handled = true;
                if (langMode == 0)
                {
                    if (buffer[JUNG_SUNG] == ' ')
                    {
                        bool noChangeChar = false;
                        switch (buffer[CHO_SUNG])
                        {
                            case 'ㅇ':
                                buffer[CHO_SUNG] = 'ㅁ';
                                break;

                            case 'ㅁ':
                                buffer[CHO_SUNG] = 'ㅇ';
                                break;

                            default:
                                buffer[CHO_SUNG] = 'ㅇ';
                                noChangeChar = true;
                                break;
                        }
                        SendKeys.SendWait((noChangeChar ? "" : "{BS}") + buffer[CHO_SUNG].ToString());

                    }
                    else
                    {
                        bool noBufferSend = false;

                        switch (buffer[JONG_SUNG])
                        {
                            case ' ':
                            case 'ㅁ':
                                buffer[JONG_SUNG] = 'ㅇ';
                                break;
                            case 'ㅇ':
                                buffer[JONG_SUNG] = 'ㅁ';
                                break;
                            case 'ㄹ':
                                buffer[JONG_SUNG] = 'ㄻ';
                                break;
                            default:
                                buffer[CHO_SUNG] = 'ㅇ';
                                buffer[JUNG_SUNG] = ' ';
                                buffer[JONG_SUNG] = ' ';
                                SendKeys.SendWait("ㅇ");
                                noBufferSend = true;
                                break;
                        }
                        if (!noBufferSend)
                        {
                            SendBuffer();
                        }
                    }
                }
            }
            n0 = true;
            if (et)
            {
                SendKeys.SendWait("{BS}");
                if (!isInstanceActivated)
                {
                
                    Hancheck(langMode==0);
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
            if (isInstanceActivated)
            {
                e.Handled = true;
                buffer[CHO_SUNG] = ' ';
                buffer[JUNG_SUNG] = ' ';
                buffer[JONG_SUNG] = ' ';
                engbuffer = ' ';
                tuk = ' ';
                SendKeys.SendWait("{BS}");
            }
        }
        

        private void KeyAddDownHangul(ref KeyEventArgs e)
        {
            if (isInstanceActivated)
            {
                e.Handled = true;
                if ((buffer[CHO_SUNG] != ' ') || (buffer[JUNG_SUNG] != ' ') || (buffer[JONG_SUNG] != ' ') || (tuk != ' '))
                {
                    buffer[CHO_SUNG] = ' ';
                    buffer[JUNG_SUNG] = ' ';
                    buffer[JONG_SUNG] = ' ';
                    tuk = ' ';
                }
                else
                {
                    SendKeys.SendWait(" ");
                }
            }
        }
        private void KeyAddDownEnglish(ref KeyEventArgs e)
        {
            if (isInstanceActivated)
            {
                e.Handled = true;
                if (engbuffer != ' ')
                {
                    engbuffer = ' ';
                }
                else
                {
                    SendKeys.SendWait(" ");
                }
            }
        }

        private void KeyDecimalDown(ref KeyEventArgs e)
        {
            if (isInstanceActivated)
            {
                e.Handled = true;
                buffer[CHO_SUNG] = ' ';
                buffer[JUNG_SUNG] = ' ';
                buffer[JONG_SUNG] = ' ';
                engbuffer = ' ';
                
                bool noChangeChar = false;

                switch (tuk)
                {
                    case ' ':
                        tuk = '.';
                        noChangeChar = true;
                        break;
                    case '.':
                        tuk = ',';
                        break;
                    case ',':
                        tuk = '?';
                        break;
                    case '?':
                        tuk = '!';
                        break;
                    case '!':
                        tuk = '.';
                        break;
                    default:
                        noChangeChar = true;
                        break;

                }
                SendKeys.SendWait((noChangeChar ? "" : "{BS}") + tuk.ToString());
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
                    Hancheck(langMode == 0);
                    isInstanceActivated = true;
                    e.Handled = true;
                    Show();
                }
                else
                {
                    SendKeys.SendWait("{BS}");
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
                engbuffer = ' ';
                
                tuk = ' ';

            }
        }

        public CheonJiInKeyHandler(HancheckFunc hancheck, ShowFunc showFunc, HideFunc hideFunc, ChangeUILangFunc changeUILangFunc)
        {
            Hancheck = hancheck;
            Show = showFunc;
            Hide = hideFunc;
            ChangeUILang = changeUILangFunc;


            InitializeKeyDictionary();
        }


        public void KeyDown(ref KeyEventArgs e)
        {
            if (KeyDownDictionary.ContainsKey(e.KeyData))
            {
                if (e.KeyData != Keys.Decimal)
                {
                    tuk = ' ';
                }
                KeyDownDictionary[e.KeyData](ref e);
            }
        }

        public void KeyUp(ref KeyEventArgs e)
        {
            if(e.KeyData == Keys.NumPad0)
            {
                n0 = false;
            }
            else if (e.KeyData == Keys.Enter)
            {
                et = false;
            }
        }
    }
}
