using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class LinkitDefaultTagAssignedViewModel
    {
        public LinkitDefaultTagAssignedViewModel()
        {
        }
        public int TagId { get; set; }
        public string LinkitDefaultTagCategory { get; set; }// one of Topic,Skill or Other
        public string Tag { get; set; }

    }
}