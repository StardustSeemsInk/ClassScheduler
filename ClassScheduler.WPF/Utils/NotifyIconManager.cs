using ClassScheduler.WPF.Data;
using System.Windows.Forms;

namespace ClassScheduler.WPF.Utils;

internal static class NotifyIconManager
{
    private static ToolStripMenuItem BuildItem_Quit()
    {
        var toolStripMenuItem_Quit = new ToolStripMenuItem()
        {
            Text = "退出",
        };
        toolStripMenuItem_Quit.Click += (_, _) =>
        {
            Instances.NotifyIcon!.Dispose();
            Instances.MainWindow!.Exit();
        };

        return toolStripMenuItem_Quit;
    }

    private static ToolStripMenuItem BuildItem_NextWallpaper()
    {
        var toolStripMenuItem_NextWallpaper = new ToolStripMenuItem()
        {
            Text = "下一张壁纸",
        };
        toolStripMenuItem_NextWallpaper.Click += (_, _) => Instances.MainWindow!.NextWallPaper();

        return toolStripMenuItem_NextWallpaper;
    }

    private static ToolStripMenuItem BuildItem_ShowWebBrowser()
    {
        var toolStripMenuItem_ShowWebBrowser = new ToolStripMenuItem()
        {
            Text = "显示背景浏览器",
            CheckOnClick = true,
            CheckState = CheckState.Unchecked,
        };
        toolStripMenuItem_ShowWebBrowser.CheckedChanged += (_, _) =>
        {
            Instances.ScheduleWindow!.SetWebViewVisibility(
                toolStripMenuItem_ShowWebBrowser.Checked
            );
        };

        return toolStripMenuItem_ShowWebBrowser;
    }

    private static ToolStripMenuItem BuildItem_PlayPrepareClassAlert()
    {
        var toolStripMenuItem_PlayPrepareClassAlert = new ToolStripMenuItem()
        {
            Text = "播放预备铃动画",
        };
        toolStripMenuItem_PlayPrepareClassAlert.Click += (_, _)
            => Instances.TopmostEffectsWindow!.PlayPrepareClassAlert();

        return toolStripMenuItem_PlayPrepareClassAlert;
    }

    private static ToolStripMenuItem BuildItem_PlayClassBeginAnimation()
    {
        var toolStripMenuItem_PlayClassBeginAnimation = new ToolStripMenuItem()
        {
            Text = "播放上课动画",
        };
        toolStripMenuItem_PlayClassBeginAnimation.Click += (_, _)
            => Instances.ScheduleWindow!.PlayClassBeginAnimation();

        return toolStripMenuItem_PlayClassBeginAnimation;
    }

    private static ToolStripMenuItem BuildItem_PlayClassOverAnimation()
    {
        var toolStripMenuItem_PlayClassOverAnimation = new ToolStripMenuItem()
        {
            Text = "播放下课动画",
        };
        toolStripMenuItem_PlayClassOverAnimation.Click += (_, _)
            => Instances.ScheduleWindow!.PlayClassOverAnimation();

        return toolStripMenuItem_PlayClassOverAnimation;
    }

    private static ToolStripMenuItem BuildContextMenu_Debug()
    {
        var toolStripMenuItem_Debug = new ToolStripMenuItem()
        {
            Text = "调试",
        };
        toolStripMenuItem_Debug.DropDownItems.Add(BuildItem_PlayPrepareClassAlert());
        toolStripMenuItem_Debug.DropDownItems.Add(BuildItem_PlayClassBeginAnimation());
        toolStripMenuItem_Debug.DropDownItems.Add(BuildItem_PlayClassOverAnimation());

        return toolStripMenuItem_Debug;
    }

    private static ContextMenuStrip BuildContextMenu_Main()
    {
        var contextMenuStrip = new ContextMenuStrip();
        contextMenuStrip.Items.Add(BuildContextMenu_Debug());
        contextMenuStrip.Items.Add(new ToolStripSeparator());
        contextMenuStrip.Items.Add(BuildItem_ShowWebBrowser());
        contextMenuStrip.Items.Add(BuildItem_NextWallpaper());
        contextMenuStrip.Items.Add(BuildItem_Quit());

        return contextMenuStrip;
    }

    internal static void BuildNotifyIcon()
    {
        Instances.NotifyIcon = new NotifyIcon()
        {
            Icon = new System.Drawing.Icon($"{GlobalData.RootPath}/Assets/icon.ico"),
            Visible = true,
            Text = "ClassScheduler",
            ContextMenuStrip = BuildContextMenu_Main(),
        };
        Instances.NotifyIcon.MouseClick += (_, e) =>
        {
            if (e.Button != MouseButtons.Left)
                return;

            Instances.MainWindow!.ComplexShow();
        };
    }
}
