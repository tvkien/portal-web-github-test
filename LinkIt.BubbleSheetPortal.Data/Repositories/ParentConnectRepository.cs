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
    public class ParentConnectRepository : IParentConnectRepository
    {
        private readonly ParentDataContext _parentDataContext;

        public ParentConnectRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _parentDataContext = ParentDataContext.Get(connectionString);
        }

        #region Implementation of IReadOnlyRepository

        public IQueryable<GetInboxMessageForParentResult> GetInboxMessageForParent(int userId, int studentId, string searchValue)
        {
            return _parentDataContext.GetInboxMessageForParent(userId, studentId, searchValue).AsQueryable();
        }

        public IQueryable<GetInboxMessageForStaffResult> GetInboxMessageForStaff(int userId, string searchValue)
        {
            return _parentDataContext.GetInboxMessageForStaff(userId, searchValue).AsQueryable();
        }

        public IQueryable<GetInboxMessageOfMainMessageResult> GetInboxMessageOfMainMessage(int userId, int messageId)
        {
            return _parentDataContext.GetInboxMessageOfMainMessage(userId, messageId).AsQueryable();
        }

        public IQueryable<GetMainMessageDetailResult> GetMainMessageDetail(int messageRef, int userId)
        {
            return _parentDataContext.GetMainMessageDetail(messageRef, userId).AsQueryable();
        }

        public IQueryable<GetSubThreadMessageDetailResult> GetSubThreadMessageDetail(int messageRef, int senderId, int receiverId, int studentId)
        {
            return _parentDataContext.GetSubThreadMessageDetail(messageRef, senderId, receiverId, studentId).AsQueryable();
        }

        public int UpdateMessageThreadAsRead(int messageRef, int senderId, int receiverId, int studentId)
        {
            return _parentDataContext.UpdateMessageThreadAsRead(messageRef, senderId, receiverId, studentId);
        }

        public int DeleteSubThreadMessage(int messageRef, int senderId, int receiverId, int studentId)
        {
            return _parentDataContext.DeleteSubThreadMessage(messageRef, senderId, receiverId, studentId);
        }

        public int DeleteMainMessage(int messageId)
        {
            return _parentDataContext.DeleteMainMessage(messageId);
        }

        private  Table<ReportGroupStudentView> _table;
        public  IQueryable<ReportGroupStudent> GetStudentGroup()
        {
            _table = _parentDataContext.GetTable<ReportGroupStudentView>();
            return _table.Select(x => new ReportGroupStudent
                                          {
                                              ReportGroupStudentId = x.ReportGroupStudentID,
                                              ReportGroupId = x.ReportGroupID,
                                              StudentId = x.StudentID,
                                              FirstName = x.FirstName,
                                              LastName = x.LastName

                                          });
        }

        private Table<StudentInClassWithParentsView> tableStudentInClassWithParentsView;
        public IQueryable<StudentInClassWithParents> GetStudentsInClass()
        {
            tableStudentInClassWithParentsView = _parentDataContext.GetTable<StudentInClassWithParentsView>();
            return tableStudentInClassWithParentsView.Select(x => new StudentInClassWithParents
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                MiddleName = x.MiddleName,
                Gender = x.Gender,
                Race = x.Race,
                Code = x.Code,
                StudentID = x.StudentID,
                Active = x.Active.GetValueOrDefault(),
                ClassID = x.ClassID,
                ID = x.ClassStudentID,
                GradeName = x.GradeName,
                Parents = x.Parents

            });
        }
        private Table<StudentInStudentGroupWithParentsView> tableStudentInStudentGroupWithParentsView;
        public IQueryable<StudentInGroupWithParents> GetActiveStudentsInGroup()
        {
            tableStudentInStudentGroupWithParentsView = _parentDataContext.GetTable<StudentInStudentGroupWithParentsView>();
            return tableStudentInStudentGroupWithParentsView.Select(x => new StudentInGroupWithParents
            {
                ReportGroupStudentID =  x.ReportGroupStudentID,
                ReportGroupID = x.ReportGroupID,
                FirstName = x.FirstName,
                MiddleName = x.MiddleName,
                LastName = x.LastName,
                StudentID = x.StudentID,
                Code = x.Code,
                Gender = x.Gender,
                Parents = x.Parents
            });
        }

        public IQueryable<GetStudentInStudentGroupWithParentForTeacherResult> GetStudentInStudentGroupWithParentForTeacher(int? groupId, int? teacherId, int? districtId)
        {
            return _parentDataContext.GetStudentInStudentGroupWithParentForTeacher(groupId, teacherId, districtId).AsQueryable();
        }
        public IQueryable<GetStudentInStudentGroupWithParentForSchoolAdminResult> GetStudentInStudentGroupWithParentForSchoolAdmin(int? groupId, int? schoolAdminId, int? districtId)
        {
            return _parentDataContext.GetStudentInStudentGroupWithParentForSchoolAdmin(groupId, schoolAdminId, districtId).AsQueryable();
        }
        public IQueryable<GetStudentInStudentGroupWithParentForDistrictPublisherResult> GetStudentInStudentGroupWithParentForDistrictPublisher(int? groupId, int? districtId)
        {
            return _parentDataContext.GetStudentInStudentGroupWithParentForDistrictPublisher(groupId, districtId).AsQueryable();
        }


        #endregion              
    }
}
