using TimeMeasure.ViewModel;
using Xamarin.Forms;

namespace TimeMeasure.View
{
    public partial class MainPage : ContentPage
	{
		public MainPage(PageApp pageApp)
		{
            BindingContext = new BindingContext(
                new Dialog(this),
                new PageSetter(pageApp, this));
			InitializeComponent();
		}
        public void Refresh()
        {
            ((BindingContext)BindingContext).Refresh();
        }
	}
}
