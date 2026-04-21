using System.Runtime.InteropServices;
using Svg;

namespace BonzoBuddo.Helpers;

internal static class TrayIconLoader
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool DestroyIcon(IntPtr handle);

    public static Icon LoadAiHelperTrayIcon()
    {
        try
        {
            var baseDir = AppContext.BaseDirectory;
            var iconPath = Path.Combine(baseDir, "ai-icon.svg");
            if (!File.Exists(iconPath))
                return SystemIcons.Application;

            var svg = SvgDocument.Open(iconPath);
            using var bmp = svg.Draw(64, 64);
            var hIcon = bmp.GetHicon();
            try
            {
                using var tempIcon = Icon.FromHandle(hIcon);
                return (Icon)tempIcon.Clone();
            }
            finally
            {
                DestroyIcon(hIcon);
            }
        }
        catch
        {
            return SystemIcons.Application;
        }
    }
}