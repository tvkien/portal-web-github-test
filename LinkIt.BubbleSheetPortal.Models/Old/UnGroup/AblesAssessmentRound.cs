using System;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AblesAssessmentRound
    {
        public int AssessmentRoundId { get; set; }
        public string Name { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int DistrictId { get; set; }      
        public string Round { get; set; }
        public int RoundIndex { get; set; }
    }
}