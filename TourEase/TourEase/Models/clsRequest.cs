using System;
using System.Collections.Generic;
using System.Text;
using TourEase.Utility;

namespace TourEase.Models
{
    public class clsRequest : INPC
    {
        public int RequestId { get; set; }

        int _SenderId;
        public int SenderId
        {
            get => _SenderId;
            set
            {
                _SenderId = value;
                OnPropertyChanged();
            }
        }

        string _SenderName;
        public string SenderName
        {
            get => _SenderName;
            set
            {
                _SenderName = value;
                OnPropertyChanged();
            }
        }

        clsUser _SenderObject;
        public clsUser SenderObject
        {
            get => _SenderObject;
            set
            {
                if (value != null)
                {
                    _SenderObject = value;
                }
                OnPropertyChanged();
            }
        }

        int _ReceiverId;
        public int ReceiverId
        {
            get => _ReceiverId;
            set
            {
                _ReceiverId = value;
                OnPropertyChanged();
            }
        }

        string _ReceiverName;
        public string ReceiverName
        {
            get => _ReceiverName;
            set
            {
                _ReceiverName = value;
                OnPropertyChanged();
            }
        }

        clsUser _ReceiverObject;
        public clsUser ReceiverObject
        {
            get => _ReceiverObject;
            set
            {
                if (value != null)
                {
                    _ReceiverObject = value;
                }
                OnPropertyChanged();
            }
        }

        string _RequestMessage;
        public string RequestMessage
        {
            get => _RequestMessage;
            set
            {
                _RequestMessage = value;
                OnPropertyChanged();
            }
        }

        bool _IsAccepted;
        public bool IsAccepted
        {
            get => _IsAccepted;
            set
            {
                _IsAccepted = value;
                OnPropertyChanged();
            }
        }

        double? _RatingValue;
        public double? RatingValue
        {
            get => _RatingValue;
            set
            {
                if (value != null)
                {
                    _RatingValue = value;
                }
                OnPropertyChanged();
            }
        }

        public clsRequest()
        {
            SenderObject = new clsUser();
            ReceiverObject = new clsUser();
            RatingValue = 0;
        }
    }
}
