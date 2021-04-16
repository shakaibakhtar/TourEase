using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TourEase.Models;
using TourEase.Popup_Views;
using TourEase.Utility;
using TourEase.Views;
using Xamarin.Forms;

namespace TourEase.ViewModels
{
    public class PasswordResetViewModel : INPC
    {
        public INavigation navigation { get; set; }

        public PasswordResetViewModel(INavigation navigation)
        {
            this.navigation = navigation;
        }

        #region Forgot Password Page
        string _Email;
        public string Email
        {
            get { return _Email; }
            set { if (value != null) _Email = value; OnPropertyChanged(); }
        }

        private bool ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(Email))
                return false;

            try
            {
                // Normalize the domain
                Email = Regex.Replace(Email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(Email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public Command GotoVerificationCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (ValidateEmail())
                    {
                        ApiCalls api = new ApiCalls(navigation);
                        bool res = await api.SendOTPToEmail(Email, this);

                        if (res)
                        {
                            await navigation.PushAsync(new CodeVerificationPage(this));
                        }
                    }
                    else
                    {
                        await navigation.PushPopupAsync(new PopupAlert("W", "Email is not valid", "OK"));
                    }
                });
            }
        }
        #endregion

        #region Verification Page
        public string CodeFromApi { get; set; }

        string _LocalVerificationCode;
        public string LocalVerificationCode
        {
            get { return _LocalVerificationCode; }
            set { if (value != null) _LocalVerificationCode = value; OnPropertyChanged(); }
        }

        public bool ResendCodeCommandCanRun { get; set; } = true;

        public Command ResendCodeCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (ResendCodeCommandCanRun)
                    {
                        ApiCalls api = new ApiCalls(navigation);
                        await api.SendOTPToEmail(Email, this);
                    }
                    else
                    {
                        PopUpData popUp = new PopUpData("I", "A code is already sent to your email address. Please wait for sometime before resending the code.", "OK");
                        await Application.Current.MainPage.Navigation.PushPopupAsync(new PopupAlert(popUp));
                    }
                });
            }
        }

        private async System.Threading.Tasks.Task<bool> ValidateCode()
        {
            bool isValid = false;

            // SecurityCode
            if ((!string.IsNullOrEmpty(LocalVerificationCode)) && (!string.IsNullOrWhiteSpace(LocalVerificationCode)))
            {
                isValid = true;
            }
            else
            {
                await navigation.PushPopupAsync(new PopupAlert("W", "Security Code cannot be empty.", "OK"));
            }

            return isValid;
        }

        public Command VerifyCodeCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (await ValidateCode())
                    {
                        if (LocalVerificationCode.Equals(CodeFromApi))
                        {
                            await navigation.PushAsync(new ResetPasswordPage(this));
                        }
                        else
                        {
                            await navigation.PushPopupAsync(new PopupAlert("E", "Invalid code.", "OK"));
                            LocalVerificationCode = "";
                        }
                    }
                    //await navigation.PushAsync(new ResetPasswordPage(this));
                });
            }
        }
        #endregion

        #region Reset Password Page
        //string _OldPassword;
        //public string OldPassword
        //{
        //    get { return _OldPassword; }
        //    set { if (value != null) _OldPassword = value; OnPropertyChanged(); }
        //}

        string _NewPassword;
        public string NewPassword
        {
            get { return _NewPassword; }
            set { if (value != null) _NewPassword = value; OnPropertyChanged(); }
        }

        string _ConfirmNewPassword;
        public string ConfirmNewPassword
        {
            get { return _ConfirmNewPassword; }
            set { if (value != null) _ConfirmNewPassword = value; OnPropertyChanged(); }
        }

        private async Task<bool> IsPasswordStrong(string password)
        {
            string ErrorMessage = string.Empty;
            var input = password ?? "";

            if (string.IsNullOrWhiteSpace(input))
            {
                ErrorMessage = "Password should not be empty";
                await PopupNavigation.Instance.PushAsync(new PopupAlert("W", ErrorMessage, "OK"));
                //await Application.Current.MainPage.DisplayAlert("Missing Information", ErrorMessage, "OK");
                return false;
            }

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{8,15}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasLowerChar.IsMatch(input))
            {
                ErrorMessage = "Password should contain at least one lower case letter.";
                await PopupNavigation.Instance.PushAsync(new PopupAlert("W", ErrorMessage, "OK"));
                //await Application.Current.MainPage.DisplayAlert("Missing Information", ErrorMessage, "OK");
                return false;
            }
            else if (!hasUpperChar.IsMatch(input))
            {
                ErrorMessage = "Password should contain at least one upper case letter.";
                await PopupNavigation.Instance.PushAsync(new PopupAlert("W", ErrorMessage, "OK"));
                //await Application.Current.MainPage.DisplayAlert("Missing Information", ErrorMessage, "OK");
                return false;
            }
            else if (!hasMiniMaxChars.IsMatch(input))
            {
                ErrorMessage = "Password should not be lesser than 8 or greater than 15 characters.";
                await PopupNavigation.Instance.PushAsync(new PopupAlert("W", ErrorMessage, "OK"));
                //await Application.Current.MainPage.DisplayAlert("Missing Information", ErrorMessage, "OK");
                return false;
            }
            else if (!hasNumber.IsMatch(input))
            {
                ErrorMessage = "Password should contain at least one numeric value.";
                await PopupNavigation.Instance.PushAsync(new PopupAlert("W", ErrorMessage, "OK"));
                //await Application.Current.MainPage.DisplayAlert("Missing Information", ErrorMessage, "OK");
                return false;
            }

            else if (!hasSymbols.IsMatch(input))
            {
                ErrorMessage = "Password should contain at least one special case character.";
                await PopupNavigation.Instance.PushAsync(new PopupAlert("W", ErrorMessage, "OK"));
                //await Application.Current.MainPage.DisplayAlert("Missing Information", ErrorMessage, "OK");
                return false;
            }
            else
            {
                return true;
            }
        }

        private async System.Threading.Tasks.Task<bool> ValidateStudentForUpdatePassword()
        {
            bool isValid = false;

            // email
            if ((!string.IsNullOrEmpty(NewPassword)) && (!string.IsNullOrWhiteSpace(NewPassword)))
            {
                if (await IsPasswordStrong(NewPassword))
                {
                    if ((!string.IsNullOrEmpty(ConfirmNewPassword)) && (!string.IsNullOrWhiteSpace(ConfirmNewPassword)))
                    {
                        if (NewPassword.Equals(ConfirmNewPassword))
                        {
                            isValid = true;
                        }
                        else
                        {
                            await navigation.PushPopupAsync(new PopupAlert("W", "New & Confirm Password must be same.", "OK"));
                        }
                    }
                    else
                    {
                        await navigation.PushPopupAsync(new PopupAlert("W", "Confirm Password cannot be empty.", "OK"));
                    }
                }
                // else part for this if condition is handled in function IsPasswordStrong()
            }
            else
            {
                await navigation.PushPopupAsync(new PopupAlert("W", "Password cannot be empty.", "OK"));
            }

            return isValid;
        }

        public Command SaveNewPasswordCommand
        {
            get
            {
                return new Command(async () =>
                {
                    clsUser User = new clsUser();
                    User.Email = Email;
                    User.Password = NewPassword;

                    if (await ValidateStudentForUpdatePassword())
                    {
                        ApiCalls api = new ApiCalls(navigation);
                        bool res = await api.UpdatePassword(User);

                        if (res)
                        {
                            await navigation.PushPopupAsync(new PopupAlert("S", "Password updated successfully.", "OK"));

                            //await navigation.PushAsync(new FinishRegistrationPage(User));
                            //Application.Current.MainPage = new AppShell();
                        }
                    }
                    //Application.Current.MainPage = new AppShell();
                });
            }
        }
        #endregion
    }
}
