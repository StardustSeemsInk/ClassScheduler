using ClassScheduler.WPF.Data;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ClassScheduler.WPF.Utils;

internal static class NotifyIconManager
{
    private static ToolStripMenuItem BuildItem(
        string text = "",
        Action<object?, EventArgs>? onClick = null,
        Action<object?, EventArgs>? onCheckChanged = null,
        bool? checkOnClick = null,
        CheckState? checkState = null,
        List<ToolStripMenuItem>? subItems = null)
    {
        var toolStripMenuItem = new ToolStripMenuItem()
        {
            Text = text,
            CheckOnClick = checkOnClick ?? false,
            CheckState = checkState ?? CheckState.Unchecked,
        };

        toolStripMenuItem.Click += (x, y) => onClick?.Invoke(x, y);
        toolStripMenuItem.CheckedChanged += (x, y) => onCheckChanged?.Invoke(x, y);

        if (subItems is not null)
            foreach (var item in subItems)
                toolStripMenuItem.DropDownItems.Add(item);

        return toolStripMenuItem;
    }

    private static ContextMenuStrip BuildContextMenu()
    {
        var contextMenuStrip = new ContextMenuStrip();
        contextMenuStrip.Items.Add(BuildItem(
            "调试",
            subItems: new()
            {
                BuildItem(
                    "播放预备铃动画",
                    onClick: (_, _) => Instances.TopmostEffectsWindow!.PlayPrepareClassAlert()
                ),
                BuildItem(
                    "播放上课动画",
                    onClick: (_, _) => Instances.ScheduleWindow!.PlayClassBeginAnimation()
                ),
                BuildItem(
                    "播放下课动画",
                    onClick: (_, _) => Instances.ScheduleWindow!.PlayClassOverAnimation()
                ),
                BuildItem(
                    "立即刷新天气数据",
                    onClick: (_, _) => Instances.ScheduleWindow!.RefreshWeather()
                ),
            }
        ));
        contextMenuStrip.Items.Add(new ToolStripSeparator());
        contextMenuStrip.Items.Add(BuildItem(
            "置顶窗口",
            subItems: new()
            {
                BuildItem(
                    "显示时钟",
                    checkOnClick: true,
                    checkState: (Instances.AppConfig!.TopmostEffectsSettings.IsDateTimeVisible ?? true)
                        ? CheckState.Checked : CheckState.Unchecked,
                    onCheckChanged: (sender, _) =>
                        Instances.TopmostEffectsWindow!.SetDateTimeVisibility(
                            (sender as ToolStripMenuItem)!.Checked
                        )
                ),
            }
        ));
        contextMenuStrip.Items.Add(BuildItem(
            "浏览器",
            subItems: new()
            {
                BuildItem(
                    "显示背景浏览器",
                    checkOnClick: true,
                    onCheckChanged: (sender, _) =>
                        Instances.ScheduleWindow!.SetWebViewVisibility(
                            (sender as ToolStripMenuItem)!.Checked
                        )
                ),
                BuildItem(
                    "刷新网页",
                    onClick: (_, _) => Instances.ScheduleWindow!.GetWebView().Reload()
                )
            }
        ));
        contextMenuStrip.Items.Add(BuildItem(
            "壁纸",
            subItems: new()
            {
                BuildItem(
                    "下一张",
                    onClick: (_, _) => Instances.MainWindow!.NextWallPaper()
                )
            }
        ));
        contextMenuStrip.Items.Add(BuildItem(
            "退出",
            onClick: (_, _) =>
            {
                Instances.NotifyIcon!.Dispose();
                Instances.MainWindow!.Exit();
            }
        ));
        return contextMenuStrip;
    }

    internal static void BuildNotifyIcon()
    {
        Instances.NotifyIcon = new NotifyIcon()
        {
            Icon = new System.Drawing.Icon($"{GlobalData.RootPath}/Assets/icon.ico"),
            Visible = true,
            Text = "ClassScheduler",
            ContextMenuStrip = BuildContextMenu(),
        };
        Instances.NotifyIcon.MouseClick += (_, e) =>
        {
            if (e.Button != MouseButtons.Left)
                return;

            Instances.MainWindow!.ComplexShow();
        };
    }

    internal static void Rebuild()
    {
        Instances.NotifyIcon!.Dispose();
        BuildNotifyIcon();
    }
}
