using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassScheduler.WPF;

internal static class Instances
{
    internal static void Init()
    {
        MainWindow = new();

        Controller = new(MainWindow);
    }

    internal static MainWindow? MainWindow { get; set; }

    internal static Controller? Controller { get; set; }
}
