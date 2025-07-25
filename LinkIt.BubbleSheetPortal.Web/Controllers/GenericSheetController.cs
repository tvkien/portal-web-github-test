using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using DevExpress.Office.Utils;
using Envoc.Core.Shared.Extensions;
using FluentValidation;
using LinkIt.BubbleService.Models.Corrections;
using LinkIt.BubbleService.Models.Reading;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.BubbleSheetAws;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Web.ViewModels.GenericSheets;
using LinkIt.BubbleSheetPortal.Web.ViewModels.Validators;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class GenericSheetController : ReviewBubbleSheetBaseController
    {
        private readonly GenericBubbleSheetService genericBubbleSheetService;
        private readonly ClassStudentService classStudentService;
        private readonly BubbleSheetFileService bubbleSheetFileService;
        private readonly BubbleSheetService bubbleSheetService;
        private readonly VirtualTestService virtualTestService;
        private readonly IValidator<AssignGenericSheetViewModel> assignGenericSheetViewModelValidator;
        private readonly IValidator<AssignGenericSheetActSatViewModel> assignGenericSheetActSatViewModelValidator;

        private readonly GenderService _genderService;
        private readonly RaceService _raceService;
        private readonly GradeService _gradeService;
        private readonly StudentService _studentService;
        private readonly ClassService _classService;
        private readonly UserService _userService;
        private readonly ClassUserService _classUserService;
        private readonly BubbleSheetGenericStudentInfoService _bubbleSheetGenericStudentInfoService;
        private GenericSheetNewStudentViewModelValidator _genericSheetNewStudentViewModel;

        public GenericSheetController(GenericBubbleSheetService genericBubbleSheetService, 
            ClassStudentService classStudentService, 
            BubbleSheetListService bubbleSheetListService,
            BubbleSheetFileService bubbleSheetFileService,
            BubbleSheetService bubbleSheetService,
            IValidator<AssignGenericSheetViewModel> assignGenericSheetViewModelValidator,
            GenderService genderService,
            RaceService raceService,
            GradeService gradeService,
            StudentService studentService,
            ClassService classService,
            UserService userService,
            ClassUserService classUserService,
            GenericSheetNewStudentViewModelValidator genericSheetNewStudentViewModel,
            BubbleSheetGenericStudentInfoService bubbleSheetGenericStudentInfoService,
            IValidator<AssignGenericSheetActSatViewModel> assignGenericSheetActSatViewModelValidator,
            VulnerabilityService vulnerabilityService,
            DistrictDecodeService districtDecodeService,
            VirtualTestService virtualTestService
            )
            : base(bubbleSheetListService, bubbleSheetService, vulnerabilityService, userService, districtDecodeService)
        {
            this.genericBubbleSheetService = genericBubbleSheetService;
            this.classStudentService = classStudentService;
            this.bubbleSheetFileService = bubbleSheetFileService;
            this.bubbleSheetService = bubbleSheetService;
            this.assignGenericSheetViewModelValidator = assignGenericSheetViewModelValidator;
            _genderService = genderService;
            _raceService = raceService;
            _gradeService = gradeService;
            _studentService = studentService;
            _genericSheetNewStudentViewModel = genericSheetNewStudentViewModel;
            _classService = classService;
            _userService = userService;
            _classUserService = classUserService;
            _bubbleSheetGenericStudentInfoService = bubbleSheetGenericStudentInfoService;
            this.assignGenericSheetActSatViewModelValidator = assignGenericSheetActSatViewModelValidator;
            this.virtualTestService = virtualTestService;
        }

        [HttpGet]
        public ActionResult AssignStudentsToTest(string id, int? classId)
        {
            var genericSheet = genericBubbleSheetService.GetGenericSheetsByTicket(id, classId).FirstOrDefault();

            var model = new GenericSheetViewModel();

            if (genericSheet.IsNull())
            {
                model.HasNoFilesUploaded = true;
                return View(model);
            }

            var virtualTest = GetVirtualTestByBubbleSheetTicketId(id);

            model.Ticket = genericSheet.Ticket;
            model.ClassId = genericSheet.ClassID;
            model.TestName = virtualTest?.Name ?? string.Empty;

            return View(model);
        }

        [HttpGet]
        public ActionResult AssignStudentsToTestMultiPage(string id, int? classId)
        {
            var genericSheet = genericBubbleSheetService.GetGenericSheetsByTicket(id, classId).FirstOrDefault();

            var model = new GenericSheetViewModel();

            if (genericSheet.IsNull())
            {
                model.HasNoFilesUploaded = true;
                return View(model);
            }

            var virtualTest = GetVirtualTestByBubbleSheetTicketId(id);

            model.Ticket = genericSheet.Ticket;
            model.ClassId = genericSheet.ClassID;
            model.TestName = virtualTest?.Name ?? string.Empty;

            return View(model);
        }

        [HttpGet]
        public ActionResult GetBubbleSheets(string ticket, int? classId)
        {
            var genericSheets = genericBubbleSheetService.GetGenericSheetsByTicket(ticket, classId).Select(x => new GenericSheetStudentViewModel
                {
                    InputFileName = x.InputFileName,
                    BubbleSheetFileId = x.BubbleSheetFileId,
                    BubbleSheetId = x.BubbleSheetId,
                    StudentName = x.LastName + ", " + x.FirstName,
                    PageNumber = x.PageNumber
                });
            var parser = new DataTableParser<GenericSheetStudentViewModel>();
            return new JsonNetResult { Data = parser.ParseForClientSide(genericSheets) };
        }

        [HttpPost]
        public ActionResult AssignAndGradeStudent(int bubbleSheetFileId)
        {
            var sheet = genericBubbleSheetService.GetGenericSheetByBubbleSheetId(bubbleSheetFileId);

            if (sheet.IsNull())
            {
                return HttpNotFound();
            }

            var model = new AssignGenericSheetViewModel
                {
                    BubbleSheetFileId = sheet.BubbleSheetFileId,
                    FileName = sheet.InputFileName,
                    ImageUrl = BubbleSheetWsHelper.GetTestImageUrl(sheet.OutputFileName, ConfigurationManager.AppSettings["ApiKey"]),
                    BubbleSheetId = sheet.BubbleSheetId,
                    ClassId =  sheet.ClassID
                };

            var selectedClassId = sheet.ClassID.HasValue ? sheet.ClassID.Value : 0;

            MapStudentLists(model, sheet, selectedClassId);

            return View("_AssignStudent", model);
        }

        [HttpPost]
        public ActionResult AssignAndGradeStudentMultiPage(List<int> bubbleSheetFileIdList)
        {
            var listModel = new AssignGenericSheetMultiPageViewModel();
            foreach (var bubbleSheetFileId in bubbleSheetFileIdList)
            {
                var sheet = genericBubbleSheetService.GetGenericSheetByBubbleSheetId(bubbleSheetFileId);
                if (sheet != null)
                {
                    string strImage = string.Empty;
                    var model = new AssignGenericSheetViewModel
                    {
                        BubbleSheetFileId = sheet.BubbleSheetFileId,
                        FileName = sheet.InputFileName,
                        ImageUrl = BubbleSheetWsHelper.GetTestImageUrl(sheet.OutputFileName, ConfigurationManager.AppSettings["ApiKey"])
                        //ImageUrl = GetImageUrl<TestImage>(sheet.OutputFileName)
                        //OnlyOnePage = CheckOnlyOneBubbleSheetFileByBubbleSheetFileId(bubbleSheetFileId, out strImage)
                    };
                    //model.ImageUrl = strImage;

                    MapStudentLists(model, sheet, sheet.ClassID.GetValueOrDefault());
                    listModel.ListAssignGenericSheetViewModels.Add(model);
                }
            }

            if (listModel.ListAssignGenericSheetViewModels.Any())
            {
                return View("_AssignStudentMultiPage", listModel);
            }
            return HttpNotFound();
        }

        private void MapStudentListsACTSAT(AssignGenericSheetViewModel model, GenericBubbleSheetACTSAT sheet)
        {
            if (sheet.ClassID.HasValue)
            {
                model.ClassId = sheet.ClassID.Value;

                var mappedStudentIds =
                    bubbleSheetService.GetBubbleSheetByTicket(sheet.Ticket)
                        .Select(x => x.StudentId)
                        .Where(x => x.Value != 0)
                        .ToList();

                var studentsInClass =
                    classStudentService.GetClassStudentsByClassId(sheet.ClassID.Value).ToList();

                model.AllStudents = studentsInClass.Select(x => new SelectListItem
                {
                    Value = x.StudentId.ToString(),
                    Text = x.FullName
                }).OrderBy(x => x.Text);

                model.RemainingStudents = studentsInClass.Where(x => !mappedStudentIds.Contains(x.StudentId))
                    .Select(x => new SelectListItem
                    {
                        Value = x.StudentId.ToString(),
                        Text = x.FullName
                    }).OrderBy(x => x.Text);
            }
            else
            {
                var classess = _classService.GetClassesByIds(sheet.ClassIds.Split(';').Select(x => int.Parse(x)).ToList()).ToList();

                var detectedStudentId = DetectStudentInBubbleSheetFile(sheet.BubbleSheetFileId);

                if (detectedStudentId > 0)
                {
                    model.IsStudentDetected = true;

                    var classStudents = classStudentService.GetClassStudentsByStudentId(detectedStudentId);
                    classess = classess.Where(x => classStudents.Any(c => c.ClassId == x.Id)).ToList();
                }

                var classWithTeacherNames =
                    classess.Select(x => new {x.Id, x.Name, TeacherName = GetTeacherName(x.Id)});

                model.Classes = classWithTeacherNames.OrderBy(x=>x.TeacherName.Split(',')[0]).ThenBy(x=>x.Name)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name + "(" + x.TeacherName + ")"
                    });
            }            
        }

        private string GetTeacherName(int classId)
        {
            // Detect primary teacher
            var classUser = _classUserService.GetClassUsersByClassId(classId).FirstOrDefault(x => x.ClassUserLOEId == 1);

            if (classUser != null)
            {
                var user = _userService.GetUserById(classUser.UserId);
                if (user != null)
                {
                    if (string.IsNullOrEmpty(user.LastName) || string.IsNullOrEmpty(user.FirstName))
                    {
                        return (user.LastName + user.FirstName);
                    }

                    return user.LastName + ", " + user.FirstName;
                }

            }

            return "";
        }

        private int DetectStudentInBubbleSheetFile(int bubbleSheetFileId)
        {
            var bubblesheetGenericStudentInfo = _bubbleSheetGenericStudentInfoService.Select()
                    .FirstOrDefault(x => x.BubbleSheetFileId == bubbleSheetFileId);

            // Filter class if already detect student information
            if (bubblesheetGenericStudentInfo != null)
            {
                var student =
                    _studentService.GetStudentByCode(bubblesheetGenericStudentInfo.StudentCode);
                if (student != null &&
                    student.FirstName != null && student.FirstName.Length >= 3 &&
                    student.LastName != null && student.LastName.Length >= 4 &&
                    bubblesheetGenericStudentInfo.FirstName != null &&
                    bubblesheetGenericStudentInfo.FirstName.Length >= 3 &&
                    bubblesheetGenericStudentInfo.LastName != null &&
                    bubblesheetGenericStudentInfo.LastName.Length >= 4 &&
                    student.FirstName.Substring(0, 3).ToLower() ==
                    bubblesheetGenericStudentInfo.FirstName.Substring(0, 3).ToLower() &&
                    student.LastName.Substring(0, 4).ToLower() ==
                    bubblesheetGenericStudentInfo.LastName.Substring(0, 4).ToLower())
                {
                    return student.Id;
                }
            }

            return 0;
        }

        private void MapStudentLists(AssignGenericSheetViewModel model, GenericBubbleSheet sheet, int classId)
        {
            var mappedStudentIds = bubbleSheetService.GetBubbleSheetByTicket(sheet.Ticket).Select(x => x.StudentId).Where(x => x.Value != 0).ToList();
            var studentsInClass = classStudentService.GetClassStudentsByClassId(classId).ToList();
            
            model.AllStudents = studentsInClass.Select(x => new SelectListItem
                {
                    Value = x.StudentId.ToString(),
                    Text = x.FullName
                }).OrderBy(x => x.Text);

            model.RemainingStudents = studentsInClass.Where(x => !mappedStudentIds.Contains(x.StudentId)).Select(x => new SelectListItem
                {
                    Value = x.StudentId.ToString(),
                    Text = x.FullName
                }).OrderBy(x => x.Text);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult SubmitGenericSheetAssignment(AssignGenericSheetViewModel model)
        {
            model.SetValidator(assignGenericSheetViewModelValidator);
            if (!model.IsValid)
            {
                return Json(new { success = false, ErrorList = model.ValidationErrors });
            }

            var bubbleSheetFile = bubbleSheetFileService.GetBubbleSheetFileById(model.BubbleSheetFileId);
            if (bubbleSheetFile == null)
            {
                return new HttpNotFoundResult("No such bubble sheet file");
            }

            bubbleSheetFile.IsUnmappedGeneric = true;
            bubbleSheetFileService.SaveBubbleSheetFile(bubbleSheetFile);

            var studentId = model.IsAllStudentsChecked ? model.SelectedAllStudentsId : model.SelectedRemainingStudentsId;

            var bubbleSheet = bubbleSheetService.CopyBubbleSheetForStudent(bubbleSheetFile.BubbleSheetId, studentId);
            if (bubbleSheet == null)
            {
                return new HttpNotFoundResult("No such student");
            }

            var changeResultBarcode = new ChangeResultBarcodeRequest
                                      {
                                          Barcode = bubbleSheetFile.InputFilePath,
                                          UserAssignedBarcode = bubbleSheet.Id.ToString(),
                                          RelatedImage = bubbleSheetFile.OutputFileName
                                      };
            //correctionsService.SubmitResultWithBarcode(new ChangeResultBarcodeRequest
            //{
            //    Barcode = bubbleSheetFile.InputFilePath,
            //    UserAssignedBarcode = bubbleSheet.Id.ToString(),
            //    RelatedImage = bubbleSheetFile.OutputFileName
            //});
            BubbleSheetWsHelper.SubmitResultWithBarcode(changeResultBarcode);

            return Json(new { success = true, isAllStudentsSelected = model.IsAllStudentsChecked, bubblesheetFileId = model.BubbleSheetFileId });
        }

        //\ACT/SAT Generict Sheet

        [HttpGet]
        public ActionResult AssignStudentsToTestActSat(string qticket, int qclassId)
        {
            var genericSheet = genericBubbleSheetService.GetGenericSheetsACTSATByTicketAndClassId(qticket, qclassId).FirstOrDefault();

            var model = new GenericSheetViewModel() { VirtualTestSubTypeId = 0 };
            if (genericSheet.IsNull())
            {
                model.HasNoFilesUploaded = true;
                return View(model);
            }

            var virtualTest = GetVirtualTestByBubbleSheetTicketId(qticket);

            model.Ticket = genericSheet.Ticket;
            //model.ClassId = genericSheet.ClassID.GetValueOrDefault();
            model.ClassId = qclassId;
            model.VirtualTestSubTypeId = genericSheet.VirtualTestSubTypeId;
            model.TestName = virtualTest?.Name ?? string.Empty;

            return View(model);
        }

        [HttpGet]
        public ActionResult GetBubbleSheetsActSat(string ticket, int classId)
        {
            var genericSheets = genericBubbleSheetService.GetGenericSheetsACTSATByTicketAndClassId(ticket,classId).Select(x => new GenericSheetStudentViewModel
            {
                InputFileName = x.InputFileName,
                BubbleSheetFileId = x.BubbleSheetFileId,
                BubbleSheetId = x.BubbleSheetId,                
                StudentName = x.LastName + ", " + x.FirstName
            });
            var parser = new DataTableParser<GenericSheetStudentViewModel>();
            return new JsonNetResult { Data = parser.ParseForClientSide(genericSheets) };
        }

        [HttpPost]
        public ActionResult AssignAndGradeStudentActSat(int bubbleSheetFileId, string ticket)
        {
            //var sheet = genericBubbleSheetService.GetGenericSheetACTSATByBubbleSheetId(bubbleSheetFileId);
            var sheet = genericBubbleSheetService.GetGenericSheetACTSATByBubbleSheetIdAndTicket(ticket, bubbleSheetFileId);
            if (sheet.IsNull())
            {
                return HttpNotFound();
            }
            string strImageUrl = string.Empty;
            var model = new AssignGenericSheetViewModel
            {
                BubbleSheetFileId = sheet.BubbleSheetFileId,
                FileName = sheet.InputFileName,
                //ImageUrl = BubbleSheetWsHelper.GetTestImageUrl(sheet.OutputFileName, ConfigurationManager.AppSettings["ApiKey"]),
                BubbleSheetId = sheet.BubbleSheetId,
                VirtualTestSubTypeId = sheet.VirtualTestSubTypeId,
                OnlyOnePage = CheckOnlyOneBubbleSheetFileByBubbleSheetFileId(bubbleSheetFileId, out strImageUrl),
                Ticket = ticket
            };
            model.ImageUrl = strImageUrl;

            MapStudentListsACTSAT(model, sheet);

            return View("_AssignStudentActSat", model);
        }
        
        [HttpPost]
        [AjaxOnly]
        public ActionResult SubmitGenericSheetAssignmentMultipage(List<int> bubbleSheetFileIdList, int studentID)
        {
            foreach (var bubbleSheetFileId in bubbleSheetFileIdList)
            {
                var bubbleSheetFile = bubbleSheetFileService.GetBubbleSheetFileById(bubbleSheetFileId);
                if (bubbleSheetFile == null)
                {
                    return new HttpNotFoundResult("No such bubble sheet file");
                }

                bubbleSheetFile.IsUnmappedGeneric = true;
                bubbleSheetFileService.SaveBubbleSheetFile(bubbleSheetFile);

                var bubbleSheet = bubbleSheetService.CopyBubbleSheetForStudent(bubbleSheetFile.BubbleSheetId, studentID);
                if (bubbleSheet == null)
                {
                    return new HttpNotFoundResult("No such student");
                }

                var changeResultBarcode = new ChangeResultBarcodeRequest
                {
                    Barcode = bubbleSheetFile.InputFilePath,
                    UserAssignedBarcode = bubbleSheet.Id.ToString(),
                    RelatedImage = bubbleSheetFile.OutputFileName
                };

                BubbleSheetWsHelper.SubmitResultWithBarcode(changeResultBarcode);
            }
            return Json(new { success = true });
        }

        public ActionResult GetStudents(string ticket, int bubbleSheetFileId, int classId, bool showAllStudent)
        {
            //var sheet = genericBubbleSheetService.GetGenericSheetACTSATByBubbleSheetId(bubbleSheetFileId);
            var sheet = genericBubbleSheetService.GetGenericSheetACTSATByBubbleSheetIdAndTicket(ticket, bubbleSheetFileId);

            if (sheet.IsNull())
            {
                return HttpNotFound();
            }

            var studentsInClass =
                classStudentService.GetClassStudentsByClassId(classId).ToList();

            // If already detect student ==> return this student only
            var detectedStudentId = DetectStudentInBubbleSheetFile(bubbleSheetFileId);
            if (detectedStudentId > 0 && studentsInClass.Any(x=>x.StudentId == detectedStudentId))
            {
                var data =
                    studentsInClass.Where(x=>x.StudentId == detectedStudentId).Select(x => new ListItem { Id = x.StudentId, Name = x.LastName + ", " + x.FirstName })
                        .OrderBy(x => x.Name);

                return Json(data, JsonRequestBehavior.AllowGet);
            }

            if (!showAllStudent)
            {
                var mappedStudentIds =
                    bubbleSheetService.GetBubbleSheetByTicket(sheet.Ticket)
                        .Select(x => x.StudentId)
                        .Where(x => x.Value != 0)
                        .ToList();

                var remainingStudents = studentsInClass.Where(x => !mappedStudentIds.Contains(x.StudentId)).ToList();

                var data =
                    remainingStudents.Select(x => new ListItem {Id = x.StudentId, Name = x.LastName + ", " + x.FirstName})
                        .OrderBy(x => x.Name);

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data =
                    studentsInClass.Select(x => new ListItem { Id = x.StudentId, Name = x.LastName + ", " + x.FirstName })
                        .OrderBy(x => x.Name);

                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult SubmitGenericSheetAssignmentActSat(AssignGenericSheetActSatViewModel model)
        {
            model.SetValidator(assignGenericSheetActSatViewModelValidator);
            if (!model.IsValid)
            {
                return Json(new { success = false, ErrorList = model.ValidationErrors });
            }

            var bubbleSheetFile = bubbleSheetFileService.GetBubbleSheetFileById(model.BubbleSheetFileId);
            if (bubbleSheetFile == null)
            {
                return new HttpNotFoundResult("No such bubble sheet file");
            }

            bubbleSheetFile.IsUnmappedGeneric = true;
            bubbleSheetFileService.SaveBubbleSheetFile(bubbleSheetFile);

            var bubbleSheet = bubbleSheetService.CopyBubbleSheetForStudent(bubbleSheetFile.BubbleSheetId,
                model.SelectStudent, model.SelectClass);
            if (bubbleSheet == null)
            {
                return new HttpNotFoundResult("No such student");
            }
            //\Save BubbleSheetMap
            var obj = new BubbleSheetGenericMap()
            {
                GenericBubbleSheetID = bubbleSheetFile.BubbleSheetId,
                StudentBubbleSheetID = bubbleSheet.Id,
                TestIndex = bubbleSheetFile.GenericTestIndex
            };
            bubbleSheetFileService.SaveBubbleSheetMap(obj);

            var lstChangeResultBarcodeRequest = new List<ChangeResultBarcodeRequest>();

            var lstBubbleSheetFileSub = bubbleSheetFileService.GetBubbleSheetFileSubByBubbleSheetIdForGenericActSat(bubbleSheetFile.BubbleSheetFileId).ToList();

            if (lstBubbleSheetFileSub.Count > 0)
            {
                foreach (var sub in lstBubbleSheetFileSub)
                {

                    lstChangeResultBarcodeRequest.Add(new ChangeResultBarcodeRequest
                                        {
                                            Barcode = sub.InputFilePath, //bubbleSheetFile.InputFilePath,
                                            UserAssignedBarcode = bubbleSheet.Id.ToString(),
                                            RelatedImage = sub.OutputFileName //bubbleSheetFile.OutputFileName
                                        });
                }
            }
            BubbleSheetWsHelper.SubmitResultWithBarcodeGenericActSatAPI(lstChangeResultBarcodeRequest);
            return Json(new { success = true, isAllStudentsSelected = model.IsAllStudentsChecked, bubblesheetFileId = model.BubbleSheetFileId });
        }

        public ActionResult GetListBubbelSheetFileSubByBubblesheetFileId(int id)
        {
            var vSubs = bubbleSheetFileService.GetBubbleSheetFileSubByBubbleSheetIdForGenericActSat(id)
                .OrderBy(o => o.PageNumber)
                .ToList();
            if (vSubs.Count > 0)
            {
                var lst = new BubbleSheetFileSubListViewModel();
                foreach (BubbleSheetFileSub bubbleSheetFileSub in vSubs)
                {
                    lst.ListFileSubViewModels.Add(new BubbleSheetFileSubViewModel()
                    {
                        ImageUrl = BubbleSheetWsHelper.GetTestImageUrl(bubbleSheetFileSub.OutputFileName, ConfigurationManager.AppSettings["ApiKey"]),
                        SubFileName = bubbleSheetFileSub.InputFileName
                    });
                }
                return PartialView("_DisplaySubFile", lst);
            }
            return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult CreateStudentAndAssignmentGenericSheetForActSat(GenericSheetNewStudentViewModel model)
        {
            model.SetValidator(_genericSheetNewStudentViewModel);
            if (!model.IsValid)
            {
                return Json(new { success = false, ErrorList = model.ValidationErrors });
            }
            var vStudent = CreateAndAssignStudentGeneric(model);
            
            if (vStudent == null || vStudent.Id <= 0)
            {
                return new HttpNotFoundResult("Can not create Student");
            }
            var studentId = vStudent.Id;
            var bubbleSheetFile = bubbleSheetFileService.GetBubbleSheetFileById(model.BubbleSheetFileId);
            if (bubbleSheetFile == null)
            {
                return new HttpNotFoundResult("No such bubble sheet file");
            }
            bubbleSheetFile.IsUnmappedGeneric = true;
            bubbleSheetFileService.SaveBubbleSheetFile(bubbleSheetFile);

            var bubbleSheet = bubbleSheetService.CopyBubbleSheetForStudent(bubbleSheetFile.BubbleSheetId, studentId, model.ClassId.GetValueOrDefault());
            if (bubbleSheet == null)
            {
                return new HttpNotFoundResult("No such student");
            }
            //\Save BubbleSheetMap
            var obj = new BubbleSheetGenericMap()
            {
                GenericBubbleSheetID = bubbleSheetFile.BubbleSheetId,
                StudentBubbleSheetID = bubbleSheet.Id,
                TestIndex = bubbleSheetFile.GenericTestIndex
            };
            bubbleSheetFileService.SaveBubbleSheetMap(obj);

            var lstChangeResultBarcodeRequest = new List<ChangeResultBarcodeRequest>();

            var lstBubbleSheetFileSub = bubbleSheetFileService.GetBubbleSheetFileSubByBubbleSheetIdForGenericActSat(bubbleSheetFile.BubbleSheetFileId).ToList();

            if (lstBubbleSheetFileSub.Count > 0)
            {
                foreach (var sub in lstBubbleSheetFileSub)
                {

                    lstChangeResultBarcodeRequest.Add(new ChangeResultBarcodeRequest
                    {
                        Barcode = sub.InputFilePath, //bubbleSheetFile.InputFilePath,
                        UserAssignedBarcode = bubbleSheet.Id.ToString(),
                        RelatedImage = sub.OutputFileName //bubbleSheetFile.OutputFileName
                    });
                }
            }
            BubbleSheetWsHelper.SubmitResultWithBarcodeGenericActSatAPI(lstChangeResultBarcodeRequest);
            return Json(new { success = true, studentName = vStudent.LastName + ", " + vStudent.FirstName, bubblesheetFileId = model.BubbleSheetFileId });
        }

        public ActionResult AddNewStudentForGeneric(int id, int? classId)
        {
            var vGengericFileInfor = bubbleSheetFileService.GetGenericBubbleSheetInfor(id);
            if (vGengericFileInfor != null)
            {
                if (!vGengericFileInfor.ClassID.HasValue || vGengericFileInfor.ClassID == 0 && classId.HasValue)
                {
                    vGengericFileInfor.ClassID = classId.GetValueOrDefault();
                    var vClass = _classService.GetClassById(classId.Value);
                    if (vClass != null)
                    {
                        vGengericFileInfor.ClassName = vClass.Name;
                    }
                }

                var obj = new GenericSheetNewStudentViewModel()
                {
                    BubbleSheetFileId = vGengericFileInfor.BubbleSheetFileId,
                    SchoolId = vGengericFileInfor.SchoolID,
                    SchoolName = vGengericFileInfor.SchoolName,
                    ClassId = vGengericFileInfor.ClassID,
                    ClassName = vGengericFileInfor.ClassName,
                    DistrictId = vGengericFileInfor.DistrictId,
                    FirstName = vGengericFileInfor.FirstName,
                    LastName = vGengericFileInfor.LastName,
                    StudentLocalId = vGengericFileInfor.StudentCode
                };
                obj.Genders = _genderService.GetAllGenders().Select(x =>
                                new SelectListItem
                                {
                                    Text = x.Name,
                                    Value = x.GenderID.ToString(CultureInfo.InvariantCulture)
                                }).ToList();
                obj.Races = _raceService.GetRacesByDistrictID(vGengericFileInfor.DistrictId).Select(
                            x => new SelectListItem {Text = x.Name, Value = x.Id.ToString(CultureInfo.InvariantCulture)})
                        .ToList();
                obj.Grades = _gradeService.GetGradesByDistrictId(vGengericFileInfor.DistrictId)
                        .Select(x => new SelectListItem {Text = x.Name, Value = x.GradeID.ToString()}).ToList();

                return PartialView("_AddNewStudentForGeneric", obj);
            }
            return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        }

        private Student CreateAndAssignStudentGeneric(GenericSheetNewStudentViewModel model)
        {
            var newStudent = new Student
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                MiddleName = model.MiddleName,
                RaceId = _raceService.GetRaceForCreateStudent(model.RaceId, model.DistrictId),
                Code = model.StudentLocalId,
                AltCode = model.StudentStateId,
                GenderId = model.GenderId,
                Password = string.IsNullOrEmpty(model.Password) ? model.StudentLocalId : model.Password,
                CurrentGradeId = model.GradeId,
                AdminSchoolId = model.SchoolId,
                Status = 1,
                DistrictId = model.DistrictId
            };
            _studentService.Save(newStudent);
            if (newStudent.Id > 0)
            {
                //Assign Student to class
                var v = new ClassStudent()
                {
                    ClassId = model.ClassId.Value,
                    StudentId = newStudent.Id,
                    Active = true,
                    Code = string.Empty
                };
                classStudentService.SaveClassStudent(v);
                return newStudent;
            }
            return null;
        }
        
        private bool CheckOnlyOneBubbleSheetFileByBubbleSheetFileId(int Id, out string strImg)
        {
            strImg = string.Empty;
            var vSubs = bubbleSheetFileService.GetBubbleSheetFileSubs(Id).OrderBy(x => x.PageNumber).ToList();
            if (vSubs.Count > 0)
            {
                strImg = BubbleSheetWsHelper.GetTestImageUrl(vSubs.First().OutputFileName,
                    ConfigurationManager.AppSettings["ApiKey"]);

                return vSubs.Count == 1;
            }
            var bubblesheetFile = bubbleSheetFileService.GetBubbleSheetFileById(Id);
            if (bubblesheetFile != null)
            {
                strImg = BubbleSheetWsHelper.GetTestImageUrl(bubblesheetFile.OutputFileName,
                     ConfigurationManager.AppSettings["ApiKey"]);
                return true;
            }
            return false;
        }

        private VirtualTestData GetVirtualTestByBubbleSheetTicketId(string ticket)
        {
            var bubbleSheetId = bubbleSheetService.GetBubbleSheetByTicket(ticket).FirstOrDefault()?.TestId ?? 0;
            return virtualTestService.GetVirtualTestById(bubbleSheetId);
        }
    }
}
