using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.QuestionGroup;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QuestionGroupService
    {
        private readonly IRepository<QuestionGroup> _questiongroupRepository;
        private readonly IRepository<VirtualQuestionGroup> _virtualquestiongroupRepository;

        public QuestionGroupService(IRepository<QuestionGroup> questiongroupRepository, IRepository<VirtualQuestionGroup> virtualquestiongroupRepository)
        {
            _questiongroupRepository = questiongroupRepository;
            _virtualquestiongroupRepository = virtualquestiongroupRepository;
        }


        public void SaveQuestionGroup(QuestionGroup item)
        {
            _questiongroupRepository.Save(item);
        }

        public void SaveVirtualQuestionGroup(VirtualQuestionGroup item)
        {
            _virtualquestiongroupRepository.Save(item);
        }

        public QuestionGroup GetQuestionGroupById(int questionGroupId)
        {
            return _questiongroupRepository.Select().Where(o => o.QuestionGroupID == questionGroupId).FirstOrDefault();
        }

        /// <summary>
        /// Delete All VirtualQuestionGroup by questiongroupId & QuestionGroup.
        /// </summary>
        /// <param name="questiongroup"></param>
        /// <returns>Bool</returns>
        public bool DeleteQuestionGroup(QuestionGroup questiongroup)
        {
            try
            {
                var lst = _virtualquestiongroupRepository.Select().Where(o => o.QuestionGroupID == questiongroup.QuestionGroupID).ToList();
                if (lst != null && lst.Count > 0)
                {
                    for (int i = 0; i < lst.Count; i++)
                    {
                        _virtualquestiongroupRepository.Delete(lst[i]);
                    }
                }
                _questiongroupRepository.Delete(questiongroup);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Get List VirtualQuestionIds in QuestionGroup
        /// </summary>
        /// <param name="questionGroupId"></param>
        /// <returns>List VirtualQuestionId</returns>
        public List<int> GetQuestionIdsInQuestionGroup(int questionGroupId)
        {
            try
            {
                var lst = _virtualquestiongroupRepository.Select()
                    .Where(o => o.QuestionGroupID == questionGroupId)
                    .Select(o=>o.VirtualQuestionID)
                    .ToList();
                return lst;
            }
            catch (Exception)
            {
                return new List<int>();
            }
        }

        /// <summary>
        /// Get List QuestionGroup by virtualtestId
        /// </summary>
        /// <param name="virtualtestId"></param>
        /// <returns></returns>
        public List<QuestionGroup> GetListQuestionGroupByVirtualTestId(int virtualtestId)
        {
            return _questiongroupRepository.Select().Where(o=>o.VirtualTestId == virtualtestId).ToList();
        }

        /// <summary>
        /// Get List QuestionGroup by virtualtestId
        /// </summary>
        /// <param name="virtualtestId"></param>
        /// <param name="oldvirtualSectionId"></param>
        /// <returns></returns>
        public List<QuestionGroup> GetListQuestionGroupByVirtualTestIdAndSectionId(int virtualtestId, int oldvirtualSectionId)
        {
            return _questiongroupRepository.Select().Where(o => o.VirtualTestId == virtualtestId && o.VirtualSectionID == oldvirtualSectionId).ToList();
        }

        /// <summary>
        /// Get List VirtualQuestionGroup By VirtualTestId
        /// </summary>
        /// <param name="virtualtestId"></param>
        /// <returns></returns>
        public List<VirtualQuestionGroup> GetListVirtualQuestionGroupByVirtualTestId(int virtualtestId)
        {
            List<int> lstQuestionGroupIds = _questiongroupRepository.Select()
                .Where(o => o.VirtualTestId == virtualtestId)
                .Select(o=>o.QuestionGroupID)
                .ToList();
            if(lstQuestionGroupIds != null && lstQuestionGroupIds.Count > 0)
            {
                var lstVirtualQuestionGroup = _virtualquestiongroupRepository.Select()
                    .Where(o => lstQuestionGroupIds.Contains(o.QuestionGroupID))
                    .ToList();
                return lstVirtualQuestionGroup;
            }
            return new List<VirtualQuestionGroup>();
        }

        /// <summary>
        /// Total QuestionGroup
        /// </summary>
        /// <param name="virtualtestId"></param>
        /// <param name="virtualsectionId"></param>
        /// <returns></returns>
        public int CountQuestionGroupPerSection(int virtualtestId, int? virtualsectionId)
        {
            if (virtualsectionId.HasValue && virtualsectionId.Value > 0)
            {
                return _questiongroupRepository.Select()
                    .Count(o => o.VirtualTestId == virtualtestId && o.VirtualSectionID == virtualsectionId);
            }
            else
            {
                return _questiongroupRepository.Select()
                    .Count(o => o.VirtualTestId == virtualtestId && (o.VirtualSectionID == null || o.VirtualSectionID == 0));
            }            
        }

        /// <summary>
        /// Update SectionID of questiongroup belong Default Section.
        /// </summary>
        /// <param name="sectionId"></param>
        /// <param name="lstQuestionGroup"></param>
        public void UpdateSectionIdToQuestionGroup(int sectionId, List<QuestionGroup>lstQuestionGroup)
        {
            if(lstQuestionGroup != null && lstQuestionGroup.Count > 0 && sectionId > 0)
            {
                foreach(var item in lstQuestionGroup)
                {
                    item.VirtualSectionID = sectionId;
                    _questiongroupRepository.Save(item);
                }
            }
        }

        public bool HasQuestionGroup(int virtualtestId)
        {
            return _questiongroupRepository.Select().Any(o => o.VirtualTestId == virtualtestId);
        }

        public int GetMaxOrderVirtualQuestionInGroup(int questionGroupId)
        {
            var vqInGroup = _virtualquestiongroupRepository.Select().OrderByDescending(x => x.Order).FirstOrDefault();
            if (vqInGroup != null)
                return vqInGroup.Order;
            return 0;
        }

        public int GetGroupIdByVirtualQuestionId(int virtualquestionId)
        {
            var qg =
                _virtualquestiongroupRepository.Select().FirstOrDefault(x => x.VirtualQuestionID == virtualquestionId);
            return qg == null ? 0 : qg.QuestionGroupID;
        }

        public VirtualQuestionGroup GetVirtualQuestionGroupByVirtualQuestionId(int virtualQuestionId)
        {
            return _virtualquestiongroupRepository.Select().Where(o => o.VirtualQuestionID == virtualQuestionId).FirstOrDefault();
        }

        public QuestionGroup GetQuestionGroup(int virtualTestId, int questionGroupId)
        {
            return _questiongroupRepository.Select().FirstOrDefault(o => o.VirtualTestId == virtualTestId && o.QuestionGroupID == questionGroupId);
        }

        public List<QuestionGroup> GetQuestionGroups(int virtualTestId, List<int> questionGroupIds)
        {
            return _questiongroupRepository
                .Select()
                .Where(o => o.VirtualTestId == virtualTestId && questionGroupIds.Contains(o.QuestionGroupID))
                .ToList();
        }

        public int GetFirstQuestionInGroup( int questionGroupId )
        {
            var questionIngroup = _virtualquestiongroupRepository.Select()
                .Where(o=>o.QuestionGroupID == questionGroupId)
                .OrderBy(o=>o.Order)
                .FirstOrDefault();
            return questionIngroup == null ? 0 : questionIngroup.VirtualQuestionID;
        }

        public List<VirtualQuestionGroup> GetVirtualQuestionGroupsByVirtualQuestionIds(List<int> virtualQuestionIds)
        {
            return _virtualquestiongroupRepository
                .Select()
                .Where(o => virtualQuestionIds.Contains(o.VirtualQuestionID))
                .ToList();
        }
    }
}
