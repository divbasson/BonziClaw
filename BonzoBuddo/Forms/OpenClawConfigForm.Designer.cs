namespace BonzoBuddo.Forms
{
    partial class OpenClawConfigForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            urlLabel = new Label();
            urlTextBox = new TextBox();
            tokenLabel = new Label();
            tokenTextBox = new TextBox();
            passwordLabel = new Label();
            passwordTextBox = new TextBox();
            modelLabel = new Label();
            modelTextBox = new TextBox();
            telegramTokenLabel = new Label();
            telegramTokenTextBox = new TextBox();
            telegramEnabledLabel = new Label();
            telegramEnabledCheckBox = new CheckBox();
            ttsUrlLabel = new Label();
            ttsUrlTextBox = new TextBox();
            ttsVoiceModelIdLabel = new Label();
            ttsVoiceModelIdTextBox = new TextBox();
            ttsTargetLabel = new Label();
            ttsTargetTextBox = new TextBox();
            saveButton = new Button();
            resetButton = new Button();
            statusLabel = new Label();
            SuspendLayout();
            //
            // urlLabel
            //
            urlLabel.AutoSize = true;
            urlLabel.Location = new Point(12, 16);
            urlLabel.Name = "urlLabel";
            urlLabel.Size = new Size(193, 24);
            urlLabel.TabIndex = 0;
            urlLabel.Text = "OpenClaw Gateway URL";
            //
            // urlTextBox
            //
            urlTextBox.Location = new Point(12, 43);
            urlTextBox.Name = "urlTextBox";
            urlTextBox.Size = new Size(540, 31);
            urlTextBox.TabIndex = 1;
            //
            // tokenLabel
            //
            tokenLabel.AutoSize = true;
            tokenLabel.Location = new Point(12, 90);
            tokenLabel.Name = "tokenLabel";
            tokenLabel.Size = new Size(183, 24);
            tokenLabel.TabIndex = 2;
            tokenLabel.Text = "Bearer Token (optional)";
            //
            // tokenTextBox
            //
            tokenTextBox.Location = new Point(12, 117);
            tokenTextBox.Name = "tokenTextBox";
            tokenTextBox.Size = new Size(540, 31);
            tokenTextBox.TabIndex = 3;
            tokenTextBox.UseSystemPasswordChar = true;
            //
            // passwordLabel
            //
            passwordLabel.AutoSize = true;
            passwordLabel.Location = new Point(12, 164);
            passwordLabel.Name = "passwordLabel";
            passwordLabel.Size = new Size(200, 24);
            passwordLabel.TabIndex = 4;
            passwordLabel.Text = "Gateway Password (opt.)";
            //
            // passwordTextBox
            //
            passwordTextBox.Location = new Point(12, 191);
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.Size = new Size(540, 31);
            passwordTextBox.TabIndex = 5;
            passwordTextBox.UseSystemPasswordChar = true;
            //
            // modelLabel
            //
            modelLabel.AutoSize = true;
            modelLabel.Location = new Point(12, 238);
            modelLabel.Name = "modelLabel";
            modelLabel.Size = new Size(132, 24);
            modelLabel.TabIndex = 6;
            modelLabel.Text = "Model (OpenAI)";
            //
            // modelTextBox
            //
            modelTextBox.Location = new Point(12, 265);
            modelTextBox.Name = "modelTextBox";
            modelTextBox.Size = new Size(540, 31);
            modelTextBox.TabIndex = 7;
            //
            // telegramTokenLabel
            //
            telegramTokenLabel.AutoSize = true;
            telegramTokenLabel.Location = new Point(12, 312);
            telegramTokenLabel.Name = "telegramTokenLabel";
            telegramTokenLabel.Size = new Size(150, 24);
            telegramTokenLabel.TabIndex = 8;
            telegramTokenLabel.Text = "Telegram Bot Token";
            //
            // telegramTokenTextBox
            //
            telegramTokenTextBox.Location = new Point(12, 339);
            telegramTokenTextBox.Name = "telegramTokenTextBox";
            telegramTokenTextBox.Size = new Size(540, 31);
            telegramTokenTextBox.TabIndex = 9;
            telegramTokenTextBox.UseSystemPasswordChar = true;
            //
            // telegramEnabledLabel
            //
            telegramEnabledLabel.AutoSize = true;
            telegramEnabledLabel.Location = new Point(12, 386);
            telegramEnabledLabel.Name = "telegramEnabledLabel";
            telegramEnabledLabel.Size = new Size(164, 24);
            telegramEnabledLabel.TabIndex = 10;
            telegramEnabledLabel.Text = "Enable Telegram Bot";
            //
            // telegramEnabledCheckBox
            //
            telegramEnabledCheckBox.AutoSize = true;
            telegramEnabledCheckBox.Location = new Point(12, 413);
            telegramEnabledCheckBox.Name = "telegramEnabledCheckBox";
            telegramEnabledCheckBox.Size = new Size(22, 24);
            telegramEnabledCheckBox.TabIndex = 11;
            telegramEnabledCheckBox.UseVisualStyleBackColor = true;
            //
            // ttsUrlLabel
            //
            ttsUrlLabel.AutoSize = true;
            ttsUrlLabel.Location = new Point(12, 460);
            ttsUrlLabel.Name = "ttsUrlLabel";
            ttsUrlLabel.Size = new Size(102, 24);
            ttsUrlLabel.TabIndex = 12;
            ttsUrlLabel.Text = "TTS API URL";
            //
            // ttsUrlTextBox
            //
            ttsUrlTextBox.Location = new Point(12, 487);
            ttsUrlTextBox.Name = "ttsUrlTextBox";
            ttsUrlTextBox.Size = new Size(540, 31);
            ttsUrlTextBox.TabIndex = 13;
            //
            // ttsVoiceModelIdLabel
            //
            ttsVoiceModelIdLabel.AutoSize = true;
            ttsVoiceModelIdLabel.Location = new Point(12, 534);
            ttsVoiceModelIdLabel.Name = "ttsVoiceModelIdLabel";
            ttsVoiceModelIdLabel.Size = new Size(206, 24);
            ttsVoiceModelIdLabel.TabIndex = 14;
            ttsVoiceModelIdLabel.Text = "TTS Voice Model ID (optional)";
            //
            // ttsVoiceModelIdTextBox
            //
            ttsVoiceModelIdTextBox.Location = new Point(12, 561);
            ttsVoiceModelIdTextBox.Name = "ttsVoiceModelIdTextBox";
            ttsVoiceModelIdTextBox.Size = new Size(540, 31);
            ttsVoiceModelIdTextBox.TabIndex = 15;
            //
            // ttsTargetLabel
            //
            ttsTargetLabel.AutoSize = true;
            ttsTargetLabel.Location = new Point(12, 608);
            ttsTargetLabel.Name = "ttsTargetLabel";
            ttsTargetLabel.Size = new Size(174, 24);
            ttsTargetLabel.TabIndex = 16;
            ttsTargetLabel.Text = "TTS Target (ex: wav)";
            //
            // ttsTargetTextBox
            //
            ttsTargetTextBox.Location = new Point(12, 635);
            ttsTargetTextBox.Name = "ttsTargetTextBox";
            ttsTargetTextBox.Size = new Size(540, 31);
            ttsTargetTextBox.TabIndex = 17;
            //
            // saveButton
            //
            saveButton.Location = new Point(12, 684);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(264, 47);
            saveButton.TabIndex = 18;
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            //
            // resetButton
            //
            resetButton.Location = new Point(288, 684);
            resetButton.Name = "resetButton";
            resetButton.Size = new Size(264, 47);
            resetButton.TabIndex = 19;
            resetButton.Text = "Reset to Defaults";
            resetButton.UseVisualStyleBackColor = true;
            resetButton.Click += resetButton_Click;
            //
            // statusLabel
            //
            statusLabel.AutoSize = true;
            statusLabel.ForeColor = Color.DimGray;
            statusLabel.Location = new Point(12, 744);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(0, 24);
            statusLabel.TabIndex = 20;
            //
            // OpenClawConfigForm
            //
            AutoScaleDimensions = new SizeF(10F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(564, 784);
            Controls.Add(statusLabel);
            Controls.Add(resetButton);
            Controls.Add(saveButton);
            Controls.Add(ttsTargetTextBox);
            Controls.Add(ttsTargetLabel);
            Controls.Add(ttsVoiceModelIdTextBox);
            Controls.Add(ttsVoiceModelIdLabel);
            Controls.Add(ttsUrlTextBox);
            Controls.Add(ttsUrlLabel);
            Controls.Add(telegramEnabledCheckBox);
            Controls.Add(telegramEnabledLabel);
            Controls.Add(telegramTokenTextBox);
            Controls.Add(telegramTokenLabel);
            Controls.Add(modelTextBox);
            Controls.Add(modelLabel);
            Controls.Add(passwordTextBox);
            Controls.Add(passwordLabel);
            Controls.Add(tokenTextBox);
            Controls.Add(tokenLabel);
            Controls.Add(urlTextBox);
            Controls.Add(urlLabel);
            Font = new Font("Comic Sans MS", 10.2F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "OpenClawConfigForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Bonzi OpenClaw Configuration";
            ResumeLayout(false);
            PerformLayout();
        }

        private Label urlLabel;
        private TextBox urlTextBox;
        private Label tokenLabel;
        private TextBox tokenTextBox;
        private Label passwordLabel;
        private TextBox passwordTextBox;
        private Label modelLabel;
        private TextBox modelTextBox;
        private Label telegramTokenLabel;
        private TextBox telegramTokenTextBox;
        private Label telegramEnabledLabel;
        private CheckBox telegramEnabledCheckBox;
        private Label ttsUrlLabel;
        private TextBox ttsUrlTextBox;
        private Label ttsVoiceModelIdLabel;
        private TextBox ttsVoiceModelIdTextBox;
        private Label ttsTargetLabel;
        private TextBox ttsTargetTextBox;
        private Button saveButton;
        private Button resetButton;
        private Label statusLabel;
    }
}
