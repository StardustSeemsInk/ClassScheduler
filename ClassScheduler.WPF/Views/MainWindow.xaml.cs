using ClassScheduler.WPF.Data;
using ClassScheduler.WPF.Models;
using ClassScheduler.WPF.Utils;
using System;
using System.ComponentModel;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ClassScheduler.WPF.Views;

public partial class MainWindow : Window
{
    private readonly Timer wallPaperTimer;

    public ScheduleWindow? deskWindow = null;

    public MainWindow()
    {
        InitializeComponent();

        Events.OnSetRootPath += () =>
        {
            var icon = new BitmapImage(
                new(
                    $"{GlobalData.RootPath}/Assets/icon.ico",
                    UriKind.Absolute
                )
            );

            Icon = icon;

            NotifyIconManager.BuildNotifyIcon();
        };

        Loaded += MainWindow_Loaded;

        wallPaperTimer = new()
        {
            Interval = 10 * 60 * 1000
        };
        wallPaperTimer.Elapsed += (_, _) => NextWallPaper();
        wallPaperTimer.Start();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        e.Cancel = true;
        ComplexHide();

        base.OnClosing(e);
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Instances.ScheduleWindow?.Show();

        ComplexHide();

        RefreshClasses();

        RefreshCurrentWallpaperStyle();

        NextWallPaper();
    }

    public void ComplexShow()
    {
        Show();
        ShowInTaskbar = true;
    }

    public void ComplexHide()
    {
        Hide();
        ShowInTaskbar = false;
    }

    public void Pause()
    {
        Instances.ScheduleWindow?.Close();
        Instances.ScheduleWindow = null;
    }

    public void Exit()
    {
        Application.Current.Shutdown();
        Environment.Exit(0);
    }

    private void Button_Add_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var name = TextBox_ClassName.Text;
            var begin = DateTime.Parse(DatePicker_BeginTime.Text);
            var end = DateTime.Parse(DatePicker_EndTime.Text);
            var weekDay = int.Parse(TextBox_WeekDay.Text);

            var classVar = new ClassModel()
            {
                Name = name,
                BeginTime = begin,
                EndTime = end,
                WeekDay = (byte)(1 << (weekDay - 1))
            };

            Instances.Classes!.ClassesList.Add(classVar);
            Instances.Classes!.Sort();
            Instances.Classes!.Save();

            RefreshClasses();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"{ex.Message}\n{ex.StackTrace}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }

    private void Button_1_Click(object sender, RoutedEventArgs e) => TextBox_WeekDay.Text = "1";

    private void Button_2_Click(object sender, RoutedEventArgs e) => TextBox_WeekDay.Text = "2";

    private void Button_3_Click(object sender, RoutedEventArgs e) => TextBox_WeekDay.Text = "3";

    private void Button_4_Click(object sender, RoutedEventArgs e) => TextBox_WeekDay.Text = "4";

    private void Button_5_Click(object sender, RoutedEventArgs e) => TextBox_WeekDay.Text = "5";

    private void Button_6_Click(object sender, RoutedEventArgs e) => TextBox_WeekDay.Text = "6";

    private void Button_7_Click(object sender, RoutedEventArgs e) => TextBox_WeekDay.Text = "7";

    private void RefreshClasses()
    {
        ListBox_Classes.Items.Clear();

        var index = -1;

        foreach (var classVar in Instances.Classes!.ClassesList)
        {
            var currentIndex = index + 1;
            index++;

            var textBlock = new TextBlock()
            {
                Text = classVar.ToString(),
            };
            textBlock.MouseRightButtonDown += (_, _) =>
            {
                Instances.Classes.ClassesList.RemoveAt(currentIndex);
                Instances.Classes.Save();
                RefreshClasses();
            };

            ListBox_Classes.Items.Add(textBlock);
        }
    }

    private void ListBox_Classes_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedIndex = ListBox_Classes.SelectedIndex;

        if (selectedIndex == -1) return;

        var selection = Instances.Classes!.ClassesList[selectedIndex];

        TextBox_ClassName.Text = selection.Name;
        TextBox_WeekDay.Text = selection.DayOfWeek.ToString();
        DatePicker_BeginTime.Text = selection.BeginTime?.ToString("HH:mm");
        DatePicker_EndTime.Text = selection.EndTime?.ToString("HH:mm");
    }

    private void RefreshWallpapers()
    {
        if (Instances.AppConfig!.WallPaperSettings.WallPapersPath is null) return;

        ListBox_WallPapers.Items.Clear();

        var path = Instances.AppConfig!.WallPaperSettings.WallPapersPath;
        path = Path.GetFullPath(path);

        TextBox_WallPapersPath.Text = path;

        var dirInfo = new DirectoryInfo(path);
        foreach (var file in dirInfo.GetFiles())
        {
            ListBox_WallPapers.Items.Add(Path.GetFileName(file.FullName));
        }
    }

    private void Button_SetWallPapersPath_Click(object sender, RoutedEventArgs e)
    {
        using var dialog = new System.Windows.Forms.FolderBrowserDialog();
        System.Windows.Forms.DialogResult result = dialog.ShowDialog();

        if (dialog.SelectedPath is not null && Directory.Exists(dialog.SelectedPath))
        {
            Instances.AppConfig!.WallPaperSettings.WallPapersPath = dialog.SelectedPath;
            Instances.AppConfig!.Save();

            RefreshWallpapers();
        }
    }

    private void Button_Refresh_Click(object sender, RoutedEventArgs e) => RefreshWallpapers();

    internal void NextWallPaper()
    {
        var index = Instances.AppConfig!.WallPaperSettings.CurrentWallPaperIndex + 1;
        var path = Instances.AppConfig!.WallPaperSettings.WallPapersPath;

        if (path is null) return;

        var wallPapers = new DirectoryInfo(path).GetFiles();
        var count = wallPapers.Length;

        if (index >= count)
            index = 0;

        try
        {
            wallPapers[index].FullName.SetWallPaper(
                Instances.AppConfig.WallPaperSettings.WallPaperStyle ?? WallPaperStyle.Stretched
            );
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"{ex.Message}\n{ex.StackTrace}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        Instances.AppConfig!.WallPaperSettings.CurrentWallPaperIndex = index;
        Instances.AppConfig!.Save();
    }

    private void RefreshCurrentWallpaperStyle()
    {
        var text = "";

        switch (Instances.AppConfig!.WallPaperSettings.WallPaperStyle)
        {
            case WallPaperStyle.Tiled:
                text = "平铺";
                break;
            case WallPaperStyle.Centered:
                text = "居中";
                break;
            case WallPaperStyle.Stretched:
                text = "拉伸";
                break;
        }

        TextBox_CurrentWallpaperStyle.Text = text;
    }

    private void Button_SetWallpaperTiled_Click(object sender, RoutedEventArgs e)
    {
        Instances.AppConfig!.WallPaperSettings.WallPaperStyle = WallPaperStyle.Tiled;
        Instances.AppConfig!.Save();

        RefreshCurrentWallpaperStyle();
    }

    private void Button_SetWallpaperCentered_Click(object sender, RoutedEventArgs e)
    {
        Instances.AppConfig!.WallPaperSettings.WallPaperStyle = WallPaperStyle.Centered;
        Instances.AppConfig!.Save();

        RefreshCurrentWallpaperStyle();
    }

    private void Button_SetWallpaperStretched_Click(object sender, RoutedEventArgs e)
    {
        Instances.AppConfig!.WallPaperSettings.WallPaperStyle = WallPaperStyle.Stretched;
        Instances.AppConfig!.Save();

        RefreshCurrentWallpaperStyle();
    }
}
