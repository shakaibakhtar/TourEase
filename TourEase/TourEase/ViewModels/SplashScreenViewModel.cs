using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TourEase.Admin_Views;
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
                bool IsAdmin = Convert.ToBoolean(await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyIsAdmin));

                ApiCalls api = new ApiCalls(navigation);

                if ((!string.IsNullOrEmpty(loginVM.User.Email)) && (!string.IsNullOrWhiteSpace(loginVM.User.Email)))
                {
                    if (await loginVM.Login(IsAdmin))
                    {
                        if (IsAdmin)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Application.Current.MainPage = new NavigationPage(new UsersListPage());
                            });
                        }
                        else
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
