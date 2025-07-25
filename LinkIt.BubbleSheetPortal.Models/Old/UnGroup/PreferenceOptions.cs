using System;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class PreferenceOptions
    {
        public bool? TimeLimit { get; set; }
        public int? Duration { get; set; }
        public DateTime? Deadline { get; set; }
        public string MultipleChoiceClickMethod { get; set; }
        public bool? BranchingTest { get; set; }
        public bool? TestSchedule { get; set; }
        public DateTime? TestScheduleToDate { get; set; }
        public DateTime? TestScheduleFromDate { get; set; }
        public double? TestScheduleTimezoneOffset { get; set; }
        public string TestScheduleDayOfWeek { get; set; }
        public string QuestionNumberLabel { get; set; }
        public TimeSpan? TestScheduleStartTime { get; set; }
        public TimeSpan? TestScheduleEndTime { get; set; }
        public bool TestScheduleSeparateDateAndTime { get; set; }
        public bool SectionAvailability { get; set; }
        public List<int> OpenSections { get; set; }
        public bool AnonymizedScoring { get; set; }
    }
}
