using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class UserLogon
    {
        public int UserID { get; set; }
        public string GUIDSession { get; set; }
    }
}