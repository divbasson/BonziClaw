using BonzoBuddo.Helpers;

namespace BonzoBuddo.Forms;

/// <summary>
///     A persistent chat window that lets the user type questions and have
///     Bonzi speak the answers from the local OpenClaw AI gateway.
/// </summary>
public partial class AskAIForm : Form
{
    private readonly BonziHelper _helper;

    public AskAIForm(BonziHelper helper)
    {
        InitializeComponent();
        _helper = helper;

        // Allow pressing Enter (without Shift) to submit the question
        inputText.KeyDown += (s, e) =>
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                askButton_Click(s, EventArgs.Empty);
            }
        };

        statusLabel.Text = "Ask me anything!";
    }

    private async void askButton_Click(object sender, EventArgs e)
    {
        var question = inputText.Text.Trim();
        if (string.IsNullOrEmpty(question))
            return;

        askButton.Enabled = false;
        statusLabel.Text = "Thinking...";

        try
        {
            var response = await OpenClawHelper.SendMessageAsync(question);
            var cleaned = OpenClawHelper.StripMarkdown(response);

            _helper.Play("Think");
            _helper.Speak(cleaned);

            inputText.Clear();
            statusLabel.Text = "Ask me anything!";
        }
        catch (HttpRequestException ex)
        {
            statusLabel.Text = $"Couldn't reach OpenClaw: {ex.Message}";
        }
        catch (Exception ex)
        {
            statusLabel.Text = $"Error: {ex.Message}";
        }
        finally
        {
            askButton.Enabled = true;
            inputText.Focus();
        }
    }

    private void closeButton_Click(object sender, EventArgs e) => Close();
}
