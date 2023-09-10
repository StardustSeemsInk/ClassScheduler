using System;
using System.Timers;
using System.Windows;

namespace ClassScheduler.WPF.Views;

public partial class ScheduleWindow : Window
{
    private Timer _timer;

    public ScheduleWindow()
    {
        InitializeComponent();

        //this.MoveToBottom();

        UpdateDatas();

        _timer = new Timer()
        {
            Interval = 10 * 1000,
        };

        _timer.Elapsed += (_, _) => UpdateDatas();
    }

    public void UpdateDatas()
    {
        var now = DateTime.Now;
        var high_school_entrance_day = DateTime.Parse("06-06");

        if (high_school_entrance_day < now)
            high_school_entrance_day = high_school_entrance_day.AddYears(1);

        TextBlock_Time.Text = now.ToString("HH:mm");
        TextBlock_Date.Text = now.ToString("MM 月 dd 日");
        TextBlock_WeekDay.Text = now.ToString("dddd");
        TextBlock_DaysLeft.Text = Math.Floor((high_school_entrance_day - now).TotalDays).ToString();
    }
}
