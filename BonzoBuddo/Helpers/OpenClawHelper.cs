using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Web;

namespace BonzoBuddo.Helpers;

/// <summary>
///     Helper for communicating with a local OpenClaw gateway instance via its
///     OpenAI-compatible HTTP API.
/// </summary>
public static class OpenClawHelper
{
    private static readonly HttpClient _client = new()
    {
        Timeout = TimeSpan.FromSeconds(60)
    };

    private sealed class AuthVariant
    {
        public string Name { get; init; } = string.Empty;
        public AuthenticationHeaderValue? Authorization { get; init; }
        public Dictionary<string, string> ExtraHeaders { get; init; } = new();
    }

    /// <summary>
    ///     Sends a user message to the OpenClaw gateway and returns the assistant's reply text.
    /// </summary>
    /// <param name="message">The user message to send.</param>
    /// <returns>The assistant reply text.</returns>
    public static async Task<string> SendMessageAsync(string message)
    {
        var config = AppConfigStore.GetCurrent();
        var configuredUrl = string.IsNullOrWhiteSpace(config.OpenClawUrl) ? Keys.OpenClawUrl() : config.OpenClawUrl;
        var httpUrl = NormalizeGatewayHttpBaseUrl(configuredUrl);
        var wsUrl = NormalizeGatewayWebSocketBaseUrl(configuredUrl);
        var token = string.IsNullOrWhiteSpace(config.OpenClawToken) ? Keys.OpenClawToken() : config.OpenClawToken;
        var password = config.OpenClawPassword;
        var model = string.IsNullOrWhiteSpace(config.OpenClawModel) ? "openclaw" : config.OpenClawModel;

        try
        {
            // Try WebSocket first (no CORS issues, more reliable for local connections)
            return await SendMessageViaWebSocketAsync(wsUrl, token, password, message);
        }
        catch
        {
            // Fall back to HTTP if WebSocket fails
            return await SendMessageViaHttpAsync(httpUrl, token, password, model, message);
        }
    }

    private static async Task<string> SendMessageViaHttpAsync(string url, string token, string password, string model,
        string message)
    {
        var endpoints = new[]
        {
            $"{url}/v1/chat/completions",
            $"{url}/chat/completions"
        };

        var authVariants = BuildAuthVariants(token, password);
        var diagnostics = new List<string>();

        foreach (var endpoint in endpoints)
        foreach (var variant in authVariants)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            if (variant.Authorization is not null)
                request.Headers.Authorization = variant.Authorization;

            foreach (var kvp in variant.ExtraHeaders)
                request.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);

