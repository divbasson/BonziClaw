namespace BonzoBuddo.Helpers;

internal static class Keys
{
    public static string NinjaKey() => "YOUR_API_NINJA_KEY";
    public static string NewsKey() => "YOUR_NEWSCATCHER_KEY";
    public static string OpenWeatherKey() => "YOUR_OPENWEATHER_KEY";

    // OpenClaw gateway defaults are resolved from environment variables.
    public static string OpenClawUrl() => Environment.GetEnvironmentVariable("BONZO_OPENCLAW_URL") ??
                                          "ws://127.0.0.1:18789";
    public static string OpenClawToken() => Environment.GetEnvironmentVariable("BONZO_OPENCLAW_TOKEN") ??
                                            string.Empty;
}
