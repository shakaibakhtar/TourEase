using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourEase.Admin_ViewModels;
using TourEase.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TourEase.Admin_Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserDetailPage : ContentPage
	{
		public UserDetailPage (UsersListViewModel vm)
		{
			InitializeComponent ();

			BindingContext = vm;
		}
	}
}