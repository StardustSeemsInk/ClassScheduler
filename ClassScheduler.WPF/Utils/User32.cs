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
    public static partial nint FindWindowEx(
        nint hwndParent,
        nint hwndChildAfter,
        string className,
        string? winName);

    [LibraryImport(user32dllPath)]
    internal static partial nint SendMessageTimeout(
        nint hwnd,
        uint msg,
        nint wParam,
        nint lParam,
        uint fuFlage,
        uint timeout,
        nint result);
}

public static class User32Extensions
{
    public static void MoveToBottom(this Window wpfWindow)
    {
        var programIntPtr = User32.FindWindow("Progman", null);

        if (programIntPtr == nint.Zero) // Ensure we found the `progman`
            return;

        var result = nint.Zero;

        User32.SendMessageTimeout(
            programIntPtr,
            0x052C,
            nint.Zero,
            nint.Zero,
            0x0000,
            1000,
            nint.Zero
        );

        // Foreach top layer windows
        User32.EnumWindows((hwnd, lParam) =>
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
                User32.ShowWindow(tempHwnd, 0);
            }
            return true;
        }, nint.Zero);

        User32.SetParent(
            new System.Windows.Interop.WindowInteropHelper(wpfWindow).Handle,
            programIntPtr
        );
    }
}
