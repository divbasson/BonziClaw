namespace BonzoBuddo.Forms
{
    partial class AskAIForm
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
            this.labelPrompt = new System.Windows.Forms.Label();
            this.inputText = new System.Windows.Forms.TextBox();
            this.askButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // labelPrompt
            this.labelPrompt.AutoSize = false;
            this.labelPrompt.Location = new System.Drawing.Point(12, 10);
            this.labelPrompt.Size = new System.Drawing.Size(460, 28);
            this.labelPrompt.TabIndex = 0;
            this.labelPrompt.Text = "What would you like to ask Bonzi?";

            // inputText
            this.inputText.Location = new System.Drawing.Point(12, 42);
            this.inputText.Multiline = true;
            this.inputText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.inputText.Size = new System.Drawing.Size(460, 90);
            this.inputText.TabIndex = 1;

            // askButton
            this.askButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.askButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.askButton.Location = new System.Drawing.Point(12, 146);
            this.askButton.Size = new System.Drawing.Size(220, 44);
            this.askButton.TabIndex = 2;
            this.askButton.Text = "Ask Bonzi!";
            this.askButton.UseVisualStyleBackColor = true;
            this.askButton.Click += new System.EventHandler(this.askButton_Click);

            // closeButton
            this.closeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.closeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.closeButton.Location = new System.Drawing.Point(252, 146);
            this.closeButton.Size = new System.Drawing.Size(220, 44);
            this.closeButton.TabIndex = 3;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);

            // statusLabel
            this.statusLabel.AutoSize = false;
            this.statusLabel.ForeColor = System.Drawing.Color.DimGray;
            this.statusLabel.Location = new System.Drawing.Point(12, 198);
            this.statusLabel.Size = new System.Drawing.Size(460, 24);
            this.statusLabel.TabIndex = 4;
            this.statusLabel.Text = string.Empty;

            // AskAIForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 232);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.askButton);
            this.Controls.Add(this.inputText);
            this.Controls.Add(this.labelPrompt);
            this.Font = new System.Drawing.Font("Comic Sans MS", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AskAIForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ask Bonzi (OpenClaw)";
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label labelPrompt;
        private System.Windows.Forms.TextBox inputText;
        private System.Windows.Forms.Button askButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label statusLabel;
    }
}
