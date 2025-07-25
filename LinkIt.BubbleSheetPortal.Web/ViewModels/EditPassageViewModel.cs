using System.Collections.Generic;
using System.Web.Mvc;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Helpers.Media;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class EditPassageViewModel : ValidatableEntity<EditPassageViewModel>
    {
        public EditPassageViewModel()
        {
            FileNotFound = false;
            GradeIdFilter = 0;
            TextTypeIdFilter = 0;
            TextSubTypeIdFilter = 0;
            FleschKincaidIdFilter = 0;
            QtiRefObjectId = 0;
            FromPassageEditor = false;
            FromItemSetEditor = false;
            FromItemEditor = false;
            FromVirtualTestEditor = false;
            FromTestEditor = false;
        }
        public int QtiRefObjectId { get; set; }
        public string Name { get; set; }
        public string OldMasterCode { get; set; }
        public int TypeId { get; set; }
        public int UserId { get; set; }
        public int QtiRefObjectFileRef { get; set; }
        public string Subject { get; set; }
        public int? GradeId { get; set; }
        public int? TextTypeId { get; set; }
        public int? TextSubTypeId { get; set; }
        public int? FleschKincaidId { get; set; }
        public string XmlContentPassage { get; set; }
        //Remember filter
        public string NameFilter { get ; set; }
        public int? GradeIdFilter { get; set; }
        public string SubjectFilter { get; set; }
        public int? TextTypeIdFilter { get; set; }
        public int? TextSubTypeIdFilter { get; set; }
        public int? FleschKincaidIdFilter { get; set; }
        public string SearchBoxFilter { get; set; }
        public bool FileNotFound { get; set; }
        public string FileName { get; set; }
        public string SubjectText { get; set; }
        public bool FromPassageEditor { get; set; }
        public bool FromItemSetEditor { get; set; }
        public bool FromItemEditor { get; set; }
        public bool FromVirtualTestEditor { get; set; }
        public bool FromTestEditor { get; set; }
        public int QtiItemGroupId { get; set; }
        public int? PassageNumber { get; set; }
        public MediaModel MediaModel { get; set; }
        public string QtiItemIdsAssignPassage { get; set; }
        public int? VirtualTestId { get; set; }
    }
}
