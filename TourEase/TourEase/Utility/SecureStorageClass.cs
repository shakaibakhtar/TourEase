using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace TourEase.Utility
{
    public class SecureStorageClass
    {
        #region SecureStorage Keys
        public static string keyUserId { get; set; } = "UserId";
        public static string keyUserFullName { get; set; } = "UserFullName";
        public static string keyUserEmail { get; set; } = "UserEmail";
        public static string keyUserPassword { get; set; } = "UserPassword";
        public static string keyUserType { get; set; } = "UserContact";
        #endregion

        public static async Task<bool> SetValueAgainstKey(string key, string val)
        {
            bool res = false;

            try
            {
                await SecureStorage.SetAsync(key, val);

                res = true;
            }
            catch (Exception ex)
            {
                res = false;
                //App.Current.Properties.Remove(keyStudentId);
                //App.Current.Properties.Remove(keyStudentIdNumber);
                //App.Current.Properties.Remove(keyStudentPassword);
                //await App.Current.SavePropertiesAsync();
            }

            return res;
        }

        public static bool RemoveValueAgainstKey(string key)
        {
            bool res = false;

            try
            {
                SecureStorage.Remove(key);

                res = true;
            }
            catch (Exception ex)
            {
                res = false;
                //App.Current.Properties.Remove(keyStudentId);
                //App.Current.Properties.Remove(keyStudentIdNumber);
                //App.Current.Properties.Remove(keyStudentPassword);
                //await App.Current.SavePropertiesAsync();
            }

            return res;
        }

        public static async Task<string> GetValueAgainstKey(string key)
        {
            string val = "";

            try
            {
                val = await SecureStorage.GetAsync(key);
            }
            catch (Exception ex)
            {
                val = "";
                //App.Current.Properties.Remove(keyStudentId);
                //App.Current.Properties.Remove(keyStudentIdNumber);
                //App.Current.Properties.Remove(keyStudentPassword);
                //await App.Current.SavePropertiesAsync();
            }

            return val;
        }

        public static bool ClearAll()
        {
            bool res = false;

            try
            {
                SecureStorage.RemoveAll();

                res = true;
            }
            catch (Exception)
            {
                res = false;
            }

            return res;
        }
    }
}
