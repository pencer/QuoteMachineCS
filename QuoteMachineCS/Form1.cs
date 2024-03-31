using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

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

        const int HOTKEY_ID1  = 0x0001;
        const int HOTKEY_ID2  = 0x0002;
        const int HOTKEY_ID3  = 0x0003;
        const int HOTKEY_ID4  = 0x0004;
        const int HOTKEY_ID5  = 0x0005;
        const int HOTKEY_ID6  = 0x0006;
        const int HOTKEY_ID7  = 0x0007;

        string m_history = "";

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

        private void RemoveQuote()
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.Text))
            {
                string str = (string)data.GetData(DataFormats.Text);
                string[] lines = str.Split('\n');
                string res = "";
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if ((line == "") && (i == lines.Length - 1)) { break; }
                    if (line.StartsWith(">"))
                    {
                        if (line[1] == ' ')
                        {
                            res += line.Substring(2);
                        }
                        else if (line[1] == '>' || line[1] == '\r')
                        {
                            res += line.Substring(1);
                        }
                    }
                    //if (line.StartsWith("> "))
                    //{
                    //    res += line.Substring(2);
                    //}
                    //else if (line == ">\r")
                    //{
                    //    res += line.Substring(1);
                    //}
                    else
                    {
                        res += line;
                    }
                    res += "\n";
                }
                Clipboard.SetDataObject(res, true);
            }
        }

        private void RemoveEmptyLine()
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.Text))
            {
                string str = (string)data.GetData(DataFormats.Text);
                string[] lines = str.Split('\n');
                string res = "";
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if ((line == "") && (i == lines.Length - 1)) { break; }
                    if (line[0] != '\r')
                    {
                        res += line;
                        res += "\n";
                    }
                }
                Clipboard.SetDataObject(res, true);
            }
        }
        private void AddMarkdownQuote(bool append = false)
        {
            // Add three back-quote before and after the contents
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.Text))
            {
                string str = (string)data.GetData(DataFormats.Text);
                string[] lines = str.Split('\n');
                string res = "";
                if (append)
                {
                    res = m_history + "\r\n";
                }
                res += "```\r\n";
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if ((line == "") && (i == lines.Length - 1)) { break; }
                    if (line[0] != '\r')
                    {
                        res += line;
                        res += "\n";
                    }
                }
                res += "```\r\n";
                Clipboard.SetDataObject(res, true);
                m_history = res; // Set as history
            }
        }
        private void AddQuote()
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.Text))
            {
                string str = (string)data.GetData(DataFormats.Text);
                string[] lines = str.Split('\n');
                string res = "";
                bool first = true;
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if ((line == "") && (i == lines.Length - 1)) { break; }
                    if (!first) { res += "\n"; }
                    res += "> " + line;
                    first = false;
                }
                Clipboard.SetDataObject(res, true);
            }
        }

        private void ConvOneNotePathForSlack()
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.Text))
            {
                string str = (string)data.GetData(DataFormats.Text);
                string[] lines = str.Split('\n');
                string res = "";
                bool first = true;
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if ((line == "") && (i == lines.Length - 1)) { break; }
                    if (!first) { res += "\n"; }
                    if (line.StartsWith("onenote:///"))
                    {
                        res += "onenote:" + line.Substring(11).Replace('\\', '/');
                    }
                    else
                    {
                        res += line;
                    }
                    first = false;
                }
                Clipboard.SetDataObject(res, true);
            }

        }

        private void ConvFileURIToUNC()
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.Text))
            {
                string str = (string)data.GetData(DataFormats.Text);
                string[] lines = str.Split('\n');
                string res = "";
                bool first = true;
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if ((line == "") && (i == lines.Length - 1)) { break; }
                    if (!first) { res += "\n"; }
                    if (line.StartsWith("file://"))
                    {
                        string curpath = line.Substring(7);
                        if (Regex.IsMatch(curpath, "^[a-zA-Z]:/"))
                        {
                            res += curpath.Replace('/', '\\');
                        }
                        else
                        {
                            res += "\\\\" + curpath.Replace('/', '\\');
                        }
                    }
                    else
                    {
                        res += line;
                    }
                    first = false;
                }
                Clipboard.SetDataObject(res, true);
            }

        }

        private void ConvUNCToFileURI()
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.Text))
            {
                string str = (string)data.GetData(DataFormats.Text);
                string[] lines = str.Split('\n');
                string res = "";
                bool first = true;
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if ((line == "") && (i == lines.Length - 1)) { break; }
                    if (!first) { res += "\n"; }
                    if (line.StartsWith("\\\\")) // hostname is specified
                    {
                        res += "file:" + line.Replace('\\', '/');
                    }
                    else if (Regex.IsMatch(line, "^[a-zA-Z]:\\\\")) // localhost
                    {
                        res += "file://" + line.Replace('\\', '/');
                    }
                    else
                    {
                        res += line;
                    }
                    first = false;
                }
                Clipboard.SetDataObject(res, true);
            }

        }

        private void OpenExplorer()
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.Text))
            {
                string str = (string)data.GetData(DataFormats.Text);
                string[] lines = str.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    string path = line.Trim();
                    if (System.IO.Directory.Exists(path))
                    {
                        System.Diagnostics.Process.Start("EXPLORER.EXE", path);
                    }
                    break;
                }
            }
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            // Register Hotkey
            RegisterHotKey(this.Handle, HOTKEY_ID1, MOD_CONTROL, (int)Keys.F9);
            RegisterHotKey(this.Handle, HOTKEY_ID2, MOD_CONTROL, (int)Keys.F8);
            RegisterHotKey(this.Handle, HOTKEY_ID3, MOD_CONTROL, (int)Keys.F10);
            RegisterHotKey(this.Handle, HOTKEY_ID4, MOD_CONTROL, (int)Keys.F11);
            RegisterHotKey(this.Handle, HOTKEY_ID5, MOD_CONTROL, (int)Keys.F12);
            RegisterHotKey(this.Handle, HOTKEY_ID6, MOD_CONTROL, (int)Keys.F7);
            RegisterHotKey(this.Handle, HOTKEY_ID7, MOD_CONTROL, (int)Keys.F6);

            // Hide in task tray
            // http://csharp-cafe.info/c/c%E3%81%A7%E3%82%BF%E3%82%B9%E3%82%AF%E3%83%88%E3%83%AC%E3%82%A4%E5%B8%B8%E9%A7%90%E5%9E%8B%E3%82%A2%E3%83%97%E3%83%AA%E4%BD%9C%E6%88%90%E6%B3%952.html
            Hide();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Unregister Hotkey
            UnregisterHotKey(this.Handle, HOTKEY_ID1);
            UnregisterHotKey(this.Handle, HOTKEY_ID2);
            UnregisterHotKey(this.Handle, HOTKEY_ID3);
            UnregisterHotKey(this.Handle, HOTKEY_ID4);
            UnregisterHotKey(this.Handle, HOTKEY_ID5);
            UnregisterHotKey(this.Handle, HOTKEY_ID6);
            UnregisterHotKey(this.Handle, HOTKEY_ID7);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_HOTKEY)
            {
                if ((int)m.WParam == HOTKEY_ID1)
                {
                    AddQuote();
                    //this.notifyIcon1.BalloonTipText = "Quote added.";
                    //this.notifyIcon1.BalloonTipTitle = "A";
                    //this.notifyIcon1.Visible = true;
                    //this.notifyIcon1.ShowBalloonTip(1000);
                }
                if ((int)m.WParam == HOTKEY_ID2)
                {
                    RemoveQuote();
                    //this.notifyIcon1.BalloonTipText = "Quote removed.";
                    //this.notifyIcon1.BalloonTipTitle = "A";
                    //this.notifyIcon1.Visible = true;
                    //this.notifyIcon1.ShowBalloonTip(1000);
                }
                if ((int)m.WParam == HOTKEY_ID3)
                {
                    AddMarkdownQuote();
                    //ConvOneNotePathForSlack();
                    //this.notifyIcon1.BalloonTipText = "Converted OneNote Path for Slack.";
                    //this.notifyIcon1.ShowBalloonTip(1000);
                }
                if ((int)m.WParam == HOTKEY_ID4)
                {
                    AddMarkdownQuote(true);
                    //ConvFileURIToUNC();
                    //this.notifyIcon1.BalloonTipText = "Converted File URI to UNC.";
                    //this.notifyIcon1.ShowBalloonTip(1000);
                }
                if ((int)m.WParam == HOTKEY_ID5)
                {
                    ConvUNCToFileURI();
                    //this.notifyIcon1.BalloonTipText = "Converted UNC to File URI.";
                    //this.notifyIcon1.ShowBalloonTip(1000);
                }
                if ((int)m.WParam == HOTKEY_ID6)
                {
                    OpenExplorer();
                }
                if ((int)m.WParam == HOTKEY_ID7)
                {
                    RemoveEmptyLine();
                }
            }
        }
    

        private void exitXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.notifyIcon1.Visible = false;
            Application.Exit();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }
    }
}
