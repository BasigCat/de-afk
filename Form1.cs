using System;
using System.Runtime.InteropServices;
using System.Media;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Deafk
{

    public partial class Form1 : Form
    {
        [DllImport("jinput.dll", CharSet = CharSet.Unicode)]
        public static extern int init();
        [DllImport("jinput.dll", CharSet = CharSet.Unicode)]
        public static extern int updateJoyState(int Joy);
        [DllImport("jinput.dll", CharSet = CharSet.Unicode)]
        public static extern int getJoyBtn(int Joy, int Btn);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetKeyState(int vKey);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int keybd_event(int bVk, int bScan, UInt32 dwFlags, int dwExtraInfo);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);

        /*Manually params*/
        int joystics;
        int keyvoice = 69;
        int activationKey;
        int voicekeymode = 0; //default 0 - keyboard
        int voicejoykey;
        int active = 0;
        bool launched = false;

        INIManager cfg = new INIManager("./config.ini");

        public Form1()
        {
            InitializeComponent();
            joystics = init();

            //loading params
            keyvoice = Int32.Parse(cfg.GetPrivateString("data", "keyVoice"));
            activationKey = Int32.Parse(cfg.GetPrivateString("data", "activationKey"));
            voicekeymode = Int32.Parse(cfg.GetPrivateString("data", "voiceKeyMode"));
            voicejoykey = Int32.Parse(cfg.GetPrivateString("data", "voiceJoyKey"));
            checkBox1.Checked = bool.Parse(cfg.GetPrivateString("data", "autoStart"));

            textBox1.Text = cfg.GetPrivateString("interface", "SZactivationKey");
            textBox3.Text = cfg.GetPrivateString("interface", "SZkeyVoice");
            textBox2.Text = cfg.GetPrivateString("interface", "SZtext");

            timer5.Enabled = true;
        }

        private void playSound(string path)
        {
            SoundPlayer sound = new SoundPlayer(path);
            sound.Play();
        }

        private int getKey(int key)
        {
            return GetKeyState(key) & 0x8000;
        }

        private void Label1_MouseDown(object sender, MouseEventArgs e)
        {
            base.Capture = false;
            panel1.Capture = false;
            label1.Capture = false;
            Message m = Message.Create(base.Handle, 161, new IntPtr(2), IntPtr.Zero);
            this.WndProc(ref m);
        }

        private void Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            base.Capture = false;
            panel1.Capture = false;
            label1.Capture = false;
            Message m = Message.Create(base.Handle, 161, new IntPtr(2), IntPtr.Zero);
            this.WndProc(ref m);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            base.WindowState = FormWindowState.Minimized;
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
                timer2.Enabled = true;
            else
                timer2.Enabled = false;
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            base.Hide();
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(2000, "DE-AFK", "Вы можете найти иконку De-Afk в трее Windows.", ToolTipIcon.Info);
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            base.Show();
        }

        private void TextBox3_Enter(object sender, EventArgs e)
        {
            
        }

        private void TextBox3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void TextBox3_Leave(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (joystics < 1)
                return;

            updateJoyState(0);

            for(int i = 1; i != 32; i++)
            {
                if(getJoyBtn(0, i) > 0)
                {
                    textBox3.Text = "Joy_" + i.ToString();
                    voicejoykey = i;
                    voicekeymode = 1; //joyKeyMode
                    break;
                }
            }
        }

        private void TextBox3_KeyUp(object sender, KeyEventArgs e)
        {
            textBox3.Text = e.KeyData.ToString();
            keyvoice = e.KeyValue;
            voicekeymode = 0;
        }

        private void TextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            textBox1.Text = e.KeyData.ToString();
            activationKey = e.KeyValue;
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            if (launched)
            {
                for (int i = 0x1; i != 0x0FE; i++)
                {
                    if (getKey(i) != 0)
                    {
                        active = 1;
                        break;
                    }
                }

                if (active == 1)
                    timer3.Enabled = true;
                else
                    timer3.Enabled = false;
            }
        }

        private void Timer4_Tick(object sender, EventArgs e)
        {
            int hWnd = FindWindow(null, "Euro Truck Simulator 2 Multiplayer");

            if(hWnd == 0)
            {
                label5.Text = "ETS2 не запущен.";
                label5.ForeColor = System.Drawing.Color.FromArgb(0x00ff8c9f);

                if(launched != false)
                {
                    playSound("./etsc.wav");
                    launched = false;
                }
            }
            else
            {
                label5.Text = "ETS2 запущен.";
                label5.ForeColor = System.Drawing.Color.FromArgb(0x0092fc8b);

                if (launched != true)
                {
                    playSound("./etso.wav");
                    launched = true;
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            cfg.WritePrivateString("data", "keyVoice", keyvoice.ToString());
            cfg.WritePrivateString("data", "activationKey", activationKey.ToString());
            cfg.WritePrivateString("data", "voiceKeyMode", voicekeymode.ToString());
            cfg.WritePrivateString("data", "voiceJoyKey", voicejoykey.ToString());
            cfg.WritePrivateString("data", "autoStart", checkBox1.Checked.ToString());

            cfg.WritePrivateString("interface", "SZactivationKey", textBox1.Text);
            cfg.WritePrivateString("interface", "SZkeyVoice", textBox3.Text);
            cfg.WritePrivateString("interface", "SZtext", textBox2.Text);
        }

        private void keyUP(int key)
        {
            keybd_event(key, 0, 2, 0);
        }

        private void keyDown(int key)
        {
            keybd_event(key, 0, 0, 0);
        }

        public static Keys ConvertCharToVirtualKey(char ch)
        {
            short vkey = VkKeyScan(ch);
            Keys retval = (Keys)(vkey & 0xff);
            int modifiers = vkey >> 8;
            if ((modifiers & 1) != 0) retval |= Keys.Shift;
            if ((modifiers & 2) != 0) retval |= Keys.Control;
            if ((modifiers & 4) != 0) retval |= Keys.Alt;
            return retval;
        }

        private void Timer3_Tick(object sender, EventArgs e)
        {
            if (!launched)
                return;

            keyDown(0x59);
            keyUP(0x59);

            for(int i = 0; i != textBox2.Text.Length; i++)
            {
                keyDown((int)ConvertCharToVirtualKey(textBox2.Text[i]));
                keyUP((int)ConvertCharToVirtualKey(textBox2.Text[i]));
            }

            keyDown(0x0D);
            keyUP(0x0D);

            playSound("./beep.wav");
        }

        private void Timer5_Tick(object sender, EventArgs e)
        {
            if (!launched)
                return;

            if(voicekeymode == 0)
            {
                if(getKey(keyvoice) != 0)
                {
                    keyDown(0x58);
                    playSound("./wt.wav");
                    while (true)
                    {
                        if (getKey(keyvoice) == 0) break;
                    }
                    keyUP(0x58);
                }
            }
            else
            {
                updateJoyState(0);
                if(getJoyBtn(0, voicejoykey) > 0)
                {
                    keyDown(0x58);
                    playSound("./wt.wav");
                    while (true)
                    {
                        if (getJoyBtn(0, voicejoykey) == 0) break;
                    }
                    keyUP(0x58);
                }
            }
        }

        private void Timer6_Tick(object sender, EventArgs e)
        {
            if (!launched)
                return;

            if (getKey(activationKey) != 0)
            {
                if (active == 0)
                {
                    active = 1;
                    playSound("./activation.wav");
                    timer3.Enabled = true;
                }
                else
                {
                    active = 0;
                    playSound("./deactivation.wav");
                    timer3.Enabled = false;
                }
            }
        }
    }

    public class INIManager
    {
        public INIManager(string aPath)
        {
            path = aPath;
        }

        public INIManager() : this("") { }

        public string GetPrivateString(string aSection, string aKey)
        {
            StringBuilder buffer = new StringBuilder(SIZE);
            GetPrivateString(aSection, aKey, null, buffer, SIZE, path);
            return buffer.ToString();
        }

        public void WritePrivateString(string aSection, string aKey, string aValue)
        {
            WritePrivateString(aSection, aKey, aValue, path);
        }

        public string Path { get { return path; } set { path = value; } }

        private const int SIZE = 1024;
        private string path = null;

        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileString")]
        private static extern int GetPrivateString(string section, string key, string def, StringBuilder buffer, int size, string path);

        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileString")]
        private static extern int WritePrivateString(string section, string key, string str, string path);
    }
}
