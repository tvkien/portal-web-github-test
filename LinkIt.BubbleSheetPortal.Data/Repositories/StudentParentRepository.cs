using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class StudentParentRepository : IReadOnlyRepository<StudentParent>, IInsertDeleteRepository<StudentParent>, IStudentParentRepository
    {
        private readonly Table<StudentParentEntity> table;
        private readonly ParentDataContext _parentDataContext;

        public StudentParentRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = ParentDataContext.Get(connectionString).GetTable<StudentParentEntity>();
            _parentDataContext = ParentDataContext.Get(connectionString);
        }

        #region Implementation of IReadOnlyRepository<StudentParentRepository>

        public IQueryable<StudentParent> Select()
        {
            return table.Select(x => new StudentParent
            {
                StudentParentID = x.StudentParentID,
                StudentID = x.StudentID,
                ParentID = x.ParentID.GetValueOrDefault(),
                Relationship = x.Relationship,
                StudentDataAccess = x.StudentDataAccess
            });
        }

        #endregion

        #region Implementation of IStudentParentRepository

        public IQueryable<GetStudentParentListByDistrictIDResult> GetStudentParentListByDistrictID(int? districtID)
        {
            return _parentDataContext.GetStudentParentListByDistrictID(districtID ?? -1).AsQueryable();
        }

        public IQueryable<GetClassesWithDetailByStudentIDResult> GetClassesWithDetailByStudentID(int studentID)
        {
            return _parentDataContext.GetClassesWithDetailByStudentID(studentID).AsQueryable();
        }

        public IQueryable<GetParentsByStudentIDResult> GetParentsByStudentID(int studentID)
        {
            return _parentDataContext.GetParentsByStudentID(studentID).AsQueryable();
        }

        public IQueryable<GetStudentsByParentIDResult> GetStudentsByParentID(int parentID)
        {
            return _parentDataContext.GetStudentsByParentID(parentID).AsQueryable();
        }

        public IQueryable<GetNotAssignParentsOfStudentResult> GetNotAssignParentsOfStudent(int studentID)
        {
            return _parentDataContext.GetNotAssignParentsOfStudent(studentID).AsQueryable();
        }

        public bool IsParentHasAnyAssociation(int parentId)
        {
            var entity = table.FirstOrDefault(x => x.ParentID.Equals(parentId));
            if (entity.IsNull())
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region Implementation of IInsertDeleteRepository<StudentParent>

        public void Save(StudentParent item)
        {
            var entity = table.FirstOrDefault(x => x.StudentParentID.Equals(item.StudentParentID));
            if (entity.IsNull())
            {
                entity = new StudentParentEntity();
                table.InsertOnSubmit(entity);
            }
            BindStudentParentEntityFromStudentParentItem(entity, item);
            table.Context.SubmitChanges();
        }

        public void Delete(StudentParent item)
        {
            var entity = table.FirstOrDefault(x => x.StudentParentID.Equals(item.StudentParentID));
            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        #endregion

        private void BindStudentParentEntityFromStudentParentItem(StudentParentEntity entity, StudentParent item)
        {
            entity.StudentParentID = item.StudentParentID;
            entity.StudentID = item.StudentID;
            entity.ParentID = item.ParentID;
            entity.Relationship = item.Relationship;
            entity.StudentDataAccess = item.StudentDataAccess;
        }

        public void InsertMultipleRecord(List<StudentParent> items)
        {
            foreach (var item in items)
            {
                var entity = new StudentParentEntity();
                BindStudentParentEntityFromStudentParentItem(entity, item);
                table.InsertOnSubmit(entity);
            }

            table.Context.SubmitChanges();
        }
    }
}
