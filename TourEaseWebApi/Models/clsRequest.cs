using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TourEaseWebApi.Models
{
    public class clsRequest
    {
        public int RequestId { get; set; }
        public int? SenderId { get; set; }
        public string SenderName { get; set; }
        public int? ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string RequestMessage { get; set; }
        public bool? IsAccepted { get; set; }
    }
}