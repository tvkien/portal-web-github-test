using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class VirtualSection
    {
        public int VirtualSectionId { get; set; }
        public int VirtualTestId { get; set; }
        public int Order { get; set; }

        private string title = string.Empty;
        private string instruction = string.Empty;

        public int? ConversionSetId { get; set; }

        public string Title
        {
            get { return title; }
            set { title = value.ConvertNullToEmptyString(); }
        }
        public string Instruction
        {
            get { return instruction; }
            set { instruction = value.ConvertNullToEmptyString(); }
        }
        //Falsh use default value NULL for MediaReference, AudioRef, VideoRef, MediaSource
        public string MediaReference { get; set; }
        public string AudioRef { get; set; }
        public string VideoRef { get; set; }
        public string MediaSource { get; set; }
        public int? SubjectId { get; set; }
        public int Mode { get; set; }
    }
}