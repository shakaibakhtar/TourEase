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
    public class HomePageViewModel : INPC
    {
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
        ObservableCollection<clsUser> _GuestsHostsList;
        public ObservableCollection<clsUser> GuestsHostsList
        {
            get => _GuestsHostsList;
            set
            {
                if (value != null)
                {
                    _GuestsHostsList = value;
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

            ApiCalls api = new ApiCalls(navigation);
            //api.IsBusy = true;
            GuestsHostsList = await api.GetGuestsHostsList();
            //api.IsBusy = false;
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

        public Command AdvancedSearchCommand
        {
            get
            {
                return new Command(() =>
                {
                    //navigation.PushAsync(new AdvancedSearchPage());
                });
            }
        }
    }
}
