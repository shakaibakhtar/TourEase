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
                clsUser user = new clsUser();
                user.Email = await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserEmail);
                user.Password = await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserPassword);

                ApiCalls api = new ApiCalls(navigation);

                if ((!string.IsNullOrEmpty(user.Email)) && (!string.IsNullOrWhiteSpace(user.Email)))
                {
                    if (await api.LoginUser(user))
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
                        Application.Current.MainPage = new LoginPage();
                    });
                }
            });
        }
    }
}
