using ClassScheduler.WPF.Data;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ClassScheduler.WPF.Views;

public partial class MainWindow : Window
{
    public ScheduleWindow? deskWindow = null;

    public MainWindow()
    {
        InitializeComponent();

#if DEBUG
        new ScheduleWindow().Show();
#endif

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

    public void Pause()
    {
        Instances.DeskWindow?.Close();
        Instances.DeskWindow = null;
    }

    public void Exit()
    {
        Application.Current.Shutdown();
        Environment.Exit(0);
    }
}
