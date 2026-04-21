using System.Text.Json;

namespace BonzoBuddo.Helpers;

public sealed class AppConfig
{
    public string OpenClawUrl { get; set; } = string.Empty;
    public string OpenClawToken { get; set; } = string.Empty;
    public string OpenClawPassword { get; set; } = string.Empty;
    public string OpenClawModel { get; set; } = "openclaw";
    public string TelegramBotToken { get; set; } = string.Empty;
    public bool TelegramBotEnabled { get; set; } = false;
    // TTS API disabled for now
    // public string TtsApiUrl { get; set; } = string.Empty;
    // public string TtsVoiceModelId { get; set; } = string.Empty;
    // public string TtsTarget { get; set; } = "wav";
}

public static class AppConfigStore
{
    private static readonly object _sync = new();
    private static AppConfig _current = new();

    private static readonly string ConfigDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "BonzoBuddo");

    private static readonly string ConfigPath = Path.Combine(ConfigDirectory, "appconfig.json");

    public static AppConfig GetCurrent()
    {
        lock (_sync)
            return new AppConfig
            {
                OpenClawUrl = _current.OpenClawUrl,
                OpenClawToken = _current.OpenClawToken,
                OpenClawPassword = _current.OpenClawPassword,
                OpenClawModel = _current.OpenClawModel,
                TelegramBotToken = _current.TelegramBotToken,
                TelegramBotEnabled = _current.TelegramBotEnabled
                // TTS API disabled for now
                // TtsApiUrl = _current.TtsApiUrl,
                // TtsVoiceModelId = _current.TtsVoiceModelId,
                // TtsTarget = _current.TtsTarget
            };
    }

    public static void Load()
    {
        lock (_sync)
        {
            if (!File.Exists(ConfigPath))
            {
                _current = new AppConfig();
                SaveInternal(_current);
                return;
            }

            try
            {
                var json = File.ReadAllText(ConfigPath);
                var parsed = JsonSerializer.Deserialize<AppConfig>(json);
                _current = ApplyEnvironmentFallbacks(parsed ?? new AppConfig());
            }
            catch
            {
                _current = ApplyEnvironmentFallbacks(new AppConfig());
                SaveInternal(_current);
            }
        }
    }

    public static void Save(AppConfig config)
    {
        lock (_sync)
        {
            _current = config;
            SaveInternal(_current);
        }
    }

    private static void SaveInternal(AppConfig config)
    {
        Directory.CreateDirectory(ConfigDirectory);
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigPath, json);
    }

    private static AppConfig ApplyEnvironmentFallbacks(AppConfig config)
    {
        config.OpenClawUrl = Pick(config.OpenClawUrl, Environment.GetEnvironmentVariable("BONZO_OPENCLAW_URL"),
            "ws://127.0.0.1:18789");
        config.OpenClawToken = Pick(config.OpenClawToken, Environment.GetEnvironmentVariable("BONZO_OPENCLAW_TOKEN"),
            string.Empty);
        config.OpenClawPassword = Pick(config.OpenClawPassword,
            Environment.GetEnvironmentVariable("BONZO_OPENCLAW_PASSWORD"), string.Empty);
        config.OpenClawModel = Pick(config.OpenClawModel, Environment.GetEnvironmentVariable("BONZO_OPENCLAW_MODEL"),
            "openclaw");
        config.TelegramBotToken = Pick(config.TelegramBotToken,
            Environment.GetEnvironmentVariable("BONZO_TELEGRAM_BOT_TOKEN"), string.Empty);

        var telegramEnabledEnv = Environment.GetEnvironmentVariable("BONZO_TELEGRAM_ENABLED");
        if (!string.IsNullOrWhiteSpace(telegramEnabledEnv) && bool.TryParse(telegramEnabledEnv, out var enabled))
            config.TelegramBotEnabled = enabled;

        return config;
    }

    private static string Pick(params string?[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();
        }

        return string.Empty;
    }
}
