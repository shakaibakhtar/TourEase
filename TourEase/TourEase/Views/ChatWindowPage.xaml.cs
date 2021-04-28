using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourEase.Popup_Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TourEase.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatWindowPage : ContentPage
    {
        public ChatWindowPage()
        {
            InitializeComponent();

            webView.Source = "http://tourease.azurewebsites.net/home/index";
        }

        private void WebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new PopupLoading());
        }

        private void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            if (PopupNavigation.Instance.PopupStack.Count > 0)
                PopupNavigation.Instance.PopAllAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (PopupNavigation.Instance.PopupStack.Count > 0)
                PopupNavigation.Instance.PopAllAsync();
        }
    }
}