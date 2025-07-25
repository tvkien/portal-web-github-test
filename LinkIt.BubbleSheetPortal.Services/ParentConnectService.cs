using System.Linq;
using System.Collections.Generic;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ParentConnectService
    {
        private readonly IReadOnlyRepository<ReportGroup> _reportGroupRepository;
        private readonly IInsertDeleteRepository<ReportGroup> _reportGroupInsertDeleteRepository;

        private readonly IReadOnlyRepository<ReportGroupStudent> _reportGroupStudentRepository;
        private readonly IInsertDeleteRepository<ReportGroupStudent> _reportGroupStudentInsertDeleteRepository;

        private readonly IReadOnlyRepository<Message> _messageRepository;
        private readonly IInsertDeleteRepository<Message> _messageInsertDeleteRepository;

        private readonly IReadOnlyRepository<MessageReceiver> _messageReceiverRepository;
        private readonly IInsertDeleteRepository<MessageReceiver> _messageReceiverInsertDeleteRepository;

        private readonly IParentConnectRepository _parentConnectRepository;

        private readonly IReadOnlyRepository<StudentParent> _studentParentRepository;
        private readonly IReadOnlyRepository<Student> _studentRepository;

        private readonly IRepository<ParentDto> _parentRepository;

        public ParentConnectService(IReadOnlyRepository<ReportGroup> reportGroupRepository, IInsertDeleteRepository<ReportGroup> reportGroupInsertDeleteRepository,
            IReadOnlyRepository<ReportGroupStudent> reportGroupStudentRepository, IInsertDeleteRepository<ReportGroupStudent> reportGroupStudentInsertDeleteRepository,
            IReadOnlyRepository<Message> messageRepository, IInsertDeleteRepository<Message> messageInsertDeleteRepository,
            IReadOnlyRepository<MessageReceiver> messageReceiverRepository, IInsertDeleteRepository<MessageReceiver> messageReceiverInsertDeleteRepository,
            IParentConnectRepository parentConnectRepository,
            IReadOnlyRepository<StudentParent> studentParentRepository,
            IReadOnlyRepository<Student> studentRepository,
            IRepository<ParentDto> parentRepository
            )
        {
            _reportGroupRepository = reportGroupRepository;
            _reportGroupInsertDeleteRepository = reportGroupInsertDeleteRepository;
            _reportGroupStudentRepository = reportGroupStudentRepository;
            _reportGroupStudentInsertDeleteRepository = reportGroupStudentInsertDeleteRepository;

            _messageRepository = messageRepository;
            _messageInsertDeleteRepository = messageInsertDeleteRepository;
            _messageReceiverRepository = messageReceiverRepository;
            _messageReceiverInsertDeleteRepository = messageReceiverInsertDeleteRepository;

            _parentConnectRepository = parentConnectRepository;

            _studentParentRepository = studentParentRepository;
            _studentRepository = studentRepository;

            _parentRepository = parentRepository;
        }

        public IQueryable<ReportGroup> GetReportGroup()
        {
            return _reportGroupRepository.Select();
        }

        public IQueryable<ReportGroupStudent> GetReportGroupStudent()
        {
            return _reportGroupStudentRepository.Select();
        }

        public IQueryable<Message> GetMessage()
        {
            return _messageRepository.Select();
        }

        public Message GetMessageById(int messageId)
        {
            return _messageRepository.Select().FirstOrDefault(x => x.MessageId == messageId);
        }

        public IQueryable<MessageReceiver> GetMessageReceiver()
        {
            return _messageReceiverRepository.Select();
        }

        public IQueryable<MessageInbox> GetInboxMessageForParent(int userId, int studentId, string searchValue)
        {
            return _parentConnectRepository
                .GetInboxMessageForParent(userId, studentId, searchValue)
                .Select(x => new MessageInbox()
                                 {
                                     Acknow = x.Acknow,
                                     Boby = x.Body,
                                     CreatedDateTime =
                                         x.CreatedDateTime,
                                     MessageNoUnread = x.MessageNoUnread,
                                     MessageId = x.MessageID,
                                     Replies = x.Replies,
                                     Sender = x.Sender,
                                     SenderId = x.SenderID,
                                     StudentId = x.StudentID,
                                     Subject = x.Subject,
                                     MessageNoInThread = x.MessageNoInThread,
                                     MessageType = x.MessageType
                                 }).ToList().AsQueryable();
        }

        public IQueryable<MessageInbox> GetInboxMessageForStaff(int userId, string searchValue)
        {
            return _parentConnectRepository
                .GetInboxMessageForStaff(userId, searchValue)
                .Select(x => new MessageInbox()
                                 {
                                     Acknow = x.Acknow,
                                     Boby = x.Body,
                                     CreatedDateTime =
                                         x.CreatedDateTime,
                                     MessageNoUnread = x.MessageNoUnread,
                                     MessageId = x.MessageID,
                                     Replies = x.Replies,
                                     Sender = x.Sender,
                                     SenderId = x.SenderID,
                                     StudentId = x.StudentID,
                                     Subject = x.Subject,
                                     MessageNoInThread = x.MessageNoInThread,
                                     MessageType = x.MessageType,
                                     Recipients = x.Recipients
                                 }).ToList().AsQueryable();
        }

        public IQueryable<MessageInbox> GetInboxMessageOfMainMessage(int userId, int messageId)
        {
            return _parentConnectRepository
                .GetInboxMessageOfMainMessage(userId, messageId)
                .Select(x => new MessageInbox()
                                 {
                                     Acknow = x.Acknow,
                                     Boby = x.Body,
                                     CreatedDateTime =
                                         x.CreatedDateTime,
                                     MessageNoUnread = x.MessageNoUnread,
                                     MessageId = x.MessageID,
                                     Replies = x.Replies,
                                     Sender = x.Sender,
                                     SenderId = x.SenderID,
                                     StudentId = x.StudentID,
                                     Subject = x.Subject,
                                     MessageNoInThread = x.MessageNoInThread,
                                     MessageType = x.MessageType
                                 }).ToList().AsQueryable();
        }

        public IQueryable<MessageInDetail> GetMainMessageDetail(int messageRef, int userId)
        {
            return _parentConnectRepository
                .GetMainMessageDetail(messageRef, userId)
                .Select(x => new MessageInDetail()
                                 {
                                     Body = x.Body,
                                     CreatedDateTime =
                                         x.CreatedDateTime,
                                     IsAcknowledgeRequired = x.IsAcknowlegdeRequired,
                                     IsRead = x.IsRead,
                                     MessageId = x.MessageID,
                                     Sender = x.Sender,
                                     SenderId = x.SenderID,
                                     StudentId = x.StudentID,
                                     Subject = x.Subject,
                                     Recipients = x.Recipients
                                 }).ToList().AsQueryable();
        }

        public IQueryable<MessageInDetail> GetSubThreadMessageDetail(int messageRef, int senderId, int receiverId, int studentId)
        {
            return _parentConnectRepository
                .GetSubThreadMessageDetail(messageRef, senderId, receiverId, studentId)
                .Select(x => new MessageInDetail()
                                 {
                                     Body = x.Body,
                                     CreatedDateTime =
                                         x.CreatedDateTime,
                                     IsAcknowledgeRequired = x.IsAcknowlegdeRequired,
                                     IsRead = x.IsRead,
                                     MessageId = x.MessageID,
                                     Sender = x.Sender,
                                     SenderId = x.SenderID,
                                     StudentId = x.StudentID,
                                     Subject = x.Subject,
                                     ReplyEnabled = x.ReplyEnabled,
                                 }).ToList().AsQueryable();
        }

        public void SaveMessage(Message message)
        {
            _messageInsertDeleteRepository.Save(message);
        }
        public void DeleteOriginalMessage(Message message)
        {
            _messageInsertDeleteRepository.Delete(message);
        }

        public int UpdateMessageThreadAsRead(int messageRef, int senderId, int receiverId, int studentId)
        {
            return _parentConnectRepository.UpdateMessageThreadAsRead(messageRef, senderId, receiverId, studentId);
        }

        public int MarkAllMessageThreadAsRead(int messageRef, int receiverId)
        {
            return _parentConnectRepository.UpdateMessageThreadAsRead(messageRef, 0, receiverId, 0);
        }

        public void ReplyMessage(Message message, int receiverId, int studentId)
        {
            _messageInsertDeleteRepository.Save(message);
            var messageReceiver = new MessageReceiver()
                                      {
                                          IsDeleted = 0,
                                          IsRead = 0,
                                          MessageId = message.MessageId,
                                          StudentId = studentId,
                                          UserId = receiverId
                                      };
            _messageReceiverInsertDeleteRepository.Save(messageReceiver);
        }

        public void ReplyToAll(Message message)
        {
            var recipients = _messageReceiverRepository.Select().Where(en => en.MessageId == message.MessageRef).ToList();
            foreach (var recipient in recipients)
            {
                message.MessageId = 0;
                _messageInsertDeleteRepository.Save(message);
                var messageReceiver = new MessageReceiver()
                {
                    IsDeleted = 0,
                    IsRead = 0,
                    MessageId = message.MessageId,
                    StudentId = recipient.StudentId,
                    UserId = recipient.UserId
                };
                _messageReceiverInsertDeleteRepository.Save(messageReceiver);
            }
        }

        public List<Student> GetStudentOfParent(int userId)
        {
            var parent = _parentRepository.Select().FirstOrDefault(o => o.UserID == userId);
            if (parent != null)
            {
                var studentIds = _studentParentRepository.Select().Where(x => x.ParentID == parent.ParentID).Select(en => en.StudentID).ToList();

                var students =
                    _studentRepository.Select().Where(x => studentIds.Contains(x.Id)).OrderBy(x => x.FirstName).ThenBy(
                        x => x.LastName).ToList();
                return students;
            }

            return new List<Student>();
        }

        public void SaveMessageReceiver(MessageReceiver messageReceiver)
        {
            _messageReceiverInsertDeleteRepository.Save(messageReceiver);
            
        }
        public bool CheckUniqueGroupName(int groupId, int userId, string strGroupName)
        {
            if (groupId > 0)
            {
                return !_reportGroupRepository.Select().Any(o => o.ReportGroupId != groupId && o.Name.Equals(strGroupName) && o.UserId == userId);
            }

            return !_reportGroupRepository.Select().Any(o => o.Name.Equals(strGroupName) && o.UserId == userId);
        }
        public ReportGroup SaveStudentGroup(ReportGroup reportGroup)
        {
            if (reportGroup.IsNotNull())
            {
                _reportGroupInsertDeleteRepository.Save(reportGroup);
                return reportGroup;
            }
            return null;
        }

        public void SaveStudentGroupByGroupId(int groupId, List<string> lstStudentId)
        {
            if (groupId <= 0 || lstStudentId == null) return;
            DeleteStudentGroupByGroupId(groupId);
            for (var i = 0; i < lstStudentId.Count; i++)
            {
                int studentId;
                if (!int.TryParse(lstStudentId[i], out studentId)) continue;
                var classPrintingGroup = new ReportGroupStudent
                {
                    ReportGroupId = groupId,
                    StudentId = studentId
                };
                _reportGroupStudentInsertDeleteRepository.Save(classPrintingGroup);
            }
        }
        public IQueryable<ReportGroupStudent> GetStudentInGroupByGroupId(int reportGroupId)
        {            
            return _parentConnectRepository.GetStudentGroup().Where(x => x.ReportGroupId == reportGroupId);
        }
        
        private void DeleteStudentGroupByGroupId(int groupId)
        {
            var listReportGroupStudent = _reportGroupStudentRepository.Select().Where(o => o.ReportGroupId == groupId);
            if (listReportGroupStudent.IsNotNull())
            {
                foreach (ReportGroupStudent groupstudent in listReportGroupStudent)
                {
                    _reportGroupStudentInsertDeleteRepository.Delete(groupstudent);
                }
            }
        }

        public void DeleteReportGroup(int groupId)
        {
            DeleteStudentGroupByGroupId(groupId);
            if (groupId > 0)
            {

                ReportGroup group = _reportGroupRepository.Select().FirstOrDefault(o => o.ReportGroupId == groupId);
                if (group.IsNotNull())
                {
                    _reportGroupInsertDeleteRepository.Delete(group);
                }
            }
           
        }
        
        public IQueryable<StudentInClassWithParents> GetAllStudentInClass(int classId)
        {
            return _parentConnectRepository.GetStudentsInClass().Where(s => s.ClassID.Equals(classId));
        }
        //This method will return active students in the current district terms
        public IQueryable<StudentInClassWithParents> GetActiveStudentsInClass(List<int> studentIdList,int classId )
        {
            return _parentConnectRepository.GetStudentsInClass().Where(s => studentIdList.Contains(s.StudentID) && s.ClassID == classId);
        }
        //This method will return active students of a group who is active and in the current district terms
        public IQueryable<StudentInGroupWithParents> GetActiveStudentsInGroup(int groupId)
        {
            return _parentConnectRepository.GetActiveStudentsInGroup().Where(s => s.ReportGroupID == groupId);
        }


        public void DeleteMessages(List<MessageReceiver> deleteMessages)
        {
            foreach (var deleteMessage in deleteMessages)
            {
                var message = _messageRepository.Select().FirstOrDefault(x => x.MessageId == deleteMessage.MessageId);

                if (message != null)
                {
                    if (message.UserId == deleteMessage.UserId ) // Sender == Receiver ==> Main message
                    {
                        _parentConnectRepository.DeleteMainMessage(message.MessageId);
                    }
                    else
                    {
                        _parentConnectRepository.DeleteSubThreadMessage(message.MessageRef,
                                                                        message.UserId,
                                                                        deleteMessage.UserId,
                                                                        deleteMessage.StudentId);
                    }
                }
            }
        }

        //This method will return active students of a group who is active, in the current district terms and under control of a primary teacher for sending message
        public IQueryable<StudentInGroupWithParents> GetStudentInStudentGroupWithParentForTeacher(int groupId, int teacherId,int districtId)
        {
            return
                _parentConnectRepository
                    .GetStudentInStudentGroupWithParentForTeacher(groupId, teacherId, districtId)
                    .Select(x => new StudentInGroupWithParents
                    {
                        ReportGroupStudentID = x.ReportGroupStudentID,
                        ReportGroupID = x.ReportGroupID,
                        StudentID = x.StudentID,
                        FirstName = x.FirstName,
                        MiddleName = x.MiddleName,
                        LastName = x.LastName,
                        Gender = x.Gender,
                        Code = x.Code,
                        Parents = x.Parents
                    }).ToList().AsQueryable();

        }
        //This method will return active students of a group who is active, in the current district terms and under control of a school admin for sending message
        public IQueryable<StudentInGroupWithParents> GetStudentInStudentGroupWithParentForSchoolAdmin(int groupId, int schoolAdmin, int districtId)
        {
            return
                _parentConnectRepository
                    .GetStudentInStudentGroupWithParentForSchoolAdmin(groupId, schoolAdmin,districtId)
                    .Select(x => new StudentInGroupWithParents
                    {
                        ReportGroupStudentID = x.ReportGroupStudentID,
                        ReportGroupID = x.ReportGroupID,
                        StudentID = x.StudentID,
                        FirstName = x.FirstName,
                        MiddleName = x.MiddleName,
                        LastName = x.LastName,
                        Gender = x.Gender,
                        Code = x.Code,
                        Parents = x.Parents
                    }).ToList().AsQueryable();

        }

        //This method will return active students of a group who is active, in the current district terms and under control of a district admin for sending message
        public IQueryable<StudentInGroupWithParents> GetStudentInStudentGroupWithParentForDistrictAdminAndPublisher(int groupId,int? districtId)
        {
            return
                _parentConnectRepository
                    .GetStudentInStudentGroupWithParentForDistrictPublisher(groupId, districtId)
                    .Select(x => new StudentInGroupWithParents
                    {
                        ReportGroupStudentID = x.ReportGroupStudentID,
                        ReportGroupID = x.ReportGroupID,
                        StudentID = x.StudentID,
                        FirstName = x.FirstName,
                        MiddleName = x.MiddleName,
                        LastName = x.LastName,
                        Gender = x.Gender,
                        Code = x.Code,
                        Parents = x.Parents
                    }).ToList().AsQueryable();

        }

    }
}
