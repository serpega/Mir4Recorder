namespace WinRT.GraphicsCapture
{
    partial class ConfigMain
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
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigMain));
            this.button1 = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.chkMir2 = new System.Windows.Forms.CheckBox();
            this.chkMir1 = new System.Windows.Forms.CheckBox();
            this.chkMir0 = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(324, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Mir Recorder";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.Click += new System.EventHandler(this.notifyIcon1_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = global::WinRT.GraphicsCapture.Properties.Settings.Default.RunOnWindows;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinRT.GraphicsCapture.Properties.Settings.Default, "RunOnWindows", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox1.Location = new System.Drawing.Point(124, 65);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(140, 17);
            this.checkBox1.TabIndex = 8;
            this.checkBox1.Text = "Run on windows startup";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // chkMir2
            // 
            this.chkMir2.AutoSize = true;
            this.chkMir2.Checked = global::WinRT.GraphicsCapture.Properties.Settings.Default.chkMir2;
            this.chkMir2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMir2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinRT.GraphicsCapture.Properties.Settings.Default, "chkMir2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkMir2.Location = new System.Drawing.Point(248, 18);
            this.chkMir2.Name = "chkMir2";
            this.chkMir2.Size = new System.Drawing.Size(46, 17);
            this.chkMir2.TabIndex = 2;
            this.chkMir2.Text = "Mir2";
            this.chkMir2.UseVisualStyleBackColor = true;
            // 
            // chkMir1
            // 
            this.chkMir1.AutoSize = true;
            this.chkMir1.Checked = global::WinRT.GraphicsCapture.Properties.Settings.Default.chkMir1;
            this.chkMir1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinRT.GraphicsCapture.Properties.Settings.Default, "chkMir1", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkMir1.Location = new System.Drawing.Point(186, 18);
            this.chkMir1.Name = "chkMir1";
            this.chkMir1.Size = new System.Drawing.Size(46, 17);
            this.chkMir1.TabIndex = 1;
            this.chkMir1.Text = "Mir1";
            this.chkMir1.UseVisualStyleBackColor = true;
            // 
            // chkMir0
            // 
            this.chkMir0.AutoSize = true;
            this.chkMir0.Checked = global::WinRT.GraphicsCapture.Properties.Settings.Default.chkMir0;
            this.chkMir0.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMir0.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinRT.GraphicsCapture.Properties.Settings.Default, "chkMir0", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkMir0.Location = new System.Drawing.Point(124, 18);
            this.chkMir0.Name = "chkMir0";
            this.chkMir0.Size = new System.Drawing.Size(46, 17);
            this.chkMir0.TabIndex = 0;
            this.chkMir0.Text = "Mir0";
            this.chkMir0.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 3600000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(101, 96);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // ConfigMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 121);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.chkMir2);
            this.Controls.Add(this.chkMir1);
            this.Controls.Add(this.chkMir0);
            this.Name = "ConfigMain";
            this.Text = "MirRecorder";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConfigMain_FormClosed);
            this.Load += new System.EventHandler(this.ConfigMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkMir0;
        private System.Windows.Forms.CheckBox chkMir1;
        private System.Windows.Forms.CheckBox chkMir2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}