using TimeMeasure.ViewModel;
using Xamarin.Forms;

namespace TimeMeasure.View
{
    public partial class MainPage : ContentPage
	{
		public MainPage()
		{
            BindingContext = new BindingContext(
                new Dialog(this));
			InitializeComponent();
		}
	}
}
