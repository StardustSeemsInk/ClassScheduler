using ClassScheduler.WPF.Data;
using ClassScheduler.WPF.Models;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ClassScheduler.WPF.Views;

public partial class MainWindow : Window
{
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

            var toolStripMenuItem_Quit = new System.Windows.Forms.ToolStripMenuItem()
            {
                Text = "退出",
            };
            toolStripMenuItem_Quit.Click += (_, _) =>
            {
                Instances.NotifyIcon!.Dispose();
                Exit();
            };

            var contextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            contextMenuStrip.Items.Add(toolStripMenuItem_Quit);

            Instances.NotifyIcon = new System.Windows.Forms.NotifyIcon()
            {
                Icon = new System.Drawing.Icon($"{GlobalData.RootPath}/Assets/icon.ico"),
                Visible = true,
                Text = "ClassScheduler",
                ContextMenuStrip = contextMenuStrip,
            };
            Instances.NotifyIcon.MouseClick += (_, e) =>
            {
                if (e.Button != System.Windows.Forms.MouseButtons.Left)
                    return;

                ComplexShow();
            };
        };

        Loaded += MainWindow_Loaded;
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
            Instances.Classes!.Save("./Data/Classes.json");

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
}