            var body = new JsonObject
            {
                ["model"] = model,
                ["messages"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["role"] = "user",
                        ["content"] = message
                    }
                },
                ["stream"] = false
            };

            request.Content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json");

            HttpResponseMessage? response = null;
            try
            {
                response = await _client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var parsed = JsonNode.Parse(responseBody);
                    return parsed?["choices"]?[0]?["message"]?["content"]?.ToString()
                           ?? "I couldn't get a response from OpenClaw.";
                }

                diagnostics.Add(
                    $"{(int)response.StatusCode} {response.ReasonPhrase} via {variant.Name} @ {endpoint}: {TruncateForError(responseBody)}");
            }
            catch (Exception ex)
            {
                diagnostics.Add($"Request failed via {variant.Name} @ {endpoint}: {ex.Message}");
            }
            finally
            {
                response?.Dispose();
            }
        }

        throw new HttpRequestException("OpenClaw HTTP request failed. " + string.Join(" | ", diagnostics));
    }

    // Paths to probe when connecting to the OpenClaw WebSocket endpoint.
    private static readonly string[] _wsPaths =
        new[] { "", "/ws", "/api/ws", "/operator", "/api/operator", "/socket", "/gateway" };

    private static async Task<string> SendMessageViaWebSocketAsync(string url, string token, string password,
        string message)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(90));

        // Try each candidate path until one connects (HTTP 101) instead of 404.
        ClientWebSocket? socket = null;
        var lastEx = new Exception("No WebSocket paths succeeded.");
        foreach (var path in _wsPaths)
        {
            socket?.Dispose();
            socket = new ClientWebSocket();
            // Mimic browser origin so CORS policies pass.
            socket.Options.SetRequestHeader("Origin", url.Replace("wss://", "https://").Replace("ws://", "http://"));
            try
            {
                var candidate = new Uri(url.TrimEnd('/') + path);
                await socket.ConnectAsync(candidate, cts.Token);
                break; // connected
            }
            catch (Exception ex)
            {
                lastEx = ex;
                // Continue to next path
            }
        }

        if (socket is null || socket.State != WebSocketState.Open)
        {
            socket?.Dispose();
            throw new HttpRequestException("OpenClaw WebSocket unreachable: " + lastEx.Message);
        }

        using var _ = socket;

        var connectParams = new JsonObject
        {
            ["minProtocol"] = 3,
            ["maxProtocol"] = 3,
            ["client"] = new JsonObject
            {
                ["id"] = "openclaw-control-ui",
                ["version"] = "bonzobuddo",
                ["platform"] = Environment.OSVersion.Platform.ToString().ToLowerInvariant(),
                ["mode"] = "webchat",
                ["instanceId"] = Guid.NewGuid().ToString()
            },
            ["role"] = "operator",
            ["scopes"] = new JsonArray("operator.admin", "operator.read", "operator.write", "operator.approvals",
                "operator.pairing"),
            ["caps"] = new JsonArray("tool-events")
        };

        var auth = new JsonObject();
        if (!string.IsNullOrWhiteSpace(token))
            auth["token"] = token;
        if (!string.IsNullOrWhiteSpace(password))
            auth["password"] = password;
        if (auth.Count > 0)
            connectParams["auth"] = auth;

        var connectResponse = await SendRpcAsync(socket, "connect", connectParams, cts.Token);
        if (!connectResponse["ok"]?.GetValue<bool>() ?? true)
        {
            var error = connectResponse["error"]?["message"]?.ToString() ?? "OpenClaw connect failed";
            throw new HttpRequestException(error);
        }

        var sessionsResponse = await SendRpcAsync(socket, "sessions.list", new JsonObject(), cts.Token);
        var sessionKey = sessionsResponse["payload"]?["sessions"]?[0]?["key"]?.ToString() ?? "main";

        var runId = Guid.NewGuid().ToString();
        var sendResponse = await SendRpcAsync(socket, "chat.send", new JsonObject
        {
            ["sessionKey"] = sessionKey,
            ["message"] = message,
            ["deliver"] = false,
            ["idempotencyKey"] = runId
        }, cts.Token);

        if (!sendResponse["ok"]?.GetValue<bool>() ?? true)
        {
            var error = sendResponse["error"]?["message"]?.ToString() ?? "chat.send failed";
            throw new HttpRequestException(error);
        }

        var finalText = await WaitForFinalChatMessageAsync(socket, sessionKey, runId, cts.Token);
        return string.IsNullOrWhiteSpace(finalText) ? "I couldn't get a response from OpenClaw." : finalText;
    }

    private static async Task<JsonNode> SendRpcAsync(ClientWebSocket socket, string method, JsonObject? parameters,
        CancellationToken ct)
    {
        var id = Guid.NewGuid().ToString("N");
        var request = new JsonObject
        {
            ["type"] = "req",
            ["id"] = id,
            ["method"] = method,
            ["params"] = parameters ?? new JsonObject()
        };

        await SendJsonAsync(socket, request.ToJsonString(), ct);

        while (true)
        {
            var raw = await ReceiveTextAsync(socket, ct);
            var parsed = JsonNode.Parse(raw);
            if (parsed?["type"]?.ToString() == "res" && parsed?["id"]?.ToString() == id)
                return parsed;
        }
    }

    private static async Task<string> WaitForFinalChatMessageAsync(ClientWebSocket socket, string sessionKey,
        string runId, CancellationToken ct)
    {
        string? latestDelta = null;

        while (true)
        {
            var raw = await ReceiveTextAsync(socket, ct);
            var parsed = JsonNode.Parse(raw);
            if (parsed?["type"]?.ToString() != "event" || parsed?["event"]?.ToString() != "chat")
                continue;

            var payload = parsed["payload"];
            if (payload is null)
                continue;

            if (!string.Equals(payload["sessionKey"]?.ToString(), sessionKey, StringComparison.Ordinal))
                continue;

            if (!string.Equals(payload["runId"]?.ToString(), runId, StringComparison.Ordinal))
                continue;

            var state = payload["state"]?.ToString();
            var message = payload["message"];

            if (string.Equals(state, "delta", StringComparison.Ordinal))
            {
                var delta = ExtractMessageText(message);
                if (!string.IsNullOrWhiteSpace(delta))
                    latestDelta = delta;
                continue;
            }

            if (string.Equals(state, "final", StringComparison.Ordinal) ||
                string.Equals(state, "aborted", StringComparison.Ordinal))
            {
                var final = ExtractMessageText(message);
                return !string.IsNullOrWhiteSpace(final) ? final : latestDelta ?? string.Empty;
            }

            if (string.Equals(state, "error", StringComparison.Ordinal))
                throw new HttpRequestException(payload["errorMessage"]?.ToString() ?? "OpenClaw chat error");
        }
    }

    private static string? ExtractMessageText(JsonNode? message)
    {
        if (message is null)
            return null;

        if (message["content"] is JsonArray contentArray)
        {
            var chunks = contentArray
                .Select(c => c?["type"]?.ToString() == "text" ? c?["text"]?.ToString() : null)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();
            if (chunks.Length > 0)
                return string.Join("\n", chunks);
        }

        var directText = message["text"]?.ToString();
        if (!string.IsNullOrWhiteSpace(directText))
            return directText;

        return message.ToString();
    }

    private static async Task SendJsonAsync(ClientWebSocket socket, string json, CancellationToken ct)
    {
        var buffer = Encoding.UTF8.GetBytes(json);
        await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, ct);
    }

    private static async Task<string> ReceiveTextAsync(ClientWebSocket socket, CancellationToken ct)
    {
        var buffer = new byte[8192];
        using var ms = new MemoryStream();

        while (true)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), ct);
            if (result.MessageType == WebSocketMessageType.Close)
                throw new HttpRequestException("OpenClaw socket closed before response completed.");

            ms.Write(buffer, 0, result.Count);
            if (result.EndOfMessage)
                break;
        }

        return Encoding.UTF8.GetString(ms.ToArray());
    }

    private static List<AuthVariant> BuildAuthVariants(string token, string password)
    {
        var variants = new List<AuthVariant>();

        if (!string.IsNullOrWhiteSpace(token))
        {
            variants.Add(new AuthVariant
            {
                Name = "bearer-token",
                Authorization = new AuthenticationHeaderValue("Bearer", token)
            });
        }

        if (!string.IsNullOrWhiteSpace(password))
        {
            var basicOpenClaw = Convert.ToBase64String(Encoding.UTF8.GetBytes($"openclaw:{password}"));
            var basicBlankUser = Convert.ToBase64String(Encoding.UTF8.GetBytes($":{password}"));

            variants.Add(new AuthVariant
            {
                Name = "password-header",
                ExtraHeaders = new Dictionary<string, string>
                {
                    ["x-openclaw-password"] = password,
                    ["x-openclaw-basic"] = basicOpenClaw
                }
            });

            variants.Add(new AuthVariant
            {
                Name = "basic-openclaw-user",
                Authorization = new AuthenticationHeaderValue("Basic", basicOpenClaw)
            });

            variants.Add(new AuthVariant
            {
                Name = "basic-blank-user",
                Authorization = new AuthenticationHeaderValue("Basic", basicBlankUser)
            });

            variants.Add(new AuthVariant
            {
                Name = "bearer-password",
                Authorization = new AuthenticationHeaderValue("Bearer", password)
            });
        }

        if (variants.Count == 0)
            variants.Add(new AuthVariant { Name = "no-auth" });

        return variants;
    }

    private static string NormalizeGatewayHttpBaseUrl(string rawUrl)
    {
        if (!Uri.TryCreate(rawUrl.Trim(), UriKind.Absolute, out var uri))
            return rawUrl.Trim().TrimEnd('/');

        var scheme = uri.Scheme switch
        {
            "ws" => "http",
            "wss" => "https",
            _ => uri.Scheme
        };

        var builder = new UriBuilder(uri)
        {
            Scheme = scheme,
            Path = string.Empty,
            Query = string.Empty,
            Fragment = string.Empty
        };

        return builder.Uri.ToString().TrimEnd('/');
    }

    private static string NormalizeGatewayWebSocketBaseUrl(string rawUrl)
    {
        if (!Uri.TryCreate(rawUrl.Trim(), UriKind.Absolute, out var uri))
            return rawUrl.Trim().TrimEnd('/');

        var scheme = uri.Scheme switch
        {
            "http" => "ws",
            "https" => "wss",
            _ => uri.Scheme
        };

        var builder = new UriBuilder(uri)
        {
            Scheme = scheme,
            Path = string.Empty,
            Query = string.Empty,
            Fragment = string.Empty
        };

        return builder.Uri.ToString().TrimEnd('/');
    }

    private static string TruncateForError(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "<empty response body>";

        var singleLine = text.Replace("\r", " ").Replace("\n", " ").Trim();
        return singleLine.Length <= 240 ? singleLine : singleLine[..240] + "...";
    }

    /// <summary>
    ///     Strips common Markdown syntax so Bonzi's TTS engine doesn't read
    ///     symbols like asterisks and backticks aloud.
    /// </summary>
    public static string StripMarkdown(string text)
    {
        // Code blocks → brief placeholder
        text = Regex.Replace(text, @"```[\s\S]*?```", " code block ");
        // Inline code → bare text
        text = Regex.Replace(text, @"`([^`]+)`", "$1");
        // ATX headings
        text = Regex.Replace(text, @"^#{1,6}\s+", string.Empty, RegexOptions.Multiline);
        // Bold / italic
        text = Regex.Replace(text, @"\*{1,3}([^*\n]+)\*{1,3}", "$1");
        text = Regex.Replace(text, @"_{1,3}([^_\n]+)_{1,3}", "$1");
        // Markdown links
        text = Regex.Replace(text, @"\[([^\]]+)\]\([^\)]+\)", "$1");
        // Horizontal rules
        text = Regex.Replace(text, @"^[-*_]{3,}\s*$", string.Empty, RegexOptions.Multiline);
        // Collapse multiple blank lines / newlines into a pause
        text = Regex.Replace(text, @"\r?\n{2,}", ". ");
        text = Regex.Replace(text, @"\r?\n", " ");
        // Collapse repeated spaces
        text = Regex.Replace(text, @"  +", " ");
        return text.Trim();
    }
}
