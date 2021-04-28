using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TourEaseWebApi.Models;

namespace TourEaseWebApi.Controllers
{
    public class ApiController : Controller
    {
        TourEaseDb db = new TourEaseDb();
        // GET: Api
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Register(clsUser user)
        {
            bool status = false;
            string message = "";
            int returnId = 0;
            int otp = GenerateOTP();

            if (user != null)
            {
                try
                {
                    tblUser tmp = new tblUser()
                    {
                        Full_Name = user.Full_Name,
                        Email = user.Email,
                        Password = user.Password,
                        Contact_Number = user.Contact_Number,
                        Location_City = user.Location_City,
                        Location_Area = user.Location_Area,
                        Fake_Reported_Count = user.Fake_Reported_Count,
                        User_Type = user.User_Type,
                        Is_Verified = false
                    };

                    db.tblUsers.Add(tmp);
                    db.SaveChanges();

                    if (SendEmail("OTP - TourEase", "Use this otp code: '" + otp + "' to verify your email in the TourEase app.", user.Email))
                    {
                        status = true;
                        message = "An otp is sent at your email. Please use it to verify your email.";
                    }
                    else
                    {
                        status = true;
                        message = "An error occurred while sending the otp.";
                        otp = 0;
                    }
                    returnId = db.tblUsers.OrderByDescending(x => x.UserId).Where(x => x.User_Type == user.User_Type).FirstOrDefault().UserId;
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                    returnId = 0;
                    otp = 0;
                }
            }
            else
            {
                status = false;
                message = "Inavlid user sent to api";
                returnId = 0;
                otp = 0;
            }
            return Json(new { status = status, message = message, returnId = returnId, otp = otp }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Login(clsUser user)
        {
            bool status = false;
            string message = "";

            if (user != null)
            {
                try
                {
                    var tmp = db.tblUsers.Where(x => x.Email == user.Email && x.Password == user.Password).Select(x => new clsUser()
                    {
                        Email = x.Email,
                        Full_Name = x.Full_Name,
                        Contact_Number = x.Contact_Number,
                        Location_City = x.Location_City,
                        Location_Area = x.Location_Area,
                        UserId = x.UserId,
                        Fake_Reported_Count = x.Fake_Reported_Count,
                        User_Type = x.User_Type,
                        //UserTypeName = db.tblUserTypes.Where(y => y.UserTypeId == x.User_Type).Select(y => y.UserTypeName).FirstOrDefault(),
                        Is_Verified = x.Is_Verified
                    }).FirstOrDefault();
                    if (tmp != null)
                    {
                        status = true;
                        message = "You have login successfully.";
                        user = tmp;
                    }
                    else
                    {
                        status = false;
                        message = "Invalid username or password.";
                        user = null;
                    }
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                    user = null;
                }
            }
            else
            {
                status = false;
                message = "Invalid user sent to api";
                user = null;
            }

            return Json(new { status = status, message = message, user = user }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpGet]
        public ActionResult GetGuestsOrHosts(int id, int userTypeId) // userType = 1 for Guests, and 2 for Hosts
        {
            bool status = false;
            string message = "";
            List<clsUser> GuestsHostsList = new List<clsUser>();

            if (id != 0 && userTypeId != 0)
            {
                try
                {
                    GuestsHostsList = db.tblUsers.Where(x => x.UserId != id && x.User_Type != userTypeId).Select(x => new clsUser()
                    {
                        UserId = x.UserId,
                        Email = x.Email,
                        Contact_Number = x.Contact_Number,
                        Location_City = x.Location_City,
                        Location_Area = x.Location_Area,
                        Full_Name = x.Full_Name,
                        Fake_Reported_Count = x.Fake_Reported_Count,
                        User_Type = x.User_Type,
                        Is_Verified = x.Is_Verified,
                        RatingValue = userTypeId == 1 ? db.tblGuestRequests.Where(y => y.RatingValue != 0).Select(y => y.RatingValue).Average() : db.tblHostRequests.Where(y => y.RatingValue != 0).Select(y => y.RatingValue).Average()
                    }).ToList<clsUser>();

                    status = true;
                    message = "";
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                    GuestsHostsList = null;
                }
            }
            else
            {
                status = false;
                message = "Invalid user or user type sent to api";
                GuestsHostsList = null;
            }
            return Json(new { status = status, message = message, guestsList = GuestsHostsList }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpGet]
        public ActionResult GetGuestsHostsReceivedRequests(int id, int userTypeId) // userType = 1 for Guests, and 2 for Hosts
        {
            bool status = false;
            string message = "";
            List<clsRequest> GuestsHostsRequestsList = new List<clsRequest>();

            if (id != 0 && userTypeId != 0)
            {
                try
                {
                    if (userTypeId == 1)
                    {
                        GuestsHostsRequestsList = db.tblGuestRequests.Where(x => x.GuestId == id && (x.RatingValue ?? 0) == 0).Select(x => new clsRequest()
                        {
                            RequestId = x.GuestRequestId,
                            ReceiverId = id,
                            SenderId = x.HostId ?? 0,
                            SenderObject = db.tblUsers.Where(y => y.UserId == x.HostId).Select(y => new clsUser()
                            {
                                UserId = y.UserId,
                                Full_Name = y.Full_Name,
                                Email = y.Email,
                                Contact_Number = y.Contact_Number,
                                Location_Area = y.Location_Area,
                                Location_City = y.Location_City,
                                Is_Verified = y.Is_Verified ?? false,
                                Fake_Reported_Count = y.Fake_Reported_Count ?? 0,
                                User_Type = y.User_Type
                            }).FirstOrDefault(),
                            RequestMessage = x.Message,
                            IsAccepted = x.IsAccepted
                        }).ToList<clsRequest>();
                    }
                    else
                    {
                        GuestsHostsRequestsList = db.tblHostRequests.Where(x => x.HostId == id && (x.RatingValue ?? 0) == 0).Select(x => new clsRequest()
                        {
                            RequestId = x.RequestId,
                            ReceiverId = id,
                            SenderId = x.GuestId ?? 0,
                            SenderObject = db.tblUsers.Where(y => y.UserId == x.GuestId).Select(y => new clsUser()
                            {
                                UserId = y.UserId,
                                Full_Name = y.Full_Name,
                                Email = y.Email,
                                Contact_Number = y.Contact_Number,
                                Location_Area = y.Location_Area,
                                Location_City = y.Location_City,
                                Is_Verified = y.Is_Verified ?? false,
                                Fake_Reported_Count = y.Fake_Reported_Count ?? 0,
                                User_Type = y.User_Type
                            }).FirstOrDefault(),
                            RequestMessage = x.Message,
                            IsAccepted = x.IsAccepted
                        }).ToList<clsRequest>();
                    }

                    status = true;
                    message = "";
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                    GuestsHostsRequestsList = null;
                }
            }
            else
            {
                status = false;
                message = "Invalid user or user type sent to api";
                GuestsHostsRequestsList = null;
            }
            return Json(new { status = status, message = message, requestsList = GuestsHostsRequestsList }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpGet]
        public ActionResult GetGuestsHostsSentRequests(int id, int userTypeId) // userType = 1 for Guests, and 2 for Hosts
        {
            bool status = false;
            string message = "";
            List<clsRequest> GuestsHostsRequestsList = new List<clsRequest>();

            if (id != 0 && userTypeId != 0)
            {
                try
                {
                    if (userTypeId == 1)
                    {
                        GuestsHostsRequestsList = db.tblHostRequests.Where(x => x.GuestId == id && (x.RatingValue ?? 0) == 0).Select(x => new clsRequest()
                        {
                            RequestId = x.RequestId,
                            ReceiverId = x.HostId,
                            ReceiverName = db.tblUsers.Where(y => y.UserId == x.HostId).Select(y => y.Full_Name).FirstOrDefault(),
                            ReceiverObject = db.tblUsers.Where(y => y.UserId == x.HostId).Select(y => new clsUser()
                            {
                                UserId = y.UserId,
                                Full_Name = y.Full_Name,
                                Email = y.Email,
                                Contact_Number = y.Contact_Number,
                                Location_Area = y.Location_Area,
                                Location_City = y.Location_City,
                                Is_Verified = y.Is_Verified ?? false,
                                Fake_Reported_Count = y.Fake_Reported_Count ?? 0,
                                User_Type = y.User_Type
                            }).FirstOrDefault(),
                            SenderId = id,
                            RequestMessage = x.Message,
                            IsAccepted = x.IsAccepted
                        }).ToList<clsRequest>();
                    }
                    else
                    {
                        GuestsHostsRequestsList = db.tblGuestRequests.Where(x => x.HostId == id && (x.RatingValue ?? 0) == 0).Select(x => new clsRequest()
                        {
                            RequestId = x.GuestRequestId,
                            ReceiverId = x.GuestId,
                            ReceiverObject = db.tblUsers.Where(y => y.UserId == x.GuestId).Select(y => new clsUser()
                            {
                                UserId = y.UserId,
                                Full_Name = y.Full_Name,
                                Email = y.Email,
                                Contact_Number = y.Contact_Number,
                                Location_Area = y.Location_Area,
                                Location_City = y.Location_City,
                                Is_Verified = y.Is_Verified ?? false,
                                Fake_Reported_Count = y.Fake_Reported_Count ?? 0,
                                User_Type = y.User_Type
                            }).FirstOrDefault(),
                            SenderId = id,
                            RequestMessage = x.Message,
                            IsAccepted = x.IsAccepted
                        }).ToList<clsRequest>();
                    }

                    status = true;
                    message = "";
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                    GuestsHostsRequestsList = null;
                }
            }
            else
            {
                status = false;
                message = "Invalid user or user type sent to api";
                GuestsHostsRequestsList = null;
            }
            return Json(new { status = status, message = message, requestsList = GuestsHostsRequestsList }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult VerifyUser(int id)
        {
            bool status = false;
            string message = "";

            if (id != 0)
            {
                try
                {
                    var tmp = db.tblUsers.Where(x => x.UserId == id).FirstOrDefault();
                    if (tmp != null)
                    {
                        tmp.Is_Verified = true;
                        db.SaveChanges();

                        status = true;
                        message = "Your account has been verified successfully.";
                    }
                    else
                    {
                        status = false;
                        message = "System ran into a problem, please try again later.";
                    }
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                }
            }
            else
            {
                status = false;
                message = "Invalid user id sent to api";
            }

            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ReportUser(int id)
        {
            bool status = false;
            string message = "";

            if (id != 0)
            {
                try
                {
                    var tmp = db.tblUsers.Where(x => x.UserId == id).FirstOrDefault();
                    if (tmp != null)
                    {
                        tmp.Fake_Reported_Count += 1;
                        db.SaveChanges();

                        status = true;
                        message = tmp.Full_Name + " has been reported as fake.";
                    }
                    else
                    {
                        status = false;
                        message = "System ran into a problem, please try again later.";
                    }
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                }
            }
            else
            {
                status = false;
                message = "Invalid user id sent to api";
            }

            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult RequestGuest(int userId, int guestId, string requestMessage)
        {
            bool status = false;
            string message = "";

            if (userId != 0 && guestId != 0)
            {
                try
                {
                    if (db.tblGuestRequests.Where(x => x.HostId == userId && x.GuestId == guestId && (x.IsAccepted ?? false) == false).Count() == 0)
                    {
                        tblGuestRequest tmp = new tblGuestRequest()
                        {
                            HostId = userId,
                            GuestId = guestId,
                            Message = requestMessage,
                            IsAccepted = false
                        };

                        db.tblGuestRequests.Add(tmp);
                        db.SaveChanges();

                        status = true;
                        message = "Your request is sent successfully.";
                    }
                    else
                    {
                        status = false;
                        message = "Your have already sent request to this user. Please see sent requests.";
                    }
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                }
            }
            else
            {
                status = false;
                message = "Invalid ids sent to api";
            }

            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult RequestHost(int userId, int hostId, string requestMessage)
        {
            bool status = false;
            string message = "";

            if (userId != 0 && hostId != 0)
            {
                try
                {
                    if (db.tblHostRequests.Where(x => x.GuestId == userId && x.HostId == hostId && (x.IsAccepted ?? false) == false).Count() == 0)
                    {
                        tblHostRequest tmp = new tblHostRequest()
                        {
                            GuestId = userId,
                            HostId = hostId,
                            Message = requestMessage,
                            IsAccepted = false
                        };

                        db.tblHostRequests.Add(tmp);
                        db.SaveChanges();

                        status = true;
                        message = "Your request is sent successfully.";
                    }
                    else
                    {
                        status = false;
                        message = "Your have already sent request to this user. Please see sent requests.";
                    }
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                }
            }
            else
            {
                status = false;
                message = "Invalid ids sent to api";
            }

            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult UpdatePassword(clsUser User)
        {
            bool status = false;
            string message = "";
            bool IsVerified = false;

            if (User != null)
            {
                try
                {
                    var tmp = db.tblUsers.Where(x => x.Email == User.Email).FirstOrDefault();
                    if (tmp != null)
                    {
                        tmp.Password = User.Password;
                        db.SaveChanges();

                        User.Contact_Number = tmp.Contact_Number;
                        User.Full_Name = tmp.Full_Name;
                        User.Location_City = tmp.Location_City;
                        User.Location_Area = tmp.Location_Area;
                        User.UserId = tmp.UserId;
                        User.User_Type = tmp.User_Type;
                        User.UserTypeName = db.tblUserTypes.Where(x => x.UserTypeId == tmp.User_Type).Select(x => x.UserTypeName).FirstOrDefault();

                        status = true;
                        message = "Your password has been changed successfully.";
                        IsVerified = tmp.Is_Verified ?? false;
                    }
                    else
                    {
                        status = false;
                        message = "Email not found.";
                        IsVerified = false;
                    }
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                    IsVerified = false;
                }
            }
            else
            {
                status = false;
                message = "Invalid user id sent to api";
                IsVerified = false;
            }

            return Json(new { status = status, message = message, IsVerified = IsVerified, UserObject = User }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpGet]
        public ActionResult SendOTPToEmail(string email)
        {
            bool status = false;
            string message = "";
            int code = GenerateOTP();

            if (!(string.IsNullOrEmpty(email) && string.IsNullOrWhiteSpace(email)))
            {
                try
                {
                    if (SendEmail("OTP - TourEase", "Use this otp code: '" + code + "' to verify your email in the TourEase app.", email))
                    {
                        status = true;
                        message = "An otp is sent at your email. Please use it to verify your email.";
                    }
                    else
                    {
                        status = true;
                        message = "An error occurred while sending the otp.";
                        code = 0;
                    }
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                    code = 0;
                }
            }
            else
            {
                status = false;
                message = "Inavlid user sent to api";
                code = 0;
            }
            return Json(new { status = status, message = message, code = code }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpGet]
        public ActionResult SendCodeAgain(int id)
        {
            bool status = false;
            string message = "";
            int code = GenerateOTP();

            if (id != 0)
            {
                try
                {
                    string email = db.tblUsers.Where(x => x.UserId == id).Select(x => x.Email).FirstOrDefault();
                    if (!string.IsNullOrEmpty(email))
                    {
                        if (SendEmail("OTP - TourEase", "<p>Use this otp code: '" + code + "' to verify your email in the TourEase app.</p>", email))
                        {
                            status = true;
                            message = "An otp is sent at your email. Please use it to verify your email.";
                        }
                        else
                        {
                            status = false;
                            message = "An error occurred while sending the otp.";
                            code = 0;
                        }
                    }
                    else
                    {
                        status = false;
                        message = "User not found";
                        code = 0;
                    }
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                    code = 0;
                }
            }
            else
            {
                status = false;
                message = "Inavlid user sent to api";
                code = 0;
            }
            return Json(new { status = status, message = message, code = code }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult AcceptGuestRequest(int requestId)
        {
            bool status = false;
            string message = "";

            if (requestId != 0)
            {
                try
                {
                    var tmp = db.tblGuestRequests.Where(x => x.GuestRequestId == requestId).FirstOrDefault();
                    if (tmp != null)
                    {
                        tmp.IsAccepted = true;
                        db.SaveChanges();

                        status = true;
                        message = "Request has been accepted successfully.";
                    }
                    else
                    {
                        status = false;
                        message = "System ran into a problem, please try again later.";
                    }
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                }
            }
            else
            {
                status = false;
                message = "Invalid user id sent to api";
            }

            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult AcceptHostRequest(int requestId)
        {
            bool status = false;
            string message = "";

            if (requestId != 0)
            {
                try
                {
                    var tmp = db.tblHostRequests.Where(x => x.RequestId == requestId).FirstOrDefault();
                    if (tmp != null)
                    {
                        tmp.IsAccepted = true;
                        db.SaveChanges();

                        status = true;
                        message = "Request has been accepted successfully.";
                    }
                    else
                    {
                        status = false;
                        message = "System ran into a problem, please try again later.";
                    }
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                }
            }
            else
            {
                status = false;
                message = "Invalid user id sent to api";
            }

            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult SaveGuestHostRequestRating(int requestId = 0, int receiverId = 0, double rating = 0)
        {
            bool status = false;
            string message = "";

            if (requestId != 0 && receiverId != 0 && rating != 0)
            {
                try
                {
                    var tmp = db.tblGuestRequests.Where(x => x.GuestRequestId == requestId && x.HostId == receiverId).FirstOrDefault();
                    if (tmp != null)
                    {
                        if (tmp.IsAccepted ?? false)
                        {
                            tmp.RatingValue = rating;
                            db.SaveChanges();

                            status = true;
                            message = "Rating has been saved successfully.";
                        }
                    }
                    else
                    {
                        var tmp1 = db.tblHostRequests.Where(x => x.RequestId == requestId && x.GuestId == receiverId).FirstOrDefault();
                        if (tmp1 != null)
                        {
                            if (tmp1.IsAccepted ?? false)
                            {
                                tmp1.RatingValue = rating;
                                db.SaveChanges();

                                status = true;
                                message = "Rating has been saved successfully.";
                            }
                        }
                        else
                        {
                            status = false;
                            message = "System ran into a problem, please try again later.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                }
            }
            else
            {
                status = false;
                message = "Invalid user id sent to api";
            }

            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        //[ValidateInput(false)]
        //[HttpPost]
        //public ActionResult SaveHostRequestRating(int requestId = 0, decimal rating = 0)
        //{
        //    bool status = false;
        //    string message = "";

        //    if (requestId != 0)
        //    {
        //        try
        //        {
        //            var tmp = db.tblGuestRequests.Where(x => x.GuestRequestId == requestId).FirstOrDefault();
        //            if (tmp != null)
        //            {
        //                tmp.RatingValue = rating;
        //                db.SaveChanges();

        //                status = true;
        //                message = "Rating has been saved successfully.";
        //            }
        //            else
        //            {
        //                status = false;
        //                message = "System ran into a problem, please try again later.";
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            status = false;
        //            message = ex.Message;
        //        }
        //    }
        //    else
        //    {
        //        status = false;
        //        message = "Invalid user id sent to api";
        //    }

        //    return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        //}

        #region SendEmail
        public int GenerateOTP()
        {
            return new Random().Next(1000, 9999);
        }
        public bool SendEmail(string subject, string message, string receiver)
        {
            bool res = false;
            try
            {
                var senderEmail = new MailAddress("smart.e.tutor@gmail.com", subject);
                var receiverEmail = new MailAddress(receiver, "Receiver");
                var password = "ClashOfClans786-";
                var sub = subject;
                var body = message;
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password)
                };
                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(mess);
                    res = true;
                }
            }
            catch (Exception ex)
            {
                res = false;
            }

            return res;
        }
        #endregion
    }
}