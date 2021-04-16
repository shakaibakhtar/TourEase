using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using TourEase.Models;
using TourEase.Popup_Views;
using TourEase.Utility;
using TourEase.Views;
using Xamarin.Forms;

namespace TourEase.ViewModels
{
    public class RegisterViewModel : INPC
    {
        public string LocalVerificationCode { get; set; }
        public string CodeFromApi { get; set; }
        public bool ResendCodeCommandCanRun { get; set; } = true;

        clsUserType _SelectedUserType;
        public clsUserType SelectedUserType
        {
            get => _SelectedUserType;
            set
            {
                if (value != null)
                {
                    _SelectedUserType = value;
                    User.User_Type = _SelectedUserType.UserTypeId;
                }
                OnPropertyChanged();
            }
        }

        ObservableCollection<clsUserType> _UserTypes;
        public ObservableCollection<clsUserType> UserTypes
        {
            get => _UserTypes;
            set
            {
                if (value != null)
                {
                    _UserTypes = value;
                }
                OnPropertyChanged();
            }
        }

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

        public RegisterViewModel(INavigation navigation)
        {
            this.navigation = navigation;
            User = new clsUser();
            UserTypes = new ObservableCollection<clsUserType>()
            {
                new clsUserType(){ UserTypeId = 1, UserTypeName = "Guest" },
                new clsUserType(){ UserTypeId = 2, UserTypeName = "Host" },
            };
        }

        private async Task<bool> ValidateInputs()
        {
            bool res = true;

            return res;
        }

        public Command SignUpCommand
        {
            get
            {
                return new Command(async () =>
                {
                    ApiCalls api = new ApiCalls(navigation);
                    api.IsBusy = true;

                    if (await ValidateInputs())
                    {
                        if (await api.RegisterUser(User))
                        {
                            Application.Current.MainPage = new NavigationPage(new HomePage());
                            //await Application.Current.MainPage.DisplayAlert("Success", "Paa g Registration Success", "OK");
                        }

                        api.IsBusy = false;
                    }
                });
            }
        }

        public Command NavigateToSignInCommand
        {
            get
            {
                return new Command(() =>
                {
                    Application.Current.MainPage = new NavigationPage(new LoginPage());
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
                            Application.Current.MainPage = new HomePage();
                            //await navigation.PushAsync(new FinishRegistrationPage(User));
                        }
                    }
                    else
                    {
                        await PopupNavigation.Instance.PushAsync(new PopupAlert("E", "Invalid code entered", "OK"));
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
                        await api.SendOTP(User.UserId, null, this);
                    }
                    else
                    {
                        await PopupNavigation.Instance.PushAsync(new PopupAlert("W", "A verification code is already sent to your account. Please wait for some time.", "OK"));
                        //await Application.Current.MainPage.DisplayAlert("Warning!","A verification code is already sent to your account. Please wait for some time and then try again.","OK");
                    }
                });
            }
        }
    }
}
