using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class SGOGroupService
    {
        private readonly IRepository<SGOGroup> sgoGroupRepository;
        private readonly IRepository<SGOStudent> sgoStudentRepository;
        private readonly ISGORepository sgoRepository;

        
        public SGOGroupService(IRepository<SGOGroup> sgoGroupRepository, IRepository<SGOStudent> sgoStudentRepository, ISGORepository sgoRepository)
        {
            this.sgoGroupRepository = sgoGroupRepository;
            this.sgoStudentRepository = sgoStudentRepository;
            this.sgoRepository = sgoRepository;
        }

        public IQueryable<SGOGroup> GetGroupBySgoID(int SGOID)
        {
            return sgoGroupRepository.Select().Where(x => x.SGOID == SGOID);
        }

        public SGOGroup GetGroupById(int sgoGroupId)
        {
            return sgoGroupRepository.Select().FirstOrDefault(x => x.SGOGroupID == sgoGroupId);
        }

        public void InitialDefaultGroupForSGO(int SGOID)
        {
            var toBePlaceGroup = new SGOGroup
                                 {
                                     SGOID = SGOID,
                                     Name = Constanst.ToBePlacedGroupName,
                                     Order = Constanst.ToBePlacedGroupOrder
                                 };
            var excludedGroup = new SGOGroup
                                {
                                    SGOID = SGOID,
                                    Name = Constanst.ExcludedGroupName,
                                    Order = Constanst.ExcludedGroupOrder
                                };
            sgoGroupRepository.Save(toBePlaceGroup);
            sgoGroupRepository.Save(excludedGroup);

            //assign all students to To Be Placed group
            //var listStudent = sgoStudentRepository.Select().Where(x => x.SGOID == SGOID).ToList();
            //foreach (var sgoStudent in listStudent)
            //{
            //    sgoStudent.SGOGroupID = toBePlaceGroup.SGOGroupID;
            //    sgoStudentRepository.Save(sgoStudent);
            //}
            MoveStudentHasNoGroupToDefaultGroup(SGOID);
        }

        public IQueryable<SGOGroup> GetGroupWithOut9899BySgoID(int SGOID)
        {
            return sgoGroupRepository.Select().Where(x => x.SGOID == SGOID && x.Order != Constanst.ToBePlacedGroupOrder && x.Order != Constanst.ExcludedGroupOrder);
        }

        public void SaveSGOGroup(int sgoId, List<ListItem> lstSGOGroup)
        {
            var groupNameInOrder = string.Join(",", lstSGOGroup.OrderBy(x => x.Id).Select(x => x.Name));
            sgoRepository.UpdateGroup(sgoId, groupNameInOrder);
        }

        public void Save(SGOGroup sgoGroup)
        {
            sgoGroupRepository.Save(sgoGroup);
        }

        public void Delete(SGOGroup sgoGroup)
        {
            sgoGroupRepository.Delete(sgoGroup);
        }

        public void MoveStudentHasNoGroupToDefaultGroup(int SGOID)
        {
            //var toBePlaceGroup =
            //    sgoGroupRepository.Select().FirstOrDefault(x => x.SGOID == SGOID && x.Order == ToBePlacedGroupOrder);

            //if (toBePlaceGroup != null)
            //{
            //    //assign no-group students to To Be Placed group
            //    var listStudent =
            //        sgoStudentRepository.Select().Where(x => x.SGOID == SGOID && x.SGOGroupID == null).ToList();
            //    foreach (var sgoStudent in listStudent)
            //    {
            //        sgoStudent.SGOGroupID = toBePlaceGroup.SGOGroupID;
            //        sgoStudentRepository.Save(sgoStudent);
            //    }
            //}
            sgoRepository.MoveStudentHasNoGroupToDefaultGroup(SGOID);
        }

        public void AssignStudentToGroup(int sgoID, int sgoGroupID, string studentIDs)
        {
            sgoRepository.AssignStudentToGroup(sgoID, sgoGroupID, studentIDs);
        }

        public List<SGOAutoGroupStudentData> AutoGroup(int sgoID, bool includeUpdate, int scoreType)
        {
            return sgoRepository.AutoAssignStudentToGroup(sgoID, includeUpdate, scoreType);
        }

        /// <summary>
        /// Only Update '% Student at Target Score',  'Teacher SGO Score', 'Weight (based on students per group)'
        /// </summary>
        /// <param name="lstGroups"></param>
        public void UpdateSGOResult(List<SGOGroup> lstGroups)
        {
            foreach (var sgoGroup in lstGroups)
            {
                var vgroup = sgoGroupRepository.Select().FirstOrDefault(o => o.SGOGroupID == sgoGroup.SGOGroupID);
                if (vgroup != null)
                {
                    vgroup.PercentStudentAtTargetScore = sgoGroup.PercentStudentAtTargetScore;
                    vgroup.TeacherSGOScore = sgoGroup.TeacherSGOScore;
                    vgroup.Weight = sgoGroup.Weight;
                    sgoGroupRepository.Save(vgroup);
                }
            }
        }

        public bool CheckSavedResultScore(int sgoId)
        {
            return sgoGroupRepository
                .Select().Any(o => o.SGOID == sgoId &&
                                   (o.PercentStudentAtTargetScore.HasValue ||
                                    o.TeacherSGOScore.HasValue
                                    || o.Weight.HasValue ||
                                    (o.TeacherSGOScoreCustom != "" && o.TeacherSGOScoreCustom != null)));
        }
    }
}