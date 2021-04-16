using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TourEase.Popup_Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PopupAlert : Rg.Plugins.Popup.Pages.PopupPage
	{
        public PopupAlert()
        {
            InitializeComponent();
            OkButton.Clicked += OkButton_Clicked;
        }
        public PopupAlert(string Image, string Description, String ButtonText)
        {
            InitializeComponent();
            OkButton.Clicked += OkButton_Clicked;
            PopUpData popUpData = new PopUpData(Image, Description, ButtonText);
            BindingContext = popUpData;
        }
        public PopupAlert(PopUpData popUpData)
        {
            InitializeComponent();
            OkButton.Clicked += OkButton_Clicked;
            BindingContext = popUpData;
        }



        private void OkButton_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync(true);
        }
    }

    public class PopUpData
    {
        public string Image { get; set; }
        //public string Title { get; set; }
        public string Description { get; set; }
        public string ButtonText { get; set; }
        public Command OkCommand { get; set; }

        public PopUpData()
        {
            OkCommand = new Command(Ok);
        }

        public PopUpData(string Image, string Description, string ButtonText)
        {
            switch (Image)
            {
                case "E":
                    this.Image = "crossRed.png";
                    break;
                case "W":
                    this.Image = "warning.png";
                    break;
                case "I":
                    this.Image = "info.png";
                    break;
                case "S":
                    this.Image = "tick.png";
                    break;
            }
            //this.Image = Image;
            //this.Title = Title;
            this.Description = Description;
            this.ButtonText = ButtonText;
        }
        void Ok()
        {
            PopupNavigation.Instance.PopAsync(true);
        }
    }
}