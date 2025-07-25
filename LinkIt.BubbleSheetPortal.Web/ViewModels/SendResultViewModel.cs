using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Web.Models.ParentConnect;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class SendResultViewModel
    {
        public  SendResultViewModel()
        {
            EmailErrorList = new List<SendResultErrorStudent>();
            SmsErrorList = new List<SendResultErrorStudent>();
            CountMessageReceiver = 0;
            CountEmailSent = 0;
            CountSmsSent = 0;
            CountSms = 0;
            ComposeModel = new ComposeModel();
        }

        public int CountMessageReceiver { get; set; }
        public int CountEmailSent { get; set; }
        public int CountSmsSent { get; set; }
        public int CountEmail { get; set; }
        public int CountSms { get; set; }
        public List<SendResultErrorStudent> EmailErrorList;
        public List<SendResultErrorStudent> SmsErrorList;
        public int Count { get; set; }
        public ComposeModel ComposeModel { get; set; }

        public string FromNumberError { get; set; }

        public class SendResultErrorStudent
        {
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string Gender { get; set; }
            public string LocalId { get; set; }
            public string ParentEmail { get; set; }
            public string ParentPhone { get; set; }
            public string Error { get; set; } 
        }
    }
}