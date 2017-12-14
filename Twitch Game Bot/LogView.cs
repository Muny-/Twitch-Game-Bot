using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Twitch_Game_Bot
{
    public partial class LogView : Form
    {
        public LogView()
        {
            InitializeComponent();
        }

        public void AddLog(string text)
        {
            richTextBox1.Invoke(new MethodInvoker(delegate()
            {
                richTextBox1.AppendText(text + "\n");
                richTextBox1.ScrollToCaret();
            }));
        }

        private void LogView_SizeChanged(object sender, EventArgs e)
        {
            
        }
    }
}
