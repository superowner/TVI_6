namespace FBXCube
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
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.glControl = new OpenTK.GLControl();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.animationTrackBar = new System.Windows.Forms.TrackBar();
            this.frameLabel = new System.Windows.Forms.Label();
            this.boneTrackBarRY = new System.Windows.Forms.TrackBar();
            this.boneTrackBarRX = new System.Windows.Forms.TrackBar();
            this.boneTrackBarRZ = new System.Windows.Forms.TrackBar();
            this.rotXlabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.resetAllBoneTransformButton = new System.Windows.Forms.Button();
            this.negRZcheckBox = new System.Windows.Forms.CheckBox();
            this.negRYcheckBox = new System.Windows.Forms.CheckBox();
            this.negRXcheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.orderComboBox = new System.Windows.Forms.ComboBox();
            this.transZlabel = new System.Windows.Forms.Label();
            this.transYlabel = new System.Windows.Forms.Label();
            this.transXlabel = new System.Windows.Forms.Label();
            this.boneTrackBarTZ = new System.Windows.Forms.TrackBar();
            this.boneTrackBarTY = new System.Windows.Forms.TrackBar();
            this.boneTrackBarTX = new System.Windows.Forms.TrackBar();
            this.resetBoneTransformButton = new System.Windows.Forms.Button();
            this.rotZlabel = new System.Windows.Forms.Label();
            this.rotYlabel = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.drawBonesCheckBox = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.animationTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.boneTrackBarRY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.boneTrackBarRX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.boneTrackBarRZ)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.boneTrackBarTZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.boneTrackBarTY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.boneTrackBarTX)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // logTextBox
            // 
            this.logTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logTextBox.Location = new System.Drawing.Point(382, 420);
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logTextBox.Size = new System.Drawing.Size(520, 78);
            this.logTextBox.TabIndex = 0;
            // 
            // glControl
            // 
            this.glControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.glControl.BackColor = System.Drawing.Color.Black;
            this.glControl.Location = new System.Drawing.Point(382, 12);
            this.glControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.glControl.Name = "glControl";
            this.glControl.Size = new System.Drawing.Size(520, 402);
            this.glControl.TabIndex = 1;
            this.glControl.VSync = false;
            this.glControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseDown);
            this.glControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseMove);
            this.glControl.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseWheel);
            this.glControl.Resize += new System.EventHandler(this.glControl_Resize);
            // 
            // timer1
            // 
            this.timer1.Interval = 16;
            this.timer1.Tick += new System.EventHandler(this.RenderFrame);
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeView1.Location = new System.Drawing.Point(6, 19);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(352, 65);
            this.treeView1.TabIndex = 2;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // animationTrackBar
            // 
            this.animationTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.animationTrackBar.Location = new System.Drawing.Point(12, 504);
            this.animationTrackBar.Name = "animationTrackBar";
            this.animationTrackBar.Size = new System.Drawing.Size(798, 45);
            this.animationTrackBar.TabIndex = 3;
            this.animationTrackBar.ValueChanged += new System.EventHandler(this.animationTrackBar_ValueChanged);
            // 
            // frameLabel
            // 
            this.frameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.frameLabel.AutoSize = true;
            this.frameLabel.Location = new System.Drawing.Point(816, 504);
            this.frameLabel.Name = "frameLabel";
            this.frameLabel.Size = new System.Drawing.Size(35, 13);
            this.frameLabel.TabIndex = 4;
            this.frameLabel.Text = "label1";
            // 
            // boneTrackBarRY
            // 
            this.boneTrackBarRY.LargeChange = 15;
            this.boneTrackBarRY.Location = new System.Drawing.Point(85, 55);
            this.boneTrackBarRY.Maximum = 180;
            this.boneTrackBarRY.Minimum = -180;
            this.boneTrackBarRY.Name = "boneTrackBarRY";
            this.boneTrackBarRY.Size = new System.Drawing.Size(273, 45);
            this.boneTrackBarRY.TabIndex = 5;
            this.boneTrackBarRY.TickFrequency = 15;
            this.boneTrackBarRY.ValueChanged += new System.EventHandler(this.boneTransformChanged);
            // 
            // boneTrackBarRX
            // 
            this.boneTrackBarRX.LargeChange = 15;
            this.boneTrackBarRX.Location = new System.Drawing.Point(85, 19);
            this.boneTrackBarRX.Maximum = 180;
            this.boneTrackBarRX.Minimum = -180;
            this.boneTrackBarRX.Name = "boneTrackBarRX";
            this.boneTrackBarRX.Size = new System.Drawing.Size(273, 45);
            this.boneTrackBarRX.TabIndex = 6;
            this.boneTrackBarRX.TickFrequency = 15;
            this.boneTrackBarRX.ValueChanged += new System.EventHandler(this.boneTransformChanged);
            // 
            // boneTrackBarRZ
            // 
            this.boneTrackBarRZ.LargeChange = 15;
            this.boneTrackBarRZ.Location = new System.Drawing.Point(85, 91);
            this.boneTrackBarRZ.Maximum = 180;
            this.boneTrackBarRZ.Minimum = -180;
            this.boneTrackBarRZ.Name = "boneTrackBarRZ";
            this.boneTrackBarRZ.Size = new System.Drawing.Size(273, 45);
            this.boneTrackBarRZ.TabIndex = 7;
            this.boneTrackBarRZ.TickFrequency = 15;
            this.boneTrackBarRZ.ValueChanged += new System.EventHandler(this.boneTransformChanged);
            // 
            // rotXlabel
            // 
            this.rotXlabel.AutoSize = true;
            this.rotXlabel.Location = new System.Drawing.Point(6, 23);
            this.rotXlabel.Name = "rotXlabel";
            this.rotXlabel.Size = new System.Drawing.Size(62, 13);
            this.rotXlabel.TabIndex = 8;
            this.rotXlabel.Text = "RotX:  000°";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.resetAllBoneTransformButton);
            this.groupBox1.Controls.Add(this.negRZcheckBox);
            this.groupBox1.Controls.Add(this.negRYcheckBox);
            this.groupBox1.Controls.Add(this.negRXcheckBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.orderComboBox);
            this.groupBox1.Controls.Add(this.transZlabel);
            this.groupBox1.Controls.Add(this.transYlabel);
            this.groupBox1.Controls.Add(this.transXlabel);
            this.groupBox1.Controls.Add(this.boneTrackBarTZ);
            this.groupBox1.Controls.Add(this.boneTrackBarTY);
            this.groupBox1.Controls.Add(this.boneTrackBarTX);
            this.groupBox1.Controls.Add(this.resetBoneTransformButton);
            this.groupBox1.Controls.Add(this.rotZlabel);
            this.groupBox1.Controls.Add(this.rotYlabel);
            this.groupBox1.Controls.Add(this.boneTrackBarRZ);
            this.groupBox1.Controls.Add(this.rotXlabel);
            this.groupBox1.Controls.Add(this.boneTrackBarRY);
            this.groupBox1.Controls.Add(this.boneTrackBarRX);
            this.groupBox1.Location = new System.Drawing.Point(12, 108);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(364, 299);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Преобразование";
            // 
            // resetAllBoneTransformButton
            // 
            this.resetAllBoneTransformButton.Location = new System.Drawing.Point(179, 0);
            this.resetAllBoneTransformButton.Name = "resetAllBoneTransformButton";
            this.resetAllBoneTransformButton.Size = new System.Drawing.Size(98, 20);
            this.resetAllBoneTransformButton.TabIndex = 23;
            this.resetAllBoneTransformButton.Text = "Сбросить Все";
            this.resetAllBoneTransformButton.UseVisualStyleBackColor = true;
            this.resetAllBoneTransformButton.Click += new System.EventHandler(this.resetAllBoneTransformButton_Click);
            // 
            // negRZcheckBox
            // 
            this.negRZcheckBox.AutoSize = true;
            this.negRZcheckBox.Location = new System.Drawing.Point(260, 276);
            this.negRZcheckBox.Name = "negRZcheckBox";
            this.negRZcheckBox.Size = new System.Drawing.Size(94, 17);
            this.negRZcheckBox.TabIndex = 22;
            this.negRZcheckBox.Text = "Инверсия RZ";
            this.negRZcheckBox.UseVisualStyleBackColor = true;
            this.negRZcheckBox.CheckedChanged += new System.EventHandler(this.boneTransformChanged);
            // 
            // negRYcheckBox
            // 
            this.negRYcheckBox.AutoSize = true;
            this.negRYcheckBox.Location = new System.Drawing.Point(134, 276);
            this.negRYcheckBox.Name = "negRYcheckBox";
            this.negRYcheckBox.Size = new System.Drawing.Size(94, 17);
            this.negRYcheckBox.TabIndex = 21;
            this.negRYcheckBox.Text = "Инверсия RY";
            this.negRYcheckBox.UseVisualStyleBackColor = true;
            this.negRYcheckBox.CheckedChanged += new System.EventHandler(this.boneTransformChanged);
            // 
            // negRXcheckBox
            // 
            this.negRXcheckBox.AutoSize = true;
            this.negRXcheckBox.Location = new System.Drawing.Point(9, 276);
            this.negRXcheckBox.Name = "negRXcheckBox";
            this.negRXcheckBox.Size = new System.Drawing.Size(94, 17);
            this.negRXcheckBox.TabIndex = 20;
            this.negRXcheckBox.Text = "Инверсия RX";
            this.negRXcheckBox.UseVisualStyleBackColor = true;
            this.negRXcheckBox.CheckedChanged += new System.EventHandler(this.boneTransformChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 253);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Порядок:";
            // 
            // orderComboBox
            // 
            this.orderComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.orderComboBox.FormattingEnabled = true;
            this.orderComboBox.Items.AddRange(new object[] {
            "XYZ",
            "XZY",
            "YXZ",
            "YZX",
            "ZXY",
            "ZYX"});
            this.orderComboBox.Location = new System.Drawing.Point(85, 250);
            this.orderComboBox.Name = "orderComboBox";
            this.orderComboBox.Size = new System.Drawing.Size(273, 21);
            this.orderComboBox.TabIndex = 18;
            this.orderComboBox.SelectedIndexChanged += new System.EventHandler(this.boneTransformChanged);
            // 
            // transZlabel
            // 
            this.transZlabel.AutoSize = true;
            this.transZlabel.Location = new System.Drawing.Point(6, 218);
            this.transZlabel.Name = "transZlabel";
            this.transZlabel.Size = new System.Drawing.Size(68, 13);
            this.transZlabel.TabIndex = 17;
            this.transZlabel.Text = "TransZ: 0,00";
            // 
            // transYlabel
            // 
            this.transYlabel.AutoSize = true;
            this.transYlabel.Location = new System.Drawing.Point(6, 182);
            this.transYlabel.Name = "transYlabel";
            this.transYlabel.Size = new System.Drawing.Size(68, 13);
            this.transYlabel.TabIndex = 16;
            this.transYlabel.Text = "TransY: 0,00";
            // 
            // transXlabel
            // 
            this.transXlabel.AutoSize = true;
            this.transXlabel.Location = new System.Drawing.Point(6, 146);
            this.transXlabel.Name = "transXlabel";
            this.transXlabel.Size = new System.Drawing.Size(68, 13);
            this.transXlabel.TabIndex = 15;
            this.transXlabel.Text = "TransX: 0,00";
            // 
            // boneTrackBarTZ
            // 
            this.boneTrackBarTZ.LargeChange = 10;
            this.boneTrackBarTZ.Location = new System.Drawing.Point(85, 214);
            this.boneTrackBarTZ.Maximum = 250;
            this.boneTrackBarTZ.Minimum = -250;
            this.boneTrackBarTZ.Name = "boneTrackBarTZ";
            this.boneTrackBarTZ.Size = new System.Drawing.Size(273, 45);
            this.boneTrackBarTZ.TabIndex = 14;
            this.boneTrackBarTZ.TickFrequency = 15;
            this.boneTrackBarTZ.ValueChanged += new System.EventHandler(this.boneTransformChanged);
            // 
            // boneTrackBarTY
            // 
            this.boneTrackBarTY.LargeChange = 10;
            this.boneTrackBarTY.Location = new System.Drawing.Point(85, 178);
            this.boneTrackBarTY.Maximum = 250;
            this.boneTrackBarTY.Minimum = -250;
            this.boneTrackBarTY.Name = "boneTrackBarTY";
            this.boneTrackBarTY.Size = new System.Drawing.Size(273, 45);
            this.boneTrackBarTY.TabIndex = 12;
            this.boneTrackBarTY.TickFrequency = 15;
            this.boneTrackBarTY.ValueChanged += new System.EventHandler(this.boneTransformChanged);
            // 
            // boneTrackBarTX
            // 
            this.boneTrackBarTX.LargeChange = 10;
            this.boneTrackBarTX.Location = new System.Drawing.Point(85, 142);
            this.boneTrackBarTX.Maximum = 250;
            this.boneTrackBarTX.Minimum = -250;
            this.boneTrackBarTX.Name = "boneTrackBarTX";
            this.boneTrackBarTX.Size = new System.Drawing.Size(273, 45);
            this.boneTrackBarTX.TabIndex = 13;
            this.boneTrackBarTX.TickFrequency = 15;
            this.boneTrackBarTX.ValueChanged += new System.EventHandler(this.boneTransformChanged);
            // 
            // resetBoneTransformButton
            // 
            this.resetBoneTransformButton.Location = new System.Drawing.Point(283, 0);
            this.resetBoneTransformButton.Name = "resetBoneTransformButton";
            this.resetBoneTransformButton.Size = new System.Drawing.Size(75, 20);
            this.resetBoneTransformButton.TabIndex = 11;
            this.resetBoneTransformButton.Text = "Сбросить";
            this.resetBoneTransformButton.UseVisualStyleBackColor = true;
            this.resetBoneTransformButton.Click += new System.EventHandler(this.resetBoneTransformbutton_Click);
            // 
            // rotZlabel
            // 
            this.rotZlabel.AutoSize = true;
            this.rotZlabel.Location = new System.Drawing.Point(6, 95);
            this.rotZlabel.Name = "rotZlabel";
            this.rotZlabel.Size = new System.Drawing.Size(62, 13);
            this.rotZlabel.TabIndex = 10;
            this.rotZlabel.Text = "RotZ:  000°";
            // 
            // rotYlabel
            // 
            this.rotYlabel.AutoSize = true;
            this.rotYlabel.Location = new System.Drawing.Point(6, 59);
            this.rotYlabel.Name = "rotYlabel";
            this.rotYlabel.Size = new System.Drawing.Size(62, 13);
            this.rotYlabel.TabIndex = 9;
            this.rotYlabel.Text = "RotY:  000°";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(12, 413);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 86);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox2";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(188, 67);
            this.label2.TabIndex = 11;
            this.label2.Text = "label2";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.drawBonesCheckBox);
            this.groupBox3.Controls.Add(this.treeView1);
            this.groupBox3.Location = new System.Drawing.Point(12, 8);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(364, 94);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Кости";
            // 
            // drawBonesCheckBox
            // 
            this.drawBonesCheckBox.AutoSize = true;
            this.drawBonesCheckBox.Location = new System.Drawing.Point(227, 0);
            this.drawBonesCheckBox.Name = "drawBonesCheckBox";
            this.drawBonesCheckBox.Size = new System.Drawing.Size(127, 17);
            this.drawBonesCheckBox.TabIndex = 3;
            this.drawBonesCheckBox.Text = "отображать базисы";
            this.drawBonesCheckBox.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(218, 413);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(158, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "Загрузить модель";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFbxFileDialog";
            this.openFileDialog1.Filter = "Filmbox models (*.fbx)|*.fbx";
            this.openFileDialog1.Title = "Загрузить модель";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.FileName = "openBvhFileDialog";
            this.openFileDialog2.Filter = "Biovision Hierarchy (*.bvh)|*.bvh";
            this.openFileDialog2.Title = "Загрузить анимацию";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(914, 548);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.frameLabel);
            this.Controls.Add(this.animationTrackBar);
            this.Controls.Add(this.glControl);
            this.Controls.Add(this.logTextBox);
            this.Name = "Form1";
            this.Text = "Килюп Л.А. ИКБО-08-18";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.animationTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.boneTrackBarRY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.boneTrackBarRX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.boneTrackBarRZ)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.boneTrackBarTZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.boneTrackBarTY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.boneTrackBarTX)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox logTextBox;
        private OpenTK.GLControl glControl;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TrackBar animationTrackBar;
        private System.Windows.Forms.Label frameLabel;
        private System.Windows.Forms.TrackBar boneTrackBarRY;
        private System.Windows.Forms.TrackBar boneTrackBarRX;
        private System.Windows.Forms.TrackBar boneTrackBarRZ;
        private System.Windows.Forms.Label rotXlabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label rotZlabel;
        private System.Windows.Forms.Label rotYlabel;
        private System.Windows.Forms.Label transZlabel;
        private System.Windows.Forms.Label transYlabel;
        private System.Windows.Forms.Label transXlabel;
        private System.Windows.Forms.TrackBar boneTrackBarTZ;
        private System.Windows.Forms.TrackBar boneTrackBarTY;
        private System.Windows.Forms.TrackBar boneTrackBarTX;
        private System.Windows.Forms.Button resetBoneTransformButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox orderComboBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox negRZcheckBox;
        private System.Windows.Forms.CheckBox negRYcheckBox;
        private System.Windows.Forms.CheckBox negRXcheckBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox drawBonesCheckBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.Button resetAllBoneTransformButton;
    }
}

