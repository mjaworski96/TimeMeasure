﻿using System.ComponentModel;
using System.Windows.Input;
using TimeMeasure.Model;

namespace TimeMeasure.ViewModel
{
    public class ViewModelPeriod: INotifyPropertyChanged
    {
        private TimePeriod period;
        private BindingContext bindingContext;
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string Start
        {
            get
            {
                return period.StartDate.ToString(
                    TimePeriod.DATE_FORMAT);
            }
        }
        public string End
        {
            get
            {
                if (period.EndDate.HasValue)
                    return period.EndDate.Value.ToString(
                        TimePeriod.DATE_FORMAT);
                else
                    return "Not Finished";
            }
        }
        public string Duration
        {  get => BindingContext.FormatTimeSpan(period.Duration); }

        public ViewModelPeriod(TimePeriod period, BindingContext bindingContext)
        {
            this.period = period;
            this.bindingContext = bindingContext;
        }
        public ICommand DeleteCommand
        { get => new DelegateCommand(DeleteAction); }
        public TimePeriod Period { get => period; set => period = value; }

        private void DeleteAction()
        {
            bindingContext.Delete(period);
        }
    }
}
