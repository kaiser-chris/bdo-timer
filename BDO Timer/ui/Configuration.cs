using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Timer
{
    public partial class Configuration : Form
    {
        public Configuration()
        {
            InitializeComponent();
        }

        private void Configuration_Load(object sender, EventArgs e)
        {
            Hide();
            ContextMenu trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Close Application", MenuExit);
            trayIcon.ContextMenu = trayMenu;
            
            timerDailyReset.Interval = CalculateDailyReset();
            timerDailyReset.Enabled = true;

            timerNight.Interval = CalculateSecondsUntilNight() * 1000;
            timerDailyReset.Enabled = true;
        }

        private void MenuOpenConfig(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void MenuExit(object sender, EventArgs e)
        {
            Close();
        }

        private string BuildGameTimeString()
        {
            double secondsIntoGameDay = CalculateSecondsIntoGameDay();
            double percentOfDayDone = secondsIntoGameDay / (200 * 60);
            double gameHour = 7 + (22 - 7) * percentOfDayDone;
            int inGameHour = (int)(gameHour / 1);
            int inGameMinute = (int)(gameHour % 1 * 60);
            return String.Format("{0:D2}:{1:D2}", inGameHour, inGameMinute);
        }

        private string BuildNightTimeString()
        {
            int minutesUntilNight = CalculateSecondsUntilNight() / 60;
            int minutes = minutesUntilNight % 60;
            int hours = minutesUntilNight / 60;
            return String.Format("{0}h{1}m", hours, minutes);
        }

        private void TrayIcon_Click(object sender, EventArgs e)
        {
            string message = String.Format("Time: {0}\nNight in: {1}", BuildGameTimeString(), BuildNightTimeString());
            SendToolTip(message);
        }

        private void TimerDailyReset_Tick(object sender, EventArgs e)
        {
            SendToolTip("Daily Reset");
            timerDailyReset.Interval = CalculateDailyReset();
        }

        private void TimerNight_Tick(object sender, EventArgs e)
        {
            SendToolTip("Now it is Night");
            timerDailyReset.Interval = CalculateSecondsUntilNight() * 1000;
        }

        private void SendToolTip(string message)
        {
            trayIcon.ShowBalloonTip(1000, "Black Desert Online", message, ToolTipIcon.Info);
        }

        #region Calculations

        private double CalculateElapsedSeconds()
        {
            return (DateTime.UtcNow.Ticks - DateTime.UtcNow.Date.Ticks) / TimeSpan.TicksPerMillisecond / 1000;
        }

        private double CalculateSecondsIntoGameDay()
        {
            return (CalculateElapsedSeconds() + 200 * 60 + 20 * 60) % (240 * 60);
        }

        private int CalculateSecondsUntilNight()
        {
            if (CalculateSecondsIntoGameDay() >= 12000)
            {
                return 2400 - ((int)CalculateSecondsIntoGameDay() - 12000) + 12000;
            }
            else
            {
                return 12000 - (int)CalculateSecondsIntoGameDay();
            }
        }

        private int CalculateDailyReset()
        {
            return (24 * 60 * 60 - (int)CalculateElapsedSeconds()) * 1000;
        }

        #endregion
    }
}
