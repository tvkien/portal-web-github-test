using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ProgramRepository : IProgramRepository
    {
        private readonly Table<ProgramEntity> table;
        private readonly StudentDataContext datacontext;
        public ProgramRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<ProgramEntity>();
            datacontext = StudentDataContext.Get(connectionString);
            Mapper.CreateMap<Program, ProgramEntity>();
        }

        public IQueryable<Program> Select()
        {
            return table.Select(x => new Program
                {
                    Id = x.ProgramID,
                    Name = x.Name,
                    DistrictID = x.DistrictID,
                    AccessLevelID = x.AccessLevelID,
                    Code = x.Code
                });
        }

        public void Save(Program item)
        {
            var entity = table.FirstOrDefault(x => x.ProgramID.Equals(item.Id));

            if (entity.IsNull())
            {
                entity = new ProgramEntity();
                table.InsertOnSubmit(entity);                
            }
            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
        }

        public void Delete(Program item)
        {
            datacontext.DeleteProgramAndAssociationItems(item.Id);
        }

        public IQueryable<ProgramToView> GetProgramsByDistrictIdToView(int districtId, string currentDateTime)
        {
            return datacontext.GetProgramsByDistrictIDToView(districtId, currentDateTime).Select(x => new ProgramToView()
                                                                                     {
                                                                                         ProgramId = x.ProgramID,
                                                                                         Name = x.Name,
                                                                                         AccessLevelId = x.AccessLevelID,
                                                                                         Code = x.Code,
                                                                                         StudentNumber = x.StudentNumber ?? 0
                                                                                     }).AsQueryable();
        }

        public IQueryable<ListItem> GetProgramInStudentProgramByDistrictId(int districtId)
        {
            var lstProgramInStudentProgram = datacontext.ProgramInStudentProgramViews
                .Where(o => o.DistrictID == districtId).ToList();
            var lstProgramReturn = lstProgramInStudentProgram.Select(o => new ListItem()
            {
                Id = o.ProgramID,
                Name = o.Name
            }).AsQueryable();

            return lstProgramReturn;
        }

        public List<ListItem> GetSurveyProgramByRole(int districtId, int userId, int roleId)
        {
            var result = datacontext.GetSurveyProgramByRole(districtId, userId, roleId).Select(o => new ListItem()
            {
                Id = o.ProgramID,
                Name = o.Name
            }).ToList();
            return result;
        }

    }
}
