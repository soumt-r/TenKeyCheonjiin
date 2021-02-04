using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.InteropServices;

using Utilities;
using System.Diagnostics;
using CopyAndPaste;

namespace HanSon
{
    public partial class Form1 : Form
    {
        private const long WM_NOACTIVATE = 0x8000000L;
        //globalKeyboardHook Hooker = new globalKeyboardHook();

        CGlobalKeyboardHook Captain_hook = new CGlobalKeyboardHook();

        bool activated = false;

        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            const int margin = 10;
            int x = Screen.PrimaryScreen.WorkingArea.Right -
                this.Width - margin;
            int y = Screen.PrimaryScreen.WorkingArea.Bottom -
                this.Height - margin;
            this.Location = new Point(x, y);



            Captain_hook.hook();
            Captain_hook.KeyDown += keydown;
            Captain_hook.KeyUp += keyup;

            
        }

        char GlobalCho = ' ';
        char GlobalJung = ' ';
        char GlobalJong = ' ';


       public static char hangleHap(char c1, char c2, char c3)
       ﻿{
            char[] cho = { 'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };
            char[] jung = { 'ㅏ', 'ㅐ', 'ㅑ', 'ㅒ', 'ㅓ', 'ㅔ', 'ㅕ', 'ㅖ', 'ㅗ', 'ㅘ', 'ㅙ', 'ㅚ', 'ㅛ', 'ㅜ', 'ㅝ',
                            'ㅞ', 'ㅟ', 'ㅠ', 'ㅡ', 'ㅢ', 'ㅣ' };
            char[] jong = { ' ', 'ㄱ', 'ㄲ', 'ㄳ', 'ㄴ', 'ㄵ', 'ㄶ', 'ㄷ', 'ㄹ', 'ㄺ', 'ㄻ', 'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ',
                            'ㅀ', 'ㅁ', 'ㅂ', 'ㅄ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };

            int cho_i = 0, jung_i = 0, jong_i = 0;
            for (int i = 0; i < cho.Length; i++)
                if (cho[i] == c1) cho_i = i;
            for (int i = 0; i < jung.Length; i++)
                if (jung[i] == c2) jung_i = i;
            for (int i = 0; i < jong.Length; i++)
                if (jong[i] == c3) jong_i = i;
            int uniValue = (cho_i * 21 * 28) + (jung_i * 28) + (jong_i) + 0xAC00;
            return (char)uniValue;
        }

        void keydown(object sender, KeyEventArgs e)
        {
            if (activated)
            {
                
                if (e.KeyData == Keys.NumPad1)
                {
                    e.Handled = true;
                    panel7.BackColor = Color.FromArgb(55, 74, 94);
                    
                    if (GlobalJung == ' ')
                    {
                        if (GlobalCho == ' ')
                        {
                            GlobalCho = 'ㅂ';
                            SendKeys.SendWait("ㅂ");
                        }
                        else if (GlobalCho == 'ㅂ')
                        {
                            GlobalCho = 'ㅍ';
                            SendKeys.SendWait("{BS}ㅍ");
                        }
                        else if (GlobalCho == 'ㅍ')
                        {
                            GlobalCho = 'ㅃ';
                            SendKeys.SendWait("{BS}ㅃ");
                        }
                        else if (GlobalCho == 'ㅃ')
                        {
                            GlobalCho = 'ㅂ';
                            SendKeys.SendWait("{BS}ㅂ");
                        }
                        else
                        {
                            GlobalCho = 'ㅂ';
                            SendKeys.SendWait("ㅂ");
                        }
                    }
                    else
                    {
                        if (GlobalJong == ' ')
                        {
                            GlobalJong = 'ㅂ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㅂ')
                        {
                            GlobalJong = 'ㅍ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㅍ')
                        {
                            GlobalJong = 'ㅂ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㄹ')
                        {
                            GlobalJong = 'ㄼ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㄼ')
                        {
                            GlobalJong = 'ㄿ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else
                        {
                            GlobalCho = 'ㅂ';
                            GlobalJung = ' ';
                            GlobalJong = ' ';
                            SendKeys.SendWait("ㅂ");
                        }
                    }
                }
                if (e.KeyData == Keys.NumPad2)
                {
                    e.Handled = true;
                    panel8.BackColor = Color.FromArgb(55, 74, 94);
                    
                    if (GlobalJung == ' ')
                    {
                        if (GlobalCho == ' ')
                        {
                            GlobalCho = 'ㅅ';
                            SendKeys.SendWait("ㅅ");
                        }
                        else if (GlobalCho == 'ㅅ')
                        {
                            GlobalCho = 'ㅎ';
                            SendKeys.SendWait("{BS}ㅎ");
                        }
                        else if (GlobalCho == 'ㅎ')
                        {
                            GlobalCho = 'ㅆ';
                            SendKeys.SendWait("{BS}ㅆ");
                        }
                        else if (GlobalCho == 'ㅆ')
                        {
                            GlobalCho = 'ㅅ';
                            SendKeys.SendWait("{BS}ㅅ");
                        }
                        else
                        {
                            GlobalCho = 'ㅅ';
                            SendKeys.SendWait("ㅅ");
                        }
                    }
                    else
                    {
                        if (GlobalJong == ' ')
                        {
                            GlobalJong = 'ㅅ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㅅ')
                        {
                            GlobalJong = 'ㅎ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㅎ')
                        {
                            GlobalJong = 'ㅆ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㅆ')
                        {
                            GlobalJong = 'ㅅ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㄴ')
                        {
                            GlobalJong = 'ㄶ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㄹ')
                        {
                            GlobalJong = 'ㄽ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㄽ')
                        {
                            GlobalJong = 'ㅀ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㅀ')
                        {
                            GlobalJong = 'ㄽ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }

                        else if (GlobalJong == 'ㅂ')
                        {
                            GlobalJong = 'ㅄ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }

                        else if (GlobalJong == 'ㅄ')
                        {
                            GlobalJong = 'ㅂ';
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                            GlobalCho = 'ㅆ';
                            GlobalJung = ' ';
                            GlobalJong = ' ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait("ㅆ");
                        }

                        else if (GlobalJong == 'ㄶ')
                        {
                            GlobalJong = 'ㄴ';
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                            GlobalCho = 'ㅅ';
                            GlobalJung = ' ';
                            GlobalJong = ' ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait("ㅅ");
                        }
                        else
                        {
                            GlobalCho = 'ㅅ';
                            GlobalJung = ' ';
                            GlobalJong = ' ';
                            SendKeys.SendWait("ㅅ");
                        }
                    }
                }
                if (e.KeyData == Keys.NumPad3)
                {
                    e.Handled = true;
                    panel9.BackColor = Color.FromArgb(55, 74, 94);
                    
                    if (GlobalJung == ' ')
                    {
                        if (GlobalCho == ' ')
                        {
                            GlobalCho = 'ㅈ';
                            SendKeys.SendWait("ㅈ");
                        }
                        else if (GlobalCho == 'ㅈ')
                        {
                            GlobalCho = 'ㅊ';
                            SendKeys.SendWait("{BS}ㅊ");
                        }
                        else if (GlobalCho == 'ㅊ')
                        {
                            GlobalCho = 'ㅉ';
                            SendKeys.SendWait("{BS}ㅉ");
                        }
                        else if (GlobalCho == 'ㅉ')
                        {
                            GlobalCho = 'ㅈ';
                            SendKeys.SendWait("{BS}ㅈ");
                        }
                        else
                        {
                            GlobalCho = 'ㅈ';
                            SendKeys.SendWait("ㅈ");
                        }
                    }
                    else
                    {
                        if (GlobalJong == ' ')
                        {
                            GlobalJong = 'ㅈ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㅈ')
                        {
                            GlobalJong = 'ㅊ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㅊ')
                        {
                            GlobalJong = 'ㅈ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㄴ')
                        {
                            GlobalJong = 'ㄵ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }

                        else if (GlobalJong == 'ㄵ')
                        {
                            GlobalJong = 'ㄴ';
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                            GlobalCho = 'ㅉ';
                            GlobalJung = ' ';
                            GlobalJong = ' ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait("ㅉ");
                        }

                        else
                        {
                            GlobalCho = 'ㅈ';
                            GlobalJung = ' ';
                            GlobalJong = ' ';
                            SendKeys.SendWait("ㅈ");
                        }
                    }
                }
                if (e.KeyData == Keys.NumPad4)
                {
                    e.Handled = true;
                    panel4.BackColor = Color.FromArgb(55, 74, 94);
                    
                    if (GlobalJung == ' ')
                    {
                        if (GlobalCho == ' ')
                        {
                            GlobalCho = 'ㄱ';
                            SendKeys.SendWait("ㄱ");
                        }
                        else if (GlobalCho == 'ㄱ')
                        {
                            GlobalCho = 'ㅋ';
                            SendKeys.SendWait("{BS}ㅋ");
                        }
                        else if (GlobalCho == 'ㅋ')
                        {
                            GlobalCho = 'ㄲ';
                            SendKeys.SendWait("{BS}ㄲ");
                        }
                        else if (GlobalCho == 'ㄲ')
                        {
                            GlobalCho = 'ㄱ';
                            SendKeys.SendWait("{BS}ㄱ");
                        }
                        else
                        {
                            GlobalCho = 'ㄱ';
                            SendKeys.SendWait("ㄱ");
                        }
                    }
                    else
                    {
                        if (GlobalJong == ' ')
                        {
                            GlobalJong = 'ㄱ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㄱ')
                        {
                            GlobalJong = 'ㅋ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㅋ')
                        {
                            GlobalJong = 'ㄲ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㄲ')
                        {
                            GlobalJong = 'ㄱ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㄹ')
                        {
                            GlobalJong = 'ㄺ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else
                        {
                            GlobalCho = 'ㄱ';
                            GlobalJung = ' ';
                            GlobalJong = ' ';
                            SendKeys.SendWait("ㄱ");
                        }
                    }
                }
                if (e.KeyData == Keys.NumPad5)
                {
                    e.Handled = true;
                    panel5.BackColor = Color.FromArgb(55, 74, 94);
                    
                    if (GlobalJung == ' ')
                    {
                        if (GlobalCho == ' ')
                        {
                            GlobalCho = 'ㄴ';
                            SendKeys.SendWait("ㄴ");
                        }
                        else if (GlobalCho == 'ㄴ')
                        {
                            GlobalCho = 'ㄹ';
                            SendKeys.SendWait("{BS}ㄹ");
                        }
                        else if (GlobalCho == 'ㄹ')
                        {
                            GlobalCho = 'ㄴ';
                            SendKeys.SendWait("{BS}ㄴ");
                        }
                        else
                        {
                            GlobalCho = 'ㄴ';
                            SendKeys.SendWait("ㄴ");
                        }
                    }
                    else
                    {
                        if (GlobalJong == ' ')
                        {
                            GlobalJong = 'ㄴ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㄴ')
                        {
                            GlobalJong = 'ㄹ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㄹ')
                        {
                            GlobalJong = 'ㄴ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else
                        {
                            GlobalCho = 'ㄴ';
                            GlobalJung = ' ';
                            GlobalJong = ' ';
                            SendKeys.SendWait("ㄴ");
                        }
                    }
                }
                if (e.KeyData == Keys.NumPad6)
                {
                    e.Handled = true;
                    panel6.BackColor = Color.FromArgb(55, 74, 94);
                    if (GlobalJung == ' ')
                    {
                        if (GlobalCho == ' ')
                        {
                            GlobalCho = 'ㄷ';
                            SendKeys.SendWait("ㄷ");
                        }
                        else if (GlobalCho == 'ㄷ')
                        {
                            GlobalCho = 'ㅌ';
                            SendKeys.SendWait("{BS}ㅌ");
                        }
                        else if (GlobalCho == 'ㅌ')
                        {
                            GlobalCho = 'ㄸ';
                            SendKeys.SendWait("{BS}ㄸ");
                        }
                        else if (GlobalCho == 'ㄸ')
                        {
                            GlobalCho = 'ㄷ';
                            SendKeys.SendWait("{BS}ㄷ");
                        }
                        else
                        {
                            GlobalCho = 'ㄷ';
                            SendKeys.SendWait("ㄷ");
                        }
                    }
                    else
                    {
                        if (GlobalJong == ' ')
                        {
                            GlobalJong = 'ㄷ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㄷ')
                        {
                            GlobalJong = 'ㅌ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㅌ')
                        {
                            GlobalJong = 'ㄷ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㄹ')
                        {
                            GlobalJong = 'ㄾ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㄾ')
                        {
                            GlobalJong = 'ㄹ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                            GlobalCho = 'ㄷ';
                            GlobalJung = ' ';
                            GlobalJong = ' ';
                            SendKeys.SendWait("ㄷ");
                        }
                        else
                        {
                            GlobalCho = 'ㄷ';
                            GlobalJung = ' ';
                            GlobalJong = ' ';
                            SendKeys.SendWait("ㄷ");
                        }
                    }
                }
                if (e.KeyData == Keys.NumPad7)
                {

                    e.Handled = true;
                    panel1.BackColor = Color.FromArgb(55, 74, 94);
                    if (GlobalJong == ' ')
                    {
                        if (GlobalCho == ' ' && GlobalJung == ' ')
                        {
                            GlobalJung = 'l';
                            SendKeys.SendWait("l");
                        }
                        else if (GlobalJung == 'ㅓ')
                        {
                            GlobalJung = 'ㅔ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJung == 'ㆍ')
                        {
                            GlobalJung = 'ㅓ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJung == 'ᆢ')
                        {
                            GlobalJung = 'ㅕ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJung == 'ㅡ')
                        {
                            GlobalJung = 'ㅢ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJung == 'ㅗ')
                        {
                            GlobalJung = 'ㅚ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJung == 'ㅜ')
                        {
                            GlobalJung = 'ㅟ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJung == 'ㅏ')
                        {
                            GlobalJung = 'ㅐ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJung == 'ㅑ')
                        {
                            GlobalJung = 'ㅒ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        
                        else if (GlobalJung == 'ㅕ')
                        {
                            GlobalJung = 'ㅖ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJung == 'ㅠ')
                        {
                            GlobalJung = 'ㅝ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJung == 'ㅝ')
                        {
                            GlobalJung = 'ㅞ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJung == 'ㅘ')
                        {
                            GlobalJung = 'ㅙ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJung == 'ㅣ')
                        {
                            if (GlobalCho == ' ')
                                GlobalJong = ' ';
                            GlobalCho = ' ';
                            GlobalJung = 'ㅣ';
                            

                            SendKeys.SendWait(GlobalCho.ToString());
                            SendKeys.SendWait("ㅣ");
                        }
                        else if (GlobalJung == ' ')
                        {
                            GlobalJung = 'ㅣ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        
                    }
                    else if (GlobalJong != ' ')
                    {
                        SendKeys.SendWait("{BS}");
                        SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, ' ').ToString());
                        GlobalJung = 'ㅣ';
                        GlobalCho = GlobalJong;
                        GlobalJong = ' ';
                        
                        SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                    }

                }
                if (e.KeyData == Keys.NumPad8)
                {

                    e.Handled = true;
                    panel2.BackColor = Color.FromArgb(55, 74, 94);
                    
                    if (GlobalJong == ' ')
                    {
                        if (GlobalCho == ' ' && GlobalJung == ' ')
                        {
                            GlobalJung = 'ㆍ';
                            SendKeys.SendWait("ㆍ");
                        }
                        else if (GlobalJung == 'ㆍ')
                        {
                            GlobalJung = 'ᆢ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJung == ' ')
                        {
                            GlobalJung = 'ㆍ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJung == 'ㅣ')
                        {
                            GlobalJung = 'ㅏ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJung == 'ㅏ')
                        {
                            GlobalJung = 'ㅑ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJung == 'ㅡ')
                        {
                            GlobalJung = 'ㅜ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJung == 'ㅜ')
                        {
                            GlobalJung = 'ㅠ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJung == 'ㅚ')
                        {
                            GlobalJung = 'ㅘ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJung == 'ㅟ')
                        {
                            GlobalJung = 'ㅝ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                    }
                    else if (GlobalJong != ' ')
                    {
                        SendKeys.SendWait("{BS}");
                        SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, ' ').ToString());
                        GlobalJung = 'ㆍ';
                        GlobalCho = GlobalJong;
                        GlobalJong = ' ';

                        SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                    }
                }
                if (e.KeyData == Keys.NumPad9)
                {
                    e.Handled = true;
                    panel3.BackColor = Color.FromArgb(55, 74, 94);
                    if (GlobalJong == ' ')
                    {   
                        if(GlobalCho == ' ' && GlobalJung == ' ')
                        {
                            GlobalJung = 'ㅡ';
                            SendKeys.SendWait("ㅡ");
                        }
                        else if (GlobalJung == 'ㅡ')
                        {
                            if (GlobalCho == ' ')
                                GlobalJong = ' ';
                            GlobalCho = ' ';
                            GlobalJung = 'ㅡ';


                            SendKeys.SendWait(GlobalCho.ToString());
                            SendKeys.SendWait("ㅡ");
                        }
                        else if (GlobalJung == 'ㆍ')
                        {
                            GlobalJung = 'ㅗ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJung == 'ᆢ')
                        {
                            GlobalJung = 'ㅛ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJung == ' ')
                        {
                            GlobalJung = 'ㅡ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());

                        }
                    }
                    else if (GlobalJong != ' ')
                    {
                        SendKeys.SendWait("{BS}");
                        SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, ' ').ToString());
                        GlobalJung = 'ㅡ';
                        GlobalCho = GlobalJong;
                        GlobalJong = ' ';

                        SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                    }
                }
                if (e.KeyData == Keys.NumPad0)
                {
                    e.Handled = true;
                    panel10.BackColor = Color.FromArgb(55, 74, 94);
                    
                    if (GlobalJung == ' ')
                    {
                        if (GlobalCho == ' ')
                        {
                            GlobalCho = 'ㅇ';
                            SendKeys.SendWait("ㅇ");
                        }
                        else if (GlobalCho == 'ㅇ')
                        {
                            GlobalCho = 'ㅁ';
                            SendKeys.SendWait("{BS}ㅁ");
                        }
                        else if (GlobalCho == 'ㅁ')
                        {
                            GlobalCho = 'ㅇ';
                            SendKeys.SendWait("{BS}ㅇ");
                        }
                        else
                        {
                            GlobalCho = 'ㅇ';
                            SendKeys.SendWait("ㅇ");
                        }
                    }
                    else
                    {
                        if (GlobalJong == ' ')
                        {
                            GlobalJong = 'ㅇ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㅇ')
                        {
                            GlobalJong = 'ㅁ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else if (GlobalJong == 'ㅁ')
                        {
                            GlobalJong = 'ㅇ';
                            SendKeys.SendWait("{BS}");
                            SendKeys.SendWait(hangleHap(GlobalCho, GlobalJung, GlobalJong).ToString());
                        }
                        else
                        {
                            GlobalCho = 'ㅇ';
                            GlobalJung = ' ';
                            GlobalJong = ' ';
                            SendKeys.SendWait("ㅇ");
                        }
                    }
                }
                if(e.KeyData == Keys.Subtract)
                {
                    e.Handled = true;
                    GlobalCho = ' ';
                    GlobalJung = ' ';
                    GlobalJong = ' ';
                    GlobalTuk = ' ';
                    SendKeys.SendWait("{BS}");
                }
                if (e.KeyData == Keys.Add)
                {
                    e.Handled = true;
                    if ((GlobalCho != ' ') || (GlobalJung != ' ') || (GlobalJong != ' '))
                    {
                        GlobalCho = ' ';
                        GlobalJung = ' ';
                        GlobalJong = ' ';
                        GlobalTuk = ' ';
                    }
                    else
                        SendKeys.SendWait(" ");
                }
                if(e.KeyData == Keys.Decimal)
                {
                    e.Handled = true;
                    GlobalCho = ' ';
                    GlobalJung = ' ';
                    GlobalJong = ' ';
                    if(GlobalTuk == ' ')
                    {
                        GlobalTuk = '.';
                        SendKeys.SendWait(".");
                    }
                    else if (GlobalTuk == '.')
                    {
                        GlobalTuk = ',';
                        SendKeys.SendWait("{BS}");
                        SendKeys.SendWait(",");
                    }
                    else if (GlobalTuk==',')
                    {
                        GlobalTuk = '?';
                        SendKeys.SendWait("{BS}");
                        SendKeys.SendWait("?");
                    }
                    else if (GlobalTuk == '?')
                    {
                        GlobalTuk = '!';
                        SendKeys.SendWait("{BS}");
                        SendKeys.SendWait("!");
                    }
                    else if (GlobalTuk == '!')
                    {
                        GlobalTuk = '.';
                        SendKeys.SendWait("{BS}");
                        SendKeys.SendWait(".");
                    }
                }
                else
                    GlobalTuk = ' ';
            }
            
            if (e.KeyData == Keys.NumPad0)
            {
                N0 = true;
                if (ET)
                {
                    if (!activated)
                    {
                        SendKeys.SendWait("{BS}");
                        Hancheck();
                        activated = true;
                        e.Handled = true;
                        Show();
                    }
                    else
                    {
                        activated = false;
                        e.Handled = true;
                        Hide();
                    }
                }

            }
            if (e.KeyData == Keys.Enter)
            {
                ET = true;
                if (N0)
                {
                    if (!activated)
                    {
                        SendKeys.SendWait("{BS}");
                        Hancheck();
                        activated = true;
                        e.Handled = true;
                        Show();
                    }
                    else
                    {
                        activated = false;
                        e.Handled = true;
                        Hide();
                    }
                }
                if(activated)
                {
                    GlobalCho = ' ';
                    GlobalJong = ' ';
                    GlobalJung = ' ';
                    GlobalTuk = ' ';
                }
            }
           
            if (e.KeyData == Keys.NumPad1)
                panel7.BackColor = Color.FromArgb(55, 74, 94);
            else if (e.KeyData == Keys.NumPad2)
                panel8.BackColor = Color.FromArgb(55, 74, 94);
            else if (e.KeyData == Keys.NumPad3)
                panel9.BackColor = Color.FromArgb(55, 74, 94);
            else if (e.KeyData == Keys.NumPad4)
                panel4.BackColor = Color.FromArgb(55, 74, 94);
            else if (e.KeyData == Keys.NumPad5)
                panel5.BackColor = Color.FromArgb(55, 74, 94);
            else if (e.KeyData == Keys.NumPad6)
                panel6.BackColor = Color.FromArgb(55, 74, 94);
            else if (e.KeyData == Keys.NumPad7)
                panel1.BackColor = Color.FromArgb(55, 74, 94);
            else if (e.KeyData == Keys.NumPad8)
                panel2.BackColor = Color.FromArgb(55, 74, 94);
            else if (e.KeyData == Keys.NumPad9)
                panel3.BackColor = Color.FromArgb(55, 74, 94);
            
        }
        
        
        char GlobalTuk = ' ';
        bool N0 = false;
        bool ET = false;
        
        void keyup(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.NumPad1)
                panel7.BackColor = Color.FromArgb(47, 63, 80);
            else if(e.KeyData == Keys.NumPad2)
                panel8.BackColor = Color.FromArgb(47, 63, 80);
            else if(e.KeyData == Keys.NumPad3)
                panel9.BackColor = Color.FromArgb(47, 63, 80);
            else if(e.KeyData == Keys.NumPad4)
                panel4.BackColor = Color.FromArgb(47, 63, 80);
            else if(e.KeyData == Keys.NumPad5)
                panel5.BackColor = Color.FromArgb(47, 63, 80);
            else if(e.KeyData == Keys.NumPad6)
                panel6.BackColor = Color.FromArgb(47, 63, 80);
            else if(e.KeyData == Keys.NumPad7)
                panel1.BackColor = Color.FromArgb(47, 63, 80);
            else if(e.KeyData == Keys.NumPad8)
                panel2.BackColor = Color.FromArgb(47, 63, 80);
            else if(e.KeyData == Keys.NumPad9)
                panel3.BackColor = Color.FromArgb(47, 63, 80);
            else if(e.KeyData == Keys.NumPad0)
            {
                panel10.BackColor = Color.FromArgb(47, 63, 80);
                N0 = false;
            }
            else if(e.KeyData == Keys.Enter)
                ET = false;
        }


        private void label9_Click(object sender, EventArgs e)
        {
            
        }

        [DllImport("imm32.dll")]
        private static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr IParam);

        private const int WM_IME_CONTROL = 643;


        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        void Hancheck()
        {
            if (!IsHangeulNow())
            {
                keybd_event((byte)Keys.HanguelMode, 0, 0, 0);                 //  누름
                keybd_event((byte)Keys.HanguelMode, 0, 0x02, 0);  //  누름 해제 
            }
        }
        bool IsHangeulNow()
        {

            IntPtr hwnd = GetForegroundWindow();
            IntPtr hime = ImmGetDefaultIMEWnd(hwnd);
            IntPtr status = SendMessage(hime, WM_IME_CONTROL, new IntPtr(0x5), new IntPtr(0));

            if (status.ToInt32() != 0)
                return true;

            else
                return false;
        }
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

    }
}
