using System;
using System.Windows;
using System.Windows.Threading;

namespace DeskTopPlayer
{
    /// <summary>
    /// DeskWindow.xaml 的交互逻辑
    /// </summary>

    public partial class DeskWindow : Window
    {
        private DispatcherTimer? timer;

        public DeskWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(DTW_Loaded);
            this.Clock.Text = DateTime.Now.ToString("t");
        }

        private void DTW_Loaded(object sender, RoutedEventArgs e)
        {
             //设置定时器
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(600000000);   //时间间隔为60秒
            timer.Tick += new EventHandler(timer_Tick);

            //开启定时器
            timer.Start();
        }

    public void ChangeDays(string text)
        {
            LeftDays.Text= text;
        }
        
        public void ChangeCharts(string text)
        {
            int a = text.Length;
            string retext = "";
            for (int i = 0; i < a; i++)
            {
                string s = text[i] + "\n";
                retext += s;
            }
            Charts.Text= retext;
        }

        public void MediaPlay(string videoPath)
        {
            MediaPlayer.Source = new Uri(videoPath);

            MediaPlayer.Play();
        }

        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Position -= MediaPlayer.Position;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            this.Clock.Text = DateTime.Now.ToString("t");
        }

    }
}
