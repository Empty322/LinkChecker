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
using Link11Checker.ViewModels;
using Logger;
using Link11Checker.Core;

namespace Link11Checker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            ILogger logger = new PrimitiveLogger("log.txt", LogLevel.Error);
            InitializeComponent();
            DataContext = new WindowViewModel(new SeanseManager(""), "0.4.0", logger);
        }
    }
}
