using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class BubbleSheetFileService
    {
        private readonly IBubbleSheetFileRepository repository;
        private readonly IRepository<BubbleSheet> bubbleSheetRepository;
        private readonly IReadOnlyRepository<BubbleSheetFileSub> _bubbleSheetFileSubRepository;
        private readonly IRepository<VirtualTestData> virtualTestRepository;
        private readonly IRepository<BubbleSheetGenericMap> _bubbleSheetGenericMap;
        private readonly StudentRosterService studentRosterService;

        public BubbleSheetFileService(IBubbleSheetFileRepository repository,
            IRepository<BubbleSheet> bubbleSheetRepository,
            IReadOnlyRepository<BubbleSheetFileSub> bubbleSheetFileSubRepository,
            IRepository<VirtualTestData> virtualTestRepository, StudentRosterService studentRosterService,
            IRepository<BubbleSheetGenericMap> bubbleSheetGenericMap)
        {
            this.repository = repository;
            this.bubbleSheetRepository = bubbleSheetRepository;
            _bubbleSheetFileSubRepository = bubbleSheetFileSubRepository;
            this.virtualTestRepository = virtualTestRepository;
            _bubbleSheetGenericMap = bubbleSheetGenericMap;
            this.studentRosterService = studentRosterService;
        }

        public BubbleSheetFile GetBubbleSheetFileById(int bubbleSheetFileId)
        {
            return repository.Select().FirstOrDefault(x => x.BubbleSheetFileId.Equals(bubbleSheetFileId));
        }

        public int GetBubbleSheetFileCount(string fileName, DateTime uploadedDate, int userId)
        {
            var bubbleSheetFile = repository.Select().Where(x => x.InputFileName.Equals(fileName)
                && x.UserId.Equals(userId) && x.RosterPosition != -1).Select(x => x.BubbleSheetFileId);
            var count = bubbleSheetFile.Count();
            var bubbleSheetFileSub =
                GetBubbleSheetFileSubByFileName(fileName);
            var subCount = bubbleSheetFileSub.Select(x => x.OutputFileName).Distinct().Count();
            if (subCount > count)
            {
                count = subCount;
            }

            return count;
        }

        public int GetBubbleSheetTestCount(string fileName, int userId)
        {

            var testCount = GetBubbleSheetTest(fileName, userId).Select(x => x.BubbleSheetId).Distinct().Count();
            return testCount;
        }

        public List<BubbleSheetFile> GetBubbleSheetTest(string fileName, int userId)
        {
            var testList = repository.Select()
                .Where(x => x.InputFileName.Equals(fileName)
                    && x.UserId.Equals(userId) && x.RosterPosition != -1).ToList();

            if (!testList.Any())
            {
                var subList =
                    GetBubbleSheetFileSubByFileName(fileName)
                        .Where(x => testList.Select(y => y.BubbleSheetFileId).Contains(x.BubbleSheetFileId))
                        .ToList();
                if (subList.Any())
                {
                    var bubbleSheetFileIds = subList.Select(x => x.BubbleSheetFileId).ToList();
                    testList = repository.Select().Where(x => bubbleSheetFileIds.Contains(x.BubbleSheetFileId)).ToList();
                }
            }

            var nonGenericBubbleSheet =
                bubbleSheetRepository.Select().Where(x => testList.Select(y => y.BubbleSheetId).ToList().Contains(x.Id)
                                                          &&
                                                          ((x.IsGenericSheet == false) ||
                                                           (x.IsGenericSheet && x.StudentId.HasValue &&
                                                            x.StudentId.Value > 0))).Select(x => x.Id).ToList();
            testList = testList.Where(x => nonGenericBubbleSheet.Contains(x.BubbleSheetId)).ToList();

            return testList;
        }

        public IQueryable<BubbleSheetFileSub> GetBubbleSheetFileSubByFileName(string fileName)
        {
            var testList = _bubbleSheetFileSubRepository.Select().Where(x => x.InputFileName.Equals(fileName));
            return testList;
        }

        public BubbleSheetFile InitializeNewBubbleSheetFile(int districtId, int bubbleSheetId, User currentUser)
        {
            var bubbleSheetFile = new BubbleSheetFile
            {
                BubbleSheetId = bubbleSheetId,
                Date = DateTime.UtcNow.Date,
                CreatedDate = DateTime.UtcNow,
                Barcode1 = string.Format("10060000{0}", bubbleSheetId),
                Barcode2 = GetBarcode2(),
                InputFilePath = "temp",
                InputFileName = "temp",
                Resolution = string.Empty,
                PageNumber = 1,
                FileDisposition = "Created",
                OutputFileName = "temp",
                RosterPosition = 0,
                ResultString = string.Empty,
                UserId = currentUser.Id,
                DistrictId = districtId
            };

            return bubbleSheetFile;
        }

        private string GetBarcode2()
        {
            var datetime = DateTime.UtcNow;
            return string.Format("10001{0}", datetime.ToString("yyMMdd"));
        }

        public void RemoveRosterPositionError(BubbleSheetFile bubbleSheetFile, int rosterPostion)
        {
            bubbleSheetFile.RosterPosition = rosterPostion;
            repository.Save(bubbleSheetFile);
        }

        public string GetInputFilePathByBubbleSheetId(int bubbleSheetId)
        {
            var bubbleSheet = repository.Select().FirstOrDefault(x => x.BubbleSheetId.Equals(bubbleSheetId) && x.InputFilePath != "");

            if (bubbleSheet.IsNull())
            {
                return string.Empty;
            }

            return bubbleSheet.InputFilePath;
        }

        public void SaveBubbleSheetFile(BubbleSheetFile bubbleSheetFile)
        {
            repository.Save(bubbleSheetFile);
        }

        public void SaveBubbleSheetFileCorrection(int bubbleSheetFileId)
        {
            var bubbleSheetFile = repository.Select().FirstOrDefault(x => x.BubbleSheetFileId.Equals(bubbleSheetFileId) && x.IsConfirmed.Equals(false));
            if (bubbleSheetFile == null) return;
            bubbleSheetFile.IsConfirmed = true;
            SaveBubbleSheetFile(bubbleSheetFile);
        }

        public int GetNewlyCreatedBubbleSheetFileId(BubbleSheetFile newFile)
        {
            var bubbleSheetFile = repository.Select().FirstOrDefault(x => x.FileDisposition.Equals("Created")
                                                                    && x.BubbleSheetId.Equals(newFile.BubbleSheetId)
                                                                    && x.IsConfirmed.Equals(false)
                                                                    && x.RosterPosition.Equals(newFile.RosterPosition));
            return bubbleSheetFile.IsNull() ? 0 : bubbleSheetFile.BubbleSheetFileId;
        }

        public bool IsFileGenericWithNoMappings(string ticket, int classId)
        {
            var bubbleSheets =
                repository.SelectFromBubbleSheetFileTicketView()
                    .Where(x => x.Ticket.Equals(ticket) && (classId == 0 || x.ClassId == classId));

            if (bubbleSheets.Count().Equals(0))
            {
                return
                    bubbleSheetRepository.Select()
                        .FirstOrDefault(x => x.Ticket.Equals(ticket) && x.IsGenericSheet && (classId == 0 || x.ClassId == classId))
                        .IsNotNull();
            }

            return bubbleSheets.All(x => x.IsGenericSheet && !x.IsUnmappedGeneric);
        }

        public bool HasGenericSheets(string ticket)
        {
            return repository.SelectFromBubbleSheetFileTicketView().Any(x => x.Ticket.Equals(ticket) && x.IsGenericSheet);
        }

        //\
        /// <summary>
        /// Get BubbleSheetFileSub by bubblesheetFileID
        /// </summary>
        /// <param name="bubbleSheetFileId"></param>
        /// <returns></returns>
        public IQueryable<BubbleSheetFileSub> GetBubbleSheetFileSubs(int bubbleSheetFileId)
        {
            if (bubbleSheetFileId > 0)
            {
                return _bubbleSheetFileSubRepository.Select().Where(o => o.BubbleSheetFileId == bubbleSheetFileId);
            }
            return null;
        }

        public IQueryable<BubbleSheetFileSub> GetBubbleSheetFileSubsByListId(List<int> listBubbleSheetFileID)
        {
            return _bubbleSheetFileSubRepository.Select().Where(o => listBubbleSheetFileID.Contains(o.BubbleSheetFileId));
        }

        public List<BubbleSheetFileSub> GetLastestBubbleSheetFileSubByBubbleSheetId(int bubbleSheetId)
        {
            var lstResult = new List<BubbleSheetFileSub>();
            var lst = repository.Select().Where(x => x.BubbleSheetId == bubbleSheetId)
                .OrderByDescending(o => o.BubbleSheetFileId).ToList();
            if (lst.Count > 0)
            {
                var artifactFile = lst.FirstOrDefault(x => x.PageNumber == -1);
                BubbleSheetFileSub artifactFileSub = null;
                if (artifactFile != null)
                {
                    artifactFileSub = _bubbleSheetFileSubRepository.Select().FirstOrDefault(x => x.BubbleSheetFileId == artifactFile.BubbleSheetFileId);
                }

                foreach (BubbleSheetFile bsf in lst)
                {
                    var lstbsfSub = _bubbleSheetFileSubRepository.Select()
                        .Where(o => o.BubbleSheetFileId == bsf.BubbleSheetFileId).ToList();
                    var bbsLstbsfSub = lstbsfSub.Where(x => x.PageNumber != -1).ToList();
                    if (bbsLstbsfSub.Count > 0)
                    {
                        foreach (BubbleSheetFileSub sub in bbsLstbsfSub)
                        {
                            if (!lstResult.Exists(o => o.PageNumber == sub.PageNumber))
                            {
                                lstResult.Add(sub);
                            }
                        }
                        if (lstResult.Count == 6) //TODO ActReport have 6 pages
                        {
                            if (artifactFileSub != null)
                                lstResult.Add(artifactFileSub);
                            return lstResult;
                        }
                    }
                }

                if (artifactFileSub != null)
                    lstResult.Add(artifactFileSub);
            }
            return lstResult;
        }

        public BubbleSheetFile GetLastesBubbleSheetFile(int bubblesheetId, int rosterPosition)
        {
            if (rosterPosition > 0)
            {
                return repository.Select().Where(x => x.BubbleSheetId == bubblesheetId
                                                     && x.RosterPosition == rosterPosition)
                    .OrderByDescending(o => o.BubbleSheetFileId).FirstOrDefault();
            }
            return repository.Select().Where(x => x.BubbleSheetId == bubblesheetId)
                 .OrderByDescending(o => o.BubbleSheetFileId).FirstOrDefault();
        }

        public List<BubbleSheetFileSub> GetLastestBubbleSheetFileById(int bubbleSheetId, int studentID)
        {
            var bubbleSheet = bubbleSheetRepository.Select().SingleOrDefault(x => x.Id == bubbleSheetId);
            if (bubbleSheet == null) return new List<BubbleSheetFileSub>();

            var isRoster = !string.IsNullOrEmpty(bubbleSheet.StudentIds);
            var lstResult = new List<BubbleSheetFileSub>();
            List<BubbleSheetFile> lst;
            var rosterPosition = 0;
            if (isRoster)
            {
                rosterPosition =
                    studentRosterService.GetStudentRosterPositionByTicketAndStudentId(bubbleSheet.Ticket, studentID,
                        bubbleSheet.ClassId.GetValueOrDefault());
                lst = repository.Select().Where(x => x.BubbleSheetId == bubbleSheetId
                                                     && x.RosterPosition == rosterPosition)
                    .OrderByDescending(o => o.BubbleSheetFileId).ToList();
            }
            else
            {
                lst = repository.Select().Where(x => x.BubbleSheetId == bubbleSheetId)
                .OrderByDescending(o => o.BubbleSheetFileId).ToList();
            }

            var artifactFile = lst.FirstOrDefault(x => x.PageNumber == -1);
            BubbleSheetFileSub artifactFileSub = null;
            if (artifactFile != null)
            {
                artifactFileSub = _bubbleSheetFileSubRepository.Select().FirstOrDefault(x => x.BubbleSheetFileId == artifactFile.BubbleSheetFileId);
            }

            if (lst.Count > 0)
            {
                foreach (BubbleSheetFile bsf in lst)
                {
                    var lstbsfSub = _bubbleSheetFileSubRepository.Select()
                        .Where(o => o.BubbleSheetFileId == bsf.BubbleSheetFileId)
                        .Where(o => o.PageNumber != -1)
                        .ToList();
                    if (lstbsfSub.Count > 0)
                    {
                        foreach (BubbleSheetFileSub sub in lstbsfSub)
                        {
                            if (isRoster)
                                sub.RosterPosition = rosterPosition;
                            if (!lstResult.Exists(o => o.PageNumber == sub.PageNumber))
                            {
                                lstResult.Add(sub);
                            }
                        }
                        if (lstResult.Count == 6) //TODO ActReport have 6 pages
                        {
                            if (artifactFileSub != null)
                                lstResult.Add(artifactFileSub);
                            return lstResult;
                        }
                    }
                }

                if (artifactFileSub != null)
                    lstResult.Add(artifactFileSub);
            }
            return lstResult;
        }

        public int GetDistinctUploadedFileByTicketAndClassIdAndStudentId(string ticket, int classId, int studentId)
        {
            return repository.SATGetDistinctPageCount(ticket, classId, studentId);
        }

        public List<BubbleSheetFileSub> GetBubbleSheetFileSubByBubbleSheetIdForGenericActSat(int bubbleSheetFileId)
        {
            return _bubbleSheetFileSubRepository.Select().Where(o => o.BubbleSheetFileId == bubbleSheetFileId).ToList();
        }

        public void SaveBubbleSheetMap(BubbleSheetGenericMap obj)
        {
            _bubbleSheetGenericMap.Save(obj);
        }

        public GenericBubbleSheet GetGenericBubbleSheetInfor(int bubblesheetfileId)
        {
            return repository.GetGenericFileInfor(bubblesheetfileId);
        }

        /// <summary>
        /// Get Lastest BubbleSheetFile by BubbleSheetID
        /// </summary>
        /// <param name="bubbleSheetId"></param>
        /// <returns></returns>
        public BubbleSheetFile GetLastestBubbleSheetFileByBubbleSheetId(int bubbleSheetId)
        {
            return
                repository.Select()
                    .Where(o => o.BubbleSheetId == bubbleSheetId)
                    .OrderByDescending(o => o.BubbleSheetFileId)
                    .FirstOrDefault();
        }

        public List<BubbleSheetFileSub> GetBubbleSheetFileSubByBubbleSheetFileId(int id)
        {
            var lstResult = new List<BubbleSheetFileSub>();
            var bubbleSheetFile = repository.Select().FirstOrDefault(o => o.BubbleSheetFileId == id);
            if (bubbleSheetFile == null)
                return lstResult;

            var lstbsfSub = _bubbleSheetFileSubRepository.Select().Where(o => o.BubbleSheetFileId == id).ToList();
            if (lstbsfSub.Count > 0)
            {
                foreach (BubbleSheetFileSub sub in lstbsfSub)
                {
                    if (!lstResult.Exists(o => o.PageNumber == sub.PageNumber))
                    {
                        lstResult.Add(sub);
                    }
                }
            }
            return lstResult;
        }

        public bool IsGenericGroup(string ticket, int classId)
        {
            if (classId == 0)
            {
                var obj = bubbleSheetRepository.Select().First(o => o.Ticket == ticket
                && (o.ClassId == 0 || !o.ClassId.HasValue)
                && o.ClassIds.Length > 0
                && o.IsGenericSheet == true);
                if (obj != null)
                {
                    return true;
                }
            }
            return false;
        }

        public List<BubbleSheetFileSub> GetBubbleSheetLatestFile(string ticket, int classID)
        {
            var result = repository.GetBubbleSheetLatestFille(ticket, classID).ToList();
            return result;
        }

        public bool ValidListBubblesheetIDs(List<int> ids)
        {
            if (ids.Contains(0)) return false;

            var count = bubbleSheetRepository.Select().Where(m => ids.Contains(m.Id)).Count();
            return count >= ids.Count;
        }

        public void SaveBubbleSheetFileCorrections(List<int> bbsfileIDs)
        {
            repository.SaveBubbleSheetFileCorrections(bbsfileIDs);
        }


    }
}
