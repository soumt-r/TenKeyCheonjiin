using System.Collections.Generic;
using System.Windows.Forms;

namespace HanSon
{
    class CheonJiInKeyHandler : IKeyHandler
    {
        private readonly HangeulAutomata _automata = new HangeulAutomata();

        public delegate void ChangeUILangFunc(int langMode);
        public delegate void HancheckFunc(bool toHangeul);
        public delegate void ShowFunc();
        public delegate void HideFunc();

        private readonly HancheckFunc _hancheck;
        private readonly ShowFunc _show;
        private readonly HideFunc _hide;
        private readonly ChangeUILangFunc _changeUILang;

        private int _langMode = 0;       // 0: Korean, 1: English
        private bool _isActive = true;
        private char _engBuf = ' ', _symBuf = ' ';
        private bool _isZeroDown, _isEnterDown, _toggleLatched;

        private struct KeyDef
        {
            public char Han; public string Eng; public bool IsV;
            public KeyDef(char h, string e, bool v) { Han = h; Eng = e; IsV = v; }
        }

        private readonly Dictionary<Keys, KeyDef> _keyDefs = new Dictionary<Keys, KeyDef> {
            { Keys.NumPad1, new KeyDef('ㅂ', "pqrsPQRS", false) },
            { Keys.NumPad2, new KeyDef('ㅅ', "tuvTUV",     false) },
            { Keys.NumPad3, new KeyDef('ㅈ', "wxyzWXYZ",   false) },
            { Keys.NumPad4, new KeyDef('ㄱ', "ghiGHI",     false) },
            { Keys.NumPad5, new KeyDef('ㄴ', "jklJKL",     false) },
            { Keys.NumPad6, new KeyDef('ㄷ', "mnoMNO",     false) },
            { Keys.NumPad7, new KeyDef('ㅣ', null,         true)  },
            { Keys.NumPad8, new KeyDef('ㆍ', "abcABC",     true)  },
            { Keys.NumPad9, new KeyDef('ㅡ', "defDEF",     true)  },
            { Keys.NumPad0, new KeyDef('ㅇ', null,         false) }
        };

        public CheonJiInKeyHandler(HancheckFunc hancheck, ShowFunc show, HideFunc hide, ChangeUILangFunc changeUI)
        {
            _hancheck = hancheck; _show = show; _hide = hide; _changeUILang = changeUI;
        }

        public void KeyDown(ref KeyEventArgs e)
        {
            if (!_isActive && e.KeyCode != Keys.NumPad0 && e.KeyCode != Keys.Enter) return;

            if (_keyDefs.TryGetValue(e.KeyCode, out var def))
            {
                e.Handled = true;
                if (_langMode == 0) Execute(_automata.ProcessConsonantOrVowel(def.Han, def.IsV));
                else if (def.Eng != null) Cycle(def.Eng, ref _engBuf);

                if (e.KeyCode == Keys.NumPad0) _isZeroDown = true;
                _symBuf = ' ';
            }
            else switch (e.KeyCode)
            {
                case Keys.Add:      e.Handled = true; Send(" "); Reset(); break;
                case Keys.Subtract: e.Handled = true; Send("{BS}"); Reset(); break;
                case Keys.Divide:   e.Handled = true; Reset(); break;
                case Keys.Decimal:  e.Handled = true; Cycle(".,?!", ref _symBuf); break;
                case Keys.Multiply: e.Handled = true; SwitchLang(); break;
                case Keys.Enter:    _isEnterDown = true; Reset(); break;
            }

            if (_isZeroDown && _isEnterDown && !_toggleLatched)
            {
                _toggleLatched = true;
                ToggleActive(ref e);
            }
        }

        public void KeyUp(ref KeyEventArgs e)
        {
            if (e.KeyCode == Keys.NumPad0) _isZeroDown = false;
            else if (e.KeyCode == Keys.Enter) _isEnterDown = false;
            if (!_isZeroDown && !_isEnterDown) _toggleLatched = false;
        }

        private void Cycle(string chars, ref char buf)
        {
            int idx = chars.IndexOf(buf);
            buf = chars[(idx + 1) % chars.Length];
            Send((idx == -1 ? "" : "{BS}") + buf);
            _automata.Reset();
        }

        private void Execute(AutomataResult res) {
            if (res == null) return;
            string cmd = (res.DeleteCount > 0 ? "{BS " + res.DeleteCount + "}" : "") + res.InsertText;
            if (!string.IsNullOrEmpty(cmd)) Send(cmd);
            _engBuf = ' ';
        }

        private void SwitchLang() {
            _langMode = 1 - _langMode;
            _changeUILang(_langMode);
            _hancheck(_langMode == 0);
            Reset();
        }

        private void ToggleActive(ref KeyEventArgs e) {
            Send("{BS}"); _isActive = !_isActive;
            if (_isActive) { _hancheck(_langMode == 0); _show(); } else _hide();
            e.Handled = true; Reset();
        }

        private void Reset() { _automata.Reset(); _engBuf = ' '; _symBuf = ' '; }
        private void Send(string s) => SendKeys.SendWait(s);
    }
}
