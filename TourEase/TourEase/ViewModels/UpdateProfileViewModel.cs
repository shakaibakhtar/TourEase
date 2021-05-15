using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using TourEase.Models;
using TourEase.Popup_Views;
using TourEase.Utility;
using Xamarin.Forms;

namespace TourEase.ViewModels
{
    public class UpdateProfileViewModel : INPC
    {
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

        public UpdateProfileViewModel(INavigation navigation)
        {
            this.navigation = navigation;
            User = new clsUser();
        }

        public Command UpdateProfileCommand
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

                    if (await api.UpdateProfile(User))
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            PopupNavigation.Instance.PushAsync(new PopupAlert("S", "Profile updated successfully", "OK"));
                        });
                    }
                });
            }
        }
    }
}
