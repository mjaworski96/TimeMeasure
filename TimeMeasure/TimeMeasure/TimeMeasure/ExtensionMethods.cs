using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TimeMeasure.Model;

namespace TimeMeasure
{
    public static class ExtensionMethods
    {
        public static ObservableCollection<T> ToObervableCollection<T>(this IEnumerable<T> enumerable)
        {
            ObservableCollection<T> observableCollection = new ObservableCollection<T>();
            foreach (var item in enumerable)
            {
                observableCollection.Add(item);
            }
            return observableCollection;
        }
        public static IEnumerable<TimePeriod> NormalizeTimePeriods(this IEnumerable<TimePeriod> periods)
        {
            foreach (var period in periods)
            {
                DateTime start = period.StartDate;
                DateTime end = period.EndDate.HasValue ? period.EndDate.Value : DateTime.Now;
                DateTime nextDay = start.AddDays(1);
                DateTime currentEnd = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day);
                while (start.Date <= end.Date)
                {
                    if (currentEnd > end)
                    {
                        yield return new TimePeriod(start, end);
                        break;
                    }               
                    else
                    {
                        yield return new TimePeriod(start, currentEnd);

                        nextDay = start.AddDays(1);
                        start = currentEnd;
                        currentEnd = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day);
                    }
                    
                }
            }
        }
    }
}
