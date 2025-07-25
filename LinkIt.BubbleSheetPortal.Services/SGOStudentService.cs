using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class SGOStudentService
    {
        private readonly IRepository<SGOStudent> sgoStudentRepository;
        private readonly IRepository<SGOGroup> sgoGroupRepository;

        public SGOStudentService(IRepository<SGOStudent> sgoStudentRepository, IRepository<SGOGroup> sgoGroupRepository)
        {
            this.sgoStudentRepository = sgoStudentRepository;
            this.sgoGroupRepository = sgoGroupRepository;
        }

        public List<SGOStudent> GetListStudentBySGOID(int sgoID)
        {
            return sgoStudentRepository.Select().Where(x => x.SGOID == sgoID).ToList();
        }

        public IQueryable<SGOStudent> GetListStudentInCustomGroupBySGOID(int SGOID)
        {
            var listCustomGroupID =
                sgoGroupRepository.Select()
                    .Where(x => x.Order != Constanst.ToBePlacedGroupOrder && x.Order != Constanst.ExcludedGroupOrder && x.SGOID == SGOID)
                    .Select(x => x.SGOGroupID)
                    .Distinct()
                    .ToList();
            return
                sgoStudentRepository.Select()
                    .Where(x => x.SGOID == SGOID && x.SGOGroupID != null && listCustomGroupID.Contains(x.SGOGroupID.Value));
        }

        public void SaveSGOStudent(SGOStudent sgoStudent)
        {
            sgoStudentRepository.Save(sgoStudent);
        }

        public bool AllStudentInGroup(int SGOID, int groupid)
        {
            return sgoStudentRepository.Select().Any(o => o.SGOID == SGOID && o.SGOGroupID == groupid) == false;
        }
    }
}