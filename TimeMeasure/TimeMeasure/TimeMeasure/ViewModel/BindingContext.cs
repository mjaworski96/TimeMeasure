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
        private IPageSetter pageSetter;
        private ViewModelPeriod lastPeriod;
        private TimePeriodContainer container;
        private ViewModelPeriod selectedPeriod;

        public event PropertyChangedEventHandler PropertyChanged;

        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            int hours = (int)timeSpan.TotalHours;
            return string.Format("{00}:{1:mm}:{1:ss}",
                hours < 10 ? "0" + hours : hours.ToString(),
                timeSpan);
        }

        public BindingContext(IDialog dialog, IPageSetter pageSetter) :
            this()
        {
            dialogManager = dialog;
            this.pageSetter = pageSetter;
            container = new TimePeriodContainer();
            Task.Run(() => UpdateTimeThread());
        }
        public BindingContext()
        {
            container = new TimePeriodContainer();
            selectedPeriod = null;
        }
        public void Refresh()
        {
            container.Load();
            UpdateUI();
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
        { get => FormatTimeSpan(container.WeekTotalDuration); }
        public string MonthTotalTime
        { get => FormatTimeSpan(container.MonthTotalDuration); }
        public string CurrentTotalTime
        {
            get => (Periods.FirstOrDefault() == null) ||
              Periods.FirstOrDefault().End != "Not Finished"
              ? "00:00:00" : Periods.FirstOrDefault().Duration;
        }
        public string MainButtonText
        { get => container.IsActive ? "Stop" : "Start"; }
        public ICommand MainButtonCommand
        { get => new DelegateCommand(MainButtonAction); }
        public ICommand ResetButtonCommand
        { get => new DelegateCommand(ResetButtonActionAsync); }
        public ViewModelPeriod SelectedPeriod
        {
            get
            {
                return selectedPeriod;
            }
            set
            {
                if(value != null && !value.Period.Active)
                {
                    pageSetter.SetEditorPage(new EditorBindingContext(pageSetter,
                        value.Period, AfterUpdate));
                }
                selectedPeriod = value;
            }
        }
        private void AfterUpdate()
        {
            UpdateUI();
            container.AfterUpdate();
        }
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
