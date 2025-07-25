using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ProgramService
    {
        private readonly IRepository<Program> repository;
        private readonly IProgramRepository programRepository;

        public ProgramService(IRepository<Program> repository, IProgramRepository programRepository)
        {
            this.repository = repository;
            this.programRepository = programRepository;
        }

        public IQueryable<Program> GetPrograms()
        {
            return repository.Select().OrderBy(p => p.Name);
        }

        public IQueryable<Program> GetProgramsByDistrictID(int districtId)
        {
            return repository.Select().Where(p => p.DistrictID.Equals(districtId)).OrderBy(p => p.Name);
        }

        public IQueryable<Program> GetProgramsNotMatchWithStudent(int districtId, List<int> existingPrograms)
        {
            return repository.Select().Where(p => p.DistrictID.Equals(districtId) && !existingPrograms.Contains(p.Id));
        }

        public Program GetProgramById(int programId)
        {
            return repository.Select().FirstOrDefault(x => x.Id.Equals(programId));
        }

        public List<ProgramToView> GetProgramsByDistrictIdToView(int districtId, string currentDateTime)
        {
            return programRepository.GetProgramsByDistrictIdToView(districtId, currentDateTime).ToList();
        }
        public void Save(Program item)
        {
            repository.Save(item);
        }
        public bool DeleteProgramAndAssociationItems(int programId)
        {
            try
            {
                var program = repository.Select().FirstOrDefault(x => x.Id.Equals(programId));
                if (program != null)
                {
                    repository.Delete(program);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IQueryable<ProgramToView> LoadPrograms(User user, int distictId, string currentDateTime)
        {
            var data = programRepository.GetProgramsByDistrictIdToView(distictId, currentDateTime);

            if (!user.IsPublisher)
            {
                data = data.Where(x => x.AccessLevelId != (int)AccessLevelEnum.LinkItOnly && x.AccessLevelId != (int)AccessLevelEnum.StateUsers);
            }
            if (user.IsSchoolAdmin)
            {
                data =
                    data.Where(
                        x =>
                            x.AccessLevelId == (int)AccessLevelEnum.DistrictAndSchoolAdmins ||
                            x.AccessLevelId == (int)AccessLevelEnum.AllUsers);
            }
            if (user.IsTeacher)
            {
                data =
                    data.Where(
                        x => x.AccessLevelId == (int)AccessLevelEnum.AllUsers);

            }
            return data;
        }

        public IQueryable<ListItem> GetProgramInStudentProgramByDistrictId(int districtId)
        {
            return programRepository.GetProgramInStudentProgramByDistrictId(districtId);
        }

        public List<ListItem> GetSurveyProgramByRole(int districtId, int userId, int roleId)
        {
            return programRepository.GetSurveyProgramByRole(districtId, userId, roleId);
        }
    }
}
