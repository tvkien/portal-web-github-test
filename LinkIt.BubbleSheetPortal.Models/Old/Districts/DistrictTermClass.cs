using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class DistrictTermClass
    {
        public int DistrictId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int ClassId { get; set; }
        public int DistrictTermId { get; set; }
        public int SchoolId { get; set; }
        public int UserId { get; set; }
        public int TermId { get; set; }
        public int TeacherId { get; set; }
    }
}