using ClassScheduler.WPF.Data;
using ClassScheduler.WPF.Views;
using System.IO;
using System.Windows.Forms;

namespace ClassScheduler.WPF;

internal static class Instances
{
    internal static void Init()
    {
        if (!Directory.Exists(Path.GetFullPath("./Data/")))
            Directory.CreateDirectory(Path.GetFullPath("./Data/"));

        Classes = "./Data/Classes.json".Load();
        Classes ??= new();
        Classes.Sort();
        Classes.Save("./Data/Classes.json");

        MainWindow = new();

        ScheduleWindow = new();

        Controller = new();
    }

    internal static MainWindow? MainWindow { get; set; }

    internal static ScheduleWindow? ScheduleWindow { get; set; }

    internal static Controller? Controller { get; set; }

    internal static Classes? Classes { get; set; }

    internal static NotifyIcon? NotifyIcon { get; set; }
}
