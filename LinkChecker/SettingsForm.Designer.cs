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
            this.MinetsToAwaitAfterEnd = new System.Windows.Forms.Label();
            this.SmoothValue = new System.Windows.Forms.Label();
            this.IntervalsK = new System.Windows.Forms.Label();
            this.AbonentsK = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.SmoothValueUpDown = new System.Windows.Forms.NumericUpDown();
            this.WorkingChartIntervalUpDown = new System.Windows.Forms.NumericUpDown();
            this.IntervalsKUpDown = new System.Windows.Forms.NumericUpDown();
            this.SynchronizeCounterLimitUpDown = new System.Windows.Forms.NumericUpDown();
            this.AbonentsKUpDown = new System.Windows.Forms.NumericUpDown();
            this.MinutesToAwaitAfterEndUpDown = new System.Windows.Forms.NumericUpDown();
            this.CopyCounterLimitUpDown = new System.Windows.Forms.NumericUpDown();
            this.UpdateCounterLimitUpDown = new System.Windows.Forms.NumericUpDown();
            this.CopyPercentTrasholdUpDown = new System.Windows.Forms.NumericUpDown();
            this.CopyLengthTrasholdUpDown = new System.Windows.Forms.NumericUpDown();
            this.CopyPercentTrashold = new System.Windows.Forms.Label();
            this.CopyLengthTrashold = new System.Windows.Forms.Label();
            this.EmptySeansesTrasholdUpDown = new System.Windows.Forms.NumericUpDown();
            this.EmptySeansesTrashold = new System.Windows.Forms.Label();
            this.HideEmptySeanses = new System.Windows.Forms.CheckBox();
            this.VenturFileExplore = new System.Windows.Forms.Button();
            this.InitialDestPathExplore = new System.Windows.Forms.Button();
            this.InitialSeansesPathExplore = new System.Windows.Forms.Button();
            this.WorkingChartInterval = new System.Windows.Forms.Label();
            this.SynchronizeCounterLimit = new System.Windows.Forms.Label();
            this.CopyCounterLimit = new System.Windows.Forms.Label();
            this.UpdateCounterLimit = new System.Windows.Forms.Label();
            this.VenturFileTextBox = new System.Windows.Forms.TextBox();
            this.VenturFile = new System.Windows.Forms.Label();
            this.InitialDestPathTextBox = new System.Windows.Forms.TextBox();
            this.InitialDestPath = new System.Windows.Forms.Label();
            this.InitialSeansesPathTextBox = new System.Windows.Forms.TextBox();
            this.InitialSeansesPath = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SmoothValueUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WorkingChartIntervalUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.IntervalsKUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SynchronizeCounterLimitUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AbonentsKUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinutesToAwaitAfterEndUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CopyCounterLimitUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpdateCounterLimitUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CopyPercentTrasholdUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CopyLengthTrasholdUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EmptySeansesTrasholdUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // OkBtn
            // 
            resources.ApplyResources(this.OkBtn, "OkBtn");
            this.OkBtn.Name = "OkBtn";
            this.OkBtn.UseVisualStyleBackColor = true;
            this.OkBtn.Click += new System.EventHandler(this.OkBtn_Click);
            // 
            // CanselBtn
            // 
            this.CanselBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.CanselBtn, "CanselBtn");
            this.CanselBtn.Name = "CanselBtn";
            this.CanselBtn.UseVisualStyleBackColor = true;
            this.CanselBtn.Click += new System.EventHandler(this.CanselBtn_Click);
            // 
            // MinetsToAwaitAfterEnd
            // 
            resources.ApplyResources(this.MinetsToAwaitAfterEnd, "MinetsToAwaitAfterEnd");
            this.MinetsToAwaitAfterEnd.Name = "MinetsToAwaitAfterEnd";
            // 
            // SmoothValue
            // 
            resources.ApplyResources(this.SmoothValue, "SmoothValue");
            this.SmoothValue.Name = "SmoothValue";
            // 
            // IntervalsK
            // 
            resources.ApplyResources(this.IntervalsK, "IntervalsK");
            this.IntervalsK.Name = "IntervalsK";
            // 
            // AbonentsK
            // 
            resources.ApplyResources(this.AbonentsK, "AbonentsK");
            this.AbonentsK.Name = "AbonentsK";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.SmoothValueUpDown);
            this.groupBox2.Controls.Add(this.WorkingChartIntervalUpDown);
            this.groupBox2.Controls.Add(this.IntervalsKUpDown);
            this.groupBox2.Controls.Add(this.SynchronizeCounterLimitUpDown);
            this.groupBox2.Controls.Add(this.AbonentsKUpDown);
            this.groupBox2.Controls.Add(this.MinutesToAwaitAfterEndUpDown);
            this.groupBox2.Controls.Add(this.CopyCounterLimitUpDown);
            this.groupBox2.Controls.Add(this.MinetsToAwaitAfterEnd);
            this.groupBox2.Controls.Add(this.UpdateCounterLimitUpDown);
            this.groupBox2.Controls.Add(this.SmoothValue);
            this.groupBox2.Controls.Add(this.CopyPercentTrasholdUpDown);
            this.groupBox2.Controls.Add(this.IntervalsK);
            this.groupBox2.Controls.Add(this.CopyLengthTrasholdUpDown);
            this.groupBox2.Controls.Add(this.AbonentsK);
            this.groupBox2.Controls.Add(this.CopyPercentTrashold);
            this.groupBox2.Controls.Add(this.CopyLengthTrashold);
            this.groupBox2.Controls.Add(this.EmptySeansesTrasholdUpDown);
            this.groupBox2.Controls.Add(this.EmptySeansesTrashold);
            this.groupBox2.Controls.Add(this.HideEmptySeanses);
            this.groupBox2.Controls.Add(this.VenturFileExplore);
            this.groupBox2.Controls.Add(this.InitialDestPathExplore);
            this.groupBox2.Controls.Add(this.InitialSeansesPathExplore);
            this.groupBox2.Controls.Add(this.WorkingChartInterval);
            this.groupBox2.Controls.Add(this.SynchronizeCounterLimit);
            this.groupBox2.Controls.Add(this.CopyCounterLimit);
            this.groupBox2.Controls.Add(this.UpdateCounterLimit);
            this.groupBox2.Controls.Add(this.VenturFileTextBox);
            this.groupBox2.Controls.Add(this.VenturFile);
            this.groupBox2.Controls.Add(this.InitialDestPathTextBox);
            this.groupBox2.Controls.Add(this.InitialDestPath);
            this.groupBox2.Controls.Add(this.InitialSeansesPathTextBox);
            this.groupBox2.Controls.Add(this.InitialSeansesPath);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // SmoothValueUpDown
            // 
            resources.ApplyResources(this.SmoothValueUpDown, "SmoothValueUpDown");
            this.SmoothValueUpDown.Maximum = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.SmoothValueUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.SmoothValueUpDown.Name = "SmoothValueUpDown";
            this.SmoothValueUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // WorkingChartIntervalUpDown
            // 
            resources.ApplyResources(this.WorkingChartIntervalUpDown, "WorkingChartIntervalUpDown");
            this.WorkingChartIntervalUpDown.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.WorkingChartIntervalUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.WorkingChartIntervalUpDown.Name = "WorkingChartIntervalUpDown";
            this.WorkingChartIntervalUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // IntervalsKUpDown
            // 
            resources.ApplyResources(this.IntervalsKUpDown, "IntervalsKUpDown");
            this.IntervalsKUpDown.Maximum = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.IntervalsKUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.IntervalsKUpDown.Name = "IntervalsKUpDown";
            this.IntervalsKUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // SynchronizeCounterLimitUpDown
            // 
            resources.ApplyResources(this.SynchronizeCounterLimitUpDown, "SynchronizeCounterLimitUpDown");
            this.SynchronizeCounterLimitUpDown.Maximum = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.SynchronizeCounterLimitUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.SynchronizeCounterLimitUpDown.Name = "SynchronizeCounterLimitUpDown";
            this.SynchronizeCounterLimitUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // AbonentsKUpDown
            // 
            resources.ApplyResources(this.AbonentsKUpDown, "AbonentsKUpDown");
            this.AbonentsKUpDown.Maximum = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.AbonentsKUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.AbonentsKUpDown.Name = "AbonentsKUpDown";
            this.AbonentsKUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // MinutesToAwaitAfterEndUpDown
            // 
            resources.ApplyResources(this.MinutesToAwaitAfterEndUpDown, "MinutesToAwaitAfterEndUpDown");
            this.MinutesToAwaitAfterEndUpDown.Maximum = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.MinutesToAwaitAfterEndUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MinutesToAwaitAfterEndUpDown.Name = "MinutesToAwaitAfterEndUpDown";
            this.MinutesToAwaitAfterEndUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // CopyCounterLimitUpDown
            // 
            resources.ApplyResources(this.CopyCounterLimitUpDown, "CopyCounterLimitUpDown");
            this.CopyCounterLimitUpDown.Maximum = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.CopyCounterLimitUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.CopyCounterLimitUpDown.Name = "CopyCounterLimitUpDown";
            this.CopyCounterLimitUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // UpdateCounterLimitUpDown
            // 
            resources.ApplyResources(this.UpdateCounterLimitUpDown, "UpdateCounterLimitUpDown");
            this.UpdateCounterLimitUpDown.Maximum = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.UpdateCounterLimitUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.UpdateCounterLimitUpDown.Name = "UpdateCounterLimitUpDown";
            this.UpdateCounterLimitUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // CopyPercentTrasholdUpDown
            // 
            resources.ApplyResources(this.CopyPercentTrasholdUpDown, "CopyPercentTrasholdUpDown");
            this.CopyPercentTrasholdUpDown.Name = "CopyPercentTrasholdUpDown";
            // 
            // CopyLengthTrasholdUpDown
            // 
            resources.ApplyResources(this.CopyLengthTrasholdUpDown, "CopyLengthTrasholdUpDown");
            this.CopyLengthTrasholdUpDown.Maximum = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.CopyLengthTrasholdUpDown.Name = "CopyLengthTrasholdUpDown";
            // 
            // CopyPercentTrashold
            // 
            resources.ApplyResources(this.CopyPercentTrashold, "CopyPercentTrashold");
            this.CopyPercentTrashold.Name = "CopyPercentTrashold";
            // 
            // CopyLengthTrashold
            // 
            resources.ApplyResources(this.CopyLengthTrashold, "CopyLengthTrashold");
            this.CopyLengthTrashold.Name = "CopyLengthTrashold";
            // 
            // EmptySeansesTrasholdUpDown
            // 
            resources.ApplyResources(this.EmptySeansesTrasholdUpDown, "EmptySeansesTrasholdUpDown");
            this.EmptySeansesTrasholdUpDown.Maximum = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.EmptySeansesTrasholdUpDown.Name = "EmptySeansesTrasholdUpDown";
            // 
            // EmptySeansesTrashold
            // 
            resources.ApplyResources(this.EmptySeansesTrashold, "EmptySeansesTrashold");
            this.EmptySeansesTrashold.Name = "EmptySeansesTrashold";
            // 
            // HideEmptySeanses
            // 
            resources.ApplyResources(this.HideEmptySeanses, "HideEmptySeanses");
            this.HideEmptySeanses.Name = "HideEmptySeanses";
            this.HideEmptySeanses.UseVisualStyleBackColor = true;
            // 
            // VenturFileExplore
            // 
            resources.ApplyResources(this.VenturFileExplore, "VenturFileExplore");
            this.VenturFileExplore.Name = "VenturFileExplore";
            this.VenturFileExplore.UseVisualStyleBackColor = true;
            this.VenturFileExplore.Click += new System.EventHandler(this.VenturFileExplore_Click);
            // 
            // InitialDestPathExplore
            // 
            resources.ApplyResources(this.InitialDestPathExplore, "InitialDestPathExplore");
            this.InitialDestPathExplore.Name = "InitialDestPathExplore";
            this.InitialDestPathExplore.UseVisualStyleBackColor = true;
            this.InitialDestPathExplore.Click += new System.EventHandler(this.InitialDestPathExplore_Click);
            // 
            // InitialSeansesPathExplore
            // 
            resources.ApplyResources(this.InitialSeansesPathExplore, "InitialSeansesPathExplore");
            this.InitialSeansesPathExplore.Name = "InitialSeansesPathExplore";
            this.InitialSeansesPathExplore.UseVisualStyleBackColor = true;
            this.InitialSeansesPathExplore.Click += new System.EventHandler(this.InitialSeansesPathExplore_Click);
            // 
            // WorkingChartInterval
            // 
            resources.ApplyResources(this.WorkingChartInterval, "WorkingChartInterval");
            this.WorkingChartInterval.Name = "WorkingChartInterval";
            // 
            // SynchronizeCounterLimit
            // 
            resources.ApplyResources(this.SynchronizeCounterLimit, "SynchronizeCounterLimit");
            this.SynchronizeCounterLimit.Name = "SynchronizeCounterLimit";
            // 
            // CopyCounterLimit
            // 
            resources.ApplyResources(this.CopyCounterLimit, "CopyCounterLimit");
            this.CopyCounterLimit.Name = "CopyCounterLimit";
            // 
            // UpdateCounterLimit
            // 
            resources.ApplyResources(this.UpdateCounterLimit, "UpdateCounterLimit");
            this.UpdateCounterLimit.Name = "UpdateCounterLimit";
            // 
            // VenturFileTextBox
            // 
            resources.ApplyResources(this.VenturFileTextBox, "VenturFileTextBox");
            this.VenturFileTextBox.Name = "VenturFileTextBox";
            // 
            // VenturFile
            // 
            resources.ApplyResources(this.VenturFile, "VenturFile");
            this.VenturFile.Name = "VenturFile";
            // 
            // InitialDestPathTextBox
            // 
            resources.ApplyResources(this.InitialDestPathTextBox, "InitialDestPathTextBox");
            this.InitialDestPathTextBox.Name = "InitialDestPathTextBox";
            // 
            // InitialDestPath
            // 
            resources.ApplyResources(this.InitialDestPath, "InitialDestPath");
            this.InitialDestPath.Name = "InitialDestPath";
            // 
            // InitialSeansesPathTextBox
            // 
            resources.ApplyResources(this.InitialSeansesPathTextBox, "InitialSeansesPathTextBox");
            this.InitialSeansesPathTextBox.Name = "InitialSeansesPathTextBox";
            // 
            // InitialSeansesPath
            // 
            resources.ApplyResources(this.InitialSeansesPath, "InitialSeansesPath");
            this.InitialSeansesPath.Name = "InitialSeansesPath";
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.OkBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CanselBtn;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.CanselBtn);
            this.Controls.Add(this.OkBtn);
            this.Name = "SettingsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SmoothValueUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WorkingChartIntervalUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.IntervalsKUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SynchronizeCounterLimitUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AbonentsKUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinutesToAwaitAfterEndUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CopyCounterLimitUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpdateCounterLimitUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CopyPercentTrasholdUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CopyLengthTrasholdUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EmptySeansesTrasholdUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button OkBtn;
        private System.Windows.Forms.Button CanselBtn;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label MinetsToAwaitAfterEnd;
        private System.Windows.Forms.Label SmoothValue;
        private System.Windows.Forms.Label IntervalsK;
        private System.Windows.Forms.Label AbonentsK;
        private System.Windows.Forms.Label WorkingChartInterval;
        private System.Windows.Forms.Label SynchronizeCounterLimit;
        private System.Windows.Forms.Label CopyCounterLimit;
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
        private System.Windows.Forms.NumericUpDown WorkingChartIntervalUpDown;
        private System.Windows.Forms.NumericUpDown SynchronizeCounterLimitUpDown;
        private System.Windows.Forms.NumericUpDown CopyCounterLimitUpDown;
        private System.Windows.Forms.NumericUpDown UpdateCounterLimitUpDown;
        private System.Windows.Forms.NumericUpDown SmoothValueUpDown;
        private System.Windows.Forms.NumericUpDown IntervalsKUpDown;
        private System.Windows.Forms.NumericUpDown AbonentsKUpDown;
        private System.Windows.Forms.NumericUpDown MinutesToAwaitAfterEndUpDown;
    }
}