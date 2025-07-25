using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models.Algorithmic;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class VirtualQuestionService
    {
        private readonly IVirtualQuestionRepository virtualQuestionRepository;
        private readonly IVirtualTestRepository _virtualTestRepository;
        private readonly IManageTestRepository _manageTestRepository;
        private readonly IQTIItemRepository _qtiitemRepository;
        private readonly IRepository<AlgorithmQTIItemGrading> _algorithmQTIItemGradingRepository;
        private readonly IAlgorithmicVirtualQuestionGradingRepository _algorithmicVirtualQuestionGradingRepository;

        public VirtualQuestionService(
            IVirtualQuestionRepository repository,
            IManageTestRepository manageTestRepository,
            IVirtualTestRepository virtualTestRepository,
            IQTIItemRepository qtiitemRepository,
            IRepository<AlgorithmQTIItemGrading> algorithmQTIItemGradingRepository,
            IAlgorithmicVirtualQuestionGradingRepository algorithmicVirtualQuestionGradingRepository)
        {
            this.virtualQuestionRepository = repository;
            this._manageTestRepository = manageTestRepository;
            this._virtualTestRepository = virtualTestRepository;
            _qtiitemRepository = qtiitemRepository;
            _algorithmQTIItemGradingRepository = algorithmQTIItemGradingRepository;
            _algorithmicVirtualQuestionGradingRepository = algorithmicVirtualQuestionGradingRepository;
        }

        public IQueryable<VirtualQuestionData> Select()
        {
            return virtualQuestionRepository.Select();
        }

        public void Save(VirtualQuestionData virtualTestData)
        {
            virtualQuestionRepository.Save(virtualTestData);
        }
        public bool IsExistScoreName(int virtualTestId, string scoreName)
        {
            return virtualQuestionRepository.Select().Any(x => x.VirtualTestID == virtualTestId && x.ScoreName == scoreName);
        }

        public VirtualQuestionData GetQuestionDataById(int id)
        {
            return virtualQuestionRepository.Select().FirstOrDefault(o => o.VirtualQuestionID == id);
        }
        public List<VirtualQuestionData> GetQuestionsHaveScoreName(List<int> virtualQuestionIds)
        {
            return virtualQuestionRepository.Select().Where(o => virtualQuestionIds.Contains(o.VirtualQuestionID) && !string.IsNullOrEmpty(o.ScoreName)).ToList();
        }
        public void UpdateQIITemIdbyQuestionId(int questionId, int qtiitemId)
        {
            var question = virtualQuestionRepository.Select().FirstOrDefault(o => o.VirtualQuestionID == questionId);
            if (question != null)
            {
                question.QTIItemID = qtiitemId;
                virtualQuestionRepository.Save(question);
            }
        }
        public List<VirtualQuestionData> GetVirtualQuestionByVirtualTestID(int virtualTestID)
        {
            return
                virtualQuestionRepository.Select()
                    .Where(x => x.VirtualTestID == virtualTestID)
                    .OrderBy(x => x.QuestionOrder)
                    .ToList();
        }

        public bool HasRightToEditVirtualQuestions(List<int> virtualQuestionIdList, User currentUser,
            out List<VirtualQuestionData> authorizedVirtualQuestionList, List<int> listDistrictId)
        {
            //avoid ajax modify parameter
            //get virtual questions

           

            var questionList = virtualQuestionRepository.Select()
                       .Where(x => virtualQuestionIdList.Contains(x.VirtualQuestionID))
                       .ToList();
            authorizedVirtualQuestionList = questionList;

            //get virtual tests of these virtual questions
            var virtualTestIdList = questionList.Select(x => x.VirtualTestID).Distinct().ToList();

            var virtualTestList = _virtualTestRepository.Select()
                .Where(x => virtualTestIdList.Contains(x.VirtualTestID))
                .Select(x => new
                {
                    x.DatasetOriginID,
                    x.BankID
                })
                .Distinct()
                .ToList();

            var isSurvey = virtualTestList.Any(x => x.DatasetOriginID == (int)DataSetOriginEnum.Survey);
            if (isSurvey) return true;
            var testBankIdList = virtualTestList.Select(x => x.BankID).ToList();
            //get item banks of these virtual tests
            var authorizedBankIdList = new List<int>();

            var filter = new GetBanksByUserIDFilter
            {
                UserID = currentUser.Id,
                RoleID = currentUser.RoleId,
                DistrictID = currentUser.DistrictId.GetValueOrDefault(),
                ShowArchived = true,
                IsSurvey = isSurvey
            };

            if (currentUser.IsNetworkAdmin)
            {
                foreach (var districtId in listDistrictId)
                {
                    filter.DistrictID = districtId;

                    authorizedBankIdList.AddRange(_manageTestRepository.GetBanksByUserID(filter).Select(x => x.BankID).ToList());
                }
            }
            else
            {

                authorizedBankIdList = _manageTestRepository.GetBanksByUserID(filter).Select(x => x.BankID).ToList();

            }
            //if there is only bank is not authorized, return false
            foreach (var testBankId in testBankIdList)
            {
                if (!authorizedBankIdList.Contains(testBankId))
                {
                    return false;
                }
            }

            return true;

        }
        public int? GetTotalPointsPossible(int virtualTestID)
        {
            if (!_virtualTestRepository.Select().Any(x => x.VirtualTestID == virtualTestID))
            {
                return 0;
            }
            else
            {
                if (virtualQuestionRepository.Select().Any(x => x.VirtualTestID == virtualTestID))
                {
                    return
                        virtualQuestionRepository.Select()
                            .Where(x => x.VirtualTestID == virtualTestID)
                            .Sum(x => x.PointsPossible);
                }
                else
                {
                    return 0;
                }

            }

        }

        public void ClearQuestionLabelQuestionLNumber(int virtualTestId)
        {
            _virtualTestRepository.ClearQuestionLabelQuestionLNumber(virtualTestId);
        }

        public List<QuestionXMLContent> GetXMLContent(int virtualTestID)
        {
            var questions = virtualQuestionRepository.Select().Where(m => m.VirtualTestID == virtualTestID).ToList();
            var qtiitemIds = questions.Select(m => m.QTIItemID).ToList();
            var qtiiems = _qtiitemRepository.Select().Where(m => qtiitemIds.Contains(m.QTIItemID)).ToList();
            var xmlContents = questions.Join(qtiiems, q => q.QTIItemID, qti => qti.QTIItemID, (q, qti) => new { Question = q, QTIItem = qti })
                                    .Select(m => new QuestionXMLContent
                                    {
                                        VirtualQuestionID = m.Question.VirtualQuestionID,
                                        XMLContent = m.QTIItem.XmlContent
                                    }).ToList();

            return xmlContents;

        }

        public bool IsHavingAnswer(int? qtiitemId, int? qtiitemSubId)
        {
            return _qtiitemRepository.IsHavingAnswer(qtiitemId, qtiitemSubId);
        }

        public void CloneAlgorithmicVirtualQuestionGrading(int oldVirtualQuestionId, int newVirtualQuestionId)
        {
            var algorithmicVirtualQuestionGradings = _algorithmicVirtualQuestionGradingRepository.Select()
                .Where(x => x.VirtualQuestionID == oldVirtualQuestionId).ToList();

            foreach (var item in algorithmicVirtualQuestionGradings)
            {
                item.AlgorithmID = 0;
                item.VirtualQuestionID = newVirtualQuestionId;
                _algorithmicVirtualQuestionGradingRepository.Save(item);
            }
        }

        public void CloneAlgorithmicVirtualQuestionGradingMultiple(List<CloneVirtualQuestion> cloneVirtualQuestions)
        {
            var oldVirtualQuestionIDs = cloneVirtualQuestions.Select(x => x.OldVirtualQuestionID);

            var algorithmicVirtualQuestionGradings = _algorithmicVirtualQuestionGradingRepository
                .Select()
                .Where(x => oldVirtualQuestionIDs.Contains(x.VirtualQuestionID))
                .ToList();

            if (algorithmicVirtualQuestionGradings.Count == 0)
            {
                return;
            }

            algorithmicVirtualQuestionGradings.ForEach(item =>
            {
                var newVirtualQuestionId = cloneVirtualQuestions.FirstOrDefault(x => x.OldVirtualQuestionID == item.VirtualQuestionID).NewVirtualQuestionID;

                item.AlgorithmID = 0;
                item.VirtualQuestionID = newVirtualQuestionId;
            });

            _algorithmicVirtualQuestionGradingRepository.InsertMultipleRecord(algorithmicVirtualQuestionGradings);
        }

        public void CloneAlgorithmQTIItemGrading(int oldQTIItemId, int newQTIItemId)
        {
            var algorithmQTIItemGradings = _algorithmQTIItemGradingRepository.Select()
                .Where(x => x.QTIItemID == oldQTIItemId).ToList();

            foreach (var item in algorithmQTIItemGradings)
            {
                item.AlgorithmID = 0;
                item.QTIItemID = newQTIItemId;
                _algorithmQTIItemGradingRepository.Save(item);
            }
        }

        public void InsertMultipleRecord(List<VirtualQuestionData> items)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }

            virtualQuestionRepository.InsertMultipleRecord(items);
        }
    }
}
