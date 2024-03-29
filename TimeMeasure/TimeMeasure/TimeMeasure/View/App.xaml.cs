﻿using Xamarin.Forms;

namespace TimeMeasure.View
{
    public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();

			MainPage = new MainPage(new PageApp(this));
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
            ((MainPage)MainPage).Refresh();

        }
	}
}
