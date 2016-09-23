using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace STAR.ViewModel {
    // Extension method modified from the following stackoverflow answer:
    // http://stackoverflow.com/a/8607159
    class RangeObservableCollection<T> : ObservableCollection<T> {
        public void AddRange(IEnumerable<T> items) {
            CheckReentrancy();
            foreach(T item in items) {
                Items.Add(item);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset)
            );
        }
    }
}
