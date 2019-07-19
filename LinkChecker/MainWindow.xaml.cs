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

            if (!File.Exists("settings.json"))
                throw new FileNotFoundException();
            string settingsFile = File.ReadAllText("settings.json", Encoding.Default);
            Settings settings = JsonConvert.DeserializeObject<Settings>(settingsFile);

            if (!File.Exists("configuration.json"))
                throw new FileNotFoundException();
            string configFile = File.ReadAllText("configuration.json", Encoding.Default);
            settings.Configuration = JsonConvert.DeserializeObject<Configuration>(configFile);

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
    }
}
