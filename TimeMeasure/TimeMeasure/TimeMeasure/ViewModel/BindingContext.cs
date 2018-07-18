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
        public const string DURATION_FORMAT = @"hh\:mm\:ss";
        private const int UPDATE_WAIT_TIME = 1000;

        private IDialog dialogManager;
        private ViewModelPeriod lastPeriod;
        private TimePeriodContainer container;

        public event PropertyChangedEventHandler PropertyChanged;

        public BindingContext(IDialog dialog)
        {
            dialogManager = dialog;
            container = new TimePeriodContainer();
            Task.Run(() => UpdateTime());
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
        {
            get
            {
                return container.TotalDuration.ToString(DURATION_FORMAT);
            }
        }
        public string DayTotalTime
        {
            get
            {
                return container.DayTotalDuration.ToString(DURATION_FORMAT);
            }
        }
        public string WeekTotalTime
        {
            get
            {
                return container.WeekTotalDuration.ToString(DURATION_FORMAT);
            }
        }
        public string MonthTotalTime
        { get => container.MonthTotalDuration.ToString(DURATION_FORMAT); }
        public string MainButtonText
        {
            get
            {
                if (container.IsActive)
                    return "STOP";
                else
                    return "START";
            }
        }
        public ICommand MainButtonCommand
        { get => new DelegateCommand(MainButtonAction); }
        public ICommand ResetButtonCommand
        { get => new DelegateCommand(ResetButtonActionAsync); }

        private void MainButtonAction()
        {
            container.PerformClick();
            NotifyPropertyChanged("MainButtonText");
            NotifyPropertyChanged("Periods");
        }
        private async void ResetButtonActionAsync()
        {
            bool result = await dialogManager.YesNoDialog("Clear all data", "Do you want remove all data?");
            if (result)
            {
                container.Clear();
                NotifyPropertyChanged("MainButtonText");
                NotifyPropertyChanged("Periods");
            }
        }
        private void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void UpdateTime()
        {
            while (true)
            {
                NotifyPropertyChanged("TotalTime");
                NotifyPropertyChanged("DayTotalTime");
                NotifyPropertyChanged("WeekTotalTime");
                NotifyPropertyChanged("MonthTotalTime");
                lastPeriod?.NotifyPropertyChanged("Duration");
                Thread.Sleep(UPDATE_WAIT_TIME);
            }
        }
        internal async void DeleteAsync(TimePeriod period)
        {
            bool result = await dialogManager.YesNoDialog("Clear data", "Do you want remove this period?");
            if (result)
            {
                container.Delete(period);
                NotifyPropertyChanged("MainButtonText");
                NotifyPropertyChanged("Periods");
            }
        }
    }
}
