using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using TourEase.Models;
using TourEase.Utility;
using TourEase.Views;
using Xamarin.Forms;

namespace TourEase.ViewModels
{
    public class RegisterViewModel : INPC
    {
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
                    Application.Current.MainPage = new LoginPage();
                });
            }
        }
    }
}
