using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using AutoMapper;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
   public  class ClassStudentCustomRepository : IReadOnlyRepository<ClassStudentCustom>
    {
        private readonly Table<ClassStudentCustomView> table;

        public ClassStudentCustomRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<ClassStudentCustomView>();
            Mapper.CreateMap<ClassStudentCustom, ClassStudentCustomView>();
        }

        public IQueryable<ClassStudentCustom> Select()
        {
            return table.Select(x => new ClassStudentCustom
                {
                    ClassId = x.ClassID,
                    StudentId = x.StudentID,
                    Status = x.Status,
                    FullName = x.StudentFullName,
                    ClassStudentId = x.ClassStudentID,
                    Active = x.Active,
                    Code = x.Code,
                    SISId = x.SISID,
                    DistrictId = x.DistrictID,
                    FirstName = x.FirstName,
                    LastName = x.LastName
                });
        }
    }
}
