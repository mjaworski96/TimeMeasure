using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace TimeMeasure.Model
{
    public class TimePeriodContainer
    {
        private List<TimePeriod> periods;
        private const string FILE_NAME = "app.data.txt";
        public TimePeriodContainer()
        {
            periods = new List<TimePeriod>();
            Load();
        }
        public string FileName { get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), FILE_NAME); }
        public void Load()
        {
            using (StreamReader reader = new StreamReader(new FileStream(
                FileName, FileMode.OpenOrCreate)))
            {
                while(!reader.EndOfStream)
                {
                    periods.Add(TimePeriod.Load(reader));
                }
            }
        }
        public void Save()
        {
            using (StreamWriter writer = new StreamWriter(FileName))
            {
                foreach (var item in periods)
                {
                    item.Save(writer);
                }
            }
        }
        public void Clear()
        {
            periods.Clear();
            Save();
        }
        public bool IsActive
        {
            get
            {
                return periods.LastOrDefault()?.Active ?? false;
            }
        }
        public void PerformClick()
        {
            if (IsActive)
                FinishLastPeriod();
            else
                StartNewPeriod();
            Save();
        }
        public void StartNewPeriod()
        {
            periods.Add(TimePeriod.BeginNew());
        }
        public void FinishLastPeriod()
        {
            periods.Last().Finish();
        }
        public TimeSpan TotalDuration
        {
            get
            {
                TimeSpan timeSpan = TimeSpan.Zero;
                foreach (var item in periods)
                {
                    timeSpan += item.Duration;
                }
                return timeSpan;
            }
        }
        private int GetWeekOfYear(DateTime date)
        {
            DateTimeFormatInfo dateTimeFormatInfo = DateTimeFormatInfo.CurrentInfo;
            Calendar calendar = dateTimeFormatInfo.Calendar;

            return calendar
                .GetWeekOfYear(date, dateTimeFormatInfo.CalendarWeekRule,
                            dateTimeFormatInfo.FirstDayOfWeek);
            
        }
        public TimeSpan DayTotalDuration
        {
            get
            {
                TimeSpan timeSpan = TimeSpan.Zero;
                DateTime today = DateTime.Now.Date;
                foreach (var item in periods.NormalizeTimePeriods().Where(x =>
                    x.StartDate.Date == today))
                {
                    timeSpan += item.Duration;
                }
                return timeSpan;
            }
        }
        public TimeSpan WeekTotalDuration
        {
            get
            {
                TimeSpan timeSpan = TimeSpan.Zero;
                DateTime now = DateTime.Now;
                int currentWeekOfYear = GetWeekOfYear(now);
                foreach (var item in periods.NormalizeTimePeriods().Where(x => 
                    GetWeekOfYear(x.StartDate) == currentWeekOfYear 
                    && x.StartDate.Year == now.Year))
                {
                    timeSpan += item.Duration;
                }
                return timeSpan;
            }
        }
        public TimeSpan MonthTotalDuration
        {
            get
            {
                TimeSpan timeSpan = TimeSpan.Zero;
                int currentMonth = DateTime.Now.Month;
                foreach (var item in periods.NormalizeTimePeriods().Where(x =>
                    x.StartDate.Month == currentMonth))
                {
                    timeSpan += item.Duration;
                }
                return timeSpan;
            }
        }

        internal void Delete(TimePeriod period)
        {
            periods.Remove(period);
            Save();
        }

        public List<TimePeriod> Periods { get => periods; }
    }
}
