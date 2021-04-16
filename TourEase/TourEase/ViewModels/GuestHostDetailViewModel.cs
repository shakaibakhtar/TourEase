using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TourEase.Models;
using TourEase.Utility;
using Xamarin.Forms;

namespace TourEase.ViewModels
{
    public class GuestHostDetailViewModel : INPC
    {
        public INavigation navigation { get; set; }

        string _Title;
        public string Title
        {
            get => _Title;
            set
            {
                if (value != null)
                {
                    _Title = value;
                }
                OnPropertyChanged();
            }
        }

        clsUser _GuestHost;
        public clsUser GuestHost
        {
            get => _GuestHost;
            set
            {
                if (value != null)
                {
                    _GuestHost = value;
                }
                OnPropertyChanged();
            }
        }
        public GuestHostDetailViewModel(INavigation navigation, clsUser GuestHost)
        {
            this.navigation = navigation;
            this.GuestHost = GuestHost;

            Task.Run(async () =>
            {
                int userType = Convert.ToInt32(await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserType));
                Title = userType == 1 ? "Host Details" : "Guest Details";
            });
        }

        public Command ShowSentRequestsCommand
        {
            get
            {
                return new Command(() =>
                {
                    //navigation.PushAsync(new RequestsPage("sent"));
                });
            }
        }

        public Command ShowReceivedRequestsCommand
        {
            get
            {
                return new Command(() =>
                {
                    //navigation.PushAsync(new RequestsPage("received"));
                });
            }
        }

        public Command SendRequestCommand
        {
            get
            {
                return new Command(() =>
                {
                    //navigation.PushAsync(new SendRequestPage());
                });
            }
        }
    }
}
