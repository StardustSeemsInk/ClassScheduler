using System;
using System.Timers;
using System.Windows;

namespace ClassScheduler.WPF;

/// <summary>
/// DeskWindow.xaml 的交互逻辑
/// </summary>

public partial class DeskWindow : Window
{
    private Timer? timer;

    public DeskWindow()
    {
        InitializeComponent();

        Loaded += new RoutedEventHandler(DTW_Loaded);

        Clock.Text = DateTime.Now.ToString("t");
    }

    private void DTW_Loaded(object sender, RoutedEventArgs e)
    {
        //设置定时器
        timer = new()
        {
            Interval = 60 * 1000,   //时间间隔为60秒
            AutoReset = true,
        };
        timer.Elapsed += (_, _) => Clock.Text = DateTime.Now.ToString("t");

        //开启定时器
        timer.Start();
    }

    public void ChangeDays(string text) => LeftDays.Text = text;

    public void ChangeCharts(string text)
    {
        var a = text.Length;
        var retext = "";

        for (var i = 0; i < a; ++i)
        {
            var s = $"{text[i]}\n";
            retext += s;
        }

        Charts.Text = retext;
    }

    public void MediaPlay(string videoPath)
    {
        MediaPlayer.Source = new(videoPath);

        MediaPlayer.Play();
    }

    private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        => MediaPlayer.Position -= MediaPlayer.Position;
}
