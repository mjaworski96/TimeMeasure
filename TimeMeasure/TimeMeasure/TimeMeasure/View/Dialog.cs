using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TimeMeasure.ViewModel;
using Xamarin.Forms;

namespace TimeMeasure.View
{
    public class Dialog : IDialog
    {
        private Page page;

        public Dialog(Page page)
        {
            this.page = page;
        }

        public async Task WarningDialog(string title, string message)
        {
            await page.DisplayAlert(title, message, "OK");
        }

        public async Task<bool> YesNoDialog(string title, string message)
        {
            return await page.DisplayAlert(title, message, "Yes", "No");
        }
    }
}
