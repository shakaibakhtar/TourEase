//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TourEaseWebApi
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblGuestRequest
    {
        public int GuestRequestId { get; set; }
        public Nullable<int> HostId { get; set; }
        public Nullable<int> GuestId { get; set; }
        public string Message { get; set; }
        public Nullable<bool> IsAccepted { get; set; }
        public Nullable<double> RatingValue { get; set; }
    }
}
