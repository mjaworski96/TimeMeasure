using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TimeMeasure
{
    public static class ExtensionMethods
    {
        public static  ObservableCollection<T> ToObervableCollection<T>(this IEnumerable<T> enumerable)
        {
            ObservableCollection<T> observableCollection = new ObservableCollection<T>();
            foreach (var item in enumerable)
            {
                observableCollection.Add(item);
            }
            return observableCollection;
        }
    }
}
