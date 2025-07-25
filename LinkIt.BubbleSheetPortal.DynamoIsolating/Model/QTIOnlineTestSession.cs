using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Model
{
    public class QTIOnlineTestSession
    {
        public int QTIOnlineTestSessionID { get; set; }
        public int VirtualTestID { get; set; }
        public int StudentID { get; set; }
        public DateTime StartDate { get; set; }
        public int StatusID { get; set; }
        public string AssignmentGUID { get; set; }
        public string SessionQuestionOrder { get; set; }
        public bool? TimeOver { get; set; }
        public string SectionFlag { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }

    public class SectionFlagData
    {
        public int VirtualSectionID { get; set; }
        public bool Status { get; set; }

        public static List<SectionFlagData> ParseFromJson(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
            {
                return new List<SectionFlagData>();
            }
            return JsonConvert.DeserializeObject<List<SectionFlagData>>(jsonData);
        }
    }
}
