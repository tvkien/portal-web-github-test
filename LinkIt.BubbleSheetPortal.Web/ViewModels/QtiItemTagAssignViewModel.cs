using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class QtiItemTagAssignViewModel
    {

        public QtiItemTagAssignViewModel()
        {
        }
        public int ItemTagId { get; set; }
        public string CategoryName { get; set; }
        public string TagName { get; set; }
        public int RubricQuestionCategoryID { get; set; }

    }
}
