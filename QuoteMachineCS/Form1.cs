﻿using System;
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

        const int HOTKEY_ID1  = 0x0001;
        const int HOTKEY_ID2  = 0x0002;
        const int HOTKEY_ID3  = 0x0003;

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
                bool first = true;
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if ((line == "") && (i == lines.Length - 1)) { break; }
                    if (line.StartsWith("> "))
                    {
                        if (!first) { res += "\n"; }
                        res += line.Substring(2);
                    }else
                    {
                        res += line;
                    }
                    first = false;
                }
                Clipboard.SetDataObject(res, true);
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

        private void Form1_Load(object sender, EventArgs e)
        {
            // Register Hotkey
            RegisterHotKey(this.Handle, HOTKEY_ID1, MOD_CONTROL, (int)Keys.F9);
            RegisterHotKey(this.Handle, HOTKEY_ID2, MOD_CONTROL, (int)Keys.F8);
            RegisterHotKey(this.Handle, HOTKEY_ID3, MOD_CONTROL, (int)Keys.F7);

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
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_HOTKEY)
            {
                if ((int)m.WParam == HOTKEY_ID1)
                {
                    AddQuote();
                    this.notifyIcon1.BalloonTipText = "Quote added.";
                }
                if ((int)m.WParam == HOTKEY_ID2)
                {
                    RemoveQuote();
                    this.notifyIcon1.BalloonTipText = "Quote removed.";
                }
                if ((int)m.WParam == HOTKEY_ID3)
                {
                    ConvOneNotePathForSlack();
                    this.notifyIcon1.BalloonTipText = "Converted OneNote Path for Slack.";
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
