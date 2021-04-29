using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourEase.Models;
using TourEase.Popup_Views;
using TourEase.Utility;
using TourEase.Views;
using Xamarin.Forms;

namespace TourEase.ViewModels
{
    public class HomePageViewModel : INPC
    {
        #region Advanced Search
        string _FullName;
        public string FullName
        {
            get => _FullName;
            set
            {
                _FullName = value;
                OnPropertyChanged();
            }
        }

        string _Contact;
        public string Contact
        {
            get => _Contact;
            set
            {
                _Contact = value;
                OnPropertyChanged();
            }
        }

        ObservableCollection<string> _Cities;
        public ObservableCollection<string> Cities
        {
            get => _Cities;
            set
            {
                _Cities = value;
                OnPropertyChanged();
            }
        }

        string _SelectedCity;
        public string SelectedCity
        {
            get => _SelectedCity;
            set
            {
                _SelectedCity = value;
                Areas = new ObservableCollection<string>(GuestsHostsList.Where(x => x.Location_City.Equals(_SelectedCity)).Select(x => x.Location_Area).ToList());
                OnPropertyChanged();
            }
        }

        ObservableCollection<string> _Areas;
        public ObservableCollection<string> Areas
        {
            get => _Areas;
            set
            {
                _Areas = value;
                OnPropertyChanged();
            }
        }

        string _SelectedArea;
        public string SelectedArea
        {
            get => _SelectedArea;
            set
            {
                _SelectedArea = value;
                OnPropertyChanged();
            }
        }

        public Command AdvanceSearchCommand
        {
            get
            {
                return new Command(() =>
                {
                    Search();
                });
            }
        }

        public void Search()
        {
            //if (string.IsNullOrEmpty(query) || string.IsNullOrWhiteSpace(query))
            //{
            //    SearchableList = GuestsHostsList;
            //}
            //else
            //{
            PopupNavigation.Instance.PushAsync(new PopupLoading());

            List<clsUser> filter = GuestsHostsList.ToList<clsUser>();
            List<clsUser> filterFullName = new List<clsUser>();
            List<clsUser> filterContact = new List<clsUser>();
            List<clsUser> filterCity = new List<clsUser>();
            List<clsUser> filterArea = new List<clsUser>();

            if (!string.IsNullOrEmpty(FullName))
            {
                filterFullName = GuestsHostsList.Where(x => ((!string.IsNullOrEmpty(x.Full_Name)) && x.Full_Name.ToLower().Contains(FullName))).ToList();

                filter = filter.Intersect(filterFullName).ToList();
            }
            if (!string.IsNullOrEmpty(Contact))
            {
                filterContact = GuestsHostsList.Where(x => ((!string.IsNullOrEmpty(x.Contact_Number)) && x.Contact_Number.ToLower().Contains(Contact))).ToList();

                filter = filter.Intersect(filterContact).ToList();
            }
            if (!string.IsNullOrEmpty(SelectedCity))
            {
                filterCity = GuestsHostsList.Where(x => ((!string.IsNullOrEmpty(x.Location_City)) && x.Location_City.ToLower().Contains(SelectedCity))).ToList();

                filter = filter.Intersect(filterCity).ToList();
            }
            if (!string.IsNullOrEmpty(SelectedArea))
            {
                filterArea = GuestsHostsList.Where(x => ((!string.IsNullOrEmpty(x.Location_Area)) && x.Location_Area.ToLower().Contains(SelectedArea))).ToList();

                filter = filter.Intersect(filterArea).ToList();
            }

            SearchableList = new ObservableCollection<clsUser>(filter);

            PopupNavigation.Instance.PopAsync();
            navigation.PopAsync();
            //}
        }
        #endregion

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

        string _HomeTitle;
        public string HomeTitle
        {
            get => _HomeTitle;
            set
            {
                if (value != null)
                {
                    _HomeTitle = value;
                }
                OnPropertyChanged();
            }
        }

        string _CompanionEventTitle;
        public string CompanionEventTitle
        {
            get => _CompanionEventTitle;
            set
            {
                if (value != null)
                {
                    _CompanionEventTitle = value;
                }
                OnPropertyChanged();
            }
        }

