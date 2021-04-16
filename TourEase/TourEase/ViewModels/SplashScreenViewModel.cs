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
    public class SplashScreenViewModel
    {
        public INavigation navigation { get; set; }
        public SplashScreenViewModel(INavigation navigation)
        {
            this.navigation = navigation;

            Task.Run(async () =>
            {
                LoginViewModel loginVM = new LoginViewModel(navigation);
                loginVM.User = new clsUser();
                loginVM.User.Email = await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserEmail);
                loginVM.User.Password = await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserPassword);

                ApiCalls api = new ApiCalls(navigation);

                if ((!string.IsNullOrEmpty(loginVM.User.Email)) && (!string.IsNullOrWhiteSpace(loginVM.User.Email)))
                {
                    if (await loginVM.Login())
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Application.Current.MainPage = new NavigationPage(new HomePage());
                        });

                    }
                }
                else
                {
                    SecureStorageClass.ClearAll();
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Application.Current.MainPage = new NavigationPage(new LoginPage());
                    });
                }
            });
        }
    }
}
