using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DeskTopPlayer
{
    /// <summary>
    /// DeskWindow.xaml 的交互逻辑
    /// </summary>

    public partial class DeskWindow : Window
    {

        public DeskWindow()
        {
            InitializeComponent();
            //this.Icon = new BitmapImage(new Uri(Environment.CurrentDirectory + @"\assets\th.ico"));
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
    }
}
