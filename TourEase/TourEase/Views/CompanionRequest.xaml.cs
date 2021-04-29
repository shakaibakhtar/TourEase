using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourEase.Utility;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TourEase.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompanionRequest : ContentPage
    {
        public CompanionRequest()
        {
            InitializeComponent();

            BindingContext = new CompanionVM(Navigation);
        }
    }

    public class CompanionVM : INPC
    {
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

        public INavigation navigation { get; set; }

        public CompanionVM(INavigation navigation)
        {
            this.navigation = navigation;
        }

        public Command SendCompanionRequestCommand
        {
            get
            {
                return new Command(async () =>
                {
                    int userId = Convert.ToInt32(await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserId));
                    ApiCalls api = new ApiCalls(navigation);

                    api.IsBusy = true;
                    await api.CompanionRequest(userId, RequestMessage);
                });
            }
        }
    }
}