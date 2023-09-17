using ClassScheduler.WPF.Utils;
using ClassScheduler.WPF.Utils.Converter;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ClassScheduler.WPF.Views;

public partial class ScheduleWindow : Window
{
    private readonly Timer mainTimer;

    private double? classProgress;
    private bool isPlayingClassOverAnimation = false;
    private bool isPlayingClassBeginAnimation = false;

    public ScheduleWindow()
    {
        InitializeComponent();

        Loaded += ScheduleWindow_Loaded;

        UpdateDatas();

        mainTimer = new Timer()
        {
            Interval = 5 * 1000,
        };

        mainTimer.Elapsed += (_, _) =>
        {
            Dispatcher.Invoke(new(() =>
            {
                UpdateDatas();
            }));
        };
        mainTimer.Start();
    }

    private void ScheduleWindow_Loaded(object sender, RoutedEventArgs e)
    {
        this.MoveToBottom();
        this.SetBottom();

        Left = 0; Top = 0;

        (Resources["Storyboard_ClassBegin"] as Storyboard)!.Completed += (_, _) =>
        {
            isPlayingClassBeginAnimation = false;
        };
        (Resources["Storyboard_ClassOver"] as Storyboard)!.Completed += (_, _) =>
        {
            isPlayingClassOverAnimation = false;
            Container_ClassProgress.Visibility = Visibility.Hidden;
            Container_ClassProgress.Opacity = 1;
        };
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
            x => x.EndTime < DateTime.Now && x.DayOfWeek == DateTime.Now.DayOfWeek.ToInt()
        ).Count() * 1.0;

        foreach (var classModel in Instances.Classes!.ClassesList)
        {
            if (classModel.DayOfWeek! != DateTime.Now.DayOfWeek.ToInt()) continue;

            var tb = new TextBlock()
            {
                Text = classModel.Name,
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 32,
            };

            var now = DateTime.Now;
            var begin = DateTime.Parse(classModel.BeginTime?.ToString("HH:mm")!);
            var end = DateTime.Parse(classModel.EndTime?.ToString("HH:mm")!);

            // 不是已完成的课 用于显示当前课程的详细信息
            //if (now < end)
            //{
            //    TextBlock_ClassName.Text = classModel.Name;
            //    TextBlock_ClassDate.Text = now.Date.ToString("M");
            //    TextBlock_ClassWeekDay.Text = now.DayOfWeek.ToString();
            //    TextBlock_ClassTeacher.Text = classModel.Teacher?.Name;
            //    TextBlock_ClassBeginTime.Text = classModel.BeginTime?.ToString("HH:mm");
            //    TextBlock_ClassEndTime.Text = classModel.EndTime?.ToString("HH:mm");
            //}
                

            // 正在上的课
            if (now >= begin && now <= end)
            {
                inClass = true;

                TextBlock_ClassName.Text = classModel.Name;
                TextBlock_ClassDate.Text = now.Date.ToString("M");
                TextBlock_ClassWeekDay.Text = now.DayOfWeek.ToString();
                TextBlock_ClassTeacher.Text = classModel.Teacher?.Name;
                TextBlock_ClassBeginTime.Text = classModel.BeginTime?.ToString("HH:mm");
                TextBlock_ClassEndTime.Text = classModel.EndTime?.ToString("HH:mm");

                classProgress = (now - begin).TotalSeconds / (end - begin).TotalSeconds * 100;

                tb.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0x5E, 0x5E));

