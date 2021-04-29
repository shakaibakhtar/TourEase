using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourEase.Admin_ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TourEase.Admin_Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UsersListPage : ContentPage
	{
		public UsersListPage ()
		{
			InitializeComponent ();

			BindingContext = new UsersListViewModel(Navigation);
		}
	}
}