using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class BubbleSheetErrorService
    {
        private readonly IRepository<BubbleSheetError> repository;
        private readonly IReadOnlyRepository<BubbleSheetError> viewRepository; 

        public BubbleSheetErrorService(IRepository<BubbleSheetError> repository, IReadOnlyRepository<BubbleSheetError> viewRepository)
        {
            this.repository = repository;
            this.viewRepository = viewRepository;
        }

        public BubbleSheetError GetBubbleSheetErrorById(int bubbleSheetErrorId)
        {
            return repository.Select().FirstOrDefault(x => x.BubbleSheetErrorId.Equals(bubbleSheetErrorId) && x.ErrorCode != -1);
        }

        public BubbleSheetError GetBubbleSheetErrorFromViewByIdAndErrorCode(int id, int errorCode)
        {
            return viewRepository.Select().FirstOrDefault(x => x.BubbleSheetErrorId.Equals(id) && x.ErrorCode.Equals(errorCode));
        }

        public IQueryable<BubbleSheetError> GetBubbleSheetErrors()
        {
            return viewRepository.Select().Where(x => x.CreatedDate != null);
        }

        public void CorrectBubbleSheetError(int bubbleSheetErrorId)
        {
            var bubbleSheetError = GetBubbleSheetErrorById(bubbleSheetErrorId);
            bubbleSheetError.IsCorrected = true;
            repository.Save(bubbleSheetError);
        }

        public BubbleSheetError GetBubbleSheetErrorFromViewById(int bubbleSheetErrorId)
        {
            return viewRepository.Select().FirstOrDefault(x => x.BubbleSheetErrorId.Equals(bubbleSheetErrorId) && x.ErrorCode != -1);
        }

        public int GetBubbleSheetErrorCount(string fileName, DateTime uploadedDate, int userId)
        {
            //return viewRepository.Select().Count(x => x.FileName.Equals(fileName) && x.CreatedDate.Value > uploadedDate && x.UserId.Value.Equals(userId));
            return viewRepository.Select().Count(x => x.FileName.Equals(fileName) 
                //&& x.CreatedDate.Value > uploadedDate 
                && x.UserId.Value.Equals(userId));
        }

        public List<BubbleSheetError> GetBubbleSheetErrorByFileNameAndUserId(string fileName, int userId)
        {
            return
                viewRepository.Select()
                    .Where(x => x.FileName.Equals(fileName) && x.UserId.Value.Equals(userId))
                    .ToList();
        }

        public BubbleSheetError GetBubbleSheetErrorViewById(int id)
        {
            return viewRepository.Select().FirstOrDefault(x => x.BubbleSheetErrorId.Equals(id));
        }
    }
}