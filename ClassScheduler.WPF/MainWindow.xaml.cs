using ClassScheduler.WPF.Data;
using ClassScheduler.WPF.Utils;
using KitX.Contract.CSharp;
using KitX.Web.Rules;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClassScheduler.WPF;

public partial class MainWindow : Window
{
    private readonly Controller controller;

    public DeskWindow? deskWindow = null;

    public string? mediaPath;

    internal Action<Command>? sendCommandAction;

    public MainWindow()
    {
        InitializeComponent();

        controller = new(this);

        var args = Environment.GetCommandLineArgs();

        if (args.Length > 1)
        {
            var video_path = args[1];

            ShowDesk();
            ShowParaVideo(video_path);
        }

        Events.OnSetRootPath += () =>
        {
            Icon = new BitmapImage(
                new(
                    $"{GlobalData.RootPath}/Assets/icon.ico",
                    UriKind.Absolute
                )
            );
        };
    }

    private static int DaysDiff(DateTime start, DateTime end)
    {
        // 计算时间间隔
        TimeSpan sp = end.Subtract(start);
        return sp.Days;
    }

    private static string MakeChart(DateTime dateTime)
    {
        // 按星期选择课表
        if (GlobalData.RootPath != null)
        {
            string path = GlobalData.RootPath + @"\Assets\ChartData.txt";
            string[] allCharts = System.IO.File.ReadAllLines(path);
            return allCharts[Convert.ToInt32(dateTime.DayOfWeek) + 1];
        }
        else return string.Empty;
    }

    private void ShowDeskBg_Click(object sender, RoutedEventArgs e) => ShowDesk();

    private void ShowDesk()
    {
        if (GlobalData.RootPath is not null)
        {
            var path = GlobalData.RootPath + @"\Assets\ChartData.txt";
            var allCharts = System.IO.File.ReadAllLines(path);
            var GKyear = Convert.ToInt32(allCharts[8]); // 高考年份

            var leftDays = DaysDiff(
                DateTime.Today,
                new DateTime(GKyear, 6, 7)
            );

            if (deskWindow is null)
            {
                // 创建 DeskWindow 窗口
                deskWindow = new()
                {
                    // 设置总宽/长度
                    Width = SystemParameters.PrimaryScreenWidth,
                    Height = SystemParameters.PrimaryScreenHeight
                };
                deskWindow.ChangeDays(leftDays.ToString());
                deskWindow.ChangeCharts(MakeChart(DateTime.Today));
                deskWindow.Show();

                User32.SetDeskTop(deskWindow);
            }
            else
            {
                deskWindow.ChangeDays(leftDays.ToString());
                deskWindow.ChangeCharts(MakeChart(DateTime.Today));
            }
        }
    }

    private void ReWriteCharts_Click(object sender, RoutedEventArgs e)
    {
        // 重写课表
        if (deskWindow is null) return;

        deskWindow.ChangeCharts(chartsInput.Text);
    }

    private int color_counts = 0;

    private SolidColorBrush ColorTextNow = Brushes.White;

    private void ChangeFColorBg_Click(object sender, RoutedEventArgs e)
    {
        // 更换字体颜色
        if (deskWindow is null) return;

        switch (color_counts)
        {
            case 0:
                ColorTextNow = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#66CCFF")
                );
                color_counts++;
                break;
            case 1:
                ColorTextNow = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("yellow")
                );
                color_counts++;
                break;
            case 2:
                ColorTextNow = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("red")
                );
                color_counts++;
                break;
            case 3:
                ColorTextNow = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("black")
                );
                color_counts++;
                break;
            default:
                ColorTextNow = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("white")
                );
                color_counts = 0;
                break;
        }

        deskWindow.LeftDaysTitle.Foreground = ColorTextNow;
        deskWindow.ChartsTitle.Foreground = ColorTextNow;
        deskWindow.Charts.Foreground = ColorTextNow;
        deskWindow.TextDay.Foreground = ColorTextNow;
    }

    private void ChooseVideoBg_Click(object sender, RoutedEventArgs e)
    {
        // 选择视频并播放
        var dialog = new OpenFileDialog
        {
            Filter = "Video File(*.avi;*.mp4;*.mkv;*.wav;*.rmvb)|*.avi;*.mp4;*.mkv;*.wav;*.rmvb|All File(*.*)|*.*"
        };

        if (!dialog.ShowDialog().GetValueOrDefault())
        {
            return;
        }

        if (deskWindow is null)
        {
            return;
        }

        mediaPath = dialog.FileName;
        deskWindow.MediaPlay(mediaPath);

        deskWindow.MediaPlayer.Volume = (double)volumeSlider.Value;
    }

    private void ShowParaVideo(string video_path)
    {
        if (deskWindow is null) return;

        volumeSlider.Value = 0;

        deskWindow.MediaPlay(video_path);

        deskWindow.MediaPlayer.Volume = (double)volumeSlider.Value;

        ColorTextNow = new SolidColorBrush(
            (Color)ColorConverter.ConvertFromString("#66CCFF")
        );

        deskWindow.LeftDaysTitle.Foreground = ColorTextNow;
        deskWindow.ChartsTitle.Foreground = ColorTextNow;
        deskWindow.Charts.Foreground = ColorTextNow;
        deskWindow.TextDay.Foreground = ColorTextNow;
    }

    private void ChangeVolume(object sender, RoutedPropertyChangedEventArgs<double> args)
    {
        // 音量调节
        if (deskWindow is null) return;

        deskWindow.MediaPlayer.Volume = (double)volumeSlider.Value;
    }

    private void StopDeskBg_Click(object sender, RoutedEventArgs e)
    {
        // 关闭deskwindow、deskhild
        if (deskWindow is not null)
        {
            deskWindow.Close();

            deskWindow = null;
        }

        //if (deskChild != null)
        //{
        //    deskChild.Close();
        //    deskChild = null;
        //}
    }

    private void Window_Closed(object sender, System.EventArgs e)
    {
        Application.Current.Shutdown();

        Environment.Exit(0);
    }

    public void WinPause()
    {
        // 关闭deskwindow
        if (deskWindow is not null)
        {
            deskWindow.Close();
            deskWindow = null;
        }
    }

    public void WinExit()
    {
        WinPause();
        Close();
        Application.Current.Shutdown();
        Environment.Exit(0);
    }
}
