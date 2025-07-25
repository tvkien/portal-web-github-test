using System;

namespace LinkIt.BubbleSheetPortal.Models.DataLocker
{
    public class DTLAutoSaveResultData
    {
        public int DTLAutoSaveResultDataId { get; set; }
        public int VirtualTestId { get; set; }
        public int ClassId { get; set; }
        public int UserId { get; set; }
        public string StudentTestResultScoresJson { get; set; }
        public string StudentTestResultSubScoresJson { get; set; }
        public string ActualTestResultScoresJson { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ResultDate { get; set; }
    }
}
