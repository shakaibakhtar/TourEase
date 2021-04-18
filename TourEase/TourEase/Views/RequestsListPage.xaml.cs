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
	public partial class RequestsListPage : ContentPage
	{
		public RequestsListPage (string option)
		{
			InitializeComponent ();

			BindingContext = new RequestsListViewModel(Navigation, option);
		}
	}
}