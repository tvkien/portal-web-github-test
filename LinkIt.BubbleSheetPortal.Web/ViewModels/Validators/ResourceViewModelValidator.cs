using System;
using System.Web.Mvc;
using FluentValidation;
using System.Net;
using LinkIt.BubbleSheetPortal.Web.Security;
using System.Net.Http;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class ResourceViewModelValidator : AbstractValidator<ResourceViewModel>
    {
        public ResourceViewModelValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.LessonName)
                .NotEmpty()
               .WithMessage("Resource Name is required.");

            RuleFor(x => x.SelectedLessonProviderId)
               .NotNull()
              .WithMessage("Content Provider is required.");

            RuleFor(x => x.LessonProviderId)
               .GreaterThan(0)
              .WithMessage("Content Provider is required.");


            RuleFor(x => x.LessonContentTypeId)
              .NotNull()
             .WithMessage("Resource Type is required.");

            RuleFor(x => x.LessonContentTypeId)
               .GreaterThan(0)
              .WithMessage("Resource Type is required.");

            RuleFor(x => x.SubjectId)
            .NotNull()
           .WithMessage("Subject is required.");

            RuleFor(x => x.SubjectId)
               .GreaterThan(0)
              .WithMessage("Subject is required.");

            RuleFor(x => x.LessonPath)
                .NotEmpty()
                .When(x => x.LessonSelection.Equals("link"))
                .WithMessage("Lesson Path is required.");

            RuleFor(x => x.LessonPath).Must(ValidateResourceLink)
                .WithMessage("Lesson Path must be an valid url.");


            //if user enter external guide, it must begin with http
            RuleFor(x => x.GuidePath).Must(ValidateReferenceLink)
                .When(x => !string.IsNullOrEmpty(x.GuidePath) && x.GuideSelection.Equals("link"))
                .WithMessage("Reference Path must be an valid url.");
           

        }
        private bool ValidateResourceLink(ResourceViewModel instance, string link)
        {
            if (instance.LessonSelection.Equals("link"))
            {
                return ValidateLink(link);
            }
            else
            {
                return true;
            }
        }
        private bool ValidateReferenceLink(ResourceViewModel instance, string link)
        {
            if (instance.GuideSelection.Equals("link"))
            {
                return ValidateLink(link);
            }
            else
            {
                return true;
            }
        }
        private bool ValidateLink(string link)
        {
            try
            {
                if (string.IsNullOrEmpty(link))
                {
                    return false;
                }
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        HttpResponseMessage response = client.GetAsync(link).Result;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        PortalAuditManager.LogException(ex);
                        return false;
                    }
                }
            }
            catch
            {
                return false;                
            }
        }
    }
}
