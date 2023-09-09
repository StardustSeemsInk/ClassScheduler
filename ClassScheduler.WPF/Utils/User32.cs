using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace ClassScheduler.WPF.Utils;

public partial class User32
{
    private const string user32dllPath = "user32.dll";

    [LibraryImport(user32dllPath)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool ShowWindow(nint hwnd, int nCmdShow);

    [LibraryImport(user32dllPath, StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint FindWindow(string className, string? winName);

    [LibraryImport(user32dllPath)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool EnumWindows(EnumWindowsProc proc, nint lParam);
    public delegate bool EnumWindowsProc(nint hwnd, nint lParam);

    [LibraryImport(user32dllPath)]
    internal static partial nint SetParent(nint hwnd, nint parentHwnd);

    [LibraryImport(user32dllPath, StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint FindWindowEx(nint hwndParent, nint hwndChildAfter, string className, string? winName);

    [LibraryImport(user32dllPath)]
    internal static partial nint SendMessageTimeout(nint hwnd, uint msg, nint wParam, nint lParam, uint fuFlage, uint timeout, nint result);

    public static void SetDeskTop(Window wpfWindow)
    {
        var programIntPtr = FindWindow("Progman", null);

        Console.WriteLine(programIntPtr);

        // 窗口句柄有效
        if (programIntPtr != nint.Zero)
        {
            var result = nint.Zero;

            SendMessageTimeout(
                programIntPtr,
                0x052C,
                nint.Zero,
                nint.Zero,
                0x0000,
                1000,
                nint.Zero
            );

            // 遍历顶级窗口
            EnumWindows((hwnd, lParam) =>
            {
                // 找到包含 SHELLDLL_DefView 这个窗口句柄的 WorkerW
                if (FindWindowEx(
                        hwnd,
                        nint.Zero,
                        "SHELLDLL_DefView",
                        null
                    ) != nint.Zero)
                {
                    // 找到当前 WorkerW 窗口的，后一个 WorkerW 窗口。 
                    var tempHwnd = FindWindowEx(
                        nint.Zero,
                        hwnd,
                        "WorkerW",
                        null
                    );

                    // 隐藏这个窗口
                    ShowWindow(tempHwnd, 0);
                }
                return true;
            }, nint.Zero);
        }

        SetParent(
            new System.Windows.Interop.WindowInteropHelper(wpfWindow).Handle,
            programIntPtr
        );

    }
}
