using BonzoBuddo.Helpers;

namespace BonzoBuddo.Forms;

public partial class OpenClawConfigForm : Form
{
    public OpenClawConfigForm()
    {
        InitializeComponent();
        LoadConfig();
    }

    private void LoadConfig()
    {
        var config = AppConfigStore.GetCurrent();
        urlTextBox.Text = config.OpenClawUrl;
        tokenTextBox.Text = config.OpenClawToken;
        passwordTextBox.Text = config.OpenClawPassword;
        modelTextBox.Text = config.OpenClawModel;
        telegramTokenTextBox.Text = config.TelegramBotToken;
        telegramEnabledCheckBox.Checked = config.TelegramBotEnabled;
        // TTS API disabled for now
        // ttsUrlTextBox.Text = config.TtsApiUrl;
        // ttsVoiceModelIdTextBox.Text = config.TtsVoiceModelId;
        // ttsTargetTextBox.Text = config.TtsTarget;
        statusLabel.Text = "Settings loaded.";
    }

    private void saveButton_Click(object sender, EventArgs e)
    {
        var url = urlTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(url))
        {
            statusLabel.Text = "OpenClaw URL is required.";
            return;
        }

        var config = new AppConfig
        {
            OpenClawUrl = url,
            OpenClawToken = tokenTextBox.Text.Trim(),
            OpenClawPassword = passwordTextBox.Text,
            OpenClawModel = string.IsNullOrWhiteSpace(modelTextBox.Text) ? "openclaw" : modelTextBox.Text.Trim(),
            TelegramBotToken = telegramTokenTextBox.Text.Trim(),
            TelegramBotEnabled = telegramEnabledCheckBox.Checked
            // TTS API disabled for now
            // TtsApiUrl = ttsUrlTextBox.Text.Trim(),
            // TtsVoiceModelId = ttsVoiceModelIdTextBox.Text.Trim(),
            // TtsTarget = string.IsNullOrWhiteSpace(ttsTargetTextBox.Text) ? "wav" : ttsTargetTextBox.Text.Trim()
        };

        AppConfigStore.Save(config);

        // Update Telegram bot based on new config
        if (config.TelegramBotEnabled && !string.IsNullOrWhiteSpace(config.TelegramBotToken))
        {
            TelegramBotHelper.StopPolling();
            TelegramBotHelper.StartPolling();
            statusLabel.Text = "Saved. Telegram bot started.";
        }
        else
        {
            TelegramBotHelper.StopPolling();
            statusLabel.Text = "Saved. Telegram bot stopped.";
        }
    }

    private void resetButton_Click(object sender, EventArgs e)
    {
        var defaults = new AppConfig();
        AppConfigStore.Save(defaults);
        LoadConfig();
        statusLabel.Text = "Reset to defaults.";
    }
}
