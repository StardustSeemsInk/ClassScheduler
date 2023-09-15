using ClassScheduler.WPF.Utils;
using System;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClassScheduler.WPF.Views;

public partial class ScheduleWindow : Window
{
    private readonly Timer _timer;

    private double? classProgress;

    public ScheduleWindow()
    {
        InitializeComponent();

        Loaded += ScheduleWindow_Loaded;

        UpdateDatas();

        _timer = new Timer()
        {
            Interval = 5 * 1000,
        };

        _timer.Elapsed += (_, _) =>
        {
            Dispatcher.Invoke(new(() =>
            {
                UpdateDatas();
            }));
        };
        _timer.Start();
    }

    private void ScheduleWindow_Loaded(object sender, RoutedEventArgs e)
    {
        this.MoveToBottom();
        this.SetBottom();

        Left = 0; Top = 0;
    }

    public void UpdateDatas()
    {
        var now = DateTime.Now;
        var today = new DateTime(now.Year, now.Month, now.Day);
        var high_school_entrance_day = new DateTime(now.Year, 6, 6);

        if (high_school_entrance_day < now)
            high_school_entrance_day = high_school_entrance_day.AddYears(1);

        high_school_entrance_day -= high_school_entrance_day.TimeOfDay;

        TextBlock_Time.Text = now.ToString("HH:mm");
        TextBlock_Date.Text = now.ToString("MM 月 dd 日");
        TextBlock_WeekDay.Text = now.ToString("dddd");
        TextBlock_DaysLeft.Text = Convert.ToInt32((high_school_entrance_day - today).TotalDays).ToString();

        RefreshClasses();
    }

    private void RefreshClasses()
    {
        WrapPanel_ClassesContainer.Children.Clear();

        Instances.Classes!.Sort();

        var inClass = false;
        var passedClassesIndex = 0;
        var totalPassesClassesCount = Instances.Classes!.ClassesList.Where(
            x => x.EndTime < DateTime.Now && x.DayOfWeek == (int)DateTime.Now.DayOfWeek
        ).Count() * 1.0;

        foreach (var classModel in Instances.Classes!.ClassesList)
        {
            if (classModel.DayOfWeek! != (int)DateTime.Now.DayOfWeek) continue;

            var tb = new TextBlock()
            {
                Text = classModel.Name,
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 32,
            };

            var now = DateTime.Now;
            var begin = DateTime.Parse(classModel.BeginTime?.ToString("HH:mm")!);
            var end = DateTime.Parse(classModel.EndTime?.ToString("HH:mm")!);

            if (now >= begin && now <= end)
            {
                inClass = true;

                classProgress = (now - begin).TotalSeconds / (end - begin).TotalSeconds * 100;

                tb.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0x5E, 0x5E));
            }
            else if (now >= end)
            {
                ++passedClassesIndex;

                var originColor = 140;
                var colorRange = 60;
                var targetColor = (byte)(originColor + (passedClassesIndex / totalPassesClassesCount) * colorRange);

                tb.Foreground = new SolidColorBrush(
                    Color.FromRgb(targetColor, targetColor, targetColor)
                );
            }
            else if (now >= (begin - new TimeSpan(0, 2, 0)) && now < begin)
                tb.Foreground = new SolidColorBrush(Color.FromRgb(0x03, 0xFC, 0xA5));

            //9dd1a8

            WrapPanel_ClassesContainer.Children.Add(tb);
        }

        classProgress = inClass ? classProgress : null;

        if (classProgress is null)
        {
            Container_ClassProgress.Visibility = Visibility.Hidden;
        }
        else
        {
            Container_ClassProgress.Visibility = Visibility.Visible;
            TextBlock_ClassesProgress.Text = $"{classProgress:f2} %";
        }
    }

    private void Button_OpenSettings_Click(object sender, RoutedEventArgs e)
    {
        Instances.MainWindow?.ComplexShow();
    }

    private void Button_QuitApp_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}
