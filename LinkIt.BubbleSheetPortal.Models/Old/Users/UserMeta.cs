using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class UserMeta
    {
        public int UserMetaId { get; set; }
        public int UserId { get; set; }
        public string MetaValue { get; set; }
        public UserMetaValue UserMetaValue
        {
            get
            {
                return new JavaScriptSerializer().Deserialize<UserMetaValue>(MetaValue);
            }
            set
            {
                MetaValue = new JavaScriptSerializer().Serialize(value);
            }
        }
    }
}