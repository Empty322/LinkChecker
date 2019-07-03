using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Link11Checker.ViewModels.Base;
using Link11Checker.Core;


namespace Link11Checker.ViewModels
{
    public class SeanseViewModel : BaseViewModel
    {                 
        public Mode Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                OnPropertyChanged("Mode");
            }
        }

        public int MaxSize
        {
            get { return maxSize; }
            set
            {
                maxSize = value;
                OnPropertyChanged("MaxSize");
            }
        }

        public float AverageSize
        {
            get { return (float)Math.Round(averageSize, 2); }
            set
            {
                averageSize = value;
                OnPropertyChanged("AverageSize");
            }
        }

        public string LastActiveTime
        {
            get { return lastActiveTime.ToShortTimeString(); }
            set
            {
                lastActiveTime = DateTime.Parse(value);
                OnPropertyChanged("LastActiveTime");
            }
        }
        public string LastWorkingTime
        {
            get { return lastWorkingTime.ToShortTimeString(); }
            set
            {
                lastWorkingTime = DateTime.Parse(value);
                OnPropertyChanged("LastWorkingTime");
            }
        }

        public string Position
        {
            get { return position; }
            set
            {
                position = value;
                OnPropertyChanged("Position");
            }
        }
        public string Coordinates { 
            get {return coordinates}  
            set {
                coordinates = value;
                OnPropertyChanged("Coordinates");
            } }
        public float Freq
        {
            get { return freq; }
            set
            {
                freq = value;
                OnPropertyChanged("Freq");
            }
        }

        private Mode mode;
        private int maxSize;
        private float averageSize;
        private DateTime lastActiveTime;
        private DateTime lastWorkingTime;
        private string position;
        private string coordinates;
        private float freq;
    }
}
