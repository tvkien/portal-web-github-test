using System;
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class BubbleSheetProcessingReadResultRepository : IRepository<BubbleSheetProcessingReadResult>
    {
        private readonly Table<BubbleSheetProcessingReadResultEntity> table; 

        public BubbleSheetProcessingReadResultRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = BubbleSheetDataContext.Get(connectionString).GetTable<BubbleSheetProcessingReadResultEntity>();
            Mapper.CreateMap<BubbleSheetProcessingReadResult, BubbleSheetErrorEntity>();
        }

        public void Delete(BubbleSheetProcessingReadResult item)
        {
            throw new NotImplementedException();
        }

        public void Save(BubbleSheetProcessingReadResult item)
        {
            throw new NotImplementedException();
        }

        public IQueryable<BubbleSheetProcessingReadResult> Select()
        {
            return table.Select(x => new BubbleSheetProcessingReadResult {
                ReadResultId = x.ReadResultId,
                OutputFile = x.OutputFile,
                ListQuestion = x.ListQuestions,
                UserId = x.UserId,
                DistrictId = x.DistrictId,
                Barcode1 = x.Barcode1,
                Barcode2 = x.Barcode2,
                InputPath = x.InputPath,
                InputFileName = x.InputFileName,
                Dpi = x.Dpi,
                PageNumber = x.PageNumber,
                FileDisposition = x.FileDisposition,
                UrlSafeOutputFile = x.UrlSafeOutputFile,
                ProcessingTime = x.ProcessingTime,
                CompletedTime = x.CompletedTime,
                RosterPosition = x.RosterPosition,
                QuestionCount = x.QuestionCount,
                IsRoster = x.IsRoster,
                SheetPageId = x.SheetPageId,
                TestType = x.TestType,
                ACTPageIndex = x.ACTPageIndex,
                RawResult = x.RawResult,
                SecondValidationStatus = x.SecondValidationStatus
            });
        }
    }
}