using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Link11.Core
{
    public class AbonentInfo : INotifyPropertyChanged, IComparable<AbonentInfo>
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public int Name { get; set; }
        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
                OnPropertyChanged("Count");
            }
        }
        public ReadOnlyDictionary<int, int> Intervals { get; set; }

        private Dictionary<int, int> intervals;
        private int count;

        public AbonentInfo(int name)
        {
            this.Name = name;
            this.intervals = new Dictionary<int, int>();
            this.Intervals = new ReadOnlyDictionary<int, int>(intervals);
        }

        public void UpdateIntervals(Dictionary<int, int> abonentIntervals)
        {
            foreach (var interval in abonentIntervals)
            {
                if (Intervals.Keys.Contains(interval.Key))
                    intervals[interval.Key] = interval.Value;
                else
                    intervals.Add(interval.Key, interval.Value);
            }
        }


        private void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public int CompareTo(AbonentInfo other)
        {
            return this.Count - other.Count;
        }
    }
}
