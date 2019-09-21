using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using Link11.Core;
using Link11Checker.Core;
using Logger;

namespace Link11Checker
{
    public partial class SettingsForm : Form
    {
        private ILogger logger;
        public SettingsForm(ILogger logger)
        {
            this.logger = logger;
            InitializeComponent();
            AbonentsK.Text = "Считать корреспондентов с количеством \nвхождений превышающим процент от количества \nвхождений самого частого корреспондента:";
            IntervalsK.Text = "Cчитать корреспондентов с определенным итнервалом, \nколичество вхождений которого превышает процент от \nколичества вхождений всех остальных интервалов \nкорреспондента:";
            SmoothValue.Text = "Усреднение расстройки:";
            MinetsToAwaitAfterEnd.Text = "Время ожидания конца сигнала (мин):";

            InitialSeansesPath.Text = "Начальная папка для добавления сеансов:";
            InitialDestPath.Text = "Начальная папка для накопления:";
            VenturFile.Text = "last.lf файл:";
            UpdateCounterLimit.Text = "Счетчик для таймера обновления (сек):";
            CopyCounterLimit.Text = "Счетчик для таймера копирования (сек):";
            SynchronizeCounterLimit.Text = "Счетчик для таймера синхронизации (сек):";
            WorkingChartInterval.Text = "Интервал для оси Х графика работы:";
            EmptySeansesTrashold.Text = "Максимальное количество НЕ ошибок в скрытых сеансах";
            CopyLengthTrashold.Text = "Минимальный размер копируемых сеансов (байты)";
            CopyPercentTrashold.Text = "Минимальный процент ошибок копируемых сеансов";

            AbonentsKUpDown.Value = (decimal)IoCContainer.Settings.Configuration.AbonentsK * 100;
            IntervalsKUpDown.Value = (decimal)IoCContainer.Settings.Configuration.IntervalsK * 100;
            SmoothValueUpDown.Value = IoCContainer.Settings.Configuration.SmoothValue;
            MinutesToAwaitAfterEndUpDown.Value = IoCContainer.Settings.Configuration.MinutesToAwaitAfterEnd;

            InitialSeansesPathTextBox.Text = IoCContainer.Settings.InitialSeansesPath;
            InitialDestPathTextBox.Text = IoCContainer.Settings.InitialDestPath;
            foreach (string file in IoCContainer.Settings.LastFiles) 
            {
                LastFilesListBox.Items.Add(file);
            }
            UpdateCounterLimitUpDown.Value = IoCContainer.Settings.UpdateCounterLimit;
            CopyCounterLimitUpDown.Value = IoCContainer.Settings.CopyCounterLimit;
            SynchronizeCounterLimitUpDown.Value = IoCContainer.Settings.SynchronizeCounterLimit;
            WorkingChartIntervalUpDown.Value = IoCContainer.Settings.WorkingChartInterval;
            HideEmptySeanses.Checked = IoCContainer.Settings.Configuration.HideEmptySeanses;

            EmptySeansesTrasholdUpDown.Minimum = 0;
            EmptySeansesTrasholdUpDown.Maximum = decimal.MaxValue;
            EmptySeansesTrasholdUpDown.Value = IoCContainer.Settings.Configuration.Trashold < 0 ? 0 : IoCContainer.Settings.Configuration.Trashold;

            CopyLengthTrasholdUpDown.Minimum = 0;
            CopyLengthTrasholdUpDown.Maximum = decimal.MaxValue;
            CopyLengthTrasholdUpDown.Value = IoCContainer.Settings.Configuration.CopyLengthTrashold < 0 ? 0 : IoCContainer.Settings.Configuration.CopyLengthTrashold;

            CopyPercentTrasholdUpDown.Minimum = 0;
            CopyPercentTrasholdUpDown.Value = IoCContainer.Settings.Configuration.CopyPercentTrashold < 0 ? 0 : IoCContainer.Settings.Configuration.CopyPercentTrashold;
        }

        private void CanselBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            if (SaveSettings())
                this.Close();
        }

        private bool SaveSettings()
        {
            bool result = false;
            try
            {
                Settings newSettings = new Settings();
                Configuration cfg = new Configuration
                {
                    AbonentsK = (double)AbonentsKUpDown.Value / 100,
                    IntervalsK = (double)IntervalsKUpDown.Value / 100,
                    SmoothValue = (int)SmoothValueUpDown.Value,
                    MinutesToAwaitAfterEnd = (int)MinutesToAwaitAfterEndUpDown.Value,
                    HideEmptySeanses = HideEmptySeanses.Checked,

                    Trashold = EmptySeansesTrasholdUpDown.Value,
                    CopyLengthTrashold = CopyLengthTrasholdUpDown.Value,
                    CopyPercentTrashold = (int)CopyPercentTrasholdUpDown.Value
                };
                newSettings.Configuration = cfg;

                newSettings.InitialSeansesPath = InitialSeansesPathTextBox.Text;
                newSettings.InitialDestPath = InitialDestPathTextBox.Text;
                List<string> lastFiles = new List<string>();
                foreach (var item in LastFilesListBox.Items)
                    lastFiles.Add(item.ToString());
                newSettings.LastFiles = lastFiles;
                newSettings.UpdateCounterLimit = (int)UpdateCounterLimitUpDown.Value;
                newSettings.CopyCounterLimit = (int)CopyCounterLimitUpDown.Value;
                newSettings.SynchronizeCounterLimit = (int)SynchronizeCounterLimitUpDown.Value;
                newSettings.WorkingChartInterval = (int)WorkingChartIntervalUpDown.Value;

                string settingsFile = JsonConvert.SerializeObject(newSettings);
                File.WriteAllText("settings.json", settingsFile, Encoding.Default);

                DialogResult dialogResult = MessageBox.Show("Применить настройки?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    IoCContainer.Settings = newSettings;
                    result = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Исключение", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.LogMessage(ex.Message, LogLevel.Error);
            }
            return result;
        }

        private void InitialSeansesPathExplore_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (Directory.Exists(IoCContainer.Settings.InitialSeansesPath))
                fbd.SelectedPath = IoCContainer.Settings.InitialSeansesPath;
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                IoCContainer.Settings.InitialSeansesPath = InitialSeansesPathTextBox.Text = fbd.SelectedPath;
            }
        }

        private void InitialDestPathExplore_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (Directory.Exists(IoCContainer.Settings.InitialDestPath))
                fbd.SelectedPath = IoCContainer.Settings.InitialDestPath;
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                IoCContainer.Settings.InitialDestPath = InitialDestPathTextBox.Text = fbd.SelectedPath;
            }
        }

        private void VenturFileExplore_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "last file (*.lf) | *.lf";
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(ofd.FileName))
            {
                LastFilesListBox.Items.Add(ofd.FileName);
            }
        }

        private void RemoveLastFileBtn_Click(object sender, EventArgs e)
        {
            if (LastFilesListBox.SelectedItem != null)
            {
                LastFilesListBox.Items.RemoveAt(LastFilesListBox.SelectedIndex);
            }
        }
    }
}
