using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class StudentUpdateInfoViewModel : AccountViewModelBase
    {
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression("(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^*()_+-]*).{7,}", ErrorMessage = "Valid password must be seven characters long and consist of at least 1 uppcase letter, lowercase letter, and number. Passwords may contain the special characters: ! @ # $ % ^ * ( ) _ + -")]
        public string Password { get; set; }

        [System.Web.Mvc.Compare("Password", ErrorMessage = "Password must match")]
        public string ConfirmPassword { get; set; }

        public int StudentId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int DistrictId { get; set; }

        public bool KeepLogged { get; set; }

        public List<SelectListItem> Questions { get; set; }

        [Required(ErrorMessage = "Question is required")]
        public string Question { get; set; }

        [Required(ErrorMessage = "Answer is required")]
        public string Answer { get; set; }
    }
}