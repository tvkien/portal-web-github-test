using System.Collections.Generic;
using System.Web.Mvc;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ManageClassEnhancement
{
    public class ManageClassEnhancement
    {
        public Class Class { get; set; }
        public List<ListItem> Programs { get; set; }
        public List<ListItem> Grades { get; set; }
        public bool IsShowAddNewStudent { get; set; }
    }
}