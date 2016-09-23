using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAR.Extensions {
    // Extension method modified from the following stackoverflow answer:
    // http://stackoverflow.com/a/8607159
    class RangeObservableCollection<T> : ObservableCollection<T> {
        public void AddRange(IEnumerable<T> items) {
            this.CheckReentrancy();
            foreach(T item in items) {
                this.Items.Add(item);
            }
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset)
            );
        }
    }
}
