using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourEase.Models;
using TourEase.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TourEase.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GuestHostDetailPage : ContentPage
	{
		public GuestHostDetailPage (clsUser GuestHost)
		{
			InitializeComponent();

			BindingContext = new GuestHostDetailViewModel(Navigation, GuestHost);
		}
	}
}