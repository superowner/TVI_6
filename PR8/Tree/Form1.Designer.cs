namespace Tree
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.glControl1 = new OpenTK.GLControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.elevationTrack = new System.Windows.Forms.TrackBar();
            this.azimuthTrack = new System.Windows.Forms.TrackBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.elevationTrack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.azimuthTrack)).BeginInit();
            this.SuspendLayout();
            // 
            // glControl1
            // 
            this.glControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.glControl1.BackColor = System.Drawing.Color.Black;
            this.glControl1.Location = new System.Drawing.Point(9, 9);
            this.glControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(1220, 844);
            this.glControl1.TabIndex = 0;
            this.glControl1.VSync = false;
            this.glControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseDown);
            this.glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseMove);
            this.glControl1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseWheel);
            this.glControl1.Resize += new System.EventHandler(this.glControl1_Resize);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.elevationTrack);
            this.groupBox1.Controls.Add(this.azimuthTrack);
            this.groupBox1.Location = new System.Drawing.Point(1254, 9);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(201, 506);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Источник света";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Возвышение";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Азимут";
            // 
            // elevationTrack
            // 
            this.elevationTrack.Location = new System.Drawing.Point(4, 92);
            this.elevationTrack.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.elevationTrack.Maximum = 180;
            this.elevationTrack.Name = "elevationTrack";
            this.elevationTrack.Size = new System.Drawing.Size(193, 45);
            this.elevationTrack.TabIndex = 1;
            this.elevationTrack.TickFrequency = 15;
            this.elevationTrack.Value = 90;
            // 
            // azimuthTrack
            // 
            this.azimuthTrack.Location = new System.Drawing.Point(4, 30);
            this.azimuthTrack.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.azimuthTrack.Maximum = 180;
            this.azimuthTrack.Minimum = -180;
            this.azimuthTrack.Name = "azimuthTrack";
            this.azimuthTrack.Size = new System.Drawing.Size(193, 45);
            this.azimuthTrack.TabIndex = 0;
            this.azimuthTrack.TickFrequency = 15;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 16;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1443, 845);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.glControl1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form1";
            this.Text = "Килюп Л.А. ИКБО-08-18";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.elevationTrack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.azimuthTrack)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private OpenTK.GLControl glControl1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TrackBar elevationTrack;
        private System.Windows.Forms.TrackBar azimuthTrack;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}

