using System.Collections.Generic;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs;

namespace LinkIt.BubbleSheetPortal.InteractiveRubric.Repositories
{
    public interface IRubricTestResultScoreRepository : IRepository<RubricTestResultScore>
    {
        void SaveRubricTestResultScores(IEnumerable<RubricTestResultScoreDto> rubricTestResultScores, int qTIOnlineTestSessionId, int virtualQuestionId, int createdBy);

        void Delete(IEnumerable<RubricTestResultScore> rubricTestResultScores);

        void Delete(IEnumerable<int> rubricTestResultScoresIds);
    }
}
