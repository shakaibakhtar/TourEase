using System;
using System.Collections.Generic;
using System.Text;
using TourEase.Utility;

namespace TourEase.Models
{
    public class clsRequest:INPC
    {
        public int RequestId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string RequestMessage { get; set; }
    }
}