        ObservableCollection<clsUser> _GuestsHostsList;
        public ObservableCollection<clsUser> GuestsHostsList
        {
            get => _GuestsHostsList;
            set
            {
                if (value != null)
                {
                    _GuestsHostsList = value;
                    SearchableList = _GuestsHostsList;
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
                    Cities = new ObservableCollection<string>(GuestsHostsList.Select(x => x.Location_City).ToList());
                }
                OnPropertyChanged();
            }
        }

        public INavigation navigation { get; set; }

        public HomePageViewModel(INavigation navigation)
        {
            this.navigation = navigation;

            Task.Run(async () =>
            {
                await OnLoad();
            });

            //GuestsHostsList = new ObservableCollection<clsUser>()
            //{
            //    new clsUser() { Full_Name = "Name 1", Email = "email@email.com", Contact_Number = "03123456781", Fake_Reported_Count = 0, Location_City = "Lahore", Location_Area = "Gulberg", UserId = 0, User_Type = 1 },
            //    new clsUser() { Full_Name = "Name 2", Email = "email@email.com", Contact_Number = "03123456782", Fake_Reported_Count = 0, Location_City = "Islamabad", Location_Area = "Gulberg", UserId = 1, User_Type = 1 },
            //    new clsUser() { Full_Name = "Name 3", Email = "email@email.com", Contact_Number = "03123456783", Fake_Reported_Count = 0, Location_City = "Gujrat", Location_Area = "Service Mor", UserId = 2, User_Type = 1 },
            //    new clsUser() { Full_Name = "Name 4", Email = "email@email.com", Contact_Number = "03123456781", Fake_Reported_Count = 0, Location_City = "Rawalpindi", Location_Area = "Chaklala", UserId = 3, User_Type = 1 },
            //};
        }

        private async Task OnLoad()
        {
            int userType = Convert.ToInt32(await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserType));

            HomeTitle = userType == 1 ? "Hosts List" : "Guests List"; // 1 means guest is login so title will be "Hosts List",,, otherwise title will be "Guests List"
            CompanionEventTitle = userType == 1 ? "Companion Request" : "Event Request"; // Guest can only send companion request, and host can only send event request

            await RefreshGuestsHostsList();
        }

        private async Task RefreshGuestsHostsList()
        {
            ApiCalls api = new ApiCalls(navigation);
            //api.IsBusy = true;
            GuestsHostsList = await api.GetGuestsHostsList();
            //api.IsBusy = false;
        }

        public Command RefreshGuestsHostsListCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;
                    await RefreshGuestsHostsList();
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

        public Command GotoAdvancedSearchCommand
        {
            get
            {
                return new Command(() =>
                {
                    navigation.PushAsync(new AdvancedSearchPage(this));
                });
            }
        }

        public Command<clsUser> OpenGuestHostDetailCommand
        {
            get
            {
                return new Command<clsUser>((clsUser GuestHost) =>
                {
                    navigation.PushAsync(new GuestHostDetailPage(GuestHost));
                });
            }
        }

        public Command ShowSentRequestsCommand
        {
            get
            {
                return new Command(() =>
                {
                    navigation.PushAsync(new RequestsListPage("sent"));
                });
            }
        }

        public Command ShowReceivedRequestsCommand
        {
            get
            {
                return new Command(() =>
                {
                    navigation.PushAsync(new RequestsListPage("received"));
                });
            }
        }

        public Command OpenChatWindowCommand
        {
            get
            {
                return new Command(() =>
                {
                    navigation.PushAsync(new ChatWindowPage());
                });
            }
        }

        #region Companion Event Request
        clsRequest _Request;
        public clsRequest Request
        {
            get => _Request;
            set
            {
                if(value != null)
                {
                    _Request = value;
                }
                OnPropertyChanged();
            }
        }

        public Command CompanionEventRequestsCommand
        {
            get
            {
                return new Command(async () =>
                {
                    int userType = Convert.ToInt32(await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserType));
                    int userId = Convert.ToInt32(await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserId));

                    //userType == 1 ? "Companion Request" : "Event Request";
                    if (userType == 1)
                    {
                        await navigation.PushAsync(new CompanionRequest());
                    }
                    else
                    {
                        await navigation.PushAsync(new EventRequest());
                    }
                });
            }
        }
        #endregion
    }
}
