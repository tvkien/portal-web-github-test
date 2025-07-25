using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.SSO
{
    public class SSOResultDTO
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public bool AllowLinkAccount { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public string DisplayAccountInfo { get; set; }
    }
}
