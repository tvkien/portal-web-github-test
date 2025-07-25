using LinkIt.BubbleSheetPortal.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.LTI
{
    public class IdTokenDto
    {
        [Required]
        public string Iss { get; set; }
        [Required]
        public string Nonce { get; set; }
        public IList<string> Aud { get; set; }
        [Required]
        [CustomValidation(typeof(DateTimeAttribute), nameof(DateTimeAttribute.IatValidate))]
        public double? Iat { get; set; }
        [Required]
        [CustomValidation(typeof(DateTimeAttribute), nameof(DateTimeAttribute.ExpValidate))]
        public double? Exp { get; set; }
        public string Azp { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string DeploymentId { get; set; }
    }

    public class DateTimeAttribute : ValidationAttribute
    {
        public static ValidationResult IatValidate(double date)
        {
            if (DateTime.Compare(date.UnixTimeStampToDateTime(), DateTime.UtcNow) > 0)
                return new ValidationResult("The IAT is Invalid");

            return ValidationResult.Success;
        }

        public static ValidationResult ExpValidate(double date)
        {
            if (DateTime.Compare(date.UnixTimeStampToDateTime(), DateTime.UtcNow) < 0)
                return new ValidationResult("The EXP is Invalid");

            return ValidationResult.Success;
        }
    }
}
