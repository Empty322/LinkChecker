namespace Link11Checker
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.OkBtn = new System.Windows.Forms.Button();
            this.CanselBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.MinutesToAwaitAfterEndTextBox = new System.Windows.Forms.TextBox();
            this.MinetsToAwaitAfterEnd = new System.Windows.Forms.Label();
            this.SmoothValueTextBox = new System.Windows.Forms.TextBox();
            this.SmoothValue = new System.Windows.Forms.Label();
            this.IntervalsKTextBox = new System.Windows.Forms.TextBox();
            this.IntervalsK = new System.Windows.Forms.Label();
            this.AbonentsKTextBox = new System.Windows.Forms.TextBox();
            this.AbonentsK = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.HideEmptySeanses = new System.Windows.Forms.CheckBox();
            this.VenturFileExplore = new System.Windows.Forms.Button();
            this.InitialDestPathExplore = new System.Windows.Forms.Button();
            this.InitialSeansesPathExplore = new System.Windows.Forms.Button();
            this.WorkingChartIntervalTextBox = new System.Windows.Forms.TextBox();
            this.WorkingChartInterval = new System.Windows.Forms.Label();
            this.SynchronizeCounterLimitTextBox = new System.Windows.Forms.TextBox();
            this.SynchronizeCounterLimit = new System.Windows.Forms.Label();
            this.CopyCounterLimitTextBox = new System.Windows.Forms.TextBox();
            this.CopyCounterLimit = new System.Windows.Forms.Label();
            this.UpdateCounterLimitTextBox = new System.Windows.Forms.TextBox();
            this.UpdateCounterLimit = new System.Windows.Forms.Label();
            this.VenturFileTextBox = new System.Windows.Forms.TextBox();
            this.VenturFile = new System.Windows.Forms.Label();
            this.InitialDestPathTextBox = new System.Windows.Forms.TextBox();
            this.InitialDestPath = new System.Windows.Forms.Label();
            this.InitialSeansesPathTextBox = new System.Windows.Forms.TextBox();
            this.InitialSeansesPath = new System.Windows.Forms.Label();
            this.EmptySeansesTrashold = new System.Windows.Forms.Label();
            this.EmptySeansesTrasholdUpDown = new System.Windows.Forms.NumericUpDown();
            this.CopyLengthTrashold = new System.Windows.Forms.Label();
            this.CopyPercentTrashold = new System.Windows.Forms.Label();
            this.CopyLengthTrasholdUpDown = new System.Windows.Forms.NumericUpDown();
            this.CopyPercentTrasholdUpDown = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EmptySeansesTrasholdUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CopyLengthTrasholdUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CopyPercentTrasholdUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // OkBtn
            // 
            this.OkBtn.Location = new System.Drawing.Point(716, 430);
            this.OkBtn.Name = "OkBtn";
            this.OkBtn.Size = new System.Drawing.Size(75, 23);
            this.OkBtn.TabIndex = 0;
            this.OkBtn.Text = "ОК";
            this.OkBtn.UseVisualStyleBackColor = true;
            this.OkBtn.Click += new System.EventHandler(this.OkBtn_Click);
            // 
            // CanselBtn
            // 
            this.CanselBtn.Location = new System.Drawing.Point(797, 430);
            this.CanselBtn.Name = "CanselBtn";
            this.CanselBtn.Size = new System.Drawing.Size(75, 23);
            this.CanselBtn.TabIndex = 1;
            this.CanselBtn.Text = "Отмена";
            this.CanselBtn.UseVisualStyleBackColor = true;
            this.CanselBtn.Click += new System.EventHandler(this.CanselBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.MinutesToAwaitAfterEndTextBox);
            this.groupBox1.Controls.Add(this.MinetsToAwaitAfterEnd);
            this.groupBox1.Controls.Add(this.SmoothValueTextBox);
            this.groupBox1.Controls.Add(this.SmoothValue);
            this.groupBox1.Controls.Add(this.IntervalsKTextBox);
            this.groupBox1.Controls.Add(this.IntervalsK);
            this.groupBox1.Controls.Add(this.AbonentsKTextBox);
            this.groupBox1.Controls.Add(this.AbonentsK);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(420, 412);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Конфигурация вычислений";
            // 
            // MinutesToAwaitAfterEndTextBox
            // 
            this.MinutesToAwaitAfterEndTextBox.Location = new System.Drawing.Point(22, 207);
            this.MinutesToAwaitAfterEndTextBox.Name = "MinutesToAwaitAfterEndTextBox";
            this.MinutesToAwaitAfterEndTextBox.Size = new System.Drawing.Size(392, 20);
            this.MinutesToAwaitAfterEndTextBox.TabIndex = 7;
            // 
            // MinetsToAwaitAfterEnd
            // 
            this.MinetsToAwaitAfterEnd.AutoSize = true;
            this.MinetsToAwaitAfterEnd.Location = new System.Drawing.Point(19, 191);
            this.MinetsToAwaitAfterEnd.Name = "MinetsToAwaitAfterEnd";
            this.MinetsToAwaitAfterEnd.Size = new System.Drawing.Size(131, 13);
            this.MinetsToAwaitAfterEnd.TabIndex = 6;
            this.MinetsToAwaitAfterEnd.Text = "EntriesCountToStartSignal";
            // 
            // SmoothValueTextBox
            // 
            this.SmoothValueTextBox.Location = new System.Drawing.Point(22, 168);
            this.SmoothValueTextBox.Name = "SmoothValueTextBox";
            this.SmoothValueTextBox.Size = new System.Drawing.Size(392, 20);
            this.SmoothValueTextBox.TabIndex = 5;
            // 
            // SmoothValue
            // 
            this.SmoothValue.AutoSize = true;
            this.SmoothValue.Location = new System.Drawing.Point(19, 152);
            this.SmoothValue.Name = "SmoothValue";
            this.SmoothValue.Size = new System.Drawing.Size(70, 13);
            this.SmoothValue.TabIndex = 4;
            this.SmoothValue.Text = "SmoothValue";
            // 
            // IntervalsKTextBox
            // 
            this.IntervalsKTextBox.Enabled = false;
            this.IntervalsKTextBox.Location = new System.Drawing.Point(22, 129);
            this.IntervalsKTextBox.Name = "IntervalsKTextBox";
            this.IntervalsKTextBox.Size = new System.Drawing.Size(392, 20);
            this.IntervalsKTextBox.TabIndex = 3;
            // 
            // IntervalsK
            // 
            this.IntervalsK.AutoSize = true;
            this.IntervalsK.Location = new System.Drawing.Point(19, 87);
            this.IntervalsK.Name = "IntervalsK";
            this.IntervalsK.Size = new System.Drawing.Size(54, 13);
            this.IntervalsK.TabIndex = 2;
            this.IntervalsK.Text = "IntervalsK";
            // 
            // AbonentsKTextBox
            // 
            this.AbonentsKTextBox.Location = new System.Drawing.Point(22, 64);
            this.AbonentsKTextBox.Name = "AbonentsKTextBox";
            this.AbonentsKTextBox.Size = new System.Drawing.Size(392, 20);
            this.AbonentsKTextBox.TabIndex = 1;
            // 
            // AbonentsK
            // 
            this.AbonentsK.AutoSize = true;
            this.AbonentsK.Location = new System.Drawing.Point(19, 16);
            this.AbonentsK.Name = "AbonentsK";
            this.AbonentsK.Size = new System.Drawing.Size(59, 13);
            this.AbonentsK.TabIndex = 0;
            this.AbonentsK.Text = "AbonentsK";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.CopyPercentTrasholdUpDown);
            this.groupBox2.Controls.Add(this.CopyLengthTrasholdUpDown);
            this.groupBox2.Controls.Add(this.CopyPercentTrashold);
            this.groupBox2.Controls.Add(this.CopyLengthTrashold);
            this.groupBox2.Controls.Add(this.EmptySeansesTrasholdUpDown);
            this.groupBox2.Controls.Add(this.EmptySeansesTrashold);
            this.groupBox2.Controls.Add(this.HideEmptySeanses);
            this.groupBox2.Controls.Add(this.VenturFileExplore);
            this.groupBox2.Controls.Add(this.InitialDestPathExplore);
            this.groupBox2.Controls.Add(this.InitialSeansesPathExplore);
            this.groupBox2.Controls.Add(this.WorkingChartIntervalTextBox);
            this.groupBox2.Controls.Add(this.WorkingChartInterval);
            this.groupBox2.Controls.Add(this.SynchronizeCounterLimitTextBox);
            this.groupBox2.Controls.Add(this.SynchronizeCounterLimit);
            this.groupBox2.Controls.Add(this.CopyCounterLimitTextBox);
            this.groupBox2.Controls.Add(this.CopyCounterLimit);
            this.groupBox2.Controls.Add(this.UpdateCounterLimitTextBox);
            this.groupBox2.Controls.Add(this.UpdateCounterLimit);
            this.groupBox2.Controls.Add(this.VenturFileTextBox);
            this.groupBox2.Controls.Add(this.VenturFile);
            this.groupBox2.Controls.Add(this.InitialDestPathTextBox);
            this.groupBox2.Controls.Add(this.InitialDestPath);
            this.groupBox2.Controls.Add(this.InitialSeansesPathTextBox);
            this.groupBox2.Controls.Add(this.InitialSeansesPath);
            this.groupBox2.Location = new System.Drawing.Point(452, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(420, 412);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Настройки приложения";
            // 
            // HideEmptySeanses
            // 
            this.HideEmptySeanses.AutoSize = true;
            this.HideEmptySeanses.Location = new System.Drawing.Point(10, 376);
            this.HideEmptySeanses.Name = "HideEmptySeanses";
            this.HideEmptySeanses.Size = new System.Drawing.Size(156, 17);
            this.HideEmptySeanses.TabIndex = 19;
            this.HideEmptySeanses.Text = "Скрывать пустые сеансы";
            this.HideEmptySeanses.UseVisualStyleBackColor = true;
            // 
            // VenturFileExplore
            // 
            this.VenturFileExplore.Location = new System.Drawing.Point(331, 114);
            this.VenturFileExplore.Name = "VenturFileExplore";
            this.VenturFileExplore.Size = new System.Drawing.Size(83, 20);
            this.VenturFileExplore.TabIndex = 17;
            this.VenturFileExplore.Text = "Обзор...";
            this.VenturFileExplore.UseVisualStyleBackColor = true;
            this.VenturFileExplore.Click += new System.EventHandler(this.VenturFileExplore_Click);
            // 
            // InitialDestPathExplore
            // 
            this.InitialDestPathExplore.Location = new System.Drawing.Point(331, 75);
            this.InitialDestPathExplore.Name = "InitialDestPathExplore";
            this.InitialDestPathExplore.Size = new System.Drawing.Size(83, 20);
            this.InitialDestPathExplore.TabIndex = 16;
            this.InitialDestPathExplore.Text = "Обзор...";
            this.InitialDestPathExplore.UseVisualStyleBackColor = true;
            this.InitialDestPathExplore.Click += new System.EventHandler(this.InitialDestPathExplore_Click);
            // 
            // InitialSeansesPathExplore
            // 
            this.InitialSeansesPathExplore.Location = new System.Drawing.Point(331, 36);
            this.InitialSeansesPathExplore.Name = "InitialSeansesPathExplore";
            this.InitialSeansesPathExplore.Size = new System.Drawing.Size(83, 20);
            this.InitialSeansesPathExplore.TabIndex = 15;
            this.InitialSeansesPathExplore.Text = "Обзор...";
            this.InitialSeansesPathExplore.UseVisualStyleBackColor = true;
            this.InitialSeansesPathExplore.Click += new System.EventHandler(this.InitialSeansesPathExplore_Click);
            // 
            // WorkingChartIntervalTextBox
            // 
            this.WorkingChartIntervalTextBox.Location = new System.Drawing.Point(10, 270);
            this.WorkingChartIntervalTextBox.Name = "WorkingChartIntervalTextBox";
            this.WorkingChartIntervalTextBox.Size = new System.Drawing.Size(404, 20);
            this.WorkingChartIntervalTextBox.TabIndex = 14;
            // 
            // WorkingChartInterval
            // 
            this.WorkingChartInterval.AutoSize = true;
            this.WorkingChartInterval.Location = new System.Drawing.Point(7, 254);
            this.WorkingChartInterval.Name = "WorkingChartInterval";
            this.WorkingChartInterval.Size = new System.Drawing.Size(134, 13);
            this.WorkingChartInterval.TabIndex = 13;
            this.WorkingChartInterval.Text = "WorkingChartInterval(mins)";
            // 
            // SynchronizeCounterLimitTextBox
            // 
            this.SynchronizeCounterLimitTextBox.Location = new System.Drawing.Point(10, 231);
            this.SynchronizeCounterLimitTextBox.Name = "SynchronizeCounterLimitTextBox";
            this.SynchronizeCounterLimitTextBox.Size = new System.Drawing.Size(404, 20);
            this.SynchronizeCounterLimitTextBox.TabIndex = 12;
            // 
            // SynchronizeCounterLimit
            // 
            this.SynchronizeCounterLimit.AutoSize = true;
            this.SynchronizeCounterLimit.Location = new System.Drawing.Point(7, 215);
            this.SynchronizeCounterLimit.Name = "SynchronizeCounterLimit";
            this.SynchronizeCounterLimit.Size = new System.Drawing.Size(156, 13);
            this.SynchronizeCounterLimit.TabIndex = 11;
            this.SynchronizeCounterLimit.Text = "SynchronizeCounterLimit(*5sec)";
            // 
            // CopyCounterLimitTextBox
            // 
            this.CopyCounterLimitTextBox.Location = new System.Drawing.Point(10, 192);
            this.CopyCounterLimitTextBox.Name = "CopyCounterLimitTextBox";
            this.CopyCounterLimitTextBox.Size = new System.Drawing.Size(404, 20);
            this.CopyCounterLimitTextBox.TabIndex = 10;
            // 
            // CopyCounterLimit
            // 
            this.CopyCounterLimit.AutoSize = true;
            this.CopyCounterLimit.Location = new System.Drawing.Point(7, 176);
            this.CopyCounterLimit.Name = "CopyCounterLimit";
            this.CopyCounterLimit.Size = new System.Drawing.Size(122, 13);
            this.CopyCounterLimit.TabIndex = 9;
            this.CopyCounterLimit.Text = "CopyCounterLimit(*5sec)";
            // 
            // UpdateCounterLimitTextBox
            // 
            this.UpdateCounterLimitTextBox.Location = new System.Drawing.Point(10, 153);
            this.UpdateCounterLimitTextBox.Name = "UpdateCounterLimitTextBox";
            this.UpdateCounterLimitTextBox.Size = new System.Drawing.Size(404, 20);
            this.UpdateCounterLimitTextBox.TabIndex = 8;
            // 
            // UpdateCounterLimit
            // 
            this.UpdateCounterLimit.AutoSize = true;
            this.UpdateCounterLimit.Location = new System.Drawing.Point(7, 137);
            this.UpdateCounterLimit.Name = "UpdateCounterLimit";
            this.UpdateCounterLimit.Size = new System.Drawing.Size(133, 13);
            this.UpdateCounterLimit.TabIndex = 7;
            this.UpdateCounterLimit.Text = "UpdateCounterLimit(*5sec)";
            // 
            // VenturFileTextBox
            // 
            this.VenturFileTextBox.Location = new System.Drawing.Point(10, 114);
            this.VenturFileTextBox.Name = "VenturFileTextBox";
            this.VenturFileTextBox.Size = new System.Drawing.Size(315, 20);
            this.VenturFileTextBox.TabIndex = 6;
            // 
            // VenturFile
            // 
            this.VenturFile.AutoSize = true;
            this.VenturFile.Location = new System.Drawing.Point(7, 98);
            this.VenturFile.Name = "VenturFile";
            this.VenturFile.Size = new System.Drawing.Size(54, 13);
            this.VenturFile.TabIndex = 5;
            this.VenturFile.Text = "VenturFile";
            // 
            // InitialDestPathTextBox
            // 
            this.InitialDestPathTextBox.Location = new System.Drawing.Point(10, 75);
            this.InitialDestPathTextBox.Name = "InitialDestPathTextBox";
            this.InitialDestPathTextBox.Size = new System.Drawing.Size(315, 20);
            this.InitialDestPathTextBox.TabIndex = 4;
            // 
            // InitialDestPath
            // 
            this.InitialDestPath.AutoSize = true;
            this.InitialDestPath.Location = new System.Drawing.Point(7, 59);
            this.InitialDestPath.Name = "InitialDestPath";
            this.InitialDestPath.Size = new System.Drawing.Size(75, 13);
            this.InitialDestPath.TabIndex = 3;
            this.InitialDestPath.Text = "InitialDestPath";
            // 
            // InitialSeansesPathTextBox
            // 
            this.InitialSeansesPathTextBox.Location = new System.Drawing.Point(10, 36);
            this.InitialSeansesPathTextBox.Name = "InitialSeansesPathTextBox";
            this.InitialSeansesPathTextBox.Size = new System.Drawing.Size(315, 20);
            this.InitialSeansesPathTextBox.TabIndex = 2;
            // 
            // InitialSeansesPath
            // 
            this.InitialSeansesPath.AutoSize = true;
            this.InitialSeansesPath.Location = new System.Drawing.Point(7, 20);
            this.InitialSeansesPath.Name = "InitialSeansesPath";
            this.InitialSeansesPath.Size = new System.Drawing.Size(94, 13);
            this.InitialSeansesPath.TabIndex = 0;
            this.InitialSeansesPath.Text = "InitialSeansesPath";
            // 
            // EmptySeansesTrashold
            // 
            this.EmptySeansesTrashold.AutoSize = true;
            this.EmptySeansesTrashold.Location = new System.Drawing.Point(7, 298);
            this.EmptySeansesTrashold.Name = "EmptySeansesTrashold";
            this.EmptySeansesTrashold.Size = new System.Drawing.Size(118, 13);
            this.EmptySeansesTrashold.TabIndex = 20;
            this.EmptySeansesTrashold.Text = "EmptySeansesTrashold";
            // 
            // EmptySeansesTrasholdUpDown
            // 
            this.EmptySeansesTrasholdUpDown.Location = new System.Drawing.Point(331, 296);
            this.EmptySeansesTrasholdUpDown.Name = "EmptySeansesTrasholdUpDown";
            this.EmptySeansesTrasholdUpDown.Size = new System.Drawing.Size(83, 20);
            this.EmptySeansesTrasholdUpDown.TabIndex = 21;
            // 
            // CopyLengthTrashold
            // 
            this.CopyLengthTrashold.AutoSize = true;
            this.CopyLengthTrashold.Location = new System.Drawing.Point(7, 324);
            this.CopyLengthTrashold.Name = "CopyLengthTrashold";
            this.CopyLengthTrashold.Size = new System.Drawing.Size(136, 13);
            this.CopyLengthTrashold.TabIndex = 22;
            this.CopyLengthTrashold.Text = "CopyLengthTrashold(bytes)";
            // 
            // CopyPercentTrashold
            // 
            this.CopyPercentTrashold.AutoSize = true;
            this.CopyPercentTrashold.Location = new System.Drawing.Point(7, 350);
            this.CopyPercentTrashold.Name = "CopyPercentTrashold";
            this.CopyPercentTrashold.Size = new System.Drawing.Size(151, 13);
            this.CopyPercentTrashold.TabIndex = 24;
            this.CopyPercentTrashold.Text = "CopyPercentTrashold(percent)";
            // 
            // CopyLengthTrasholdUpDown
            // 
            this.CopyLengthTrasholdUpDown.Location = new System.Drawing.Point(331, 322);
            this.CopyLengthTrasholdUpDown.Name = "CopyLengthTrasholdUpDown";
            this.CopyLengthTrasholdUpDown.Size = new System.Drawing.Size(83, 20);
            this.CopyLengthTrasholdUpDown.TabIndex = 25;
            // 
            // CopyPercentTrasholdUpDown
            // 
            this.CopyPercentTrasholdUpDown.Location = new System.Drawing.Point(331, 348);
            this.CopyPercentTrasholdUpDown.Name = "CopyPercentTrasholdUpDown";
            this.CopyPercentTrasholdUpDown.Size = new System.Drawing.Size(83, 20);
            this.CopyPercentTrasholdUpDown.TabIndex = 26;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 465);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.CanselBtn);
            this.Controls.Add(this.OkBtn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.Text = "Настройки";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EmptySeansesTrasholdUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CopyLengthTrasholdUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CopyPercentTrasholdUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button OkBtn;
        private System.Windows.Forms.Button CanselBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox MinutesToAwaitAfterEndTextBox;
        private System.Windows.Forms.Label MinetsToAwaitAfterEnd;
        private System.Windows.Forms.TextBox SmoothValueTextBox;
        private System.Windows.Forms.Label SmoothValue;
        private System.Windows.Forms.TextBox IntervalsKTextBox;
        private System.Windows.Forms.Label IntervalsK;
        private System.Windows.Forms.TextBox AbonentsKTextBox;
        private System.Windows.Forms.Label AbonentsK;
        private System.Windows.Forms.TextBox WorkingChartIntervalTextBox;
        private System.Windows.Forms.Label WorkingChartInterval;
        private System.Windows.Forms.TextBox SynchronizeCounterLimitTextBox;
        private System.Windows.Forms.Label SynchronizeCounterLimit;
        private System.Windows.Forms.TextBox CopyCounterLimitTextBox;
        private System.Windows.Forms.Label CopyCounterLimit;
        private System.Windows.Forms.TextBox UpdateCounterLimitTextBox;
        private System.Windows.Forms.Label UpdateCounterLimit;
        private System.Windows.Forms.TextBox VenturFileTextBox;
        private System.Windows.Forms.Label VenturFile;
        private System.Windows.Forms.TextBox InitialDestPathTextBox;
        private System.Windows.Forms.Label InitialDestPath;
        private System.Windows.Forms.TextBox InitialSeansesPathTextBox;
        private System.Windows.Forms.Label InitialSeansesPath;
        private System.Windows.Forms.Button VenturFileExplore;
        private System.Windows.Forms.Button InitialDestPathExplore;
        private System.Windows.Forms.Button InitialSeansesPathExplore;
        private System.Windows.Forms.CheckBox HideEmptySeanses;
        private System.Windows.Forms.Label EmptySeansesTrashold;
        private System.Windows.Forms.NumericUpDown CopyPercentTrasholdUpDown;
        private System.Windows.Forms.NumericUpDown CopyLengthTrasholdUpDown;
        private System.Windows.Forms.Label CopyPercentTrashold;
        private System.Windows.Forms.Label CopyLengthTrashold;
        private System.Windows.Forms.NumericUpDown EmptySeansesTrasholdUpDown;
    }
}