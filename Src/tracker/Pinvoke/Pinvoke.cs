using System.Runtime.InteropServices;

namespace tracker.Pinvoke;

public class Pinvoke
{
    [DllImport("kernel32.dll")]
    public static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}


