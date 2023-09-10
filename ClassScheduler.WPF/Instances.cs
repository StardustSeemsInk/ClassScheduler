using ClassScheduler.WPF.Views;

namespace ClassScheduler.WPF;

internal static class Instances
{
    internal static void Init()
    {
        MainWindow = new();

        Controller = new();
    }

    internal static MainWindow? MainWindow { get; set; }

    internal static ScheduleWindow? DeskWindow { get; set; }

    internal static Controller? Controller { get; set; }
}
