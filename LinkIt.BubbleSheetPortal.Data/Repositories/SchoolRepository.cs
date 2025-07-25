using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.Commons;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SchoolRepository : IRepository<School>, ISchoolRepository
    {
        private readonly Table<SchoolEntity> table;
        private readonly DbDataContext dbContext;

        public SchoolRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<SchoolEntity>();
            dbContext = DbDataContext.Get(connectionString);
            Mapper.CreateMap<School, SchoolEntity>();
        }

        public IQueryable<School> Select()
        {
            return table.Select(x => new School
            {
                Id = x.SchoolID,
                DistrictId = x.DistrictID,
                Name = x.Name,
                Code = x.Code,
                StateCode = x.StateCode,
                Status = x.Status,
                StateId = x.StateID,
                LocationCode = x.LocationCode,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
                ModifiedUser = x.ModifiedUser,
                ModifiedBy = x.ModifiedBy
            });
        }

        public void Save(School item)
        {
            var entity = table.FirstOrDefault(x => x.SchoolID.Equals(item.Id));

            if (entity == null)
            {
                entity = new SchoolEntity();
                table.InsertOnSubmit(entity);
            }
            else
            {
                item.CreatedDate = entity.CreatedDate;
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.Id = entity.SchoolID;
        }

        public void Delete(School item)
        {
            var entity = table.FirstOrDefault(x => x.SchoolID.Equals(item.Id));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public bool CheckUserCanAccessSchool(int userId, int roleId, int schoolId)
        {
            return dbContext.CheckUserCanAccessClassOrSchool(userId, roleId, schoolId, "School")
                .Any(x => x.CanAccess == 1);
        }

        public string GetSchoolNameById(int id)
        {
            return table.Where(c => c.SchoolID == id).Select(c => c.Name).FirstOrDefault();
        }

        public TimeZoneDto GetSchoolTimeZone(int schoolId)
        {
            var timezone = dbContext.GetTimeZoneByStateWithSchoolID(schoolId)
                   .Select(x => new TimeZoneDto()
                   {
                       SchoolId = x.SchoolID,
                       TimeZoneId = x.TimeZoneID,
                       StateId = x.StateID.GetValueOrDefault()
                   }).FirstOrDefault();

            return timezone;
        }
        public ItemValue CreateSurveySchoolClass(int districtId, int termId, string surveyName, int userId)
        {
            var schoolClass = dbContext.CreateSurveySchoolClass(districtId, termId, surveyName, userId)
                   .Select(x => new ItemValue { Id1 = x.SchoolId ?? 0, Id2 = x.ClassID ?? 0}).FirstOrDefault();

            return schoolClass;
        }

        public IQueryable<School> GetSchoolsByDistrictV2(GetSchoolRequestModel input, ref int? totalRecords)
        {
            return dbContext.GetSchoolsByDistrictV2(input.DistrictId, input.SchoolId, input.StartIndex, input.PageSize, input.SortColumns, input.GeneralSearch, ref totalRecords)
                .Select(x => new School()
                {
                    Id = x.SchoolID,
                    Name = x.Name,
                    Code = x.Code,
                    StateCode = x.StateCode
                }).AsQueryable();
        }

        public List<School> GetTLDSSchoolsByDistrict(int districtId)
        {
            var schools = dbContext.SchoolEntities.Join(dbContext.SchoolMetaEntities, sc => sc.SchoolID, sm => sm.SchoolID, (sc, sm) => new { School = sc, SchoolMeta = sm })
                                                  .Where(schoolAndSchoolMeta => schoolAndSchoolMeta.School.DistrictID == districtId
                                                                                && (!schoolAndSchoolMeta.School.Status.HasValue || schoolAndSchoolMeta.School.Status != 3)
                                                                                && schoolAndSchoolMeta.SchoolMeta.Name.Equals("TLDSSchool")
                                                                                && (schoolAndSchoolMeta.SchoolMeta.Data != null
                                                                                    && schoolAndSchoolMeta.SchoolMeta.Data != ""
                                                                                    && schoolAndSchoolMeta.SchoolMeta.Data.ToLower().Equals("true")))
                                                  .Select(schoolAndSchoolMeta => new School()
                                                  {
                                                      Id = schoolAndSchoolMeta.School.SchoolID,
                                                      Name = schoolAndSchoolMeta.School.Name,
                                                      Code = schoolAndSchoolMeta.School.Code,
                                                      StateCode = schoolAndSchoolMeta.School.StateCode
                                                  }).ToList();

            return schools;
        }
    }
}
