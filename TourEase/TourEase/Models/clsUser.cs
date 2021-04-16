using System;
using System.Collections.Generic;
using System.Text;
using TourEase.Utility;

namespace TourEase.Models
{
    public class clsUser : INPC
    {
        int _UserId;
        public int UserId
        {
            get => _UserId;
            set
            {
                _UserId = value;
                OnPropertyChanged();
            }
        }

        string _Full_Name;
        public string Full_Name
        {
            get => _Full_Name;
            set
            {
                if (value != null)
                    _Full_Name = value;
                OnPropertyChanged();
            }
        }

        string _Email;
        public string Email
        {
            get => _Email;
            set
            {
                if (value != null)
                    _Email = value;
                OnPropertyChanged();
            }
        }

        string _Password;
        public string Password
        {
            get => _Password;
            set
            {
                if (value != null)
                    _Password = value;
                OnPropertyChanged();
            }
        }

        string _Contact_Number;
        public string Contact_Number
        {
            get => _Contact_Number;
            set
            {
                if (value != null)
                    _Contact_Number = value;
                OnPropertyChanged();
            }
        }

        string _Location_City;
        public string Location_City
        {
            get => _Location_City;
            set
            {
                if (value != null)
                    _Location_City = value;
                OnPropertyChanged();
            }
        }

        string _Location_Area;
        public string Location_Area
        {
            get => _Location_Area;
            set
            {
                if (value != null)
                    _Location_Area = value;
                OnPropertyChanged();
            }
        }

        int? _User_Type;
        public int? User_Type
        {
            get => _User_Type;
            set
            {
                if (value != null)
                    _User_Type = value;
                OnPropertyChanged();
            }
        }

        int? _Fake_Reported_Count;
        public int? Fake_Reported_Count
        {
            get => _Fake_Reported_Count;
            set
            {
                if (value != null)
                    _Fake_Reported_Count = value;
                OnPropertyChanged();
            }
        }

        public clsUser()
        {
            Fake_Reported_Count = 0;
        }
    }
}
