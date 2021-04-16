using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TourEase.Models;
using TourEase.Utility;
using TourEase.Views;
using Xamarin.Forms;

namespace TourEase.ViewModels
{
    public class LoginViewModel : INPC
    {
        clsUser _User;
        public clsUser User
        {
            get => _User;
            set
            {
                if (value != null)
                    _User = value;
                OnPropertyChanged();
            }
        }
        public INavigation navigation { get; set; }
        public LoginViewModel(INavigation navigation)
        {
            this.navigation = navigation;
            User = new clsUser();
        }

        private async Task<bool> ValidateInputs()
        {
            bool res = true;

            return res;
        }

        public Command LoginCommand
        {
            get
            {
                return new Command(async () =>
                {
                    ApiCalls api = new ApiCalls(navigation);

                    if (await ValidateInputs())
                    {
                        api.IsBusy = true;
                        if (await api.LoginUser(User))
                        {
                            Application.Current.MainPage = new NavigationPage(new HomePage());
                        }
                        api.IsBusy = false;
                    }
                });
            }
        }

        public Command NavigateToSignUpCommand
        {
            get
            {
                return new Command(() =>
                {
                    Application.Current.MainPage = new RegisterPage();
                });
            }
        }
    }
}
