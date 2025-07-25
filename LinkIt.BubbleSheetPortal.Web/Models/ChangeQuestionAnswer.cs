using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.Models
{
    public class ChangeQuestionAnswer
    {
        public int UserID { get; set; }
        public string Username { get; set; }

        [Required]
        public string SelectedQuestion { get; set; }

        [Display(Name = "Current Security Question")]
        public string CurrentSecurityQuestion { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string Password { get; set; }

        [Display(Name = "New Security Question")]
        public List<SelectListItem> Questions { get; set; }

        [Required]
        [Display(Name = "New Security Answer")]
        public string Answer { get; set; }
    }
}