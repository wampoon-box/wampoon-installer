namespace PWamppConsole
{
    internal partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            // Dispose managed process handles if they exist
            if (disposing)
            {
                _apacheProcess?.Dispose();
                _mySqlProcess?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnStopApache = new System.Windows.Forms.Button();
            this.btnStartApache = new System.Windows.Forms.Button();
            this.lblApacheStatus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnStopMySql = new System.Windows.Forms.Button();
            this.btnStartMySql = new System.Windows.Forms.Button();
            this.lblMySqlStatus = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnStopApache);
            this.groupBox1.Controls.Add(this.btnStartApache);
            this.groupBox1.Controls.Add(this.lblApacheStatus);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(9, 10);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(270, 81);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Apache";
            // 
            // btnStopApache
            // 
            this.btnStopApache.Location = new System.Drawing.Point(184, 45);
            this.btnStopApache.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnStopApache.Name = "btnStopApache";
            this.btnStopApache.Size = new System.Drawing.Size(70, 24);
            this.btnStopApache.TabIndex = 3;
            this.btnStopApache.Text = "Stop Apache";
            this.btnStopApache.UseVisualStyleBackColor = true;
            this.btnStopApache.Click += new System.EventHandler(this.btnStopApache_Click);
            // 
            // btnStartApache
            // 
            this.btnStartApache.Location = new System.Drawing.Point(184, 16);
            this.btnStartApache.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnStartApache.Name = "btnStartApache";
            this.btnStartApache.Size = new System.Drawing.Size(70, 24);
            this.btnStartApache.TabIndex = 2;
            this.btnStartApache.Text = "Start Apache";
            this.btnStartApache.UseVisualStyleBackColor = true;
            this.btnStartApache.Click += new System.EventHandler(this.btnStartApache_Click);
            // 
            // lblApacheStatus
            // 
            this.lblApacheStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblApacheStatus.Location = new System.Drawing.Point(52, 24);
            this.lblApacheStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblApacheStatus.Name = "lblApacheStatus";
            this.lblApacheStatus.Size = new System.Drawing.Size(113, 21);
            this.lblApacheStatus.TabIndex = 1;
            this.lblApacheStatus.Text = "Unknown";
            this.lblApacheStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 28);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Status:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnStopMySql);
            this.groupBox2.Controls.Add(this.btnStartMySql);
            this.groupBox2.Controls.Add(this.lblMySqlStatus);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(9, 104);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Size = new System.Drawing.Size(270, 81);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "MySQL";
            // 
            // btnStopMySql
            // 
            this.btnStopMySql.Location = new System.Drawing.Point(184, 45);
            this.btnStopMySql.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnStopMySql.Name = "btnStopMySql";
            this.btnStopMySql.Size = new System.Drawing.Size(70, 24);
            this.btnStopMySql.TabIndex = 3;
            this.btnStopMySql.Text = "Stop MySQL";
            this.btnStopMySql.UseVisualStyleBackColor = true;
            this.btnStopMySql.Click += new System.EventHandler(this.btnStopMySql_Click);
            // 
            // btnStartMySql
            // 
            this.btnStartMySql.Location = new System.Drawing.Point(184, 16);
            this.btnStartMySql.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnStartMySql.Name = "btnStartMySql";
            this.btnStartMySql.Size = new System.Drawing.Size(70, 24);
            this.btnStartMySql.TabIndex = 2;
            this.btnStartMySql.Text = "Start MySQL";
            this.btnStartMySql.UseVisualStyleBackColor = true;
            this.btnStartMySql.Click += new System.EventHandler(this.btnStartMySql_Click);
            // 
            // lblMySqlStatus
            // 
            this.lblMySqlStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMySqlStatus.Location = new System.Drawing.Point(52, 24);
            this.lblMySqlStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMySqlStatus.Name = "lblMySqlStatus";
            this.lblMySqlStatus.Size = new System.Drawing.Size(113, 21);
            this.lblMySqlStatus.TabIndex = 1;
            this.lblMySqlStatus.Text = "Unknown";
            this.lblMySqlStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 28);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Status:";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 205);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusStrip1.Size = new System.Drawing.Size(321, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(641, 17);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "Ready";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(321, 227);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Process Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        // Declaration of the controls used in Form1.cs
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnStopApache;
        private System.Windows.Forms.Button btnStartApache;
        private System.Windows.Forms.Label lblApacheStatus;
        private System.Windows.Forms.Label label1; // Static label "Status:"
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnStopMySql;
        private System.Windows.Forms.Button btnStartMySql;
        private System.Windows.Forms.Label lblMySqlStatus;
        private System.Windows.Forms.Label label3; // Static label "Status:"
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}