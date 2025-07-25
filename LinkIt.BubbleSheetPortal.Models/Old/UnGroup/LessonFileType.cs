using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using  System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class LessonFileType
    {
        public int LessonFileTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ThumbnailPath { get; set; }



    }
}