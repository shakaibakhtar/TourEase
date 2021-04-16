using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TourEase.Models;
using TourEase.Popup_Views;
using Xamarin.Forms;

namespace TourEase.Utility
{
    public class ApiCalls
    {
        INavigation navigation;

        bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                if ((!_IsBusy) && value)
                    navigation?.PushPopupAsync(new PopupLoading());
                else if (_IsBusy && (!value))
                    navigation?.PopPopupAsync();
                _IsBusy = value;
            }
        }

        public ApiCalls(INavigation navigation)
        {
            this.navigation = navigation;
        }

        #region Login ApiCall
        public async System.Threading.Tasks.Task<bool> LoginUser(clsUser user)
        {
            bool res = false;
            HttpStatusCode responseStatusCode = 0;

            if (await Constants.IsInternetConnected())
            {
                try
                {
                    //IsBusy = true;
                    var Httpclient = new HttpClient();

                    var url = Utility.Constants.CompleteURL + "/Login";

                    var uri = new Uri(string.Format(url, string.Empty));


                    var json = JsonConvert.SerializeObject(user);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = null;

                    response = await Httpclient.PostAsync(uri, content);

                    responseStatusCode = response.StatusCode;

                    if (responseStatusCode == HttpStatusCode.OK)
                    {

                        var responseContent = await response.Content.ReadAsStringAsync();

                        var jObject = JObject.Parse(responseContent);
                        bool status = (bool)jObject.GetValue("status");

                        if (!status)
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                //IsBusy = false;
                                await navigation.PushPopupAsync(new PopupAlert("E", jObject.GetValue("message").ToString(), "OK"));
                            });
                        }
                        else
                        {

                            //bool verified = (bool)jObject.GetValue("IsVerified");

                            string userObjFromApi = jObject.GetValue("user").ToString();
                            clsUser usr = JsonConvert.DeserializeObject<clsUser>(userObjFromApi);

                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserId, usr.UserId.ToString());
                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserFullName, usr.Full_Name);
                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserEmail, usr.Email);
                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserPassword, user.Password);
                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserType, usr.User_Type.ToString());

                            //if (!verified)
                            //{
                            //    await SendOTP(usr.UserId, loginVM);
                            //    loginVM.User = usr;
                            //    Device.BeginInvokeOnMainThread(async () =>
                            //    {
                            //        //IsBusy = false;
                            //        //Application.Current.MainPage = new CodeVerificationPage(loginVM);
                            //        await navigation.PushAsync(new CodeVerificationPage(loginVM));
                            //    });
                            //}
                            //else
                            //{

                            //    //await SecureStorage.SetAsync("oauth_token", "secret-oauth-token-value-from-api");
                            //    Device.BeginInvokeOnMainThread(() =>
                            //    {
                            //        //IsBusy = false;
                            //        if (usr.IsProfileCompleted ?? false)
                            //        {
                            //            Application.Current.MainPage = new AppShell();
                            //        }
                            //        else
                            //        {
                            //            usr.Password = user.Password;
                            //            navigation.PushAsync(new FinishRegistrationPage(usr));
                            //        }
                            //    });
                            //}
                        }

                        res = status;
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            //IsBusy = false;
                            await navigation.PushPopupAsync(new PopupAlert("E", "Server error", "OK"));
                        });
                        //IsBusy = false;
                        //await SendEmail(responseStatusCode, memberName, sourceFilePath, sourceLineNumber, "", "Exception From Pantex Health");
                    }
                }
                catch (Exception ex)
                {
                    //IsBusy = false;
                    res = false;
                    //await SendEmail(responseStatusCode, memberName, sourceFilePath, sourceLineNumber, ex.StackTrace, "Exception From Pantex Health");
                }
            }

            //IsBusy = false;
            return res;

        }
        #endregion

        #region Register ApiCall
        public async System.Threading.Tasks.Task<bool> RegisterUser(clsUser user)
        {
            bool res = false;
            HttpStatusCode responseStatusCode = 0;

            if (await Constants.IsInternetConnected())
            {
                try
                {
                    //IsBusy = true;
                    var Httpclient = new HttpClient();

                    var url = Utility.Constants.CompleteURL + "/Register";

                    var uri = new Uri(string.Format(url, string.Empty));


                    var json = JsonConvert.SerializeObject(user);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = null;

                    response = await Httpclient.PostAsync(uri, content);

                    responseStatusCode = response.StatusCode;

                    if (responseStatusCode == HttpStatusCode.OK)
                    {

                        var responseContent = await response.Content.ReadAsStringAsync();

                        var jObject = JObject.Parse(responseContent);
                        bool status = (bool)jObject.GetValue("status");

                        if (!status)
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                //IsBusy = false;
                                await navigation.PushPopupAsync(new PopupAlert("E", jObject.GetValue("message").ToString(), "OK"));
                            });
                        }
                        else
                        {

                            //bool verified = (bool)jObject.GetValue("IsVerified");

                            string userIdFromApi = jObject.GetValue("returnId").ToString();

                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserId, userIdFromApi);
                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserFullName, user.Full_Name);
                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserEmail, user.Email);
                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserPassword, user.Password);
                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserType, user.User_Type.ToString());

                            //if (!verified)
                            //{
                            //    await SendOTP(usr.UserId, loginVM);
                            //    loginVM.User = usr;
                            //    Device.BeginInvokeOnMainThread(async () =>
                            //    {
                            //        //IsBusy = false;
                            //        //Application.Current.MainPage = new CodeVerificationPage(loginVM);
                            //        await navigation.PushAsync(new CodeVerificationPage(loginVM));
                            //    });
                            //}
                            //else
                            //{

                            //    //await SecureStorage.SetAsync("oauth_token", "secret-oauth-token-value-from-api");
                            //    Device.BeginInvokeOnMainThread(() =>
                            //    {
                            //        //IsBusy = false;
                            //        if (usr.IsProfileCompleted ?? false)
                            //        {
                            //            Application.Current.MainPage = new AppShell();
                            //        }
                            //        else
                            //        {
                            //            usr.Password = user.Password;
                            //            navigation.PushAsync(new FinishRegistrationPage(usr));
                            //        }
                            //    });
                            //}
                        }

                        res = status;
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            //IsBusy = false;
                            await navigation.PushPopupAsync(new PopupAlert("E", "Server error", "OK"));
                        });
                        //IsBusy = false;
                        //await SendEmail(responseStatusCode, memberName, sourceFilePath, sourceLineNumber, "", "Exception From Pantex Health");
                    }
                }
                catch (Exception ex)
                {
                    //IsBusy = false;
                    res = false;
                    //await SendEmail(responseStatusCode, memberName, sourceFilePath, sourceLineNumber, ex.StackTrace, "Exception From Pantex Health");
                }
            }

            //IsBusy = false;
            return res;

        }
        #endregion

        #region Get Guests List
        public async Task<ObservableCollection<clsUser>> GetGuestsHostsList()
        {
            ObservableCollection<clsUser> res = new ObservableCollection<clsUser>();

            HttpStatusCode responseStatusCode = 0;

            if (await Constants.IsInternetConnected())
            {
                try
                {
                    var Httpclient = new HttpClient();

                    string userId = await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserId);
                    string userTypeId = await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserType);
                    var url = Constants.CompleteURL + "/GetGuestsOrHosts/?id=" + userId + "&userTypeId=" + userTypeId;

                    var uri = new Uri(string.Format(url, string.Empty));

                    HttpResponseMessage response = null;

                    response = await Httpclient.GetAsync(uri);

                    responseStatusCode = response.StatusCode;

                    if (responseStatusCode == HttpStatusCode.OK)
                    {

                        var responseContent = await response.Content.ReadAsStringAsync();

                        JObject jObject = JObject.Parse(responseContent);
                        bool status = (bool)jObject.GetValue("status");
                        if (!status)
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await navigation.PushPopupAsync(new PopupAlert("E", jObject.GetValue("message").ToString(), "OK"));
                            });
                        }
                        else
                        {
                            var logj = (JArray)jObject["guestsList"];

                            res = logj.ToObject<ObservableCollection<clsUser>>();
                        }

                        //var los = JArray.Parse(responseContent);
                        //res = los.ToObject<ObservableCollection<clsUserSkill>>();
                    }
                    else
                    {
                        //IsBusy = false;
                        //await SendEmail(responseStatusCode, memberName, sourceFilePath, sourceLineNumber, "", "Exception From Pantex Health");
                    }
                }
                catch (Exception ex)
                {
                    //IsBusy = false;
                    //await SendEmail(responseStatusCode, memberName, sourceFilePath, sourceLineNumber, ex.StackTrace, "Exception From Pantex Health");
                }
            }

            //IsBusy = false;
            return res;

        }
        #endregion
    }
}
