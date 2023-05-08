using KitX.Contract.CSharp;
using System;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Windows;

namespace ClassScheduler.Runner;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// 启动事件
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">启动参数</param>
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        try
        {
            LoadPlugin("./ClassScheduler.WPF.dll");
        }
        catch (Exception o)
        {
            MessageBox.Show(o.Message, "Loader Error",
                MessageBoxButton.OK, MessageBoxImage.Error);

            Console.WriteLine(o.Message);

            Environment.Exit(1);
        }
    }

    /// <summary>
    /// 加载插件
    /// </summary>
    /// <param name="path">插件路径</param>
    private static void LoadPlugin(string path)
    {
        if (File.Exists(path))
        {
            var dirPath = Path.GetDirectoryName(path);
            var fileName = Path.GetFileName(path);

            if (dirPath is null || fileName is null)
                throw new Exception("Directory path is null!");

            var catalog = new DirectoryCatalog(dirPath, fileName);

            var container = new CompositionContainer(catalog);

            var sub = container.GetExportedValues<IIdentityInterface>();

            foreach (var item in sub)
            {
                var controller = item.GetController();

                controller.SetRootPath(Path.GetFullPath("."));

                controller.Start();

                break;
            }
        }
    }
}
