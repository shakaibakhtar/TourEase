using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourEase.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TourEase.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CodeVerificationPage : ContentPage
	{
        public CodeVerificationPage(PasswordResetViewModel vm)
        {
            InitializeComponent();

            ((ContentPage)this).ToolbarItems.RemoveAt(0);

            BindingContext = vm;
        }

        public CodeVerificationPage(RegisterViewModel vm)
        {
            InitializeComponent();

            BindingContext = vm;
        }

        public CodeVerificationPage(LoginViewModel vm)
        {
            InitializeComponent();

            NavigationPage.SetHasBackButton(this, false);

            BindingContext = vm;
        }
    }
}