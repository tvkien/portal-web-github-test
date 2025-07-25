using System.ComponentModel.DataAnnotations;

namespace LinkIt.BubbleSheetPortal.Models.Enums
{
    public enum RetakeStatus
    {
        [Display(Name = "Assigned, Not Started")]
        NotStarted = -1,
        [Display(Name = "In Progress")]
        InProgess = 1,
        [Display(Name = "In Progress")]
        InProgess_2 = 2,
        [Display(Name = "Paused")]
        Pause = 3,
        [Display(Name = "Completed")]
        Completed = 4,
        [Display(Name = "Pending Review")]
        PendingReview = 5,
    }
}
