using System.Text.Json.Nodes;

namespace BonzoBuddo.Helpers;

public static class TelegramBotHelper
{
    /// <summary>
    ///     Optional callback invoked on the UI thread when Bonzi should speak an OpenClaw response.
    ///     Set this before calling StartPolling() so Telegram messages also animate Bonzi.
    /// </summary>
    public static Action<string>? OnBonziSpeak { get; set; }
    private static readonly HttpClient Client = new() { Timeout = TimeSpan.FromSeconds(30) };
    private static long _lastUpdateId = 0;
    private static CancellationTokenSource? _pollCts;
    private static Task? _pollTask;

    /// <summary>
    ///     Start polling for Telegram messages in the background.
    /// </summary>
    public static void StartPolling()
    {
        var config = AppConfigStore.GetCurrent();
        if (string.IsNullOrWhiteSpace(config.TelegramBotToken))
            return;

        if (_pollCts != null)
            return; // Already running

        _pollCts = new CancellationTokenSource();
        _pollTask = Task.Run(() => PollLoop(config.TelegramBotToken, _pollCts.Token));
    }

    /// <summary>
    ///     Stop the polling task.
    /// </summary>
    public static void StopPolling()
    {
        if (_pollCts != null)
        {
            _pollCts.Cancel();
            try
            {
                _pollTask?.Wait(TimeSpan.FromSeconds(5));
            }
            catch
            {
                // Ignore timeout or cancellation exceptions
            }

            _pollCts.Dispose();
            _pollCts = null;
            _pollTask = null;
        }
    }

    private static async Task PollLoop(string botToken, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await PollUpdatesAsync(botToken, ct).ConfigureAwait(false);
                await Task.Delay(1000, ct).ConfigureAwait(false); // Poll every second
            }
            catch (OperationCanceledException)
            {
                // Expected when stopping
                break;
            }
            catch
            {
                // Swallow errors and retry
                await Task.Delay(2000, ct).ConfigureAwait(false);
            }
        }
    }

    private static async Task PollUpdatesAsync(string botToken, CancellationToken ct)
    {
        var url = $"https://api.telegram.org/bot{botToken}/getUpdates?offset={_lastUpdateId + 1}&timeout=10";

        try
        {
            var response = await Client.GetAsync(url, ct).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                return;

            var json = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            var data = JsonNode.Parse(json);

            var updates = data?["result"]?.AsArray();
            if (updates is null || updates.Count == 0)
                return;

            foreach (var update in updates)
            {
                try
                {
                    _lastUpdateId = update?["update_id"]?.GetValue<long>() ?? 0;

                    var message = update?["message"];
                    if (message is null)
                        continue;

                    var chatId = message["chat"]?["id"]?.GetValue<long>();
                    var text = message["text"]?.GetValue<string>();

                    if (!chatId.HasValue || string.IsNullOrWhiteSpace(text))
                        continue;

                    // Route message through OpenClaw (or fallback)
                    var messageResponse = await GetResponseAsync(text).ConfigureAwait(false);

                    // Send response back to Telegram
                    await SendMessageAsync(botToken, chatId.Value, messageResponse, ct).ConfigureAwait(false);
                }
                catch
                {
                    // Swallow individual update errors
                }
            }
        }
        catch
        {
            // Swallow poll errors
        }
    }

    private static async Task<string> GetResponseAsync(string userMessage)
    {
        try
        {
            var response = await OpenClawHelper.SendMessageAsync(userMessage).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(response))
            {
                // Make Bonzi speak the response (fires the registered UI callback).
                var speak = OnBonziSpeak;
                if (speak is not null)
                {
                    var cleaned = OpenClawHelper.StripMarkdown(response);
                    speak.Invoke(cleaned);
                }
                return response;
            }
        }
        catch
        {
            // Fall through to default response
        }

        // Fallback response
        return "Sorry, I couldn't process that right now. Try again later.";
    }

    private static async Task SendMessageAsync(string botToken, long chatId, string text, CancellationToken ct)
    {
        var url = $"https://api.telegram.org/bot{botToken}/sendMessage";
        var json = new JsonObject
        {
            ["chat_id"] = chatId,
            ["text"] = text
        };

        try
        {
            var content = new StringContent(json.ToJsonString(), System.Text.Encoding.UTF8, "application/json");
            await Client.PostAsync(url, content, ct).ConfigureAwait(false);
        }
        catch
        {
            // Swallow send errors
        }
    }
}
