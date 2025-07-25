using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using S3Library;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    [VersionFilter]
    public class RubricController : BaseController
    {
        private readonly RubricControllerParameters parameters;
        public RubricController(RubricControllerParameters parameters, IS3Service s3Service)
        {
            this.parameters = parameters;
            _s3Service = s3Service;
        }
        private readonly IS3Service _s3Service;

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TestdesignRubric)]
        public ActionResult Index()
        {
            var model = new RubricViewModel()
            {
                IsPublisher = CurrentUser.IsPublisher,
                IsAdmin = CurrentUser.IsAdmin
            };
            if (CurrentUser.IsNetworkAdmin)
            {
                model.IsNetworkAdmin = true;
                model.ListDistricIds = CurrentUser.GetMemberListDistrictId();
            }
            return View(model);
        }

        public ActionResult GetListRubrics(RubricCustomListViewModel model)
        {
            var parser = new DataTableParserProc<RubricCustomListViewModel>();
            if (model.DistrictId == null)
            {
                return Json(parser.Parse(new List<RubricCustomListViewModel>().AsQueryable(), 0), JsonRequestBehavior.AllowGet);
            }

            int? totalRecords = 0;
            model.PageIndex = parser.StartIndex;
            model.PageSize = parser.PageSize;
            model.SortColumns = parser.SortableColumns;

            var result = BuildRubricData(model, ref totalRecords);
            if (result == null)
            {
                return Json(parser.Parse(new List<RubricCustomListViewModel>().AsQueryable(), 0), JsonRequestBehavior.AllowGet);
            }
            var finalResult = result.Select(o => new RubricCustomListViewModel()
            {
                SubjectName = o.SubjectName,
                GradeName = o.GradeName,
                TestBankName = o.BankName,
                Author = o.Author,
                TestName = o.TestName,
                FileName = o.FileName,
                DistrictId = o.DistrictId,
                GradeId = o.GradeId,
                SubjectId = o.SubjectId,
                RubricId = o.VirtualTestFileId,
                TestId = o.TestId,
                RubricKey = o.FileKey
            });
            return new JsonNetResult { Data = parser.Parse(finalResult.AsQueryable(), totalRecords ?? 0) };
        }

        public ActionResult GetListRubricsNew(RubricCustomListViewModel model)
        {
            var parser = new DataTableParserProc<RubricCustomListViewModel>();
            var param = new RubricCustomList()
                        {
                            DistrictId = model.DistrictId,
                            UserId = CurrentUser.Id,
                            UserRole = CurrentUser.RoleId,
                            GradeId = model.GradeId.HasValue && model.GradeId.Value != 0 ? model.GradeId : null,
                            SubjectId = model.SubjectId,
                            SubjectName = model.SubjectName != null && !model.SubjectName.Equals("0") ? model.SubjectName : null,
                            Author = model.Author,
                            BankName = model.TestBankName,
                            TestName = model.TestName,
                            PageIndex = parser.StartIndex,
                            PageSize = parser.PageSize,
                            SortColumn = parser.SortableColumns
                        };
            if (!CurrentUser.IsPublisherOrNetworkAdmin)
                param.DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            int? totalRecords = 0;
            var result = parameters.ListRubricServices.GetListRubricsBySubjectName(param, ref totalRecords);
            if (result == null)
            {
                return Json(parser.Parse(new List<RubricCustomListViewModel>().AsQueryable(), 0), JsonRequestBehavior.AllowGet);
            }
            var finalResult = result.Select(o => new RubricCustomListViewModel()
            {
                SubjectName = o.SubjectName,
                GradeName = o.GradeName,
                TestBankName = o.BankName,
                Author = o.Author,
                TestName = o.TestName,
                FileName = o.FileName,
                DistrictId = o.DistrictId,
                GradeId = o.GradeId,
                SubjectId = o.SubjectId,
                RubricId = o.VirtualTestFileId,
                TestId = o.TestId,
                RubricKey = o.FileKey
            });

            return Json(parser.Parse(finalResult.AsQueryable(), totalRecords ?? 0), JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveAssocialRubric(int testId, int rubricId)
        {
            if (parameters.VirtualTestFileServices.DeleteVirtualTestFile(testId))
            {
                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = "Fail" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRubricByVirtualTest(int virtualTestId)
        {
            var virtualTestFile =
                    parameters.VirtualTestFileServices.GetFirstOrDefaultByVirtualTest(virtualTestId);

            if (virtualTestFile != null)
            {
                return Json(new { Success = true, VirtualTestFileKey = virtualTestFile.FileKey, VirtualTestFileName = virtualTestFile.FileName}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        private string GetPublishRubricPdfURL(string fileName)
        {
            var bucketName = LinkitConfigurationManager.GetS3Settings().RubricBucketName;

            var publicUrl = _s3Service.GetPublicUrl(bucketName, "/" + fileName);
            return publicUrl;
        }

        private int ValidDistrict(int? districtId)
        {
            int? vDistrictId = null;
            if ((CurrentUser.IsPublisher|| CurrentUser.IsNetworkAdmin) && districtId.HasValue)
            {
                vDistrictId = districtId;
            }
            else if (!CurrentUser.IsPublisher && CurrentUser.DistrictId.HasValue)
            {
                vDistrictId = CurrentUser.DistrictId;
            }
          
            if (vDistrictId == null) return -1;
            return (int)vDistrictId;
        }

        private IEnumerable<ListRubric> BuildRubricData(RubricCustomListViewModel model, ref int? totalRecords)
        {
            var vDistrictId = ValidDistrict(model.DistrictId);
            if (vDistrictId == -1) return null;
            //var subjectId = model.SubjectId.HasValue ? model.SubjectId.Value : 0;
            //var gradeId = model.GradeId.HasValue ? model.GradeId.Value : 0;
            //string strBankName = model.TestBankName ?? string.Empty;
            //string strAuthor = model.Author ?? string.Empty;
            //string strTestName = model.TestName ?? string.Empty;
            //var lstTeacherId = new List<int>();
            //if (CurrentUser.RoleId == (int)Permissions.SchoolAdmin)
            //{
            //    //TODO: Get List TeacherId by currernt UserId
            //    lstTeacherId = parameters.UserSchoolServices.GetListUserBySchoolAdminId(CurrentUser.Id);
            //}
            //var query = parameters.ListRubricServices.GetListRubricsByRole(CurrentUser.Id, CurrentUser.RoleId, vDistrictId, lstTeacherId);
            //query = parameters.ListRubricServices.GetListRubrics(query, gradeId, subjectId, strBankName, strAuthor,
            //                                                     strTestName);

            var data = parameters.ListRubricServices.GetRubrics(vDistrictId, CurrentUser.Id, CurrentUser.RoleId, model.GradeId,
                model.SubjectId, model.TestBankName, model.Author, model.TestName, model.PageIndex, model.PageSize, ref totalRecords, model.SortColumns);
            return data;
        }

        [UploadifyPrincipal(Order = 1)]
        public ActionResult UploadRubric(HttpPostedFileBase postedFile, int? testId)
        {
            if (!testId.HasValue)
                return Json(new { success = false, errorMessage = "Invalid TestId" }, JsonRequestBehavior.AllowGet);
            string rubricbuketName = LinkitConfigurationManager.GetS3Settings().RubricBucketName;
            string rubricFolder = LinkitConfigurationManager.GetS3Settings().RubricFolder;
            try
            {
                string fileName = string.Format("{0}/{1}_{2}", rubricFolder, DateTime.Now.Ticks, postedFile.FileName);
                var s3Result = _s3Service.UploadRubricFile(rubricbuketName, fileName, postedFile.InputStream);
                if (s3Result.IsSuccess)
                {
                    var virtualTestFile = new VirtualTestFile()
                    {
                        FileKey = Util.GenerateRandonStringKey(25),
                        FileName = postedFile.FileName,
                        FileType = (int)UploadFileType.Rubric,
                        FileUrl = s3Result.ReturnValue,
                        UploadByUserId = CurrentUser.Id,
                        UploadDate = DateTime.UtcNow,
                        VirtualTestId = testId.Value
                    };
                    parameters.VirtualTestFileServices.Save(virtualTestFile);
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, errorMessage = s3Result.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { success = false, errorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }       
    }
}
