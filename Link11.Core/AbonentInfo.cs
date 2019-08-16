using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace Link11.Core
{
    public class AbonentInfo : INotifyPropertyChanged, IComparable<AbonentInfo>
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public int Name { get; private set; }
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
        public List<Interval> Intervals { get; private set; }

        private int count;

        public AbonentInfo(int name)
        {
            this.Intervals = new List<Interval>();
            this.Name = name;
            this.count = 0;
        }

        public void UpdateIntervals(Dictionary<int, int> inAbonentIntervals)
        {
            foreach (var inInterval in inAbonentIntervals)
            {
                Interval newInterval = new Interval();
                newInterval.Name = inInterval.Key;
                newInterval.Count = inInterval.Value;
                Intervals.Add(newInterval);
            }
            Intervals = Intervals.OrderByDescending(i => i.Count).ToList();
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

    public struct Interval
    {
        public int Name { get; set; }
        public int Count { get; set; }
    }
}
