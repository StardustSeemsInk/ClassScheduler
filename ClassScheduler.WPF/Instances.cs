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

        AppConfig = "./Data/AppConfig.json".LoadAsAppConfig();
        AppConfig ??= new();
        AppConfig.Save();

        Classes = "./Data/Classes.json".LoadAsClasses();
        Classes ??= new();
        Classes.Sort();
        Classes.Save();

        MainWindow = new();

        ScheduleWindow = new();

        TopmostEffectsWindow = new();
        TopmostEffectsWindow.Show();

        Controller = new();
    }

    internal static MainWindow? MainWindow { get; set; }

    internal static ScheduleWindow? ScheduleWindow { get; set; }

    internal static TopmostEffectsWindow? TopmostEffectsWindow { get; set; }

    internal static Controller? Controller { get; set; }

    internal static AppConfig? AppConfig { get; set; }

    internal static Classes? Classes { get; set; }

    internal static NotifyIcon? NotifyIcon { get; set; }
}
