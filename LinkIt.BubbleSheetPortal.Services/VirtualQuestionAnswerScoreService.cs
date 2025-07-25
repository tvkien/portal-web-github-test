using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class VirtualQuestionAnswerScoreService
    {
        private readonly IRepository<VirtualQuestionAnswerScore> _repository;

        public VirtualQuestionAnswerScoreService(IRepository<VirtualQuestionAnswerScore> repository)
        {
            _repository = repository;
        }

        public IQueryable<VirtualQuestionAnswerScore> Select()
        {
            return _repository.Select();
        }

        public void Delete(int virtualQuestionAnswerScoreId)
        {
            var item =
                _repository.Select().FirstOrDefault(x => x.VirtualQuestionAnswerScoreId == virtualQuestionAnswerScoreId);
            if (item != null)
            {
                _repository.Delete(item);
            }
        }

        public void Save(VirtualQuestionAnswerScore item)
        {
            _repository.Save(item);
        }

        public bool CheckExistQuestionAnswerScoreByVirtualQuestionId(int virtualQuestionId)
        {
            return _repository.Select().Any(o => o.VirtualQuestionId == virtualQuestionId);
        }

        public void UpdateVirtualQuestionAnswerScore(int VirtualQuestionAnswerScoreId, int score)
        {
            var virtualQuestionAnswerScore =
                this.Select().FirstOrDefault(x => x.VirtualQuestionAnswerScoreId == VirtualQuestionAnswerScoreId);
            if (virtualQuestionAnswerScore != null)
            {
                virtualQuestionAnswerScore.Score = score;
                Save(virtualQuestionAnswerScore);
            }
        }
    }
}