using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class NotificationMessage
    {
        public int NotificationMessageId { get; set; }
        public string Status { get; set; }
        public DateTime PublishedTime { get; set; }
        public string HtmlContent { get; set; }
        public string AccessedDistrict { get; set; }
        public List<int> AccessedDistrictIds
        {
            get
            {
                if (string.IsNullOrEmpty(AccessedDistrict))
                {
                    return new List<int>();
                }else
                {
                    try
                    {
                        return AccessedDistrict.Split(';').Select(x => Convert.ToInt32(x)).ToList();
                    }
                    catch (Exception ex)
                    {
                        return new List<int>();
                    }
                }                
            }
        }
        public int? ReceivingUserID { get; set; }

        public string NotificationType { get; set; }
    }
}
