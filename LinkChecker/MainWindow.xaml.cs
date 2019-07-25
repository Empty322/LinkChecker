using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Link11Checker.ViewModels;
using Logger;
using Link11Checker.Core;
using Newtonsoft.Json;
using Link11.Core;
using System.Windows.Forms.DataVisualization.Charting;

namespace Link11Checker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            #region Loading settings

            Settings settings = new Settings();
            if (File.Exists("settings.json"))
            {
                string settingsFile = File.ReadAllText("settings.json", Encoding.Default);
                settings = JsonConvert.DeserializeObject<Settings>(settingsFile);
            }
            else
            {
                MessageBox.Show("Создан новый файл со стандартными настройками" ,"Не удалось загрузить файл настроек", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Configuration cfg = new Configuration
                {
                    AbonentsK = 0.15f,
                    IntervalsK = 0.2f,
                    SmoothValue = 5,
                    MinutesToAwaitAfterEnd = 15
                };
                settings.Configuration = cfg;

                settings.InitialSeansesPath = "";
                settings.InitialDestPath = "";
                settings.VenturFile = "";
                settings.UpdateCounterLimit = 1;
                settings.CopyCounterLimit = 180;
                settings.SynchronizeCounterLimit = 5;
                settings.WorkingChartInterval = 5;
                settings.HideEmptySeanses = false;

                string settingsFile = JsonConvert.SerializeObject(settings);
                File.WriteAllText("settings.json", settingsFile, Encoding.Default);
            }

            #endregion

            ILogger logger = new PrimitiveLogger("log.txt", LogLevel.Error);
            InitializeComponent();
            DataContext = new WindowViewModel(this, new SeanseManager(settings, new Parser(), logger), settings, logger);
        }

        public Chart GetTuningChart()
        {
            return tuningChart;
        }

        public Chart GetWorkingChart()
        {
            return workingChart;
        }

        public Chart GetSizeChart()
        {
            return sizeChart;
        }
    }
}
