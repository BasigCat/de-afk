using System;
using System.Runtime.InteropServices;
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

        /*Manually params*/
        int joystics;
        Keys keyvoice;
        Keys activationKey;
        int voicekeymode = 0; //default 0 - keyboard
        int voicejoykey;

        public Form1()
        {
            InitializeComponent();
            joystics = init();
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

            for(int i = 0; i != 32; i++)
            {
                if(getJoyBtn(0, i) != 0)
                {
                    textBox3.Text = "Joy_" + i;
                    voicejoykey = i;
                    voicekeymode = 1; //joyKeyMode
                    break;
                }
            }
        }

        private void TextBox3_KeyUp(object sender, KeyEventArgs e)
        {
            textBox3.Text = e.KeyData.ToString();
            keyvoice = e.KeyData;
            voicekeymode = 0;
        }

        private void TextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            textBox1.Text = e.KeyData.ToString();
            activationKey = e.KeyData;
        }
    }
}
