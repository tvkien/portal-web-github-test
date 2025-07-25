using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class GradeRepository : IGradeRepository
    {
        private readonly Table<GradeEntity> table;
        private readonly TestDataContext dbContext;
        private readonly StudentDataContext studentContext;
        public GradeRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<GradeEntity>();
            dbContext = TestDataContext.Get(connectionString);
            studentContext = StudentDataContext.Get(connectionString);
            Mapper.CreateMap<Grade, GradeEntity>();
        }

        public IQueryable<Grade> Select()
        {
            return table.Select(x => new Grade
            {
                Id = x.GradeID,
                Name = x.Name,
                Order = x.Order
            });
        }
        public IQueryable<Grade> GetStateSubjectGradeByStateAndSubject(string stateCode, string subject)
        {
            return
                dbContext.GetStateStandardGradesByStateAndSubject(stateCode, subject).AsQueryable().Select(
                    x => new Grade { Id = x.GradeID.Value, Name = x.Name, Order = x.GradeOrder.Value });
        }

        public List<Grade> GetGradesByUserIdDistrictIdRoleId(int userId, int districtId, int roleId)
        {
            var query = dbContext.ProcGrade(districtId, userId, roleId);
            if (query != null)
            {
                return query.Select(g => new Grade()
                {
                    Id = g.GradeID,
                    Name = g.Name,
                    Order = g.Order
                }).OrderBy(g => g.Order).ToList();
            }
            return new List<Grade>();
        }

        public List<Grade> GetGradesFormBankByUserIdDistrictIdRoleId(int userId, int districtId, int roleId, bool isFromMultiDate, bool usingMultiDate)
        {
            var query = dbContext.ProcGradeFormBank(districtId, userId, roleId, isFromMultiDate, usingMultiDate);
            if (query != null)
            {
                return query.Select(g => new Grade()
                {
                    Id = g.GradeID,
                    Name = g.Name,
                    Order = g.Order
                }).OrderBy(g => g.Order).ToList();
            }
            return new List<Grade>();
        }

        public List<Grade> GetGradesByStateID(int stateId)
        {
            var result = dbContext.GetGradesByStateIDForManageTest(stateId).ToList();
            if (result.Any())
            {
                return result.Select(o => new Grade()
                {
                    Id = o.GradeID,
                    Name = o.Name,
                    Order = o.Order
                }).ToList();
            }
            return new List<Grade>();
        }
        /// <summary>
        /// Get Grade for ACT Page
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="districtId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<Grade> ACTGetGradesByUserIdDistrictIdRoleId(int userId, int districtId, int roleId)
        {
            var query = dbContext.ACTProcGrade(districtId, userId, roleId);
            if (query != null)
            {
                return query.Select(g => new Grade()
                {
                    Id = g.GradeID,
                    Name = g.Name,
                    Order = g.Order
                }).OrderBy(g => g.Order).ToList();
            }
            return new List<Grade>();
        }

        public List<Grade> GetGradesForItemSetSaveTest(int userId, int districtId, int roleId)
        {
            var query = dbContext.GetGradesForItemSetSaveTest(districtId, userId, roleId);
            if (query != null)
            {
                return query.Select(g => new Grade()
                {
                    Id = g.GradeID,
                    Name = g.Name,
                    Order = g.Order
                }).OrderBy(g => g.Order).ToList();
            }
            return new List<Grade>();
        }

        public List<Grade> SATGetGradesByUserIdDistrictIdRoleId(int userId, int districtId, int roleId)
        {
            var query = dbContext.SATProcGrade(districtId, userId, roleId);
            if (query != null)
            {
                return query.Select(g => new Grade()
                {
                    Id = g.GradeID,
                    Name = g.Name,
                    Order = g.Order
                }).OrderBy(g => g.Order).ToList();
            }
            return new List<Grade>();
        }

        public List<Grade> StudentLookupGetGradesFilter(int userId, int districtId, int roleId)
        {
            var query = studentContext.StudentLookupGetGradesFilter(districtId, userId, roleId);
            if (query != null)
            {
                return query.Select(g => new Grade()
                {
                    Id = g.GradeID,
                    Name = g.Name,
                    Order = g.Order
                }).OrderBy(g => g.Order).ToList();
            }
            return new List<Grade>();
        }

        public List<GradeOrder> GetGradeOrders(int districtId)
        {
            var data = dbContext.GetGradeOrder(districtId);
            if (data != null)
                return data.Select(o => new GradeOrder
                {
                    GradeId = o.GradeID,
                    Order = o.Order
                }).ToList();
            return new List<GradeOrder>();
        }

        public List<Grade> GetGradesByUserId(int userId, int districtId, int roleId)
        {
            var query = dbContext.GetGradesByUserID(districtId, userId, roleId);
            return query.Select(g => new Grade()
            {
                Id = g.GradeID,
                Name = g.Name,
                Order = g.Order
            }).OrderBy(g => g.Order).ToList();
        }
    }
}
