using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeMeasure.Model;
using Xamarin.Forms;

namespace TimeMeasure.ViewModel
{
    public class BindingContext : INotifyPropertyChanged
    {
        private const int UPDATE_WAIT_TIME = 1000;

        private IDialog dialogManager;
        private ViewModelPeriod lastPeriod;
        private TimePeriodContainer container;

        public event PropertyChangedEventHandler PropertyChanged;

        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            int hours = (int)timeSpan.TotalHours;
            return string.Format("{00}:{1:mm}:{1:ss}",
                hours < 10 ? "0" + hours : hours.ToString(), 
                timeSpan);
        }

        public BindingContext(IDialog dialog) :
            this()
        {
            dialogManager = dialog;
            container = new TimePeriodContainer();
            Task.Run(() => UpdateTimeThread());
        }
        public BindingContext()
        {
            container = new TimePeriodContainer();
        }
        public void Refresh()
        {
            container.Load();
        }
        public ObservableCollection<ViewModelPeriod> Periods
        {
            get
            {
                ObservableCollection<ViewModelPeriod> periods = container
                    .Periods
                    .Select(x => new ViewModelPeriod(x, this))
                    .Reverse()
                    .ToObervableCollection();

                lastPeriod = periods.FirstOrDefault();

                return periods;
            }
        }
        public string TotalTime
            { get => FormatTimeSpan(container.TotalDuration); }
        public string DayTotalTime
            { get => FormatTimeSpan(container.DayTotalDuration); }
        public string WeekTotalTime
            { get => FormatTimeSpan(container.WeekTotalDuration);}
        public string MonthTotalTime
            { get => FormatTimeSpan(container.MonthTotalDuration); }
        public string CurrentTotalTime
            { get => lastPeriod == null ? "00:00:00" : lastPeriod.Duration; }
        public string MainButtonText
            { get => container.IsActive ? "STOP" : "START"; }
        public ICommand MainButtonCommand
            { get => new DelegateCommand(MainButtonAction); }
        public ICommand ResetButtonCommand
            { get => new DelegateCommand(ResetButtonActionAsync); }

        private void MainButtonAction()
        {
            container.PerformClick();
            UpdateUI();
        }
        private async void ResetButtonActionAsync()
        {
            bool result = await dialogManager.YesNoDialog("Clear all data", "Do you want remove all data?");
            if (result)
            {
                container.Clear();
                UpdateUI();
            }
        }
        private void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void UpdateTimeThread()
        {
            while (true)
            {
                UpdateTime();
                Thread.Sleep(UPDATE_WAIT_TIME);
            }
        }
        private void UpdateTime()
        {
            NotifyPropertyChanged("TotalTime");
            NotifyPropertyChanged("DayTotalTime");
            NotifyPropertyChanged("WeekTotalTime");
            NotifyPropertyChanged("MonthTotalTime");
            lastPeriod?.NotifyPropertyChanged("Duration");
        }
        private void UpdateUI()
        {
            UpdateTime();
            NotifyPropertyChanged("MainButtonText");
            NotifyPropertyChanged("Periods");
        }
        internal async void Delete(TimePeriod period)
        {
            bool result = await dialogManager.YesNoDialog("Clear data", "Do you want remove this period?");
            if (result)
            {
                container.Delete(period);
                UpdateUI();
            }
        }
    }
}
