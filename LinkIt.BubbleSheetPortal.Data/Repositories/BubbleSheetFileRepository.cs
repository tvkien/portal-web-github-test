using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class BubbleSheetFileRepository : IBubbleSheetFileRepository
    {
        private readonly Table<BubbleSheetFileEntity> table;
        private readonly Table<BubbleSheetFileTicketViewNew> bubbleSheetFileTicketView;
        private readonly BubbleSheetDataContext _dbContext;

        public BubbleSheetFileRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = BubbleSheetDataContext.Get(connectionString).GetTable<BubbleSheetFileEntity>();
            bubbleSheetFileTicketView = BubbleSheetDataContext.Get(connectionString).GetTable<BubbleSheetFileTicketViewNew>();
            _dbContext = BubbleSheetDataContext.Get(connectionString);
        }

        public IQueryable<BubbleSheetFile> Select()
        {
            return table.Select(x => new BubbleSheetFile
                {
                    BubbleSheetFileId = x.BubbleSheetFileID,
                    BubbleSheetId = x.BubbleSheetID,
                    OutputFileName = x.OutputFileName,
                    InputFilePath = x.InputFilePath,
                    InputFileName = x.InputFileName,
                    Barcode1 = x.Barcode1,
                    Barcode2 = x.Barcode2,
                    DistrictId = x.DistrictID,
                    UserId = x.UserID,
                    RosterPosition = x.RosterPosition,
                    ResultCount = x.ResultCount,
                    Resolution = x.Resolution,
                    Date = x.Date,
                    FileDisposition = x.FileDisposition,
                    PageNumber = x.PageNumber,
                    ProcessingTime = x.ProcessingTime,
                    ResultString = x.ResultString,
                    CreatedDate = x.CreatedDate,
                    IsConfirmed = x.IsConfirmed,
                    IsUnmappedGeneric = x.IsUnmappedGeneric,
                    GenericTestIndex = x.GenericTestIndex.GetValueOrDefault()
                });
        }

        public IQueryable<BubbleSheetFile> SelectFromBubbleSheetFileTicketView()
        {
            return bubbleSheetFileTicketView.Select(x => new BubbleSheetFile
                {
                    BubbleSheetFileId = x.BubbleSheetFileID,
                    BubbleSheetId = x.BubbleSheetID,
                    OutputFileName = x.OutputFileName,
                    InputFilePath = x.InputFilePath,
                    InputFileName = x.InputFileName,
                    Barcode1 = x.Barcode1,
                    UserId = x.UserID,
                    RosterPosition = x.RosterPosition,
                    ResultCount = x.ResultCount,
                    Date = x.Date,
                    ResultString = x.ResultString,
                    IsConfirmed = x.IsConfirmed,
                    IsUnmappedGeneric = x.IsUnmappedGeneric,
                    Ticket = x.Ticket,
                    ClassId = x.ClassID,
                    IsGenericSheet = x.IsGenericSheet ?? false
                });
        }

        public void Save(BubbleSheetFile item)
        {
            var entity = table.FirstOrDefault(x => x.BubbleSheetFileID.Equals(item.BubbleSheetFileId));

            if (entity.IsNull())
            {
                entity = new BubbleSheetFileEntity();
                table.InsertOnSubmit(entity);
            }

            BindBubbleSheetFileEntityToItem(entity, item);
            table.Context.SubmitChanges();
            item.BubbleSheetFileId = entity.BubbleSheetFileID;
        }

        public void Save(List<BubbleSheetFile> items)
        {
            var files = new List<BubbleSheetFileEntity>();
            foreach (var item in items)
            {
                var entity = new BubbleSheetFileEntity();
                table.InsertOnSubmit(entity);
                BindBubbleSheetFileEntityToItem(entity, item);
                files.Add(entity);
            }
            table.InsertAllOnSubmit(files);
            table.Context.SubmitChanges();
        }

        public void Delete(BubbleSheetFile item)
        {
            var entity = table.FirstOrDefault(x => x.BubbleSheetFileID.Equals(item.BubbleSheetFileId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public int SATGetDistinctPageCount(string ticket, int classID, int studentID)
        {
            var queryData = _dbContext.SATGetDistinctPageCount(ticket, classID, studentID).Select(x => x.Column1).FirstOrDefault();
            return queryData ?? 0;
        }

        private void BindBubbleSheetFileEntityToItem(BubbleSheetFileEntity entity, BubbleSheetFile item)
        {
            entity.BubbleSheetID = item.BubbleSheetId;
            entity.Date = item.Date;
            entity.Barcode1 = item.Barcode1;
            entity.Barcode2 = item.Barcode2;
            entity.InputFilePath = item.InputFilePath;
            entity.InputFileName = item.InputFileName;
            entity.Resolution = item.Resolution;
            entity.PageNumber = item.PageNumber;
            entity.FileDisposition = item.FileDisposition;
            entity.OutputFileName = item.OutputFileName;
            entity.ProcessingTime = item.ProcessingTime;
            entity.RosterPosition = item.RosterPosition;
            entity.ResultCount = item.ResultCount;
            entity.ResultString = item.ResultString;
            entity.UserID = item.UserId;
            entity.DistrictID = item.DistrictId;
            entity.IsConfirmed = item.IsConfirmed;
            entity.IsUnmappedGeneric = item.IsUnmappedGeneric;
            entity.CreatedDate = item.CreatedDate;
        }

        public GenericBubbleSheet GetGenericFileInfor(int bubblesheetFileId)
        {
            var obj = _dbContext.GetGenericFileInfor(bubblesheetFileId).FirstOrDefault();
            if (obj != null)
                return new GenericBubbleSheet()
                {
                    BubbleSheetFileId = obj.bubblesheetfileId,
                    BubbleSheetId = obj.bubblesheetId,
                    ClassID = obj.ClassId,
                    ClassName = obj.ClassName,
                    SchoolID = obj.SchoolId,
                    SchoolName = obj.SchoolName,
                    FirstName = obj.FirstName,
                    LastName = obj.LastName,
                    StudentCode = obj.StudentCode,
                    DistrictId = obj.DistrictId
                };
            return null;
        }

        public IQueryable<BubbleSheetFileSub> GetBubbleSheetLatestFille(string ticket, int classId)
        {
            var result = _dbContext.GetBubbleSheetLatestFile(ticket, classId)
                .Select(m => new BubbleSheetFileSub {
                    StudentID = m.StudentID,
                    BubbleSheetFileId = m.BubbleSheetFileID,
                    BubblSheetFileSubId = m.BubbleSheetFileSubID,
                    InputFileName = m.InputFileName,
                    OutputFileName = m.OutputFileName,
                    PageNumber = m.PageNumber,
                    PageType = m.PageType
                }).AsQueryable();
            return result;
        }

        public void SaveBubbleSheetFileCorrections(List<int> ids)
        {
            var strIds = string.Join(",", ids);
            _dbContext.SaveBubbleSheetFileCorrections(strIds);
        }

        public BubbleSheetProcessingReadResult GetBubbleSheetProcessingReadResult(string inputPath, string urlSafeOutputFile)
        {
            var result = _dbContext.GetBubbleSheetProcessingReadResult(inputPath, urlSafeOutputFile)
                .Select(m=> new BubbleSheetProcessingReadResult {
                    Barcode1 = m.Barcode1,
                    Barcode2 = m.Barcode2,
                    CompletedTime = m.CompletedTime,
                    DistrictId = m.DistrictId,
                    Dpi = m.Dpi,
                    InputFileName = m.InputFileName,
                    InputPath = m.InputPath,
                    IsRoster = m.IsRoster,
                    OutputFile = m.OutputFile,
                    PageNumber = m.PageNumber,
                    RosterPosition = m.RosterPosition,
                    UserId = m.UserId,
                    ListQuestion = m.ListQuestions,
                    ProcessingTime = m.ProcessingTime
                }).FirstOrDefault();
            return result;
        }
    }
}
