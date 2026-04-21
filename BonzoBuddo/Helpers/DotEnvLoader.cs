namespace BonzoBuddo.Helpers;

internal static class DotEnvLoader
{
    public static void Load()
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, ".env"),
            Path.Combine(AppContext.BaseDirectory, "..", ".env"),
            Path.Combine(AppContext.BaseDirectory, "..", "..", ".env"),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".env")
        }
        .Select(Path.GetFullPath)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .Where(File.Exists)
        .ToArray();

        foreach (var path in candidates)
        {
            foreach (var rawLine in File.ReadAllLines(path))
            {
                var line = rawLine.Trim();
                if (line.Length == 0 || line.StartsWith("#", StringComparison.Ordinal))
                    continue;

                var splitIdx = line.IndexOf('=');
                if (splitIdx <= 0)
                    continue;

                var key = line[..splitIdx].Trim();
                var value = line[(splitIdx + 1)..].Trim();

                if (value.StartsWith("\"", StringComparison.Ordinal) &&
                    value.EndsWith("\"", StringComparison.Ordinal) && value.Length >= 2)
                    value = value[1..^1];

                if (string.IsNullOrWhiteSpace(key))
                    continue;

                if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key)))
                    Environment.SetEnvironmentVariable(key, value);
            }
        }
    }
}