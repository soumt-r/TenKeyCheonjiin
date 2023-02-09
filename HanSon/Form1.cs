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

        readonly IKeyHandler keyHandler;

        public Form1()
        {
            InitializeComponent();

            // resources allocation part
            captainHook = new CGlobalKeyboardHook();
            keyHandler = new CheonJiInKeyHandler(Hancheck, FormShow, FormHide, ChangeUILang);
            
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

        public Font LabelSize(Label label, String text)
        {
            Font ft;
            Graphics gp;
            SizeF sz;
            Single Factor, FactorX, FactorY;

            gp = label.CreateGraphics();
            sz = gp.MeasureString(text, label.Font);
            gp.Dispose();

            FactorX = (label.Width) / sz.Width;
            FactorY = (label.Height) / sz.Height;

            if (FactorX > FactorY)
                Factor = FactorY;
            else
                Factor = FactorX;
            ft = label.Font;

            return new Font(ft.Name, ft.SizeInPoints * (Factor) - 1);
        }

        public void ChangeUILang(int LangMode)
        {
            if(LangMode == 0)
            {
                label1.Text = "ㅣ";
                label1.Font = LabelSize(label1, "ㅣ");
                label2.Text = "ㆍ";
                label2.Font = LabelSize(label2, "ㆍ");
                label3.Text = "ㅡ";
                label3.Font = LabelSize(label3, "ㅡ");

                label4.Text = "ㄱㅋ";
                label4.Font = LabelSize(label4, "ㄱㅋ");
                label5.Text = "ㄴㄹ";
                label5.Font = LabelSize(label5, "ㄴㄹ");
                label6.Text = "ㄷㅌ";
                label6.Font = LabelSize(label6, "ㄷㅌ");

                label7.Text = "ㅂㅍ";
                label7.Font = LabelSize(label7, "ㅂㅍ");
                label8.Text = "ㅅㅎ";
                label8.Font = LabelSize(label8, "ㅅㅎ");
                label9.Text = "ㅈㅊ";
                label9.Font = LabelSize(label9, "ㅈㅊ");

                label10.Text = "ㅇㅁ";
                label10.Font = LabelSize(label10, "ㅇㅁ");
            }
            else if (LangMode == 1)
            {
                label1.Text = "";
                label2.Text = "ABC";
                label2.Font = new Font(label1.Font.Name, 13F);
                label3.Text = "DEF";
                label3.Font = new Font(label1.Font.Name, 13F);

                label4.Text = "GHI";
                label4.Font = new Font(label1.Font.Name, 13F);
                label5.Text = "JKL";
                label5.Font = new Font(label1.Font.Name, 13F);
                label6.Text = "MNO";
                label6.Font = new Font(label1.Font.Name, 13F);

                label7.Text = "PQRS";
                label7.Font = new Font(label1.Font.Name, 13F);
                label8.Text = "TUV";
                label8.Font = new Font(label1.Font.Name, 13F);
                label9.Text = "WXYZ";
                label9.Font = new Font(label1.Font.Name, 13F);

                label10.Text = "";
            }
        }

        public void Hancheck(bool toHangeul)
        {
            if (toHangeul)
            {
                if (!IsHangeulNow())
                {
                    keybd_event((byte)Keys.HanguelMode, 0, 0, 0);     //  누름
                    keybd_event((byte)Keys.HanguelMode, 0, 0x02, 0);  //  누름 해제 
                }
            }
            else
            {
                if (IsHangeulNow())
                {
                    keybd_event((byte)Keys.HanguelMode, 0, 0, 0);     //  누름
                    keybd_event((byte)Keys.HanguelMode, 0, 0x02, 0);  //  누름 해제 
                }
            }
        }
        private bool IsHangeulNow()
        {

            IntPtr hwnd = GetForegroundWindow();
            IntPtr hime = ImmGetDefaultIMEWnd(hwnd);
            IntPtr status = SendMessage(hime, WM_IME_CONTROL, new IntPtr(0x5), new IntPtr(0));

            if (status.ToInt32() != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
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
