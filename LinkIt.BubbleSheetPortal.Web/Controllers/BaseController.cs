using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web.Mvc;
using System.Web.SessionState;
using FluentValidation.Results;
using LinkIt.BubbleService.Models.Test;
using Envoc.Core.Shared.Model;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using System.Security.Principal;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [ValidSubdomain]
    [SessionState(SessionStateBehavior.ReadOnly)]
    [CustomOutputCache(CacheProfile = "NoCache")]
    public class BaseController : Controller
    {
        protected UserPrincipal CurrentUser
        {
            get
            {
                if(User is GenericPrincipal)
                {
                    return new UserPrincipal();
                }

                return User as UserPrincipal;
            }
        }

        protected T Attempt<T>(Func<T> attempt) where T : IHasClientErrors, new()
        {
            try
            {
                return attempt.Invoke();
            }
            catch (ValidationException e)
            {
                var itemWithErrors = new T {Error = GetClientErrors(e)};
                return itemWithErrors;
            }
            catch(SecurityException e)
            {
                var itemWithErrors = new T {Error = GetClientErrors(e)};
                return itemWithErrors;
            }
        }

        protected Dictionary<string, List<string>> GetClientErrors(SecurityException e)
        {
            var result = new Dictionary<string, List<string>>();
            result.Add("ApiKey", new List<string> { "'Api Key' is incorrect." });
            return result;
        }

        protected Dictionary<string, List<string>> GetClientErrors(ValidationException e)
        {
            var result = new Dictionary<string, List<string>>();
            foreach (var error in e.Errors)
            {
                var propErrors = error.ErrorMessage.Split('\n').ToList();
                if (result.ContainsKey(error.PropertyName))
                {
                    result[error.PropertyName].AddRange(propErrors);
                }
                else
                {
                    result.Add(error.PropertyName, propErrors);
                }
            }
            return result;
        }

        protected bool IsValid<T>(ValidatableEntity<T> model) where T : class
        {
            if (!model.CanValidate)
            {
                model.SetValidator(DependencyResolver.Current.GetService<IValidator<T>>());
            }

            return ValidateModel(model);
        }

        private bool ValidateModel<T>(ValidatableEntity<T> model) where T : class
        {
            if (!model.IsValid)
            {
                foreach (var error in model.ValidationErrors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return false;
            }

            return true;
        }

        public ActionResult ShowJsonResultException<T>(T model, string errorText) where T : ValidatableEntity<T>
        {
            var validationFailures = model.ValidationErrors.ToList();
            validationFailures.Add(new ValidationFailure("error", errorText));
            return Json(new { Success = false, ErrorList = validationFailures });
        }

        protected int CurrentDistrict(int? districtId)
        {
            return CurrentUser.IsPublisherOrNetworkAdmin && districtId.HasValue ? districtId.Value : CurrentUser.DistrictId.Value;
        }

        protected internal LargeJsonResult LargeJson(object data, JsonRequestBehavior behavior)
        {
            return new LargeJsonResult { Data = data, MaxJsonLength = int.MaxValue, JsonRequestBehavior = behavior };
        }
    }
}
