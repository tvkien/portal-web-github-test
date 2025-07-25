using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TLDSDigital
{
    public class TldsLoginViewModel
    {
        public Guid Id { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string DateOfBirthString { get; set; }
    }
}
