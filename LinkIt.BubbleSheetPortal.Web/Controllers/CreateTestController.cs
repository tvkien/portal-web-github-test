using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FluentValidation.Results;
using LinkIt.BubbleSheetPortal.Models.AuthorizeItemLib;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Print;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using S3Library;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    public class CreateTestController : BaseController
    {
        private readonly CreateTestControllerParameters parameters;
        private readonly IS3Service s3Service;
        public CreateTestController(CreateTestControllerParameters parameters, IS3Service s3Service)
        {
            this.parameters = parameters;
            this.s3Service = s3Service;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TestdesignCreateTest)]
        public ActionResult Index()
        {
            var model = new ManageTestViewModel();
            if (CurrentUser.IsPublisher)
            {
                model.IsPublisher = true;
                model.StateId = -1;
            }
            else
            {
                model.IsPublisher = false;
                var vDistrict = CurrentUser.DistrictId.HasValue
                    ? parameters.DistrictService.GetDistrictById(CurrentUser.DistrictId.Value)
                    : null;
                if (vDistrict != null)
                {
                    model.StateId = vDistrict.StateId;
                }
            }

            return View(model);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult CreateTestBank(int subjectId, string testBankName)
        {
            var validationFailures = new List<ValidationFailure>();

            if(subjectId <= 0)
                validationFailures.Add(new ValidationFailure("error", "Please select a subject."));
            if(string.IsNullOrEmpty(testBankName))
                validationFailures.Add(new ValidationFailure("error", "Test Bank is required."));

            if(validationFailures.Count > 0)
                return Json(new { Success = false, ErrorList = validationFailures });

            var bank = new Bank
            {
                BankAccessID = 1,
                CreatedByUserId = CurrentUser.Id,
                Name = testBankName.Trim(),
                SubjectID = subjectId,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };
            parameters.BankService.Save(bank);

            return Json(new { Success = true, BankId = bank.Id});
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult CreateVirtualTest(int bankId, string testName)
        {
            var validationFailures = new List<ValidationFailure>();

            if (bankId <= 0)
                validationFailures.Add(new ValidationFailure("error", "Please select a test bank."));
            if (string.IsNullOrEmpty(testName))
                validationFailures.Add(new ValidationFailure("error", "Test Name is required."));
            else
            {
                if (parameters.VirtualTestService.ExistTestName(bankId, testName))
                {
                    validationFailures.Add(new ValidationFailure("error", "A test with name " + testName + " already exists in this bank."));
                }
            }

            if (validationFailures.Count > 0)
                return Json(new { Success = false, ErrorList = validationFailures });

            var vBank = parameters.BankService.GetBankById(bankId);
            var vSubejct = parameters.SubjectService.GetSubjectById(vBank.SubjectID);
            var vTest = new VirtualTestData
                            {
                                BankID = bankId,
                                Name = testName,
                                CreatedDate = DateTime.UtcNow,
                                UpdatedDate = DateTime.UtcNow,
                                AuthorUserID = CurrentUser.Id,
                                VirtualTestType = 3,
                                VirtualTestSourceID = 1,
                                StateID = vSubejct.StateId,
                                VirtualTestSubTypeID = 1,
                                TestScoreMethodID = 1
                            };
            parameters.VirtualTestService.Save(vTest);
            if (vTest.VirtualTestID > 0)
            {
                //No create default section anymore because Flash does not create default section
                //Optional: Add a new Default section to virtual test
                //try
                //{
                //    var defaultSection = new VirtualSection
                //                             {
                //                                 VirtualTestId = vTest.VirtualTestID,
                //                                 Order = 1,
                //                                 Instruction = string.Empty,
                //                                 AudioRef = string.Empty,
                //                                 Title = "Default Section",
                //                                 MediaReference = string.Empty,
                //                                 VideoRef = string.Empty,
                //                                 MediaSource = string.Empty,
                //                                 ConversionSetId = null
                //                             };
                //    parameters.VirtualSectionService.Save(defaultSection);

                //}
                //catch (Exception)
                //{

                //}
                //if (Util.UploadTestItemMediaToS3)
                {
                    var s3VirtualTest = parameters.VirtualTestService.CreateS3Object(vTest.VirtualTestID);
                    var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, s3Service);

                    if (!s3Result.IsSuccess)
                    {
                        validationFailures.Add(new ValidationFailure("error",
                                                                     "Virtual Test has been created successfully but uploading json file to S3 fail: " +
                                                                     s3Result.ErrorMessage));
                        return Json(new {Success = false, ErrorList = validationFailures});
                    }
                }

                return Json(new {Success = true, VirtualTestId = vTest.VirtualTestID});
            }
            else
            {
                validationFailures.Add(new ValidationFailure("error", "Can not create test bank."));
                return Json(new { Success = false, ErrorList = validationFailures });
            }
        }
    }
}