                if ((now - begin).TotalSeconds <= 6 && !isPlayingClassBeginAnimation)
                {
                    isPlayingClassBeginAnimation = true;
                    (Resources["Storyboard_ClassBegin"] as Storyboard)!.Begin();
                }
            }
            // 已完成的课
            else if (now >= end)
            {
                ++passedClassesIndex;

                var originColor = 180;
                var colorRange = 70;
                var targetColor = (byte)(originColor + (passedClassesIndex / totalPassesClassesCount) * colorRange);

                tb.Foreground = new SolidColorBrush(
                    Color.FromRgb(targetColor, targetColor, targetColor)
                );

                if ((now - end).TotalSeconds <= 6 && !isPlayingClassOverAnimation)
                {
                    isPlayingClassOverAnimation = true;
                    (Resources["Storyboard_ClassOver"] as Storyboard)?.Begin();
                }
            }
            // 打了预备铃
            else if (now >= (begin - new TimeSpan(0, 2, 0)) && now < begin)
            {
                tb.Foreground = new SolidColorBrush(Color.FromRgb(0x03, 0xFC, 0xA5));

                if ((now - (begin - new TimeSpan(0, 2, 0))).TotalSeconds <= 6)
                {
                    Instances.TopmostEffectsWindow!.PlayPrepareClassAlert();
                }
            }
            // 课间, 即将打预备铃
            else if (now >= (begin - new TimeSpan(0, 10, 0)) && now < begin)
                tb.Foreground = new SolidColorBrush(Color.FromRgb(0x8C, 0xC6, 0xED));

            WrapPanel_ClassesContainer.Children.Add(tb);
        }

        classProgress = inClass ? classProgress : null;

        if (isPlayingClassOverAnimation == false && classProgress is null)
        {
            Container_ClassProgress.Visibility = Visibility.Hidden;
        }
        else
        {
            Container_ClassProgress.Opacity = 1;
            Container_ClassProgress.Visibility = Visibility.Visible;
            TextBlock_ClassesProgress.Text = $"{classProgress:f2} %";
            

            if (isPlayingClassOverAnimation)
                TextBlock_ClassesProgress.Text = "";
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

    public void SetWebViewVisibility(bool visible)
    {
        MainWebView.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
    }

    public WebView2 GetWebView() => MainWebView;

    private void Animation_ScrollInClassOver_Completed(object sender, EventArgs e)
    {
        switch (Container_ClassOverAnimation.HorizontalAlignment)
        {
            case HorizontalAlignment.Left:
                Container_ClassOverAnimation.HorizontalAlignment = HorizontalAlignment.Right;
                break;
            case HorizontalAlignment.Center:
                Container_ClassOverAnimation.HorizontalAlignment = HorizontalAlignment.Stretch;
                break;
            case HorizontalAlignment.Right:
                Container_ClassOverAnimation.HorizontalAlignment = HorizontalAlignment.Left;
                break;
            case HorizontalAlignment.Stretch:
                Container_ClassOverAnimation.HorizontalAlignment = HorizontalAlignment.Center;
                break;
        }
    }

    private void Animation_ScrollInClassBegin_Completed(object sender, EventArgs e)
    {
        switch (Container_ClassBeginAnimation.HorizontalAlignment)
        {
            case HorizontalAlignment.Left:
                Container_ClassBeginAnimation.HorizontalAlignment = HorizontalAlignment.Right;
                break;
            case HorizontalAlignment.Center:
                Container_ClassBeginAnimation.HorizontalAlignment = HorizontalAlignment.Stretch;
                break;
            case HorizontalAlignment.Right:
                Container_ClassBeginAnimation.HorizontalAlignment = HorizontalAlignment.Left;
                break;
            case HorizontalAlignment.Stretch:
                Container_ClassBeginAnimation.HorizontalAlignment = HorizontalAlignment.Center;
                break;
        }
    }

    internal void PlayClassBeginAnimation()
    {
        isPlayingClassBeginAnimation = true;
        (Resources["Storyboard_ClassBegin"] as Storyboard)!.Begin();
    }

    internal void PlayClassOverAnimation()
    {
        Container_ClassProgress.Opacity = 1;
        Container_ClassProgress.Visibility = Visibility.Visible;

        isPlayingClassOverAnimation = true;
        (Resources["Storyboard_ClassOver"] as Storyboard)?.Begin();
    }
}
