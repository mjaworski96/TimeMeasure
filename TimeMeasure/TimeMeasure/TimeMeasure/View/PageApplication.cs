using Xamarin.Forms;

namespace TimeMeasure.View
{
    public class PageApp
    {
        private Application _app;

        public PageApp(Application app)
        {
            _app = app;
        }

        public Page ActualPage
        {
            get
            {
                return _app.MainPage;
            }
            set
            {
                _app.MainPage = value;
            }
        }
    }
}
