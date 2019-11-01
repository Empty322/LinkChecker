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
        public int Calls
        {
            get
            {
                return calls;
            }
            set
            {
                calls = value;
                OnPropertyChanged("Calls");
            }
        }
        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                OnPropertyChanged("Size");
            }
        }

        public List<Interval> Intervals { get; private set; }

        private int calls;
        private int size;

        public AbonentInfo(int name)
        {
            this.Intervals = new List<Interval>();
            this.Name = name;
            this.calls = 0;
            this.size = 0;
        }

        public void UpdateIntervals(Dictionary<int, int> inAbonentIntervals)
        {
            Intervals.Clear();
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
            return this.Size - other.Size;
        }
    }

    public struct Interval
    {
        public int Name { get; set; }
        public int Count { get; set; }
    }
}
