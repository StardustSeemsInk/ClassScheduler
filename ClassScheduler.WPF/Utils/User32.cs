using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ClassScheduler.WPF.Utils;

[Flags]
public enum SendMessageTimeoutFlags : uint
{
    SMTO_NORMAL = 0x0,
    SMTO_BLOCK = 0x1,
    SMTO_ABORTIFHUNG = 0x2,
    SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
    SMTO_ERRORONEXIT = 0x20
}

public enum WallPaperStyle : int
{
    Tiled,
    Centered,
    Stretched
}

public partial class User32
{
    private const string user32dllPath = "user32.dll";

    [DllImport(user32dllPath, EntryPoint = "SetParent")]
    internal static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport(user32dllPath)]
    internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndlnsertAfter, int X, int Y, int cx, int cy, uint Flags);

    [DllImport(user32dllPath, CharSet = CharSet.Ansi, EntryPoint = "FindWindowA", ExactSpelling = false, SetLastError = true)]
    internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport(user32dllPath, EntryPoint = "FindWindowEx")]
    internal static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);

    [DllImport(user32dllPath, SetLastError = true, CharSet = CharSet.Auto)]
    internal static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, UIntPtr wParam, IntPtr lParam, SendMessageTimeoutFlags fuFlags, uint uTimeout, out UIntPtr lpdwResult);

    internal delegate bool EnumWindowsCallback(IntPtr hwnd, int lParam);
    [DllImport(user32dllPath)]
    internal static extern int EnumWindows(EnumWindowsCallback callPtr, int lParam);

    [DllImport(user32dllPath, EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
    internal static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

    [DllImport(user32dllPath)]
    internal static extern int GetWindowRect(IntPtr hwnd, out Rect lpRect);

    [LibraryImport(user32dllPath)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool EnumWindows(EnumWindowsProc proc, nint lParam);
    internal delegate bool EnumWindowsProc(nint hwnd, nint lParam);

    //[LibraryImport(user32dllPath)]
    //internal static partial nint SendMessageTimeout(
    //    nint hwnd,
    //    uint msg,
    //    nint wParam,
    //    nint lParam,
    //    uint fuFlage,
    //    uint timeout,
    //    nint result);

    [DllImport(user32dllPath, SetLastError = true)]
    internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    internal const uint SWP_NOSIZE = 0x0001;
    internal const uint SWP_NOMOVE = 0x0002;
    internal const uint SWP_NOACTIVATE = 0x0010;
    internal const int GWL_HWNDPARENT = -8;

    internal static readonly IntPtr HWND_BOTTOM = new(1);

    internal const int SPI_SETDESKWALLPAPER = 20;
    internal const int SPIF_UPDATEINIFILE = 0x01;
    internal const int SPIF_SENDWININICHANGE = 0x02;

    [DllImport(user32dllPath, CharSet = CharSet.Auto)]
    internal static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
}

public static class User32Extensions
{
    public static void MoveToBottom(this Window wpfWindow)
    {
        var programIntPtr = User32.FindWindow("Progman", null);

        if (programIntPtr == nint.Zero) // Ensure we found the `progman`
            return;

        var result = nint.Zero;

        _ = User32.SendMessageTimeout(
            programIntPtr,
            0x52C,
            nuint.Zero,
            nint.Zero,
            0x0000,
            2000,
            out var temp_result
        );

        // Foreach top layer windows
        _ = User32.EnumWindows((hwnd, lParam) =>
        {
            // Find `WorkerW` which includes handle of `SHELLDLL_DefView`
            if (User32.FindWindowEx(
                    hwnd,
                    nint.Zero,
                    "SHELLDLL_DefView",
                    null
                ) != nint.Zero)
            {
                // Find the next `WorkerW` window after current one
                var tempHwnd = User32.FindWindowEx(
                    nint.Zero,
                    hwnd,
                    "WorkerW",
                    null
                );

                // Hide this window
                _ = User32.ShowWindow(tempHwnd, 0);
            }
            return true;
        }, nint.Zero);

        _ = User32.SetParent(
            new WindowInteropHelper(wpfWindow).Handle,
            programIntPtr
        );
    }

    public static void SetBottom(this Window window)
    {
        IntPtr hWnd = new WindowInteropHelper(window).Handle;
        _ = User32.SetWindowPos(
            hWnd,
            User32.HWND_BOTTOM,
            0,
            0,
            0,
            0,
            User32.SWP_NOSIZE | User32.SWP_NOMOVE | User32.SWP_NOACTIVATE
        );
    }

    public static void BottomMost(this Window window)
    {
        var hWnd = new WindowInteropHelper(window).Handle;
        var hWndProgMan = User32.FindWindow("Progman", "Program Manager");
        _ = User32.SetParent(hWnd, hWndProgMan);
        var handle = new WindowInteropHelper(Application.Current.MainWindow).Handle;

        IntPtr hprog = User32.FindWindowEx(
            User32.FindWindowEx(
                User32.FindWindow("Progman", "Program Manager"),
                IntPtr.Zero, "SHELLDLL_DefView", ""
            ),
            IntPtr.Zero, "SysListView32", "FolderView"
        );
        _ = User32.SetWindowLong(handle, User32.GWL_HWNDPARENT, hprog);
    }

    public static void SetWallPaper(this string path, WallPaperStyle style)
    {
        var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

        if (style == WallPaperStyle.Stretched)
        {
            key?.SetValue(@"WallpaperStyle", 2.ToString());
            key?.SetValue(@"TileWallpaper", 0.ToString());
        }

        if (style == WallPaperStyle.Centered)
        {
            key?.SetValue(@"WallpaperStyle", 1.ToString());
            key?.SetValue(@"TileWallpaper", 0.ToString());
        }

        if (style == WallPaperStyle.Tiled)
        {
            key?.SetValue(@"WallpaperStyle", 1.ToString());
            key?.SetValue(@"TileWallpaper", 1.ToString());
        }

        User32.SystemParametersInfo(
            User32.SPI_SETDESKWALLPAPER,
            0,
            Path.GetFullPath(path),
            User32.SPIF_UPDATEINIFILE | User32.SPIF_SENDWININICHANGE
        );
    }
}
