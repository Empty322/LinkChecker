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
        private Settings settings;
        private ILogger logger;
        public SettingsForm(Settings settings, ILogger logger)
        {
            this.settings = settings;
            this.logger = logger;
            InitializeComponent();
            AbonentsK.Text = "Считать корреспондентов с количеством \nвхождений превышающим процент от количества \nвхождений самого частого корреспондента:";
            IntervalsK.Text = "Cчитать корреспондентов с определенным итнервалом, \nколичество вхождений которого превышает процент от количества \nвхождений всех остальных интервалов корреспондента:";
            SmoothValue.Text = "Усреднение расстройки:";
            MinetsToAwaitAfterEnd.Text = "Время ожидания конца сигнала в минутах:";

            InitialSeansesPath.Text = "Начальная папка для добавления сеансов:";
            InitialDestPath.Text = "Начальная папка для накопления:";
            VenturFile.Text = "last.lf файл:";
            UpdateCounterLimit.Text = "Счетчик для таймера обновления (*5сек):";
            CopyCounterLimit.Text = "Счетчик для таймера копирования (*5сек):";
            SynchronizeCounterLimit.Text = "Счетчик для таймера синхронизации (*5сек):";
            WorkingChartInterval.Text = "Интервал для оси Х графика работы:";
            EmptySeansesTrashold.Text = "Максимальное количество НЕ ошибок в скрытых сеансах";
            CopyLengthTrashold.Text = "Минимальный размер копируемых сеансов (байты)";
            CopyPercentTrashold.Text = "Минимальный процент ошибок копируемых сеансов";

            AbonentsKTextBox.Text = settings.Configuration.AbonentsK.ToString();
            IntervalsKTextBox.Text = settings.Configuration.IntervalsK.ToString();
            SmoothValueTextBox.Text = settings.Configuration.SmoothValue.ToString();
            MinutesToAwaitAfterEndTextBox.Text = settings.Configuration.MinutesToAwaitAfterEnd.ToString();

            InitialSeansesPathTextBox.Text = settings.InitialSeansesPath;
            InitialDestPathTextBox.Text = settings.InitialDestPath;
            VenturFileTextBox.Text = settings.VenturFile;
            UpdateCounterLimitTextBox.Text = settings.UpdateCounterLimit.ToString();
            CopyCounterLimitTextBox.Text = settings.CopyCounterLimit.ToString();
            SynchronizeCounterLimitTextBox.Text = settings.SynchronizeCounterLimit.ToString();
            WorkingChartIntervalTextBox.Text = settings.WorkingChartInterval.ToString();
            HideEmptySeanses.Checked = settings.HideEmptySeanses;

            EmptySeansesTrasholdUpDown.Minimum = 0;
            EmptySeansesTrasholdUpDown.Maximum = decimal.MaxValue;
            EmptySeansesTrasholdUpDown.Value = settings.Configuration.Trashold < 0 ? 0 : settings.Configuration.Trashold;

            CopyLengthTrasholdUpDown.Minimum = 0;
            CopyLengthTrasholdUpDown.Maximum = decimal.MaxValue;
            CopyLengthTrasholdUpDown.Value = settings.Configuration.CopyLengthTrashold < 0 ? 0 : settings.Configuration.CopyLengthTrashold;

            CopyPercentTrasholdUpDown.Minimum = 0;
            CopyPercentTrasholdUpDown.Value = settings.Configuration.CopyPercentTrashold < 0 ? 0 : settings.Configuration.CopyPercentTrashold;
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
                    AbonentsK = Convert.ToDouble(AbonentsKTextBox.Text),
                    IntervalsK = Convert.ToDouble(IntervalsKTextBox.Text),
                    SmoothValue = Convert.ToInt32(SmoothValueTextBox.Text),
                    MinutesToAwaitAfterEnd = Convert.ToInt32(MinutesToAwaitAfterEndTextBox.Text),

                    Trashold = EmptySeansesTrasholdUpDown.Value,
                    CopyLengthTrashold = CopyLengthTrasholdUpDown.Value,
                    CopyPercentTrashold = (int)CopyPercentTrasholdUpDown.Value
                };
                newSettings.Configuration = cfg;

                newSettings.InitialSeansesPath = InitialSeansesPathTextBox.Text;
                newSettings.InitialDestPath = InitialDestPathTextBox.Text;
                newSettings.VenturFile = VenturFileTextBox.Text;
                newSettings.UpdateCounterLimit = Convert.ToInt32(UpdateCounterLimitTextBox.Text);
                newSettings.CopyCounterLimit = Convert.ToInt32(CopyCounterLimitTextBox.Text);
                newSettings.SynchronizeCounterLimit = Convert.ToInt32(SynchronizeCounterLimitTextBox.Text);
                newSettings.WorkingChartInterval = Convert.ToInt32(WorkingChartIntervalTextBox.Text);
                newSettings.HideEmptySeanses = HideEmptySeanses.Checked;

                string settingsFile = JsonConvert.SerializeObject(newSettings);
                File.WriteAllText("settings.json", settingsFile, Encoding.Default);

                result = true;
                MessageBox.Show("Для того, чтобы изменения вступили с силу, необходимо перезапустить программу", 
                    "Успешное изменение настроек", MessageBoxButtons.OK, MessageBoxIcon.Information);                
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
            if (Directory.Exists(settings.InitialSeansesPath))
                fbd.SelectedPath = settings.InitialSeansesPath;
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                settings.InitialSeansesPath = InitialSeansesPathTextBox.Text = fbd.SelectedPath;
            }
        }

        private void InitialDestPathExplore_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (Directory.Exists(settings.InitialDestPath))
                fbd.SelectedPath = settings.InitialDestPath;
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                settings.InitialDestPath = InitialDestPathTextBox.Text = fbd.SelectedPath;
            }
        }

        private void VenturFileExplore_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (Directory.Exists(settings.InitialDestPath))
                ofd.FileName = settings.VenturFile;
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(ofd.FileName))
            {
                settings.VenturFile = VenturFileTextBox.Text = ofd.FileName;
            }
        }
    }
}
