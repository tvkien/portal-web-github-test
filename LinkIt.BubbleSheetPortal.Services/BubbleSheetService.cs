using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleService.Models.Test;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Models.BubbleSheetReview;
using Newtonsoft.Json;
using LinkIt.BubbleSheetPortal.Data.Repositories;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class BubbleSheetService
    {
        private readonly IBubbleSheetRepository repository;
        private readonly ITestResultRepository _testResultRepository;
        private readonly SchoolService schoolService;
        private readonly BubbleSheetListService bubbleSheetListService;
        private readonly ClassStudentService classStudentService;
        private readonly ClassService classService;
        private readonly IBubbleSheetPageRepository bubbleSheetPageRepository;
        private readonly BubbleSheetFileRepository _bbsFileRepository;
        private readonly IReadOnlyRepository<BubbleSheetProcessingReadResult> _bubbleSheetProcessingReadResultRepository;
        private readonly IReadOnlyRepository<BubbleSheetProcessingRequestSheet> _bubbleSheetProcessingRequestSheetRepository;
        private readonly BubbleSheetFileService _bubbleSheetFileService;

        public BubbleSheetService(IBubbleSheetRepository repository,
            SchoolService schoolService,
            BubbleSheetListService bubbleSheetListService,
            ClassStudentService classStudentService,
            ClassService classService,
            IBubbleSheetPageRepository bubbleSheetPageRepository,
            BubbleSheetFileRepository bbsFileRepository,
            IReadOnlyRepository<BubbleSheetProcessingReadResult> bubbleSheetProcessingReadResultRepository,
            IReadOnlyRepository<BubbleSheetProcessingRequestSheet> bubbleSheetProcessingRequestSheetRepository,
            ITestResultRepository testResultRepository,
            BubbleSheetFileService bubbleSheetFileService)
        {
            this.repository = repository;
            this.schoolService = schoolService;
            this.bubbleSheetListService = bubbleSheetListService;
            this.classStudentService = classStudentService;
            this.classService = classService;
            this.bubbleSheetPageRepository = bubbleSheetPageRepository;
            _bbsFileRepository = bbsFileRepository;
            _bubbleSheetProcessingReadResultRepository = bubbleSheetProcessingReadResultRepository;
            _bubbleSheetProcessingRequestSheetRepository = bubbleSheetProcessingRequestSheetRepository;
            _testResultRepository = testResultRepository;
            _bubbleSheetFileService = bubbleSheetFileService;
        }

        public BubbleSheet GetBubbleSheetById(int? bubbleSheetId)
        {
            return repository.Select().FirstOrDefault(x => x.Id.Equals(bubbleSheetId));
        }

        public BubbleSheetProcessingRequestSheet GetBubbleSheetProcessingRequestSheetByTicket(string ticket)
        {
            return _bubbleSheetProcessingRequestSheetRepository.Select().FirstOrDefault(x => x.SheetJobTicket.Equals(ticket));
        }

        public IQueryable<BubbleSheet> GetBubbleSheetByTicket(string ticket)
        {
            return repository.Select().Where(x => x.Ticket.Equals(ticket));
        }

        public BubbleSheet CopyBubbleSheetForStudent(int bubbleSheetId, int studentId, int classId = 0)
        {
            var bubbleSheet = repository.Select().FirstOrDefault(x => x.Id.Equals(bubbleSheetId));
            if (bubbleSheet == null)
            {
                throw new ArgumentException("No such bubblesheet.");
            }

            //check existed assigned bubble sheets
            var existedBubbleSheet = repository.Select().FirstOrDefault(x => x.ClassId == bubbleSheet.ClassId
                                                                             && x.StudentId == studentId
                                                                             && x.TestId == bubbleSheet.TestId
                                                                             && x.IsGenericSheet == true
                                                                             && x.BubbleSheetCode == "na");
            BubbleSheet newBubbleSheet;
            if (existedBubbleSheet != null)
            {
                newBubbleSheet = existedBubbleSheet;
                newBubbleSheet.SubmittedDate = bubbleSheet.SubmittedDate;
            }
            else
                newBubbleSheet = new BubbleSheet
                {
                    BubbleSheetCode = "na",
                    ClassId = bubbleSheet.ClassId,
                    SchoolId = bubbleSheet.SchoolId,
                    StudentId = studentId,
                    TestId = bubbleSheet.TestId,
                    BubbleSize = bubbleSheet.BubbleSize,
                    DistrictTermId = bubbleSheet.DistrictTermId,
                    CreatedByUserId = bubbleSheet.CreatedByUserId,
                    UserId = bubbleSheet.UserId,
                    SubmittedDate = bubbleSheet.SubmittedDate,
                    IsGenericSheet = true,
                    IsManualEntry = false,
                    IsArchived = false,
                    TeacherId = bubbleSheet.TeacherId,
                    Ticket = bubbleSheet.Ticket,
                    TestExtract = bubbleSheet.TestExtract
                };

            if (newBubbleSheet.ClassId == 0)
            {
                newBubbleSheet.ClassId = classId;
                var cls = classService.GetClassById(classId);
                // Re-Assign UserId of bubblesheet to primary teacher of assigned class
                if (cls != null && cls.UserId.HasValue)
                    newBubbleSheet.UserId = cls.UserId;
            }

            repository.Save(newBubbleSheet);
            return newBubbleSheet;
        }

        public IQueryable<BubbleSheet> GetBubbleSheetsByTicketAndClass(string ticket, int classId)
        {
            return repository.Select().Where(x => x.Ticket.Equals(ticket) && x.ClassId.Equals(classId));
        }

        public IQueryable<BubbleSheet> GetBubbleSheetsByClass(int classId)
        {
            return repository.Select().Where(x => x.ClassId.Equals(classId));
        }

        public IQueryable<BubbleSheet> GetBubbleSheetsByClassIdList(List<int> listClassIds)
        {
            return repository.Select().Where(x => listClassIds.Contains(x.ClassId.GetValueOrDefault()));
        }

        public void Save(BubbleSheet bubbleSheet)
        {
            repository.Save(bubbleSheet);
        }

        public void Save(List<BubbleSheet> bubbleSheets)
        {
            repository.Save(bubbleSheets);
        }

        public RequestSheetResultResponse HandleRequestSheetResult(string ticket, string apiKey)
        {
            try
            {
                var requestResult = new RequestSheetResult
                {
                    Ticket = ticket,
                    ApiKey = apiKey
                };

                if (requestResult.Ticket.Equals(string.Empty))
                {
                    return new RequestSheetResultResponse();
                }
                return null;
            }
            catch (Exception)
            {
                return new RequestSheetResultResponse();
            }
        }

        public IEnumerable<ClassStudent> GetStudentList(int classId)
        {
            return classStudentService.GetClassStudentsByClassId(classId);
        }

        public IQueryable<BubbleSheetListItem> GetBubbleSheetList(User user)
        {
            return bubbleSheetListService.GetBubbleSheetList(user);
        }

        public IQueryable<BubbleSheetListItem> GetBubbleSheetList(int userId)
        {
            var list = new List<BubbleSheetListItem>();
            var listItems = bubbleSheetListService.GetBubbleSheetListByUserId(userId).OrderBy(x => x.DateCreated).ToList();
            foreach (var item in listItems)
            {
                if (!list.Any(o => o.Ticket.Equals(item.Ticket)))
                {
                    list.Add(item);
                }
            }
            return list.AsQueryable();
        }

        public int GetDistrictIdFromSchoolBySchoolId(int? schoolId)
        {
            return schoolService.GetSchoolById(schoolId.GetValueOrDefault()).DistrictId;
        }

        public IEnumerable<ClassStudent> GetStudentsByStudentIds(IEnumerable<string> studentIds)
        {
            return studentIds.Select(studentId => classStudentService.GetClassStudentByStudentId(Convert.ToInt32(studentId)));
        }

        public void UpdateBubbleSheetsWithTicket(IEnumerable<BubbleSheet> bubbleSheets, string ticket)
        {
            repository.UpdateBubbleSheetsWithTicket(bubbleSheets, ticket);
        }

        public void ToggleArchivedSheets(IEnumerable<BubbleSheet> sheets)
        {
            repository.ToggleArchiveBubbleSheets(sheets);
        }

        public IEnumerable<string> SearchBubbleSheetPageStartWithId(string id)
        {
            return bubbleSheetPageRepository.SearchSheetPageIdText(id, take: 10);
        }

        public ReadResult GetBubbleSheetProcessingReadResult(string inputFilePath, string urlSafeOutputFileName)
        {
            var result = _bbsFileRepository.GetBubbleSheetProcessingReadResult(inputFilePath, urlSafeOutputFileName);

            ReadResult readResult = null;
            if (result != null)
            {
                readResult = new ReadResult
                {
                    Barcode1 = result.Barcode1,
                    Barcode2 = result.Barcode2,
                    CompletedTime = result.CompletedTime,
                    DistrictId = result.DistrictId,
                    Dpi = Double.Parse(result.Dpi.ToString()),
                    InputFileName = result.InputFileName,
                    InputPath = result.InputPath,
                    IsRoster = result.IsRoster,
                    OutputFile = result.OutputFile,
                    PageNumber = result.PageNumber,
                    RosterPosition = result.RosterPosition,
                    UserId = result.UserId,
                    Questions = JsonConvert.DeserializeObject<List<ReadResultItem>>(result.ListQuestion),
                    ProcessingTime = new TimeSpan(0, 0, 0, 0, result.ProcessingTime)
                };
            }
            return readResult;
        }

        public BubbleSheet GetBubbleSheetByStudentIdAndVirtualTestId(int studentId, int virtualTestId, int classId)
        {
            var testResult = _testResultRepository.Select()
                .FirstOrDefault(x => x.StudentId == studentId && x.VirtualTestId == virtualTestId && (classId == 0 || x.ClassId == classId));

            if(testResult != null && testResult.BubbleSheetId > 0)
            {
                return repository.Select().FirstOrDefault(x => x.Id == testResult.BubbleSheetId);
            }

            return null;
        }

        public List<string> GetScrapPaperImageNames(int studentId, int virtualTestId, int classId)
        {
            var listFileNames = new List<string>();
            var bbs = GetBubbleSheetByStudentIdAndVirtualTestId(studentId, virtualTestId, classId);

            if (bbs == null)
                return listFileNames;

            var bbsRequestSheet = GetBubbleSheetProcessingRequestSheetByTicket(bbs.Ticket);
            var lastestBBSFiles = _bubbleSheetFileService.GetLastestBubbleSheetFileById(bbs.Id, studentId).OrderBy(o => o.PageNumber).ToList();

            var numberScrapPaperPages = 0;
            if (bbsRequestSheet != null)
            {
                numberScrapPaperPages = bbsRequestSheet.NumberOfGraphExtraPages + bbsRequestSheet.NumberOfLinedExtraPages + bbsRequestSheet.NumberOfPlainExtraPages;
                if (numberScrapPaperPages == 0)
                {
                    var aritfactFileSub = lastestBBSFiles.FirstOrDefault(o => o.PageNumber == -1);
                    if (aritfactFileSub != null)
                    {
                        listFileNames.Add(aritfactFileSub.OutputFileName);
                    }
                        
                    return listFileNames;
                }
            }

            var bbsSheetPages = bubbleSheetPageRepository.Select().Where(x => x.Ticket.Equals(bbs.Ticket)).ToList();
            var maxPageNumberSub = bbsSheetPages.Max(x => x.PageNumberSub);
            var scrapPaperFromPage = maxPageNumberSub - numberScrapPaperPages + 1;

            if (lastestBBSFiles.Count > 0)
            {
                foreach (BubbleSheetFileSub bubbleSheetFileSub in lastestBBSFiles.Where(o => o.PageNumber != -1))
                {
                    if(bubbleSheetFileSub.PageNumber >= scrapPaperFromPage)
                    {
                        listFileNames.Add(bubbleSheetFileSub.OutputFileName);
                    }
                }

                var aritfactFileSub = lastestBBSFiles.FirstOrDefault(o => o.PageNumber == -1);
                if (aritfactFileSub != null)
                {
                    listFileNames.Add(aritfactFileSub.OutputFileName);
                }
            }

            return listFileNames;
        }

    }
}
