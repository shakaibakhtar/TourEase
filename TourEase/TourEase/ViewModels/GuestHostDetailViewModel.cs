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
    public class GuestHostDetailViewModel : INPC
    {
        public INavigation navigation { get; set; }

        clsRequest _Request;
        public clsRequest Request
        {
            get => _Request;
            set
            {
                if (value != null)
                {
                    _Request = value;
                }
                OnPropertyChanged();
            }
        }

        string _RequestMessage;
        public string RequestMessage
        {
            get => _RequestMessage;
            set
            {
                if (value != null)
                {
                    _RequestMessage = value;
                }
                OnPropertyChanged();
            }
        }

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

        public Command SendRequestCommand
        {
            get
            {
                return new Command(() =>
                {
                    navigation.PushAsync(new SendRequestPage(this));
                });
            }
        }

        public Command SubmitRequestCommand
        {
            get
            {
                return new Command(async () =>
                {

                    ApiCalls api = new ApiCalls(navigation);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        api.IsBusy = true;
                    });

                    int userId = Convert.ToInt32(await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserId));
                    int userType = Convert.ToInt32(await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserType));

                    if (userType == 1) // user is guest
                    {
                        await api.RequestGuest(userId, GuestHost.UserId, RequestMessage);
                    }
                    else
                    {
                        await api.RequestHost(userId, GuestHost.UserId, RequestMessage);
                    }

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        api.IsBusy = false;
                    });
                });
            }
        }

        public Command ReportUserCommand
        {
            get
            {
                return new Command(async () =>
                {

                    ApiCalls api = new ApiCalls(navigation);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        api.IsBusy = true;
                    });

                    if(await api.ReportUser(GuestHost.UserId))
                    {
                        GuestHost.Fake_Reported_Count++;
                    }

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        api.IsBusy = false;
                    });
                });
            }
        }
    }
}
