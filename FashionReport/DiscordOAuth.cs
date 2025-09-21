using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using Dalamud.Configuration;
using Dalamud.Plugin;


namespace FashionReportCalculator;

internal static class DiscordOAuth
{
    private static readonly HttpClient _httpClient = new();
    private const string ProxyBaseUrl = "https://ScarBot.ddns.net:443/DiscordOAuthInformation";
    internal static DiscordConfiguration _discordConfiguration = DiscordConfiguration.Load();

    internal static async Task RequestLoginUrl(int timeoutSeconds = 60)
    {
        try
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(16);
            string state = Convert.ToHexString(bytes);
            string oauthUrl = $"https://discord.com/oauth2/authorize?client_id=1058753294400491531&response_type=code&redirect_uri=https%3A%2F%2FScarBot.ddns.net%3A443%2FDiscordOAuthInformation&scope=identify+connections+email+guilds&state={state}";
            if (OperatingSystem.IsWindows())
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(oauthUrl) { UseShellExecute = true });
            else if (OperatingSystem.IsMacOS())
                System.Diagnostics.Process.Start("open", oauthUrl);
            else if (OperatingSystem.IsLinux())
                System.Diagnostics.Process.Start("xdg-open", oauthUrl);
            string? token = await PollProxyForTokenAsync(state, timeoutSeconds);
            if (!string.IsNullOrEmpty(token))
                OnTokenReceived(token);
            else
                LOG.Debug("DiscordOAuth: Token not received within timeout.");
        }
        catch (Exception ex) { LOG.Error($"DiscordOAuth.RequestLoginAndGetToken exception: {ex}"); }
    }

    private static async Task<string?> PollProxyForTokenAsync(string state, int timeoutSeconds)
    {
        DateTime endTime = DateTime.UtcNow.AddSeconds(timeoutSeconds);
        while (DateTime.UtcNow < endTime)
        {
            try
            {
                using HttpRequestMessage request = new(HttpMethod.Get, $"{ProxyBaseUrl}/token?state={state}");
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("token", out JsonElement tokenElement))
                    {
                        string token = tokenElement.GetString() ?? string.Empty;
                        if (!string.IsNullOrEmpty(token))
                            return token;
                    }
                }
            }
            catch (Exception ex) { LOG.Error($"DiscordOAuth.PollProxyForTokenAsync exception: {ex}"); }
            await Task.Delay(1000);
        }
        return null;
    }

    internal static void OnTokenReceived(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            LOG.Debug("DiscordOAuth: Received token is empty!");
            return;
        }
        _discordConfiguration.Discord = token;
        _discordConfiguration.Save();
        LOG.Debug($"DiscordOAuth: Token received and saved: {token}");
    }

    internal static async Task<bool> VerifyToken()
    {
        if (string.IsNullOrEmpty(_discordConfiguration.Discord)) return false;
        try
        {
            using HttpRequestMessage request = new(HttpMethod.Get, $"{ProxyBaseUrl}/verify");
            request.Headers.Add("Authorization", $"Bearer {_discordConfiguration.Discord}");
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            LOG.Error($"DiscordOAuth.VerifyToken exception: {ex}");
            return false;
        }
    }
}

public class DiscordConfiguration
{
    private static readonly string ConfigFolder = Path.Combine(SERVICES.Interface.GetPluginConfigDirectory(), "DiscordPlugin");
    private static readonly string ConfigPath = Path.Combine(ConfigFolder, "discord_config.json");
    public string Discord { get; set; } = string.Empty;
    public DiscordConfiguration() => EnsureFolderExists();

    private static void EnsureFolderExists() => Directory.CreateDirectory(ConfigFolder);

    public void Save()
    {
        EnsureFolderExists();
        string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigPath, json);
    }

    public static DiscordConfiguration Load()
    {
        EnsureFolderExists();
        if (!File.Exists(ConfigPath)) return new DiscordConfiguration();
        string json = File.ReadAllText(ConfigPath);
        return JsonSerializer.Deserialize<DiscordConfiguration>(json) ?? new DiscordConfiguration();
    }
}