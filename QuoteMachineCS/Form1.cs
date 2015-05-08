using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Ref: http://inner2.hatenablog.com/entry/2013/10/27/215707
using System.Runtime.InteropServices;  // for DllImport

namespace QuoteMachineCS
{
    public partial class Form1 : Form
    {
        const int MOD_ALT     = 0x0001;
        const int MOD_CONTROL = 0x0002;
        const int MOD_SHIFT   = 0x0004;
        const int WM_HOTKEY   = 0x0312;

        const int HOTKEY_ID  = 0x0001;

        [DllImport("user32.dll")]
        extern static int RegisterHotKey(IntPtr HWnd, int ID, int MOD_KEY, int KEY);
        [DllImport("user32.dll")]
        extern static int UnregisterHotKey(IntPtr HWnd, int ID);

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddQuote();
        }

        private void AddQuote()
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.Text))
            {
                string str = (string)data.GetData(DataFormats.Text);
                string[] lines = str.Split('\n');
                string res = "";
                foreach (string line in lines)
                {
                    res += "> " + line + "\n";
                }
                Clipboard.SetDataObject(res, true);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Register Hotkey
            RegisterHotKey(this.Handle, HOTKEY_ID, MOD_CONTROL, (int)Keys.F9);

            Hide();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Unregister Hotkey
            UnregisterHotKey(this.Handle, HOTKEY_ID);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_HOTKEY)
            {
                if ((int)m.WParam == HOTKEY_ID)
                {
                    AddQuote();
                    this.notifyIcon1.BalloonTipText = "Quote added.";
                }
            }
        }

        private void exitXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.notifyIcon1.Visible = false;
            Application.Exit();
        }
    }
}
