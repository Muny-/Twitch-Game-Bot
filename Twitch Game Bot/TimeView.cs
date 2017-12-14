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
    public partial class TimeView : Form
    {
        public TimeView()
        {
            InitializeComponent();
        }

        TimeSpan elapsedTime = new TimeSpan(0, 0, 0);

        private void timer1_Tick(object sender, EventArgs e)
        {
            elapsedTime = elapsedTime.Add(new TimeSpan(0, 0, 1));

            label1.Text = elapsedTime.Days + "d " + elapsedTime.Hours + "hr " + elapsedTime.Minutes + "m " + elapsedTime.Seconds + "s";
        }
    }
}
