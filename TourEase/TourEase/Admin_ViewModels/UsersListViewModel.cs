using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using TourEase.Admin_Views;
using TourEase.Models;
using TourEase.Popup_Views;
using TourEase.Utility;
using TourEase.Views;
using Xamarin.Forms;

namespace TourEase.Admin_ViewModels
{
    public class UsersListViewModel : INPC
    {
        bool _IsRefreshing;
        public bool IsRefreshing
        {
            get => _IsRefreshing;
            set
            {
                _IsRefreshing = value;
                OnPropertyChanged();
            }
        }

        //string _Query;
        //public string Query
        //{
        //    get => _Query;
        //    set
        //    {
        //        _Query = value;
        //        OnPropertyChanged();
        //    }
        //}

        clsUser _UserSelected;
        public clsUser UserSelected
        {
            get => _UserSelected;
            set
            {
                if (value != null)
                {
                    _UserSelected = value;
                }
                OnPropertyChanged();
            }
        }

        ObservableCollection<clsUser> _UsersList;
        public ObservableCollection<clsUser> UsersList
        {
            get => _UsersList;
            set
            {
                if (value != null)
                {
                    _UsersList = value;
                    SearchableList = _UsersList;
                }
                OnPropertyChanged();
            }
        }

        ObservableCollection<clsUser> _SearchableList;
        public ObservableCollection<clsUser> SearchableList
        {
            get => _SearchableList;
            set
            {
                if (value != null)
                {
                    _SearchableList = value;
                }
                OnPropertyChanged();
            }
        }

        public INavigation navigation { get; set; }

        public UsersListViewModel(INavigation navigation)
        {
            this.navigation = navigation;

            Task.Run(async () =>
            {
                await RefreshUsersList();
            });
        }

        private async Task RefreshUsersList()
        {
            ApiCalls api = new ApiCalls(navigation);
            UsersList = await api.GetUsersList();
        }

        public Command RefreshUsersListCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;
                    await RefreshUsersList();
                    IsRefreshing = false;
                });
            }
        }

        public Command LogoutCommand
        {
            get
            {
                return new Command(() =>
                {
                    SecureStorageClass.ClearAll();
                    Application.Current.MainPage = new LoginPage();
                });
            }
        }

        public Command<clsUser> OpenUserDetailCommand
        {
            get
            {
                return new Command<clsUser>((clsUser user) =>
                {
                    UserSelected = user;
                    navigation.PushAsync(new UserDetailPage(this));
                });
            }
        }

        //public Command SearchCommand
        //{
        //    get
        //    {
        //        return new Command(() =>
        //        {
        //            Search();
        //        });
        //    }
        //}

        public Command VerifyUserCommand
        {
            get
            {
                return new Command(async () =>
                {
                    ApiCalls api = new ApiCalls(navigation);
                    if(await api.VerifyUser(UserSelected.UserId))
                    {
                        UserSelected.Is_Verified = true;
                    }
                });
            }
        }
        
        public Command EnableUserCommand
        {
            get
            {
                return new Command(async () =>
                {
                    ApiCalls api = new ApiCalls(navigation);
                    if(await api.EnableUser(UserSelected.UserId))
                    {
                        UserSelected.Is_Enabled = true;
                    }
                });
            }
        }
        
        public Command DisableUserCommand
        {
            get
            {
                return new Command(async () =>
                {
                    ApiCalls api = new ApiCalls(navigation);
                    if(await api.DisableUser(UserSelected.UserId))
                    {
                        UserSelected.Is_Enabled = false;
                    }
                });
            }
        }
    }
}
