using BonzoBuddo.Forms;
using BonzoBuddo.Helpers;

namespace BonzoBuddo;

internal sealed class TrayApplicationContext : ApplicationContext
{
    private readonly NotifyIcon _trayIcon;
    private readonly BonziBuddyControlPanel _panel;

    public TrayApplicationContext()
    {
        _panel = new BonziBuddyControlPanel();

        var menu = new ContextMenuStrip();
        menu.Items.Add("Open Control Panel", null, (_, _) => ShowControlPanel());
        menu.Items.Add("Configuration", null, (_, _) => OpenConfiguration());
        menu.Items.Add("Ask AI", null, (_, _) => _panel.OpenAskAiForm());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Exit", null, (_, _) => ExitApplication());

        _trayIcon = new NotifyIcon
        {
            Text = "BonziBuddy",
            Icon = TrayIconLoader.LoadAiHelperTrayIcon(),
            Visible = true,
            ContextMenuStrip = menu
        };

        _trayIcon.DoubleClick += (_, _) => ShowControlPanel();

        // Wire Telegram responses to Bonzi speech
        TelegramBotHelper.OnBonziSpeak = text => _panel.SpeakFromOpenClaw(text);

        // Start Telegram bot if enabled
        var config = AppConfigStore.GetCurrent();
        if (config.TelegramBotEnabled && !string.IsNullOrWhiteSpace(config.TelegramBotToken))
        {
            TelegramBotHelper.StartPolling();
        }
    }

    private void ShowControlPanel() => _panel.ShowFromTray();

    private void OpenConfiguration()
    {
        var config = new OpenClawConfigForm();
        config.Show();
        config.BringToFront();
    }

    private void ExitApplication()
    {
        TelegramBotHelper.StopPolling();
        _trayIcon.Visible = false;
        _trayIcon.Dispose();
        _panel.Close();
        ExitThread();
    }
}
