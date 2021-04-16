using System;
using TourEase.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TourEase
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new SplashScreen();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
