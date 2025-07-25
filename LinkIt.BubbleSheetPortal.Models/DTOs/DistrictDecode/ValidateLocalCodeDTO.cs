using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.DistrictDecode
{
    public class ValidateLocalCodeDTO
    {
        public List<string> Formats { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidateObjectDTO
    {
        public List<ValidateField> Fields { get; set; }
    }
    public class ValidateField
    {
        public string Label { get; set; }
        public bool Value { get; set; }
    }
}
