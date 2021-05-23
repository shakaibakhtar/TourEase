using Rg.Plugins.Popup.Extensions;
using System;
using System.Linq;
using TourEase.Popup_Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TourEase.Utility
{
    public class Constants
    {
        public static string BaseURL { get; set; } = "http://192.168.8.101:8080";
        //public static string BaseURL { get; set; } = "http://tourtestapp.azurewebsites.net";

        public static string ApiURL { get; set; } = "/api";
        public static string CompleteURL => BaseURL + ApiURL;

        #region Check Permissions
        public static async System.Threading.Tasks.Task<bool> IsInternetConnected()
        {
            var profiles = Connectivity.ConnectionProfiles;
            if (profiles.Contains(ConnectionProfile.WiFi) || profiles.Contains(ConnectionProfile.Cellular))
            {
                var current = Connectivity.NetworkAccess;
                if (current == NetworkAccess.Internet)
                {
                    return true;
                }
                else
                {
                    await Application.Current.MainPage.Navigation.PushPopupAsync(new PopupAlert("W", "Network is Unavailable", "OK"));
                    return false;
                }
            }
            else
            {
                await Application.Current.MainPage.Navigation.PushPopupAsync(new PopupAlert("E", "No Internet connection.", "OK"));
                return false;
            }
        }
        #endregion
    }
}
