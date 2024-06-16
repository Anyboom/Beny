using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Beny.Collections
{
    public class ItemObservableCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        public event EventHandler<PropertyChangedEventArgs> ItemPropertyChanged;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            base.OnCollectionChanged(args);

            if (args.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in args.NewItems)
                {
                    item.PropertyChanged += OnItemPropertyChanged;
                }
            }

            if (args.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in args.OldItems)
                {
                    item.PropertyChanged -= OnItemPropertyChanged;
                }
            }
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            ItemPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(args.PropertyName));
        }
    }
}
