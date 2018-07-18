using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TimeMeasure.Model
{
    public class TimePeriod
    {
        public const string DATE_FORMAT = "dd.MM.yyyy HH:mm:ss";
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan Duration
        {
            get
            {
                if(Active)
                    return DateTime.Now - StartDate;
                else
                    return EndDate.Value - StartDate;          
            }
        }
        public bool Active
        {
            get
            {
                return !EndDate.HasValue;
            }
        }
        public TimePeriod()
        {
        }

        public TimePeriod(DateTime startDate)
        {
            StartDate = startDate;
        }

        public TimePeriod(DateTime startDate, DateTime? endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public static TimePeriod BeginNew()
        {
            return new TimePeriod(DateTime.Now);
        }
        public void Finish()
        {
            EndDate = DateTime.Now;
        }
        public void Save(StreamWriter writer)
        {
            WriteDate(writer, StartDate);
            if (!Active)
                WriteDate(writer, EndDate.Value);
        }
        public static TimePeriod Load(StreamReader reader)
        {
            return new TimePeriod(ReadDate(reader).Value, ReadDate(reader));
        }

        private static void WriteDate(StreamWriter writer, DateTime date)
        {
            writer.WriteLine(date.ToString(DATE_FORMAT));
        }
        private static DateTime? ReadDate(StreamReader reader)
        {
            if (!reader.EndOfStream)
            {
                string date = reader.ReadLine();
                return DateTime.ParseExact(date, DATE_FORMAT, null);
            }
            return null; 
        }
    }
}
