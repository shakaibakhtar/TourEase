using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using TourEase.Models;
using TourEase.Popup_Views;
using TourEase.Utility;
using Xamarin.Forms;

namespace TourEase.ViewModels
{
    public class RequestDetailViewModel : INPC
    {
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

        string _option;
        public string option
        {
            get => _option;
            set
            {
                if (value != null)
                {
                    _option = value;
                }
                OnPropertyChanged();
            }
        }

        public INavigation navigation { get; set; }

        public RequestDetailViewModel(clsRequest request, string option, INavigation navigation)
        {
            this.navigation = navigation;
            this.option = option;
            //this.requestId = request.RequestId;
            this.Request = request;

            if (option.Equals("sent"))
            {
                GuestHost = request.ReceiverObject;
            }
            else
            {
                GuestHost = request.SenderObject;
            }
        }

        public Command AcceptRequestCommand
        {
            get
            {
                return new Command(async () =>
                {
                    // Call Accept Request Api
                    ApiCalls api = new ApiCalls(navigation);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        api.IsBusy = true;
                    });

                    if (GuestHost.User_Type == 1)
                    {
                        if (await api.AcceptHostRequest(Request.RequestId))
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                PopupNavigation.Instance.PushAsync(new PopupAlert("S", "Request accepted Succesfully", "OK"));
                            });
                        }
                    }
                    else
                    {
                        if (await api.AcceptGuestRequest(Request.RequestId))
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                PopupNavigation.Instance.PushAsync(new PopupAlert("S", "Request accepted Succesfully", "OK"));
                            });
                        }
                    }

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        api.IsBusy = false;
                    });
                });
            }
        }

        public Command RejectRequestCommand
        {
            get
            {
                return new Command(() =>
                {
                    // Call Reject Request Api
                    //ApiCalls api = new ApiCalls(navigation);
                    //Device.BeginInvokeOnMainThread(() =>
                    //{
                    //    api.IsBusy = true;
                    //});

                    //if (await api.AcceptHostRequest(Request.RequestId))
                    //{
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        PopupNavigation.Instance.PushAsync(new PopupAlert("S", "Request ignored.", "OK"));
                    });
                    //}

                    //Device.BeginInvokeOnMainThread(() =>
                    //{
                    //    api.IsBusy = false;
                    //});
                });
            }
        }

        public Command FinishAndGiveRatingCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await navigation.PushPopupAsync(new GiveRatingPopup(this));
                });
            }
        }

        public Command CancelRequestCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await navigation.PushPopupAsync(new PopupAlert("I", "You have cancelled successfully.", "OK"));
                });
            }
        }

        #region Give Rating Popup
        public Command SaveRatingCommand
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

                    // Call Save Rating Api
                    int receiverId = 0;

                    if (option.Equals("sent"))
                    {
                        receiverId = Request.ReceiverObject.UserId;
                    }
                    else
                    {
                        receiverId = Request.SenderObject.UserId;
                    }

                    if (await api.SaveGuestRequestRating(Request.RequestId, receiverId, Request.RatingValue))
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            api.IsBusy = false;
                            navigation.PopPopupAsync();
                        });
                        return;
                    }

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        api.IsBusy = false;
                        navigation.PopPopupAsync();
                    });
                });
            }
        }
        #endregion
    }
}
