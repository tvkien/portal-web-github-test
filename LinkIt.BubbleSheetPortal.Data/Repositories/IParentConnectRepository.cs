using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IParentConnectRepository
    {
        IQueryable<GetInboxMessageForParentResult> GetInboxMessageForParent(int userId, int studentId, string searchValue);
        IQueryable<GetInboxMessageForStaffResult> GetInboxMessageForStaff(int userId, string searchValue);
        IQueryable<GetInboxMessageOfMainMessageResult> GetInboxMessageOfMainMessage(int userId, int messgeId);
        IQueryable<GetMainMessageDetailResult> GetMainMessageDetail(int messageRef, int userId);
        IQueryable<GetSubThreadMessageDetailResult> GetSubThreadMessageDetail(int messageRef, int senderId, int receiverId, int studentId);
        IQueryable<ReportGroupStudent> GetStudentGroup();
        int UpdateMessageThreadAsRead(int messageRef, int senderId, int receiverId, int studentId);
        int DeleteSubThreadMessage(int messageRef, int senderId, int receiverId, int studentId);
        int DeleteMainMessage(int messageId);
        IQueryable<StudentInClassWithParents> GetStudentsInClass();
        IQueryable<StudentInGroupWithParents> GetActiveStudentsInGroup();
        IQueryable<GetStudentInStudentGroupWithParentForTeacherResult> GetStudentInStudentGroupWithParentForTeacher(int? groupId, int? teacherId, int? districtId);
        IQueryable<GetStudentInStudentGroupWithParentForSchoolAdminResult> GetStudentInStudentGroupWithParentForSchoolAdmin(int? groupId, int? schoolAdminId, int? districtId);
        IQueryable<GetStudentInStudentGroupWithParentForDistrictPublisherResult> GetStudentInStudentGroupWithParentForDistrictPublisher(int? groupId, int? districtId);
    }
}
