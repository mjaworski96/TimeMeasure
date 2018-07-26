using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using TimeMeasure.Model;

namespace TimeMeasure.ViewModel
{
    public class EditorBindingContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private IDialog dialog;
        private IPageSetter pageSetter;
        private TimePeriod timePeriod;
        private Action afterUpdateAction;
        public void SetDialog(IDialog dialog)
        {
            this.dialog = dialog;
        }

        public EditorBindingContext(IPageSetter pageSetter, TimePeriod timePeriod, Action afterUpdateAction)
        {
            this.pageSetter = pageSetter;
            this.timePeriod = timePeriod;
            this.afterUpdateAction = afterUpdateAction; 

            Reset();
        }

        public string Start { get; set; }
        public string End { get; set; }

        public ICommand ResetButtonCommand
        { get => new DelegateCommand(Reset); }
        public ICommand CommitButtonCommand
        { get => new DelegateCommand(Commit); }
        public ICommand ExitButtonCommand
        { get => new DelegateCommand(Exit); }
        public string DateTimeFormat
        { get => TimePeriod.DATE_FORMAT; }

        private void Reset()
        {
            Start = timePeriod.StartDate.ToString(TimePeriod.DATE_FORMAT);
            End = timePeriod.EndDate.Value.ToString(TimePeriod.DATE_FORMAT);
            NotifyPropertyChanged("Start");
            NotifyPropertyChanged("End");
        }
        private async void Commit()
        {
            if (!DateTime.TryParseExact(Start, TimePeriod.DATE_FORMAT, null, DateTimeStyles.None, out DateTime start))
            {
                await dialog.WarningDialog("Warning", "Start date is badly formatted!");
                return;
            }
            if (!DateTime.TryParseExact(End, TimePeriod.DATE_FORMAT, null, DateTimeStyles.None, out DateTime end))
            {
                await dialog.WarningDialog("Warning", "Start date is badly formatted!");
                return;
            }
            if (start > end)
            {
                await dialog.WarningDialog("Warning", "Start date must be before end date");
                return;
            }
            timePeriod.StartDate = start;
            timePeriod.EndDate = end;
            ExitWithoutDialog();
        }
        private void ExitWithoutDialog()
        {
            afterUpdateAction();
            pageSetter.SetMainPage();
        }
        private async void Exit()
        {
            bool result = await dialog.YesNoDialog("Exit", "Do you want return to main page? All unsaved changes will be lost!");
            if (result)
                ExitWithoutDialog();
        }
    }
}
