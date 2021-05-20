using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TourEase.Models;
using TourEase.Popup_Views;
using TourEase.ViewModels;
using TourEase.Views;
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
        public async System.Threading.Tasks.Task<bool> LoginUser(clsUser user, LoginViewModel loginVM)
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
                            res = false;
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                //IsBusy = false;
                                await navigation.PushPopupAsync(new PopupAlert("E", jObject.GetValue("message").ToString(), "OK"));
                            });
                        }
                        else
                        {
                            res = true;
                            //bool verified = (bool)jObject.GetValue("IsVerified");

                            string userObjFromApi = jObject.GetValue("user").ToString();
                            clsUser usr = JsonConvert.DeserializeObject<clsUser>(userObjFromApi);

                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserId, usr.UserId.ToString());
                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserFullName, usr.Full_Name);
                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserEmail, usr.Email);
                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserPassword, user.Password);
                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserType, usr.User_Type.ToString());
                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyIsAdmin, false.ToString());

                            if ((!(usr.Is_Verified ?? false)) || (!(usr.Is_Enabled ?? false)))
                            {
                                res = false;
                                if (!(usr.Is_Verified ?? false))
                                {
                                    loginVM.User = usr;
                                    if (await SendOTP(usr.UserId, loginVM))
                                    {
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            //IsBusy = false;
                                            Application.Current.MainPage = new NavigationPage(new CodeVerificationPage(loginVM));
                                            //await navigation.PushAsync(new CodeVerificationPage(loginVM));
                                        });
                                    }
                                    else
                                    {
                                        SecureStorageClass.ClearAll();
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            Application.Current.MainPage = new NavigationPage(new LoginPage());
                                        });
                                    }
                                }
                                else if (!(usr.Is_Enabled ?? false))
                                {
                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        PopupNavigation.Instance.PushAsync(new PopupAlert("W", "Your account is disabled. Please contact TourEase management to re-enable it. Thankyou.", "OK"));
                                    });
                                }
                                //else
                                //{
                                //    Device.BeginInvokeOnMainThread(() =>
                                //    {
                                //        PopupNavigation.Instance.PushAsync(new PopupAlert("W", "An error occurred while sending otp to your email.", "OK"));
                                //    });
                                //}
                            }
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

        public async System.Threading.Tasks.Task<bool> LoginAdmin(clsUser admin, LoginViewModel loginVM)
        {
            bool res = false;
            HttpStatusCode responseStatusCode = 0;

            if (await Constants.IsInternetConnected())
            {
                try
                {
                    //IsBusy = true;
                    var Httpclient = new HttpClient();

                    var url = Utility.Constants.CompleteURL + "/LoginAsAdmin";

                    var uri = new Uri(string.Format(url, string.Empty));


                    var json = JsonConvert.SerializeObject(admin);
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
                            res = false;
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                //IsBusy = false;
                                await navigation.PushPopupAsync(new PopupAlert("E", jObject.GetValue("message").ToString(), "OK"));
                            });
                        }
                        else
                        {
                            res = true;
                            //bool verified = (bool)jObject.GetValue("IsVerified");

                            string userObjFromApi = jObject.GetValue("user").ToString();
                            clsUser usr = JsonConvert.DeserializeObject<clsUser>(userObjFromApi);

                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserId, usr.UserId.ToString());
                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserFullName, usr.Full_Name);
                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserEmail, usr.Email);
                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserPassword, admin.Password);
                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserType, usr.User_Type.ToString());
                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyIsAdmin, true.ToString());

                            if (!(usr.Is_Verified ?? false))
                            {
                                res = false;
                                loginVM.User = usr;
                                if (await SendOTP(usr.UserId, loginVM))
                                {
                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        //IsBusy = false;
                                        Application.Current.MainPage = new NavigationPage(new CodeVerificationPage(loginVM));
                                        //await navigation.PushAsync(new CodeVerificationPage(loginVM));
                                    });
                                }
                                else
                                {
                                    SecureStorageClass.ClearAll();
                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        Application.Current.MainPage = new NavigationPage(new LoginPage());
                                    });
                                }
                                //else
                                //{
                                //    Device.BeginInvokeOnMainThread(() =>
                                //    {
                                //        PopupNavigation.Instance.PushAsync(new PopupAlert("W", "An error occurred while sending otp to your email.", "OK"));
                                //    });
                                //}
                            }
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
        public async System.Threading.Tasks.Task<bool> RegisterUser(clsUser user, RegisterViewModel regVM)
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
                        regVM.CodeFromApi = jObject.GetValue("otp").ToString();

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

                            int userIdFromApi = (Int32)jObject.GetValue("returnId");
                            regVM.User.UserId = userIdFromApi;

                            await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserId, userIdFromApi.ToString());
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

        #region Update Password ApiCall
        public async System.Threading.Tasks.Task<bool> UpdatePassword(clsUser User)
        {
            bool res = false;
            HttpStatusCode responseStatusCode = 0;

            if (await Constants.IsInternetConnected())
            {
                try
                {
                    IsBusy = true;
                    var Httpclient = new HttpClient();

                    var url = Utility.Constants.CompleteURL + "/UpdatePassword";

                    var uri = new Uri(string.Format(url, string.Empty));


                    var json = JsonConvert.SerializeObject(new { user = User });
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
                                IsBusy = false;
                                await navigation.PushPopupAsync(new PopupAlert("E", jObject.GetValue("message").ToString(), "OK"));
                            });
                        }
                        else
                        {
                            string userObjFromApi = jObject.GetValue("UserObject").ToString();
                            clsUser usr = JsonConvert.DeserializeObject<clsUser>(userObjFromApi);

                            if (usr != null)
                            {
                                await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserId, usr.UserId.ToString());
                                await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserFullName, usr.Full_Name);
                                await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserEmail, usr.Email);
                                await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserPassword, User.Password);
                                await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserType, usr.User_Type.ToString());


                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    IsBusy = false;

                                    if (usr.Is_Verified ?? false)
                                    {
                                        Application.Current.MainPage = new HomePage();
                                    }
                                    else
                                    {
                                        Application.Current.MainPage = new LoginPage();
                                    }
                                });
                            }
                        }

                        res = status;
                    }
                    else
                    {
                        IsBusy = false;
                        //await SendEmail(responseStatusCode, memberName, sourceFilePath, sourceLineNumber, "", "Exception From Pantex Health");
                    }
                }
                catch (Exception ex)
                {
                    IsBusy = false;
                    res = false;
                    //await SendEmail(responseStatusCode, memberName, sourceFilePath, sourceLineNumber, ex.StackTrace, "Exception From Pantex Health");
                }
            }

            IsBusy = false;
            return res;

        }
        #endregion

        #region Send Code ApiCall
        public async System.Threading.Tasks.Task<bool> SendOTP(int id, LoginViewModel loginVM = null, RegisterViewModel regVM = null)
        {
            bool res = false;
            HttpStatusCode responseStatusCode = 0;

            if (await Constants.IsInternetConnected())
            {
                try
                {
                    //IsBusy = true;
                    var Httpclient = new HttpClient();

                    var url = Utility.Constants.CompleteURL + "/SendCodeAgain/?id=" + id;

                    var uri = new Uri(string.Format(url, string.Empty));

                    HttpResponseMessage response = null;

                    response = await Httpclient.GetAsync(uri);

                    responseStatusCode = response.StatusCode;

                    if (responseStatusCode == HttpStatusCode.OK)
                    {

                        var responseContent = await response.Content.ReadAsStringAsync();

                        var jObject = JObject.Parse(responseContent);
                        bool status = (bool)jObject.GetValue("status");

                        if (!status)
                        {
                            //IsBusy = false;
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await navigation.PushPopupAsync(new PopupAlert("E", jObject.GetValue("message").ToString(), "OK"));
                            });
                        }
                        else
                        {
                            if (loginVM != null)
                            {
                                loginVM.ResendCodeCommandCanRun = false;
                                Device.StartTimer(TimeSpan.FromSeconds(30), () =>
                                {
                                    // Do something
                                    loginVM.ResendCodeCommandCanRun = true;
                                    return false; // True = Repeat again, False = Stop the timer
                                });

                                loginVM.CodeFromApi = jObject.GetValue("code").ToString();
                            }
                            if (regVM != null)
                            {
                                regVM.ResendCodeCommandCanRun = false;
                                Device.StartTimer(TimeSpan.FromSeconds(30), () =>
                                {
                                    // Do something
                                    regVM.ResendCodeCommandCanRun = true;
                                    return false; // True = Repeat again, False = Stop the timer
                                });

                                regVM.CodeFromApi = jObject.GetValue("code").ToString();
                            }

                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await navigation.PushPopupAsync(new PopupAlert("S", "A code is sent to your email. Please use it to verify your email.", "OK"));
                            });
                        }

                        res = status;
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
                    res = false;
                    //await SendEmail(responseStatusCode, memberName, sourceFilePath, sourceLineNumber, ex.StackTrace, "Exception From Pantex Health");
                }
            }

            IsBusy = false;
            return res;

        }

        public async System.Threading.Tasks.Task<bool> SendOTPToEmail(string email, PasswordResetViewModel passwordResetVM)
        {
            bool res = false;
            HttpStatusCode responseStatusCode = 0;

            if (await Constants.IsInternetConnected())
            {
                try
                {
                    IsBusy = true;
                    var Httpclient = new HttpClient();

                    var url = Utility.Constants.CompleteURL + "/SendOTPToEmail/?email=" + email;

                    var uri = new Uri(string.Format(url, string.Empty));

                    HttpResponseMessage response = null;

                    response = await Httpclient.GetAsync(uri);

                    responseStatusCode = response.StatusCode;

                    if (responseStatusCode == HttpStatusCode.OK)
                    {

                        var responseContent = await response.Content.ReadAsStringAsync();

                        var jObject = JObject.Parse(responseContent);
                        bool status = (bool)jObject.GetValue("status");

                        if (!status)
                        {
                            IsBusy = false;
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await navigation.PushPopupAsync(new PopupAlert("E", jObject.GetValue("message").ToString(), "OK"));
                            });
                        }
                        else
                        {
                            IsBusy = false;
                            if (passwordResetVM != null)
                            {
                                passwordResetVM.ResendCodeCommandCanRun = false;
                                Device.StartTimer(TimeSpan.FromSeconds(30), () =>
                                {
                                    // Do something
                                    passwordResetVM.ResendCodeCommandCanRun = true;
                                    return false; // True = Repeat again, False = Stop the timer
                                });

                                passwordResetVM.CodeFromApi = jObject.GetValue("code").ToString();
                            }

                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await navigation.PushPopupAsync(new PopupAlert("S", "A code is sent to your email address. Please use it to verify your email address.", "OK"));
                            });
                        }

                        res = status;
                    }
                    else
                    {
                        IsBusy = false;
                        //await SendEmail(responseStatusCode, memberName, sourceFilePath, sourceLineNumber, "", "Exception From Pantex Health");
                    }
                }
                catch (Exception ex)
                {
                    IsBusy = false;
                    res = false;
                    //await SendEmail(responseStatusCode, memberName, sourceFilePath, sourceLineNumber, ex.StackTrace, "Exception From Pantex Health");
                }
            }

            IsBusy = false;
            return res;

        }
        #endregion

        #region Verify User
        public async Task<bool> VerifyUser(int userId)
        {
            bool res = false;
            IsBusy = true;
            try
            {
                if (await Constants.IsInternetConnected())
                {
                    // Connection to internet is available
                    using (var client = new HttpClient())
                    {
                        var url = Constants.CompleteURL + "/verifyuser";

                        var uri = new Uri(string.Format(url, string.Empty));

                        //string sessionId = await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserSessionId);

                        var json = "{ 'id': " + userId + " }";
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = null;

                        response = await client.PostAsync(uri, content);
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<clsResponse>(responseContent);

                        IsBusy = false;
                        if (response.IsSuccessStatusCode)
                        {
                            if (apiResponse.status)
                            {
                                await PopupNavigation.Instance.PushAsync(new PopupAlert("S", "Account is verified successfully.", "OK"));
                                //Application.Current.MainPage = new FinishRegistrationTechPage(UserDetail);
                                //Application.Current.MainPage = new VerifyEmailOnRegistration(this);
                            }
                            else
                            {
                                await PopupNavigation.Instance.PushAsync(new PopupAlert("E", apiResponse.message, "OK"));
                                //await Application.Current.MainPage.DisplayAlert("Error", apiResponse.message, "OK");
                            }

                            res = apiResponse.status;
                        }
                        else
                        {
                            await PopupNavigation.Instance.PushAsync(new PopupAlert("E", response.Content.ReadAsStringAsync().Result, "OK"));
                            //await Application.Current.MainPage.DisplayAlert("Error", response.Content.ReadAsStringAsync().Result, "OK");
                        }
                    }

                }
                //else
                //{
                //    PopUpData popUp = new PopUpData("crossRed.png", "", GlobalData.InactiveInternet, "OK");
                //    await Application.Current.MainPage.Navigation.PushPopupAsync(new PopUpPage(popUp));
                //    //await Application.Current.MainPage.DisplayAlert("Internet Problem", "Its seems like your internet is not active. Please check your internet.", "OK");
                //}

            }
            catch (Exception e)
            {
                await PopupNavigation.Instance.PushAsync(new PopupAlert("E", "Something wrong happened. Please try again.", "OK"));
                //await Application.Current.MainPage.DisplayAlert("Error", "Something wrong happened. Try again", "OK");
            }
            IsBusy = false;

            return res;
        }
        #endregion

        #region Send Requests
        public async Task<bool> RequestGuest(int userId, int guestId, string requestMessage)
        {
            bool res = false;
            try
            {
                if (await Constants.IsInternetConnected())
                {
                    // Connection to internet is available
                    using (var client = new HttpClient())
                    {
                        var url = Constants.CompleteURL + "/requestguest";

                        var uri = new Uri(string.Format(url, string.Empty));

                        var json = JsonConvert.SerializeObject(new { userId = userId, guestId = guestId, requestMessage = requestMessage });
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = null;

                        response = await client.PostAsync(uri, content);
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<clsResponse>(responseContent);

                        IsBusy = false;
                        if (response.IsSuccessStatusCode)
                        {
                            if (apiResponse.status)
                            {
                                await PopupNavigation.Instance.PushAsync(new PopupAlert("S", "Your request is sent successfully.", "OK"));
                                //Application.Current.MainPage = new FinishRegistrationTechPage(UserDetail);
                                //Application.Current.MainPage = new VerifyEmailOnRegistration(this);
                            }
                            else
                            {
                                await PopupNavigation.Instance.PushAsync(new PopupAlert("E", apiResponse.message, "OK"));
                                //await Application.Current.MainPage.DisplayAlert("Error", apiResponse.message, "OK");
                            }

                            res = apiResponse.status;
                        }
                        else
                        {
                            await PopupNavigation.Instance.PushAsync(new PopupAlert("E", response.Content.ReadAsStringAsync().Result, "OK"));
                            //await Application.Current.MainPage.DisplayAlert("Error", response.Content.ReadAsStringAsync().Result, "OK");
                        }
                    }

                }
                //else
                //{
                //    PopUpData popUp = new PopUpData("crossRed.png", "", GlobalData.InactiveInternet, "OK");
                //    await Application.Current.MainPage.Navigation.PushPopupAsync(new PopUpPage(popUp));
                //    //await Application.Current.MainPage.DisplayAlert("Internet Problem", "Its seems like your internet is not active. Please check your internet.", "OK");
                //}

            }
            catch (Exception e)
            {
                await PopupNavigation.Instance.PushAsync(new PopupAlert("E", "Something wrong happened. Please try again.", "OK"));
                //await Application.Current.MainPage.DisplayAlert("Error", "Something wrong happened. Try again", "OK");
            }

            return res;
        }

        public async Task<bool> RequestHost(int userId, int hostId, string requestMessage)
        {
            bool res = false;
            try
            {
                if (await Constants.IsInternetConnected())
                {
                    // Connection to internet is available
                    using (var client = new HttpClient())
                    {
                        var url = Constants.CompleteURL + "/requesthost";

                        var uri = new Uri(string.Format(url, string.Empty));

                        var json = JsonConvert.SerializeObject(new { userId = userId, hostId = hostId, requestMessage = requestMessage });
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = null;

                        response = await client.PostAsync(uri, content);
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<clsResponse>(responseContent);

                        IsBusy = false;
                        if (response.IsSuccessStatusCode)
                        {
                            if (apiResponse.status)
                            {
                                await PopupNavigation.Instance.PushAsync(new PopupAlert("S", "Your request is sent successfully.", "OK"));
                                //Application.Current.MainPage = new FinishRegistrationTechPage(UserDetail);
                                //Application.Current.MainPage = new VerifyEmailOnRegistration(this);
                            }
                            else
                            {
                                await PopupNavigation.Instance.PushAsync(new PopupAlert("E", apiResponse.message, "OK"));
                                //await Application.Current.MainPage.DisplayAlert("Error", apiResponse.message, "OK");
                            }

                            res = apiResponse.status;
                        }
                        else
                        {
                            await PopupNavigation.Instance.PushAsync(new PopupAlert("E", response.Content.ReadAsStringAsync().Result, "OK"));
                            //await Application.Current.MainPage.DisplayAlert("Error", response.Content.ReadAsStringAsync().Result, "OK");
                        }
                    }

                }
                //else
                //{
                //    PopUpData popUp = new PopUpData("crossRed.png", "", GlobalData.InactiveInternet, "OK");
                //    await Application.Current.MainPage.Navigation.PushPopupAsync(new PopUpPage(popUp));
                //    //await Application.Current.MainPage.DisplayAlert("Internet Problem", "Its seems like your internet is not active. Please check your internet.", "OK");
                //}

            }
            catch (Exception e)
            {
                await PopupNavigation.Instance.PushAsync(new PopupAlert("E", "Something wrong happened. Please try again.", "OK"));
                //await Application.Current.MainPage.DisplayAlert("Error", "Something wrong happened. Try again", "OK");
            }

            return res;
        }

        public async Task<bool> EventRequest(int senderId, string requestMessage) // senderId is HostId of host who is hosting event and inviting guests
        {
            bool res = false;
            try
            {
                if (await Constants.IsInternetConnected())
                {
                    // Connection to internet is available
                    using (var client = new HttpClient())
                    {
                        var url = Constants.CompleteURL + "/EventRequest";

                        var uri = new Uri(string.Format(url, string.Empty));

                        var json = JsonConvert.SerializeObject(new { senderId = senderId, requestMessage = requestMessage });
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = null;

                        response = await client.PostAsync(uri, content);
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<clsResponse>(responseContent);

                        IsBusy = false;
                        if (response.IsSuccessStatusCode)
                        {
                            if (apiResponse.status)
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    PopupNavigation.Instance.PushAsync(new PopupAlert("S", "Event request sent successfully.", "OK"));
                                });
                                //Application.Current.MainPage = new FinishRegistrationTechPage(UserDetail);
                                //Application.Current.MainPage = new VerifyEmailOnRegistration(this);
                            }
                            else
                            {
                                await PopupNavigation.Instance.PushAsync(new PopupAlert("E", apiResponse.message, "OK"));
                                //await Application.Current.MainPage.DisplayAlert("Error", apiResponse.message, "OK");
                            }

                            res = apiResponse.status;
                        }
                        else
                        {
                            await PopupNavigation.Instance.PushAsync(new PopupAlert("E", response.Content.ReadAsStringAsync().Result, "OK"));
                            //await Application.Current.MainPage.DisplayAlert("Error", response.Content.ReadAsStringAsync().Result, "OK");
                        }
                    }

                }
                //else
                //{
                //    PopUpData popUp = new PopUpData("crossRed.png", "", GlobalData.InactiveInternet, "OK");
                //    await Application.Current.MainPage.Navigation.PushPopupAsync(new PopUpPage(popUp));
                //    //await Application.Current.MainPage.DisplayAlert("Internet Problem", "Its seems like your internet is not active. Please check your internet.", "OK");
                //}

            }
            catch (Exception e)
            {
                await PopupNavigation.Instance.PushAsync(new PopupAlert("E", "Something wrong happened. Please try again.", "OK"));
                //await Application.Current.MainPage.DisplayAlert("Error", "Something wrong happened. Try again", "OK");
            }

            return res;
        }

        public async Task<bool> CompanionRequest(int senderId, string requestMessage) // senderId is HostId of host who is hosting event and inviting guests
        {
            bool res = false;
            try
            {
                if (await Constants.IsInternetConnected())
                {
                    // Connection to internet is available
                    using (var client = new HttpClient())
                    {
                        var url = Constants.CompleteURL + "/CompanionRequest";

                        var uri = new Uri(string.Format(url, string.Empty));

                        var json = JsonConvert.SerializeObject(new { senderId = senderId, requestMessage = requestMessage });
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = null;

                        response = await client.PostAsync(uri, content);
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<clsResponse>(responseContent);

                        IsBusy = false;
                        if (response.IsSuccessStatusCode)
                        {
                            if (apiResponse.status)
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    PopupNavigation.Instance.PushAsync(new PopupAlert("S", "Companion request sent successfully.", "OK"));
                                });
                                //Application.Current.MainPage = new FinishRegistrationTechPage(UserDetail);
                                //Application.Current.MainPage = new VerifyEmailOnRegistration(this);
                            }
                            else
                            {
                                await PopupNavigation.Instance.PushAsync(new PopupAlert("E", apiResponse.message, "OK"));
                                //await Application.Current.MainPage.DisplayAlert("Error", apiResponse.message, "OK");
                            }

                            res = apiResponse.status;
                        }
                        else
                        {
                            await PopupNavigation.Instance.PushAsync(new PopupAlert("E", response.Content.ReadAsStringAsync().Result, "OK"));
                            //await Application.Current.MainPage.DisplayAlert("Error", response.Content.ReadAsStringAsync().Result, "OK");
                        }
                    }

                }
                //else
                //{
                //    PopUpData popUp = new PopUpData("crossRed.png", "", GlobalData.InactiveInternet, "OK");
                //    await Application.Current.MainPage.Navigation.PushPopupAsync(new PopUpPage(popUp));
                //    //await Application.Current.MainPage.DisplayAlert("Internet Problem", "Its seems like your internet is not active. Please check your internet.", "OK");
                //}

            }
            catch (Exception e)
            {
                await PopupNavigation.Instance.PushAsync(new PopupAlert("E", "Something wrong happened. Please try again.", "OK"));
                //await Application.Current.MainPage.DisplayAlert("Error", "Something wrong happened. Try again", "OK");
            }

            return res;
        }
        #endregion

        #region Report as fake
        public async Task<bool> ReportUser(int userId)
        {
            bool res = false;
            try
            {
                if (await Constants.IsInternetConnected())
                {
                    // Connection to internet is available
                    using (var client = new HttpClient())
                    {
                        var url = Constants.CompleteURL + "/reportuser";

                        var uri = new Uri(string.Format(url, string.Empty));

                        //string sessionId = await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserSessionId);

                        var json = JsonConvert.SerializeObject(new { id = userId });
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = null;

                        response = await client.PostAsync(uri, content);
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<clsResponse>(responseContent);

                        if (response.IsSuccessStatusCode)
                        {
                            if (!apiResponse.status)
                            {
                                await PopupNavigation.Instance.PushAsync(new PopupAlert("E", apiResponse.message, "OK"));
                                //await PopupNavigation.Instance.PushAsync(new PopupAlert("S", "Account reported successfully.", "OK"));
                                //Application.Current.MainPage = new FinishRegistrationTechPage(UserDetail);
                                //Application.Current.MainPage = new VerifyEmailOnRegistration(this);
                            }
                            //else
                            //{
                            //    await PopupNavigation.Instance.PushAsync(new PopupAlert("E", apiResponse.message, "OK"));
                            //    //await Application.Current.MainPage.DisplayAlert("Error", apiResponse.message, "OK");
                            //}

                            res = apiResponse.status;
                        }
                        else
                        {
                            await PopupNavigation.Instance.PushAsync(new PopupAlert("E", response.Content.ReadAsStringAsync().Result, "OK"));
                            //await Application.Current.MainPage.DisplayAlert("Error", response.Content.ReadAsStringAsync().Result, "OK");
                        }
                    }

                }
                //else
                //{
                //    PopUpData popUp = new PopUpData("crossRed.png", "", GlobalData.InactiveInternet, "OK");
                //    await Application.Current.MainPage.Navigation.PushPopupAsync(new PopUpPage(popUp));
                //    //await Application.Current.MainPage.DisplayAlert("Internet Problem", "Its seems like your internet is not active. Please check your internet.", "OK");
                //}

            }
            catch (Exception e)
            {
                await PopupNavigation.Instance.PushAsync(new PopupAlert("E", "Something wrong happened. Please try again.", "OK"));
                //await Application.Current.MainPage.DisplayAlert("Error", "Something wrong happened. Try again", "OK");
            }

            return res;
        }
        #endregion

        #region Get Requests List
        public async Task<ObservableCollection<clsRequest>> GetGuestsHostsReceivedRequestsList()
        {
            ObservableCollection<clsRequest> res = new ObservableCollection<clsRequest>();

            HttpStatusCode responseStatusCode = 0;

            if (await Constants.IsInternetConnected())
            {
                try
                {
                    var Httpclient = new HttpClient();

                    string userId = await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserId);
                    string userTypeId = await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserType);
                    var url = Constants.CompleteURL + "/GetGuestsHostsReceivedRequests/?id=" + userId + "&userTypeId=" + userTypeId;

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
                            var logj = (JArray)jObject["requestsList"];

                            res = logj.ToObject<ObservableCollection<clsRequest>>();
                        }

                        //var los = JArray.Parse(responseContent);
                        //res = los.ToObject<ObservableCollection<clsUserSkill>>();
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await navigation.PushPopupAsync(new PopupAlert("E", "Server ran into a problem.", "OK"));
                        });
                        //IsBusy = false;
                        //await SendEmail(responseStatusCode, memberName, sourceFilePath, sourceLineNumber, "", "Exception From Pantex Health");
                    }
                }
                catch (Exception ex)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await navigation.PushPopupAsync(new PopupAlert("E", ex.Message, "OK"));
                    });
                    //IsBusy = false;
                    //await SendEmail(responseStatusCode, memberName, sourceFilePath, sourceLineNumber, ex.StackTrace, "Exception From Pantex Health");
                }
            }

            //IsBusy = false;
            return res;

        }

        public async Task<ObservableCollection<clsRequest>> GetGuestsHostsSentRequests()
        {
            ObservableCollection<clsRequest> res = new ObservableCollection<clsRequest>();

            HttpStatusCode responseStatusCode = 0;

            if (await Constants.IsInternetConnected())
            {
                try
                {
                    var Httpclient = new HttpClient();

                    string userId = await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserId);
                    string userTypeId = await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserType);
                    var url = Constants.CompleteURL + "/GetGuestsHostsSentRequests/?id=" + userId + "&userTypeId=" + userTypeId;

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
                            var logj = (JArray)jObject["requestsList"];

                            res = logj.ToObject<ObservableCollection<clsRequest>>();
                        }

                        //var los = JArray.Parse(responseContent);
                        //res = los.ToObject<ObservableCollection<clsUserSkill>>();
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await navigation.PushPopupAsync(new PopupAlert("E", "Server ran into a problem.", "OK"));
                        });
                        //IsBusy = false;
                        //await SendEmail(responseStatusCode, memberName, sourceFilePath, sourceLineNumber, "", "Exception From Pantex Health");
                    }
                }
                catch (Exception ex)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await navigation.PushPopupAsync(new PopupAlert("E", ex.Message, "OK"));
                    });
                    //IsBusy = false;
                    //await SendEmail(responseStatusCode, memberName, sourceFilePath, sourceLineNumber, ex.StackTrace, "Exception From Pantex Health");
                }
            }

            //IsBusy = false;
            return res;

        }
        #endregion

        #region Accept Request
        public async Task<bool> AcceptGuestRequest(int requestId)
        {
            bool res = false;
            try
            {
                if (await Constants.IsInternetConnected())
                {
                    // Connection to internet is available
                    using (var client = new HttpClient())
                    {
                        var url = Constants.CompleteURL + "/AcceptGuestRequest";

                        var uri = new Uri(string.Format(url, string.Empty));

                        //string sessionId = await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserSessionId);

                        var json = JsonConvert.SerializeObject(new { requestId = requestId });
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = null;

                        response = await client.PostAsync(uri, content);
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<clsResponse>(responseContent);

                        IsBusy = false;
                        if (response.IsSuccessStatusCode)
                        {
                            if (!apiResponse.status)
                            {
                                await PopupNavigation.Instance.PushAsync(new PopupAlert("E", apiResponse.message, "OK"));
                                //await PopupNavigation.Instance.PushAsync(new PopupAlert("S", "Account reported successfully.", "OK"));
                                //Application.Current.MainPage = new FinishRegistrationTechPage(UserDetail);
                                //Application.Current.MainPage = new VerifyEmailOnRegistration(this);
                            }
                            //else
                            //{
                            //    await PopupNavigation.Instance.PushAsync(new PopupAlert("E", apiResponse.message, "OK"));
                            //    //await Application.Current.MainPage.DisplayAlert("Error", apiResponse.message, "OK");
                            //}

                            res = apiResponse.status;
                        }
                        else
                        {
                            await PopupNavigation.Instance.PushAsync(new PopupAlert("E", response.Content.ReadAsStringAsync().Result, "OK"));
                            //await Application.Current.MainPage.DisplayAlert("Error", response.Content.ReadAsStringAsync().Result, "OK");
                        }
                    }

                }
                //else
                //{
                //    PopUpData popUp = new PopUpData("crossRed.png", "", GlobalData.InactiveInternet, "OK");
                //    await Application.Current.MainPage.Navigation.PushPopupAsync(new PopUpPage(popUp));
                //    //await Application.Current.MainPage.DisplayAlert("Internet Problem", "Its seems like your internet is not active. Please check your internet.", "OK");
                //}

            }
            catch (Exception e)
            {
                await PopupNavigation.Instance.PushAsync(new PopupAlert("E", "Something wrong happened. Please try again.", "OK"));
                //await Application.Current.MainPage.DisplayAlert("Error", "Something wrong happened. Try again", "OK");
            }

            return res;
        }

        public async Task<bool> AcceptHostRequest(int requestId)
        {
            bool res = false;
            try
            {
                if (await Constants.IsInternetConnected())
                {
                    // Connection to internet is available
                    using (var client = new HttpClient())
                    {
                        var url = Constants.CompleteURL + "/AcceptHostRequest";

                        var uri = new Uri(string.Format(url, string.Empty));

                        //string sessionId = await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserSessionId);

                        var json = JsonConvert.SerializeObject(new { requestId = requestId });
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = null;

                        response = await client.PostAsync(uri, content);
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<clsResponse>(responseContent);

                        IsBusy = false;
                        if (response.IsSuccessStatusCode)
                        {
                            if (!apiResponse.status)
                            {
                                await PopupNavigation.Instance.PushAsync(new PopupAlert("E", apiResponse.message, "OK"));
                                //await PopupNavigation.Instance.PushAsync(new PopupAlert("S", "Account reported successfully.", "OK"));
                                //Application.Current.MainPage = new FinishRegistrationTechPage(UserDetail);
                                //Application.Current.MainPage = new VerifyEmailOnRegistration(this);
                            }
                            //else
                            //{
                            //    await PopupNavigation.Instance.PushAsync(new PopupAlert("E", apiResponse.message, "OK"));
                            //    //await Application.Current.MainPage.DisplayAlert("Error", apiResponse.message, "OK");
                            //}

                            res = apiResponse.status;
                        }
                        else
                        {
                            await PopupNavigation.Instance.PushAsync(new PopupAlert("E", response.Content.ReadAsStringAsync().Result, "OK"));
                            //await Application.Current.MainPage.DisplayAlert("Error", response.Content.ReadAsStringAsync().Result, "OK");
                        }
                    }

                }
                //else
                //{
                //    PopUpData popUp = new PopUpData("crossRed.png", "", GlobalData.InactiveInternet, "OK");
                //    await Application.Current.MainPage.Navigation.PushPopupAsync(new PopUpPage(popUp));
                //    //await Application.Current.MainPage.DisplayAlert("Internet Problem", "Its seems like your internet is not active. Please check your internet.", "OK");
                //}

            }
            catch (Exception e)
            {
                await PopupNavigation.Instance.PushAsync(new PopupAlert("E", "Something wrong happened. Please try again.", "OK"));
                //await Application.Current.MainPage.DisplayAlert("Error", "Something wrong happened. Try again", "OK");
            }

            return res;
        }
        #endregion

        #region Finish Request And Save Rating
        public async Task<bool> SaveGuestRequestRating(int requestId, int receiverId, double? rating = 5)
        {
            bool res = false;
            try
            {
                if (await Constants.IsInternetConnected())
                {
                    // Connection to internet is available
                    using (var client = new HttpClient())
                    {
                        var url = Constants.CompleteURL + "/SaveGuestHostRequestRating";

                        var uri = new Uri(string.Format(url, string.Empty));

                        //string sessionId = await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserSessionId);

                        var json = JsonConvert.SerializeObject(new { requestId = requestId, receiverId = receiverId, rating = Math.Round(rating ?? 0) });
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = null;

                        response = await client.PostAsync(uri, content);
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<clsResponse>(responseContent);

                        IsBusy = false;
                        if (response.IsSuccessStatusCode)
                        {
                            if (!apiResponse.status)
                            {
                                await PopupNavigation.Instance.PushAsync(new PopupAlert("E", apiResponse.message, "OK"));
                                //await PopupNavigation.Instance.PushAsync(new PopupAlert("S", "Account reported successfully.", "OK"));
                                //Application.Current.MainPage = new FinishRegistrationTechPage(UserDetail);
                                //Application.Current.MainPage = new VerifyEmailOnRegistration(this);
                            }
                            //else
                            //{
                            //    await PopupNavigation.Instance.PushAsync(new PopupAlert("E", apiResponse.message, "OK"));
                            //    //await Application.Current.MainPage.DisplayAlert("Error", apiResponse.message, "OK");
                            //}

                            res = apiResponse.status;
                        }
                        else
                        {
                            await PopupNavigation.Instance.PushAsync(new PopupAlert("E", response.Content.ReadAsStringAsync().Result, "OK"));
                            //await Application.Current.MainPage.DisplayAlert("Error", response.Content.ReadAsStringAsync().Result, "OK");
                        }
                    }

                }
                //else
                //{
                //    PopUpData popUp = new PopUpData("crossRed.png", "", GlobalData.InactiveInternet, "OK");
                //    await Application.Current.MainPage.Navigation.PushPopupAsync(new PopUpPage(popUp));
                //    //await Application.Current.MainPage.DisplayAlert("Internet Problem", "Its seems like your internet is not active. Please check your internet.", "OK");
                //}

            }
            catch (Exception e)
            {
                await PopupNavigation.Instance.PushAsync(new PopupAlert("E", "Something wrong happened. Please try again.", "OK"));
                //await Application.Current.MainPage.DisplayAlert("Error", "Something wrong happened. Try again", "OK");
            }

            return res;
        }

        //public async Task<bool> SaveHostRequestRating(int requestId, double rating)
        //{
        //    bool res = false;
        //    try
        //    {
        //        if (await Constants.IsInternetConnected())
        //        {
        //            // Connection to internet is available
        //            using (var client = new HttpClient())
        //            {
        //                var url = Constants.CompleteURL + "/SaveHostRequestRating";

        //                var uri = new Uri(string.Format(url, string.Empty));

        //                //string sessionId = await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserSessionId);

        //                var json = JsonConvert.SerializeObject(new { requestId = requestId, rating = rating });
        //                var content = new StringContent(json, Encoding.UTF8, "application/json");

        //                HttpResponseMessage response = null;

        //                response = await client.PostAsync(uri, content);
        //                var responseContent = await response.Content.ReadAsStringAsync();
        //                var apiResponse = JsonConvert.DeserializeObject<clsResponse>(responseContent);

        //                IsBusy = false;
        //                if (response.IsSuccessStatusCode)
        //                {
        //                    if (!apiResponse.status)
        //                    {
        //                        await PopupNavigation.Instance.PushAsync(new PopupAlert("E", apiResponse.message, "OK"));
        //                        //await PopupNavigation.Instance.PushAsync(new PopupAlert("S", "Account reported successfully.", "OK"));
        //                        //Application.Current.MainPage = new FinishRegistrationTechPage(UserDetail);
        //                        //Application.Current.MainPage = new VerifyEmailOnRegistration(this);
        //                    }
        //                    //else
        //                    //{
        //                    //    await PopupNavigation.Instance.PushAsync(new PopupAlert("E", apiResponse.message, "OK"));
        //                    //    //await Application.Current.MainPage.DisplayAlert("Error", apiResponse.message, "OK");
        //                    //}

        //                    res = apiResponse.status;
        //                }
        //                else
        //                {
        //                    await PopupNavigation.Instance.PushAsync(new PopupAlert("E", response.Content.ReadAsStringAsync().Result, "OK"));
        //                    //await Application.Current.MainPage.DisplayAlert("Error", response.Content.ReadAsStringAsync().Result, "OK");
        //                }
        //            }

        //        }
        //        //else
        //        //{
        //        //    PopUpData popUp = new PopUpData("crossRed.png", "", GlobalData.InactiveInternet, "OK");
        //        //    await Application.Current.MainPage.Navigation.PushPopupAsync(new PopUpPage(popUp));
        //        //    //await Application.Current.MainPage.DisplayAlert("Internet Problem", "Its seems like your internet is not active. Please check your internet.", "OK");
        //        //}

        //    }
        //    catch (Exception e)
        //    {
        //        await PopupNavigation.Instance.PushAsync(new PopupAlert("E", "Something wrong happened. Please try again.", "OK"));
        //        //await Application.Current.MainPage.DisplayAlert("Error", "Something wrong happened. Try again", "OK");
        //    }

        //    return res;
        //}
        #endregion

        #region Get Users List
        public async Task<ObservableCollection<clsUser>> GetUsersList()
        {
            ObservableCollection<clsUser> res = new ObservableCollection<clsUser>();

            HttpStatusCode responseStatusCode = 0;

            if (await Constants.IsInternetConnected())
            {
                try
                {
                    var Httpclient = new HttpClient();

                    var url = Constants.CompleteURL + "/GetAllUsers";

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
                            var logj = (JArray)jObject["usersList"];

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

        #region Enable User
        public async Task<bool> EnableUser(int userId)
        {
            bool res = false;
            IsBusy = true;
            try
            {
                if (await Constants.IsInternetConnected())
                {
                    // Connection to internet is available
                    using (var client = new HttpClient())
                    {
                        var url = Constants.CompleteURL + "/EnableUser";

                        var uri = new Uri(string.Format(url, string.Empty));

                        //string sessionId = await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserSessionId);

                        var json = JsonConvert.SerializeObject(new { id = userId });
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = null;

                        response = await client.PostAsync(uri, content);
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<clsResponse>(responseContent);

                        IsBusy = false;
                        if (response.IsSuccessStatusCode)
                        {
                            if (apiResponse.status)
                            {
                                await PopupNavigation.Instance.PushAsync(new PopupAlert("S", "Account enabled successfully.", "OK"));
                                //Application.Current.MainPage = new FinishRegistrationTechPage(UserDetail);
                                //Application.Current.MainPage = new VerifyEmailOnRegistration(this);
                            }
                            else
                            {
                                await PopupNavigation.Instance.PushAsync(new PopupAlert("E", apiResponse.message, "OK"));
                                //await Application.Current.MainPage.DisplayAlert("Error", apiResponse.message, "OK");
                            }

                            res = apiResponse.status;
                        }
                        else
                        {
                            await PopupNavigation.Instance.PushAsync(new PopupAlert("E", response.Content.ReadAsStringAsync().Result, "OK"));
                            //await Application.Current.MainPage.DisplayAlert("Error", response.Content.ReadAsStringAsync().Result, "OK");
                        }
                    }

                }
                //else
                //{
                //    PopUpData popUp = new PopUpData("crossRed.png", "", GlobalData.InactiveInternet, "OK");
                //    await Application.Current.MainPage.Navigation.PushPopupAsync(new PopUpPage(popUp));
                //    //await Application.Current.MainPage.DisplayAlert("Internet Problem", "Its seems like your internet is not active. Please check your internet.", "OK");
                //}

            }
            catch (Exception e)
            {
                await PopupNavigation.Instance.PushAsync(new PopupAlert("E", "Something wrong happened. Please try again.", "OK"));
                //await Application.Current.MainPage.DisplayAlert("Error", "Something wrong happened. Try again", "OK");
            }
            IsBusy = false;

            return res;
        }
        #endregion

        #region Disable User
        public async Task<bool> DisableUser(int userId)
        {
            bool res = false;
            IsBusy = true;
            try
            {
                if (await Constants.IsInternetConnected())
                {
                    // Connection to internet is available
                    using (var client = new HttpClient())
                    {
                        var url = Constants.CompleteURL + "/DisableUser";

                        var uri = new Uri(string.Format(url, string.Empty));

                        //string sessionId = await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserSessionId);

                        var json = JsonConvert.SerializeObject(new { id = userId });
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = null;

                        response = await client.PostAsync(uri, content);
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<clsResponse>(responseContent);

                        IsBusy = false;
                        if (response.IsSuccessStatusCode)
                        {
                            if (apiResponse.status)
                            {
                                await PopupNavigation.Instance.PushAsync(new PopupAlert("S", "Account disabled successfully.", "OK"));
                                //Application.Current.MainPage = new FinishRegistrationTechPage(UserDetail);
                                //Application.Current.MainPage = new VerifyEmailOnRegistration(this);
                            }
                            else
                            {
                                await PopupNavigation.Instance.PushAsync(new PopupAlert("E", apiResponse.message, "OK"));
                                //await Application.Current.MainPage.DisplayAlert("Error", apiResponse.message, "OK");
                            }

                            res = apiResponse.status;
                        }
                        else
                        {
                            await PopupNavigation.Instance.PushAsync(new PopupAlert("E", response.Content.ReadAsStringAsync().Result, "OK"));
                            //await Application.Current.MainPage.DisplayAlert("Error", response.Content.ReadAsStringAsync().Result, "OK");
                        }
                    }

                }
                //else
                //{
                //    PopUpData popUp = new PopUpData("crossRed.png", "", GlobalData.InactiveInternet, "OK");
                //    await Application.Current.MainPage.Navigation.PushPopupAsync(new PopUpPage(popUp));
                //    //await Application.Current.MainPage.DisplayAlert("Internet Problem", "Its seems like your internet is not active. Please check your internet.", "OK");
                //}

            }
            catch (Exception e)
            {
                await PopupNavigation.Instance.PushAsync(new PopupAlert("E", "Something wrong happened. Please try again.", "OK"));
                //await Application.Current.MainPage.DisplayAlert("Error", "Something wrong happened. Try again", "OK");
            }
            IsBusy = false;

            return res;
        }
        #endregion

        #region Update Password ApiCall
        public async Task<bool> UpdateProfile(clsUser usr)
        {
            bool res = false;
            HttpStatusCode responseStatusCode = 0;

            if (await Constants.IsInternetConnected())
            {
                try
                {
                    //IsBusy = true;
                    var Httpclient = new HttpClient();

                    var url = Utility.Constants.CompleteURL + "/UpdateProfile";

                    var uri = new Uri(string.Format(url, string.Empty));

                    usr.UserId = Convert.ToInt32(await SecureStorageClass.GetValueAgainstKey(SecureStorageClass.keyUserId));

                    var json = JsonConvert.SerializeObject(new { user = usr });
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
                                await navigation.PushPopupAsync(new PopupAlert("E", jObject.GetValue("message").ToString(), "OK"));
                            });
                        }
                        else
                        {

                            if (usr != null)
                            {
                                await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserFullName, usr.Full_Name);
                                await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserPassword, usr.Password);
                                await SecureStorageClass.SetValueAgainstKey(SecureStorageClass.keyUserPassword, usr.Password.ToString());


                                //Device.BeginInvokeOnMainThread(() =>
                                //{
                                //    IsBusy = false;

                                //    if (usr.Is_Verified ?? false)
                                //    {
                                //        Application.Current.MainPage = new HomePage();
                                //    }
                                //    else
                                //    {
                                //        Application.Current.MainPage = new LoginPage();
                                //    }
                                //});
                            }
                        }

                        res = status;
                    }
                    else
                    {
                        //IsBusy = false;
                        await navigation.PushPopupAsync(new PopupAlert("E", "Internal Server Error", "OK"));
                        //await SendEmail(responseStatusCode, memberName, sourceFilePath, sourceLineNumber, "", "Exception From Pantex Health");
                    }
                }
                catch (Exception ex)
                {
                    //IsBusy = false;
                    res = false;
                    await navigation.PushPopupAsync(new PopupAlert("E", ex.Message, "OK"));
                    //await SendEmail(responseStatusCode, memberName, sourceFilePath, sourceLineNumber, ex.StackTrace, "Exception From Pantex Health");
                }
                finally
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        IsBusy = false;
                    });
                }
            }
            return res;

        }
        #endregion
    }
}
