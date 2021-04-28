using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using TourEase.Models;
using TourEase.Utility;
using TourEase.Views;
using Xamarin.Forms;

namespace TourEase.ViewModels
{
    public class RequestsListViewModel : INPC
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

        string _option;
        public string option
        {
            get => _option;
            set
            {
                _option = value;
                OnPropertyChanged();
            }
        }

        string _Title;
        public string Title
        {
            get => _Title;
            set
            {
                _Title = value;
                OnPropertyChanged();
            }
        }
        public INavigation navigation { get; set; }

        ObservableCollection<clsRequest> _RequestsList;
        public ObservableCollection<clsRequest> RequestsList
        {
            get => _RequestsList;
            set
            {
                if (value != null)
                {
                    _RequestsList = value;
                }
                OnPropertyChanged();
            }
        }

        public RequestsListViewModel(INavigation navigation, string option)
        {
            this.navigation = navigation;
            this.option = option;

            Title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(option) + " Requests";

            ApiCalls api = new ApiCalls(navigation);
            Task.Run(async () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    api.IsBusy = true;
                });

                await GetRequestsList(api);

                Device.BeginInvokeOnMainThread(() =>
                {
                    api.IsBusy = false;
                });
            });
        }

        public Command RefreshRequestsList
        {
            get
            {
                return new Command(async () =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        IsRefreshing = true;
                    });

                    await GetRequestsList();

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        IsRefreshing = false;
                    });
                });
            }
        }

        public async Task GetRequestsList(ApiCalls tmp = null)
        {
            ApiCalls api = tmp ?? new ApiCalls(navigation);

            if (option.Equals("sent"))
            {
                RequestsList = await api.GetGuestsHostsSentRequests();
            }
            else
            {
                RequestsList = await api.GetGuestsHostsReceivedRequestsList();
            }
        }

        public Command<clsRequest> OpenGuestHostDetailCommand
        {
            get
            {
                return new Command<clsRequest>((clsRequest param) =>
                {
                    clsRequest req = param;
                    if (option.Equals("sent"))
                    {
                        navigation.PushAsync(new RequestDetailPage(req, option));
                    }
                    else
                    {
                        navigation.PushAsync(new RequestDetailPage(req, option));
                    }
                });
            }
        }
    }
}
