using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Link11.Core.Enums;
using System.Windows.Media;

namespace Link11Checker.Converters
{
    public class StateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush resultColor = new SolidColorBrush();
            switch ((SeanseState)value)
            {
                case SeanseState.Active:
                    resultColor.Color = Color.FromArgb(200 ,255, 0, 0);
                    break;
                case SeanseState.WorkingLevel0:
                    resultColor.Color = Color.FromRgb(255, 255, 255);
                    break;
                case SeanseState.WorkingLevel1:
                    resultColor.Color = Color.FromArgb(40, 0, 255, 0);
                    break;
                case SeanseState.WorkingLevel2:
                    resultColor.Color = Color.FromArgb(80, 0, 255, 0);
                    break;
                case SeanseState.WorkingLevel3:
                    resultColor.Color = Color.FromArgb(130, 0, 255, 0);
                    break;
                case SeanseState.WorkingLevel4:
                    resultColor.Color = Color.FromArgb(200, 0, 255, 0);
                    break;
                case SeanseState.WorkingLevel5:
                    resultColor.Color = Color.FromArgb(240, 0, 255, 0);
                    break;
            }
            return resultColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
