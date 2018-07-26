using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMeasure.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimeMeasure.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditorView : ContentPage
	{
		public EditorView (EditorBindingContext bindingContext)
		{
			InitializeComponent ();
            BindingContext = bindingContext;
		}
	}
}