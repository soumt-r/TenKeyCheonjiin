using CopyAndPaste;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HanSon
{
    public partial class Form1 : Form
    {
        // Keyboard coloring setting
        private readonly Color KEY_UP_COLOR = Color.FromArgb(47, 63, 80);
        private readonly Color KEY_DOWN_COLOR = Color.FromArgb(55, 74, 94);

        // Keyboard Hook
        private readonly CGlobalKeyboardHook captainHook;

        private Dictionary<Keys, System.Windows.Forms.Panel> numpadPannel;

        IKeyHandler keyHandler;

        public Form1()
        {
            InitializeComponent();

            // resources allocation part
            captainHook = new CGlobalKeyboardHook();
            keyHandler = new CheonJiInKeyHandler(Hancheck, FormShow, FormHide);
        }

        private void InitializeScreen()
        {
            const int margin = 10;
            int x = Screen.PrimaryScreen.WorkingArea.Right -
                this.Width - margin;
            int y = Screen.PrimaryScreen.WorkingArea.Bottom -
                this.Height - margin;

            this.Location = new Point(x, y);
        }
        private void InitializeKeyStroke()
        {
            captainHook.Hook();

            // Key event allocation part
            captainHook.KeyDown += FormKeyDown;
            captainHook.KeyUp += FormKeyUp;
        }

        private void InitializePanelInfo()
        {
            numpadPannel = new Dictionary<Keys, Panel>();
            numpadPannel.Add(Keys.NumPad1, panel7);
            numpadPannel.Add(Keys.NumPad2, panel8);
            numpadPannel.Add(Keys.NumPad3, panel9);
            numpadPannel.Add(Keys.NumPad4, panel4);
            numpadPannel.Add(Keys.NumPad5, panel5);
            numpadPannel.Add(Keys.NumPad6, panel6);
            numpadPannel.Add(Keys.NumPad7, panel1);
            numpadPannel.Add(Keys.NumPad8, panel2);
            numpadPannel.Add(Keys.NumPad9, panel3);
            numpadPannel.Add(Keys.NumPad0, panel10);
        }
        void Form1_Load(object sender, EventArgs e)
        {
            // initialize the Form dependent resources
            InitializeScreen();
            InitializeKeyStroke();
            InitializePanelInfo();
        }


        private void KeyColoring(Keys key, Color keyColor)
        {
            if (numpadPannel.ContainsKey(key))
            {
                (numpadPannel[key]).BackColor = keyColor;
            }
        }
        void FormKeyDown(object sender, KeyEventArgs e)
        {
            keyHandler.KeyDown(ref e);
            KeyColoring(e.KeyData, KEY_DOWN_COLOR);
        }
        void FormKeyUp(object sender, KeyEventArgs e)
        {
            keyHandler.KeyUp(ref e);
            KeyColoring(e.KeyData, KEY_UP_COLOR);
        }

        public void FormShow()
        {
            Show();
        }
        public void FormHide()
        {
            Hide();
        }

        public void Hancheck()
        {
            if (!IsHangeulNow())
            {
                keybd_event((byte)Keys.HanguelMode, 0, 0, 0);     //  누름
                keybd_event((byte)Keys.HanguelMode, 0, 0x02, 0);  //  누름 해제 
            }
        }
        private bool IsHangeulNow()
        {

            IntPtr hwnd = GetForegroundWindow();
            IntPtr hime = ImmGetDefaultIMEWnd(hwnd);
            IntPtr status = SendMessage(hime, WM_IME_CONTROL, new IntPtr(0x5), new IntPtr(0));

            if (status.ToInt32() != 0)
                return true;

            else
                return false;
        }


        [DllImport("imm32.dll")]
        private static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr IParam);

        private const int WM_IME_CONTROL = 643;


        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

    }
}
