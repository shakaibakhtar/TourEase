using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourEase.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TourEase.Popup_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GiveRatingPopup : PopupPage
    {
        public GiveRatingPopup(RequestDetailViewModel vm)
        {
            InitializeComponent();

            BindingContext = vm;
        }

        protected override bool OnBackButtonPressed()
        {
            // Prevent hide popup
            return base.OnBackButtonPressed();
            //return true;
        }

        // Invoced when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Return default value - CloseWhenBackgroundIsClicked
            //return false;
            return base.OnBackgroundClicked();
        }
    }
}