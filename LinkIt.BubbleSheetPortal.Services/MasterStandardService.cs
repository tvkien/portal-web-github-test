using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class MasterStandardService
    {
        private readonly IMasterStandardRepository masterStandardRepository;
        private readonly IQTIItemStateStandardRepository qtiItemStateStandardRepository;
        private readonly IVirtualQuestionStateStandardRepository _virtualQuestionStateStandard;

        public MasterStandardService(
            IMasterStandardRepository masterStandardRepository,
            IQTIItemStateStandardRepository qtiItemStateStandardRepository,
            IVirtualQuestionStateStandardRepository virtualQuestionStateStandard)
        {
            this.masterStandardRepository = masterStandardRepository;
            this.qtiItemStateStandardRepository = qtiItemStateStandardRepository;
            _virtualQuestionStateStandard = virtualQuestionStateStandard;
        }

        public List<StateStandardSubject> GetStateStandardsByStateCode(string stateCode)
        {
            return masterStandardRepository.GetStateStandardsByStateCode(stateCode).ToList();
        }

        public List<StateSubjectGrade> GetStateSubjectGradeByStateAndSubject(string state, string subject)
        {
            return masterStandardRepository.GetStateSubjectGradeByStateAndSubject(state, subject).ToList();
        }

        public List<MasterStandard> GetStandardsByStateCodeAndSubjectAndGradeTopLevel(string state, string subject,
            string grade)
        {
            if (state.ToLower() == TextConstants.STATE_CODE_CC || state.ToLower() == TextConstants.STATE_CODE_AC
                || state.ToLower() == TextConstants.STATE_CODE_AP)
            {
                return
                    masterStandardRepository.GetMasterStandardsByStateCodeAndSubjectAndGradesTopLevelCC(state, subject,
                        grade).ToList();
            }

            return
                masterStandardRepository.GetMasterStandardsByStateCodeAndSubjectAndGradesTopLevel(state, subject, grade)
                    .ToList();
        }

        public List<MasterStandard> GetStandardsByParentId(int parentId)
        {
            var standard = masterStandardRepository.Select().FirstOrDefault(x => x.MasterStandardID == parentId);
            if (standard != null)
            {
                return masterStandardRepository.GetETSStateStandardsByParentGUID(standard.GUID).ToList();
            }
            return new List<MasterStandard>();
        }

        public MasterStandard GetStandardsById(int masterStandardId)
        {
            var standard = masterStandardRepository.Select().FirstOrDefault(x => x.MasterStandardID == masterStandardId);
            return standard;
        }

        public MasterStandard GetStandardsByGuid(string guid)
        {
            var standard = masterStandardRepository.Select().FirstOrDefault(x => x.GUID == guid);
            return standard;
        }

        public List<MasterStandard> GetParentStandardsByChildId(int childId)
        {
            var standard = masterStandardRepository.Select().FirstOrDefault(x => x.MasterStandardID == childId);
            if (standard != null)
            {
                var parentStandard =
                    masterStandardRepository.Select().FirstOrDefault(x => x.GUID == standard.ParentGUID);
                if (parentStandard != null)
                {
                    MasterStandard grantParentStandard = null;
                    if (!string.IsNullOrWhiteSpace(parentStandard.ParentGUID))
                    {
                        grantParentStandard = masterStandardRepository.Select().FirstOrDefault(x => x.GUID == parentStandard.ParentGUID);
                    }
                    if (grantParentStandard != null)
                    {
                        return GetStandardsByParentId(grantParentStandard.MasterStandardID);
                    }
                    else
                    {
                        return GetStandardsByStateCodeAndSubjectAndGradeTopLevel(parentStandard.State,
                            standard.Subject, parentStandard.HiGrade);
                    }
                }

            }
            return new List<MasterStandard>();
        }

        public List<MasterStandard> GetParentStandardsByChildId(int childId, string grade)
        {
            var standard = masterStandardRepository.Select().FirstOrDefault(x => x.MasterStandardID == childId);
            if (standard != null)
            {
                var parentStandard =
                    masterStandardRepository.Select().FirstOrDefault(x => x.GUID == standard.ParentGUID);
                if (parentStandard != null)
                {
                    MasterStandard grantParentStandard = null;
                    if (!string.IsNullOrWhiteSpace(parentStandard.ParentGUID))
                    {
                        grantParentStandard = masterStandardRepository.Select().FirstOrDefault(x => x.GUID == parentStandard.ParentGUID);
                    }

                    if (grantParentStandard != null)
                    {
                        return GetStandardsByParentId(grantParentStandard.MasterStandardID);
                    }
                    else
                    {
                        return GetStandardsByStateCodeAndSubjectAndGradeTopLevel(parentStandard.State,
                            standard.Subject, grade);
                    }
                }

            }
            return new List<MasterStandard>();
        }


        public void AddStandardAssocoatedWithItem(int qtiItemId, int stateStandardId)
        {
            qtiItemStateStandardRepository.Save(new QTIItemStateStandard
            {
                QTIItemID = qtiItemId,
                StateStandardID = stateStandardId
            });
        }

        public void RemoveStandardAssociatedWithItem(int qtiItemId, int stateStandardId)
        {
            qtiItemStateStandardRepository.Delete(new QTIItemStateStandard
            {
                QTIItemID = qtiItemId,
                StateStandardID = stateStandardId
            });
        }

        public List<MasterStandard> GetStandardsAssociatedWithItem(int qtiItemId)
        {
            var listStandardIds =
                qtiItemStateStandardRepository.Select()
                    .Where(x => x.QTIItemID == qtiItemId)
                    .Select(x => x.StateStandardID)
                    .Distinct()
                    .ToList();

            var listStandards =
                masterStandardRepository.Select().Where(x => listStandardIds.Contains(x.MasterStandardID)).ToList();

            return listStandards;
        }
        public List<int> GetStandardIdAssociatedWithItem(int qtiItemId)
        {
            var listStandardIds =
                qtiItemStateStandardRepository.Select()
                    .Where(x => x.QTIItemID == qtiItemId)
                    .Select(x => x.StateStandardID)
                    .Distinct()
                    .ToList();

            return listStandardIds;
        }
        public IQueryable<MasterStandard> GetAll()
        {
            return masterStandardRepository.Select();
        }
        public IQueryable<QTIItemStateStandard> GetStandardsAssociatedWithItem(List<int> qtiItemIdList)
        {
            var listStandardIds =
                qtiItemStateStandardRepository.Select()
                    .Where(x => qtiItemIdList.Contains(x.QTIItemID));

            return listStandardIds;
        }
        public List<MasterStandard> GetStateStandardsForItemLibraryFilterTopLevel(string state, string subject, string grade, int? userId, int? districtId)
        {
            if (state.ToLower() == TextConstants.STATE_CODE_CC || state.ToLower() == TextConstants.STATE_CODE_AC
                || state.ToLower() == TextConstants.STATE_CODE_AP)
            {
                return
                    masterStandardRepository.GetStateStandardsForItemLibraryFilterTopLevelCC(state, subject,
                        grade, userId, districtId).ToList();
            }

            return
                masterStandardRepository.GetStateStandardsForItemLibraryFilterTopLevel(state, subject, grade, userId, districtId)
                    .ToList();
        }

        public List<MasterStandard> GetStateStandardsForItem3pLibraryFilterTopLevel(string state, string subject, string grade, int? qti3pSourceId)
        {
            if (state.ToLower() == TextConstants.STATE_CODE_CC || state.ToLower() == TextConstants.STATE_CODE_AC
                || state.ToLower() == TextConstants.STATE_CODE_AP)
            {
                return
                    masterStandardRepository.GetStateStandardsForItem3pLibraryFilterTopLevelCC(state, subject,
                        grade, qti3pSourceId).ToList();
            }

            return
                masterStandardRepository.GetStateStandardsForItem3pLibraryFilterTopLevel(state, subject, grade, qti3pSourceId)
                    .ToList();
        }
        public List<MasterStandard> GetStateStandardsNextLevelForItemLibraryFilter(int parentId, string state, string subject, string grade, bool qti3p, int? userId, int? districtId, int? qti3pSourceId)
        {

            var standard = masterStandardRepository.Select().FirstOrDefault(x => x.MasterStandardID == parentId);
            if (standard != null)
            {
                if (qti3p)
                {
                    return masterStandardRepository.GetStateStandardsNextLevelForItem3pLibraryFilter(standard.GUID, state, subject, grade, qti3pSourceId).ToList();
                }
                else
                {
                    return masterStandardRepository.GetStateStandardsNextLevelForItemLibraryFilter(standard.GUID, state, subject, grade, userId, districtId).ToList();
                }

            }
            return new List<MasterStandard>();
        }
        public List<MasterStandard> GetStateStandardsPreviousLevelForItemLibraryFilter(int childId, string state, string subject, string grade, bool qti3p, int? userId, int? districtId, int? qti3pSourceId)
        {
            var standard = masterStandardRepository.Select().FirstOrDefault(x => x.MasterStandardID == childId);
            if (standard != null)
            {
                var parentStandard =
                    masterStandardRepository.Select().FirstOrDefault(x => x.GUID == standard.ParentGUID);
                if (parentStandard != null)
                {
                    var grantParentStandard =
                        masterStandardRepository.Select().FirstOrDefault(x => x.GUID == parentStandard.ParentGUID);
                    if (grantParentStandard != null)
                    {
                        return GetStateStandardsNextLevelForItemLibraryFilter(grantParentStandard.MasterStandardID,
                                                                              state, subject, grade, qti3p, userId, districtId, qti3pSourceId);
                    }
                    else
                    {
                        if (qti3p)
                        {
                            return GetStateStandardsForItem3pLibraryFilterTopLevel(state, subject, grade, qti3pSourceId);
                        }
                        else
                        {
                            return GetStateStandardsForItemLibraryFilterTopLevel(state, subject, grade, userId, districtId);
                        }

                    }
                }

            }
            return new List<MasterStandard>();
        }
        public IQueryable<VirtualQuestionStateStandard> GetStandardsAssociatedWithVirtualQuestions(List<int> virtualQuestionIdList)
        {
            var listStandardIds =
                _virtualQuestionStateStandard.Select()
                    .Where(x => virtualQuestionIdList.Contains(x.VirtualQuestionId));

            return listStandardIds;
        }
        public List<MasterStandard> GetStandardsAssociatedWithVirtualQuestion(int virtualQuestionId)
        {
            var listStandardIds =
                _virtualQuestionStateStandard.Select()
                    .Where(x => x.VirtualQuestionId == virtualQuestionId)
                    .Select(x => x.StateStandardId)
                    .Distinct()
                    .ToList();

            var listStandards =
                masterStandardRepository.Select().Where(x => listStandardIds.Contains(x.MasterStandardID)).ToList();

            return listStandards;
        }
        public void AddStandardAssociatedWithVirtualQuestion(int virtualQuestionId, int stateStandardId)
        {
            _virtualQuestionStateStandard.Save(new VirtualQuestionStateStandard
            {
                VirtualQuestionId = virtualQuestionId,
                StateStandardId = stateStandardId
            });
        }
        public void RemoveStandardAssociatedWithVirtualQuestion(int virtualQuestionId, int stateStandardId)
        {
            var item =
                _virtualQuestionStateStandard.Select().FirstOrDefault(
                    x => x.VirtualQuestionId == virtualQuestionId && x.StateStandardId == stateStandardId);
            if (item != null)
            {
                _virtualQuestionStateStandard.Delete(item);
            }
        }

        public void CloneVirtualQuestionStateStandard(List<CloneVirtualQuestion> cloneVirtualQuestions)
        {
            var oldVirtualQuestionIDs = cloneVirtualQuestions.Select(x => x.OldVirtualQuestionID);

            var olds = _virtualQuestionStateStandard.Select().Where(o => oldVirtualQuestionIDs.Contains(o.VirtualQuestionId)).ToList();

            if (olds.Count == 0)
            {
                return;
            }

            olds.ForEach(item =>
            {
                var newVirtualQuestionId = cloneVirtualQuestions.FirstOrDefault(x => x.OldVirtualQuestionID == item.VirtualQuestionId).NewVirtualQuestionID;

                item.VirtualQuestionStateStandardId = 0;
                item.VirtualQuestionId = newVirtualQuestionId;
            });

            _virtualQuestionStateStandard.InsertMultipleRecord(olds);
        }

        public List<Qti3pItemStandardXml> GetQti3pItemStandardXml(List<int> qti3pItemIdList)
        {
            return masterStandardRepository.GetQti3pItemStandardXml().Where(x => qti3pItemIdList.Contains(x.Qti3pItemId)).ToList();
        }

        public List<MasterStandard> GetMasterStandardByIds(List<int> standardIds)
        {
            if (standardIds == null) return new List<MasterStandard>();

            var masterstandards = masterStandardRepository.Select().Where(m => standardIds.Contains(m.MasterStandardID)).ToList();
            return masterstandards;
        }

    }

}
