using System;
using System.Collections.Generic;
using System.Text;
using TourEase.Utility;

namespace TourEase.Models
{
    public class clsUserType : INPC
    {
        int _UserTypeId;
        public int UserTypeId
        {
            get => _UserTypeId;
            set
            {
                _UserTypeId = value;
                OnPropertyChanged();
            }
        }

        string _UserTypeName;
        public string UserTypeName
        {
            get => _UserTypeName;
            set
            {
                if (value != null)
                {
                    _UserTypeName = value;
                }
                OnPropertyChanged();
            }
        }
    }
}
