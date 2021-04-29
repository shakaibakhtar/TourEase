using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TourEase.Admin_Views;
using TourEase.Models;
using TourEase.Popup_Views;
using TourEase.Utility;
using TourEase.Views;
using Xamarin.Forms;

namespace TourEase.ViewModels
{
    public class LoginViewModel : INPC
    {
        public string LocalVerificationCode { get; set; }
        public string CodeFromApi { get; set; }
        public bool ResendCodeCommandCanRun { get; set; } = true;

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

        bool _IsAdmin;
        public bool IsAdmin
        {
            get => _IsAdmin;
            set
            {
                _IsAdmin = value;
                OnPropertyChanged();
            }
        }
        public INavigation navigation { get; set; }
        public LoginViewModel(INavigation navigation)
        {
            this.navigation = navigation;
            User = new clsUser();
            IsAdmin = true;
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
                        await Login(IsAdmin, api);
                        api.IsBusy = false;
                    }
                });
            }
        }

        public async Task<bool> Login(bool isAdmin = false, ApiCalls tmp = null)
        {
            bool res = false;
            ApiCalls api = tmp ?? new ApiCalls(navigation);


            if (isAdmin)
            {
                if (await api.LoginAdmin(User, this))
                {
                    res = true;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Application.Current.MainPage = new NavigationPage(new UsersListPage());
                    });
                }
            }
            else
            {
                if (await api.LoginUser(User, this))
                {
                    res = true;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Application.Current.MainPage = new NavigationPage(new HomePage());
                    });
                }
            }

            return res;
        }

        public Command NavigateToSignUpCommand
        {
            get
            {
                return new Command(() =>
                {
                    Application.Current.MainPage = new NavigationPage(new RegisterPage());
                });
            }
        }

        public Command GotoForgotPasswordCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await navigation.PushAsync(new ForgotPasswordPage());
                });
            }
        }

        public Command VerifyCodeCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (LocalVerificationCode == CodeFromApi)
                    {
                        ApiCalls api = new ApiCalls(navigation);
                        if (await api.VerifyUser(User.UserId))
                        //if (true) // Comment this line and uncomment upper line after adding the verify api on server side.
                        {

                            //await PopupNavigation.Instance.PushAsync(new PopupAlert("S", GlobalData.AccountVerified, "OK"));
                            //if (User.IsProfileCompleted)
                            //{
                            //    Application.Current.MainPage = new AppShell();
                            //}
                            //else
                            //{
                            //await navigation.PushAsync(new FinishRegistrationPage(User));
                            //}
                            Application.Current.MainPage = new NavigationPage(new HomePage());
                        }
                    }
                    else
                    {
                        await PopupNavigation.Instance.PushAsync(new PopupAlert("E", "Invalid OTP entered.", "OK"));
                    }
                });
            }
        }

        public Command ResendCodeCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (ResendCodeCommandCanRun)
                    {
                        ApiCalls api = new ApiCalls(navigation);
                        await api.SendOTP(User.UserId, this, null);
                    }
                    else
                    {
                        await PopupNavigation.Instance.PushAsync(new PopupAlert("I", "A verification code is already sent to your account. Please wait for some time", "OK"));
                        //await Application.Current.MainPage.DisplayAlert("Warning!","A verification code is already sent to your account. Please wait for some time and then try again.","OK");
                    }
                });
            }
        }
    }
}
