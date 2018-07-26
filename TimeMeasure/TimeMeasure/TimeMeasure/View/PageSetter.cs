using System;
using System.Collections.Generic;
using System.Text;
using TimeMeasure.ViewModel;
using Xamarin.Forms;

namespace TimeMeasure.View
{
    public class PageSetter: IPageSetter
    {
        private PageApp pageApp;
        private Page mainPage;

        public PageSetter(PageApp pageApp, Page mainPage)
        {
            this.pageApp = pageApp;
            this.mainPage = mainPage;
        }

        public void SetEditorPage(EditorBindingContext bindingContext)
        {
            pageApp.ActualPage = new EditorView(bindingContext);
            bindingContext.SetDialog(new Dialog(pageApp.ActualPage));
        }

        public void SetMainPage()
        {
            pageApp.ActualPage = mainPage;
        }
    }
}
