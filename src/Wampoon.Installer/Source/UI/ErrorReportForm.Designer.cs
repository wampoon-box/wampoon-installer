namespace Frostybee.Pwamp.UI
{
    partial class ErrorReportForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label exceptionTypeLabel;
        private System.Windows.Forms.Label timestampLabel;
        private System.Windows.Forms.Label errorMessageLabel;
        private System.Windows.Forms.TextBox errorMessageTextBox;
        private System.Windows.Forms.Label stackTraceLabel;
        private System.Windows.Forms.TextBox stackTraceTextBox;
        private System.Windows.Forms.Label additionalInfoLabel;
        private System.Windows.Forms.TextBox additionalInfoTextBox;
        private System.Windows.Forms.Button copyToClipboardButton;
        private System.Windows.Forms.Button reportIssueButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.PictureBox errorIconPictureBox;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorReportForm));
            this.titleLabel = new System.Windows.Forms.Label();
            this.exceptionTypeLabel = new System.Windows.Forms.Label();
            this.timestampLabel = new System.Windows.Forms.Label();
            this.errorMessageLabel = new System.Windows.Forms.Label();
            this.errorMessageTextBox = new System.Windows.Forms.TextBox();
            this.stackTraceLabel = new System.Windows.Forms.Label();
            this.stackTraceTextBox = new System.Windows.Forms.TextBox();
            this.additionalInfoLabel = new System.Windows.Forms.Label();
            this.additionalInfoTextBox = new System.Windows.Forms.TextBox();
            this.copyToClipboardButton = new System.Windows.Forms.Button();
            this.reportIssueButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.errorIconPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.errorIconPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.ForeColor = System.Drawing.Color.DarkRed;
            this.titleLabel.Location = new System.Drawing.Point(60, 15);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(247, 20);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "An unexpected error occurred";
            // 
            // exceptionTypeLabel
            // 
            this.exceptionTypeLabel.AutoSize = true;
            this.exceptionTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exceptionTypeLabel.Location = new System.Drawing.Point(12, 60);
            this.exceptionTypeLabel.Name = "exceptionTypeLabel";
            this.exceptionTypeLabel.Size = new System.Drawing.Size(99, 13);
            this.exceptionTypeLabel.TabIndex = 2;
            this.exceptionTypeLabel.Text = "Exception Type:";
            // 
            // timestampLabel
            // 
            this.timestampLabel.AutoSize = true;
            this.timestampLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.timestampLabel.Location = new System.Drawing.Point(12, 80);
            this.timestampLabel.Name = "timestampLabel";
            this.timestampLabel.Size = new System.Drawing.Size(54, 13);
            this.timestampLabel.TabIndex = 3;
            this.timestampLabel.Text = "Occurred:";
            // 
            // errorMessageLabel
            // 
            this.errorMessageLabel.AutoSize = true;
            this.errorMessageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.errorMessageLabel.Location = new System.Drawing.Point(12, 110);
            this.errorMessageLabel.Name = "errorMessageLabel";
            this.errorMessageLabel.Size = new System.Drawing.Size(92, 13);
            this.errorMessageLabel.TabIndex = 4;
            this.errorMessageLabel.Text = "Error Message:";
            // 
            // errorMessageTextBox
            // 
            this.errorMessageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.errorMessageTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.errorMessageTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.errorMessageTextBox.Location = new System.Drawing.Point(15, 130);
            this.errorMessageTextBox.Multiline = true;
            this.errorMessageTextBox.Name = "errorMessageTextBox";
            this.errorMessageTextBox.ReadOnly = true;
            this.errorMessageTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.errorMessageTextBox.Size = new System.Drawing.Size(665, 60);
            this.errorMessageTextBox.TabIndex = 5;
            // 
            // stackTraceLabel
            // 
            this.stackTraceLabel.AutoSize = true;
            this.stackTraceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stackTraceLabel.Location = new System.Drawing.Point(12, 200);
            this.stackTraceLabel.Name = "stackTraceLabel";
            this.stackTraceLabel.Size = new System.Drawing.Size(81, 13);
            this.stackTraceLabel.TabIndex = 6;
            this.stackTraceLabel.Text = "Stack Trace:";
            // 
            // stackTraceTextBox
            // 
            this.stackTraceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stackTraceTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.stackTraceTextBox.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stackTraceTextBox.Location = new System.Drawing.Point(15, 220);
            this.stackTraceTextBox.Multiline = true;
            this.stackTraceTextBox.Name = "stackTraceTextBox";
            this.stackTraceTextBox.ReadOnly = true;
            this.stackTraceTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.stackTraceTextBox.Size = new System.Drawing.Size(665, 180);
            this.stackTraceTextBox.TabIndex = 7;
            // 
            // additionalInfoLabel
            // 
            this.additionalInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.additionalInfoLabel.AutoSize = true;
            this.additionalInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.additionalInfoLabel.Location = new System.Drawing.Point(12, 410);
            this.additionalInfoLabel.Name = "additionalInfoLabel";
            this.additionalInfoLabel.Size = new System.Drawing.Size(134, 13);
            this.additionalInfoLabel.TabIndex = 8;
            this.additionalInfoLabel.Text = "Additional Information:";
            // 
            // additionalInfoTextBox
            // 
            this.additionalInfoTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.additionalInfoTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.additionalInfoTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.additionalInfoTextBox.Location = new System.Drawing.Point(15, 430);
            this.additionalInfoTextBox.Multiline = true;
            this.additionalInfoTextBox.Name = "additionalInfoTextBox";
            this.additionalInfoTextBox.ReadOnly = true;
            this.additionalInfoTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.additionalInfoTextBox.Size = new System.Drawing.Size(665, 60);
            this.additionalInfoTextBox.TabIndex = 9;
            // 
            // copyToClipboardButton
            // 
            this.copyToClipboardButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.copyToClipboardButton.BackColor = System.Drawing.Color.Green;
            this.copyToClipboardButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.copyToClipboardButton.ForeColor = System.Drawing.Color.White;
            this.copyToClipboardButton.Location = new System.Drawing.Point(15, 510);
            this.copyToClipboardButton.Name = "copyToClipboardButton";
            this.copyToClipboardButton.Size = new System.Drawing.Size(131, 30);
            this.copyToClipboardButton.TabIndex = 10;
            this.copyToClipboardButton.Text = "Copy to Clipboard";
            this.copyToClipboardButton.UseVisualStyleBackColor = false;
            this.copyToClipboardButton.Click += new System.EventHandler(this.CopyToClipboardButton_Click);
            // 
            // reportIssueButton
            // 
            this.reportIssueButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.reportIssueButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(125)))), ((int)(((byte)(192)))));
            this.reportIssueButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.reportIssueButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reportIssueButton.ForeColor = System.Drawing.Color.White;
            this.reportIssueButton.Location = new System.Drawing.Point(164, 510);
            this.reportIssueButton.Name = "reportIssueButton";
            this.reportIssueButton.Size = new System.Drawing.Size(143, 30);
            this.reportIssueButton.TabIndex = 11;
            this.reportIssueButton.Text = "Report on GitHub";
            this.reportIssueButton.UseVisualStyleBackColor = false;
            this.reportIssueButton.Click += new System.EventHandler(this.ReportIssueButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeButton.ForeColor = System.Drawing.Color.Snow;
            this.closeButton.Location = new System.Drawing.Point(605, 510);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 30);
            this.closeButton.TabIndex = 12;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // errorIconPictureBox
            // 
            this.errorIconPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("errorIconPictureBox.Image")));
            this.errorIconPictureBox.Location = new System.Drawing.Point(15, 15);
            this.errorIconPictureBox.Name = "errorIconPictureBox";
            this.errorIconPictureBox.Size = new System.Drawing.Size(32, 32);
            this.errorIconPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.errorIconPictureBox.TabIndex = 1;
            this.errorIconPictureBox.TabStop = false;
            // 
            // ErrorReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(700, 560);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.reportIssueButton);
            this.Controls.Add(this.copyToClipboardButton);
            this.Controls.Add(this.additionalInfoTextBox);
            this.Controls.Add(this.additionalInfoLabel);
            this.Controls.Add(this.stackTraceTextBox);
            this.Controls.Add(this.stackTraceLabel);
            this.Controls.Add(this.errorMessageTextBox);
            this.Controls.Add(this.errorMessageLabel);
            this.Controls.Add(this.timestampLabel);
            this.Controls.Add(this.exceptionTypeLabel);
            this.Controls.Add(this.errorIconPictureBox);
            this.Controls.Add(this.titleLabel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "ErrorReportForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Error Report";
            ((System.ComponentModel.ISupportInitialize)(this.errorIconPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}