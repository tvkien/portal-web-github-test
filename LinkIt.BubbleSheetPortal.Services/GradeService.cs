using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class GradeService
    {
        private readonly IGradeRepository _repository;
        private readonly IReadOnlyRepository<GradeDistrict> _gradeDistrictRepository;

        public GradeService(IGradeRepository repository, IReadOnlyRepository<GradeDistrict> gradeDistrictRepository)
        {
            this._repository = repository;
            this._gradeDistrictRepository = gradeDistrictRepository;
        }

        public IQueryable<Grade> GetGrades()
        {
            return _repository.Select().OrderBy(x => x.Order);
        }

        public Grade GetGradeById(int gradeId)
        {
            return _repository.Select().FirstOrDefault(x => x.Id.Equals(gradeId));
        }

        public IQueryable<GradeDistrict> GetGradesByDistrictId(int districtId)
        {
            return _gradeDistrictRepository.Select().Where(x => x.DistrictID.Equals(districtId)).OrderBy(x => x.Order);
        }

        public List<Grade> GetGradesByUserIdDistrictIdRoleId(int userId, int districtId, int roleId)
        {
            return _repository.GetGradesByUserIdDistrictIdRoleId(userId, districtId, roleId);
        }
        public Grade GetGradesByName(string gradeName)
        {
            return _repository.Select().FirstOrDefault(x => x.Name.Equals(gradeName));
        }
        public Grade GetMaxGrade()
        {
            return _repository.Select().OrderByDescending(x => x.Order).Take(1).FirstOrDefault();
        }
        public Grade GetMinGrade()
        {
            return _repository.Select().OrderBy(x=>x.Order).Take(1).FirstOrDefault();
        }
        public IQueryable<Grade> GetStateSubjectGradeByStateAndSubject(string stateCode, string subject)
        {
            return _repository.GetStateSubjectGradeByStateAndSubject(stateCode, subject);
        }

        public List<Grade> GetGradesByStateID(int stateId)
        {
            return _repository.GetGradesByStateID(stateId).OrderBy(x=>x.Order).ToList();
        }

        //\
        /// <summary>
        /// Get Grade by district for ACT Page
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="districtId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<Grade> ACTGetGradesByUserIdDistrictIdRoleId(int userId, int districtId, int roleId)
        {
            return _repository.ACTGetGradesByUserIdDistrictIdRoleId(userId, districtId, roleId);
        }
        public List<Grade> GetGradesForItemSetSaveTest(int userId, int districtId, int roleId)
        {
            return _repository.GetGradesForItemSetSaveTest(userId, districtId, roleId);
        }

        public List<Grade> SATGetGradesByUserIdDistrictIdRoleId(int userId, int districtId, int roleId)
        {
            return _repository.SATGetGradesByUserIdDistrictIdRoleId(userId, districtId, roleId);
        }

        public IQueryable<Grade> GetGradesByGradeIdList(List<int> gradeIds)
        {
            return _repository.Select().Where(x => gradeIds.Contains(x.Id));
        }

        public List<Grade>  GetGradesByUserIdDistrictIdRoleId(User currentUser, int? districtId)
        {
            int currentDistrictId = districtId ?? 0;
            if (!currentUser.IsPublisher && !currentUser.IsNetworkAdmin)
                currentDistrictId = currentUser.DistrictId.HasValue ? currentUser.DistrictId.Value : 0;
            var tmp = _repository.GetGradesByUserIdDistrictIdRoleId(currentUser.Id, currentDistrictId,
                currentUser.RoleId);
            return tmp;
        }

        public List<Grade> GetGradesFormBankByUserIdDistrictIdRoleId(User currentUser, int? districtId, bool? isFromMultiDate, bool usingMultiDate)
        {
            int currentDistrictId = districtId ?? 0;
            if (!currentUser.IsPublisher && !currentUser.IsNetworkAdmin)
                currentDistrictId = currentUser.DistrictId.HasValue ? currentUser.DistrictId.Value : 0;
            var tmp = _repository.GetGradesFormBankByUserIdDistrictIdRoleId(currentUser.Id, currentDistrictId,
                currentUser.RoleId, isFromMultiDate?? false, usingMultiDate);
            return tmp;
        }

        public List<Grade> StudentLookupGetGradesFilter(int userId, int districtId, int roleId)
        {
            return _repository.StudentLookupGetGradesFilter(userId, districtId, roleId);
        }

        public List<GradeOrder> GetGradeOrders(int districtId)
        {
            return _repository.GetGradeOrders(districtId);
        }

        public List<Grade> GetGradesByUserId(User currentUser, int? districtId)
        {
            int currentDistrictId = districtId ?? 0;
            if ((!currentUser.IsPublisher && !currentUser.IsNetworkAdmin) || districtId == 0)
                currentDistrictId = currentUser.DistrictId.HasValue ? currentUser.DistrictId.Value : 0;

            var data = _repository.GetGradesByUserId(currentUser.Id, currentDistrictId, currentUser.RoleId);
            return data;
        }
    }
}
