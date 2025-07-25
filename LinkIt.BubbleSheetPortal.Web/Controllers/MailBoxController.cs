using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Xml.Linq;
using AutoMapper;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Web.Models.ParentConnect;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [AjaxAwareAuthorize(Order = 2)] 
    public class MailBoxController : BaseController
    {
        private readonly MailBoxControllerParameters parameters;

        public MailBoxController(MailBoxControllerParameters  parameters)
        {
            this.parameters = parameters;
        }

        private int UnreadMessage
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["MailBox_UnreadMessage"].IsNull())
                {
                    return 0;
                }
                return Convert.ToInt32(System.Web.HttpContext.Current.Session["MailBox_UnreadMessage"]);
            }
            set { System.Web.HttpContext.Current.Session["MailBox_UnreadMessage"] = value; }
        }

        private int SelectedStudentId
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["MailBox_SelectedStudentId"].IsNull())
                {
                    return 0;
                }
                return Convert.ToInt32(System.Web.HttpContext.Current.Session["MailBox_SelectedStudentId"]);
            }
            set { System.Web.HttpContext.Current.Session["MailBox_SelectedStudentId"] = value; }
        }

        private int SelectedMessageId
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["MailBox_SelectedMessageId"].IsNull())
                {
                    return 0;
                }
                return Convert.ToInt32(System.Web.HttpContext.Current.Session["MailBox_SelectedMessageId"]);
            }
            set { System.Web.HttpContext.Current.Session["MailBox_SelectedMessageId"] = value; }
        }

        private int SelectedMessageRef
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["MailBox_SelectedMessageRef"].IsNull())
                {
                    return 0;
                }
                return Convert.ToInt32(System.Web.HttpContext.Current.Session["MailBox_SelectedMessageRef"]);
            }
            set { System.Web.HttpContext.Current.Session["MailBox_SelectedMessageRef"] = value; }
        }
       
        private StudentNavigationViewModel StudentNavigation
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["MailBox_StudentNavigationViewModel"].IsNull())
                {
                    return new StudentNavigationViewModel();
                }
                return
                    (StudentNavigationViewModel)
                    System.Web.HttpContext.Current.Session["MailBox_StudentNavigationViewModel"];
            }
            set { System.Web.HttpContext.Current.Session["MailBox_StudentNavigationViewModel"] = value; }
        }

        [ValidateInput(false)]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.MessageInbox)]
        public ActionResult Inbox(ComposeModel composeModel, int? replyMessageType, bool? replyMessageResult, int? studentId, bool? useState)
        {
            var model = SaveMessages(composeModel);

            GetStudentsOfParent(studentId);

            ViewBag.UserId = CurrentUser.Id;
            ViewBag.IsParent = CurrentUser.IsParent;
            ViewBag.StudentId = SelectedStudentId;            
            
            if (replyMessageType.HasValue)
            {
                ViewBag.ReplyMessageType = replyMessageType;
                ViewBag.ReplyMessageResult = replyMessageResult;
            }

            if(replyMessageType.HasValue || (useState.HasValue && useState.Value == true))
            {
                ViewBag.SaveStateMessageRef = SelectedMessageRef;
                ViewBag.SaveStateMessageId = SelectedMessageId;    
            }

            return View(model);
        }
        
        private void GetStudentsOfParent(int? studentId)
        {
            var studentNavigation = new StudentNavigationViewModel();
            if (CurrentUser.IsParent)
            {
                var students = parameters.ParentConnectService.GetStudentOfParent(CurrentUser.Id);

                if (studentId.HasValue)
                {
                    SelectedStudentId = studentId.Value;
                }
                else
                {
                    if(!(students.Any(x => x.Id == SelectedStudentId)) && students.Count > 0)
                    {
                        SelectedStudentId = students[0].Id;
                    }
                }

                studentNavigation.StudentsOfParent = students
                    .Select(en => new StudentOfParentViewModel
                                      {
                                          StudentFirstName = en.FirstName,
                                          StudentLastName = en.LastName,
                                          StudentID = en.Id
                                      }).ToList();
                studentNavigation.SelectedStudentId = SelectedStudentId;
                studentNavigation.IsParentRole = true;
            }else
            {
                studentNavigation.IsParentRole = false;
            }

            StudentNavigation = studentNavigation;
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.MessageInbox)]
        public ActionResult GetInboxMessageForStaff(int userId, string searchValue)
        {
            var data = InitStaffMessageInboxViewModel(
                parameters.ParentConnectService.GetInboxMessageForStaff(userId, searchValue));
            
            // Just count unread messages when select all inbox
            if (string.IsNullOrEmpty(searchValue))
            {
                UnreadMessage = data.Sum(en => en.MessageNoUnread);
            }else
            {
                SetStaffUnreadMessage(userId);
            }

            var parser = new DataTableParser<MessageInboxViewModel>();
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }

        private void SetStaffUnreadMessage(int userId)
        {
            var data = InitStaffMessageInboxViewModel(
                parameters.ParentConnectService.GetInboxMessageForStaff(userId, null));

            UnreadMessage = data.Sum(en => en.MessageNoUnread);
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.MessageInbox)]
        public ActionResult GetInboxMessageForParent(int userId, int studentId, string searchValue)
        {
            var data = InitParentMessageInboxViewModel(
                parameters.ParentConnectService.GetInboxMessageForParent(userId, studentId, searchValue));                

            // Just count unread messages when select all inbox
            if (string.IsNullOrEmpty(searchValue))
            {
                UnreadMessage = data.Count(en => en.MessageNoUnread > 0);
            }else
            {
                SetParentUnreadMessage(userId, studentId);
            }

            var parser = new DataTableParser<MessageInboxViewModel>();
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.MessageInbox)]
        public ActionResult GetInboxMessageOfMainMessage(int userId, int messageId)
        {
            var data = InitStaffMessageInboxViewModel(
                parameters.ParentConnectService.GetInboxMessageOfMainMessage(userId, messageId));

            var parser = new DataTableParser<MessageInboxViewModel>();
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }

        private void SetParentUnreadMessage(int userId, int studentId)
        {
            var data = InitParentMessageInboxViewModel(
                parameters.ParentConnectService.GetInboxMessageForParent(userId, studentId, null));                

            UnreadMessage = data.Count(en => en.MessageNoUnread > 0);
        }

        private IQueryable<MessageInboxViewModel> InitStaffMessageInboxViewModel(IQueryable<MessageInbox> messageInboxes)
        {
            return messageInboxes
                .Select(x => new MessageInboxViewModel
                                 {
                                     Acknow = x.MessageType == 1 ? x.Acknow.ToString() : "",
                                     BriefInfo = x.Subject,
                                     CreatedDateTimeString =
                                         Util.EmailDisplayCreatedDateTime(x.CreatedDateTime),
                                     CreatedDateTime = x.CreatedDateTime,
                                     MessageNoUnread = x.MessageNoUnread,
                                     MessageId = x.MessageId,
                                     Replies = x.MessageType == 1 ? x.Replies.ToString() : "",
                                     Sender = (x.SenderId == CurrentUser.Id
                                                   ? "me"
                                                   : x.Sender) +
                                              (x.MessageNoUnread > 0
                                                   ? " (" + x.MessageNoUnread.ToString() +
                                                     ")"
                                                   : ""),
                                     SenderId = x.SenderId,
                                     StudentId = x.StudentId,
                                     Recipients = x.Recipients,
                                     CreatedDateTimeFullString = Util.EmailDisplayFullCreatedDateTime(x.CreatedDateTime),
                                     MessageNoInThread = x.MessageNoInThread
                                 });
        }

        private IQueryable<MessageInboxViewModel> InitParentMessageInboxViewModel(IQueryable<MessageInbox> messageInboxes)
        {
            return messageInboxes
                .Select(x => new MessageInboxViewModel
                {
                    Acknow = x.MessageType == 1 ? x.Acknow.ToString() : "",
                    BriefInfo = x.Subject,
                    CreatedDateTimeString =
                        Util.EmailDisplayCreatedDateTime(x.CreatedDateTime),
                    CreatedDateTime = x.CreatedDateTime,
                    MessageNoUnread = x.MessageNoUnread,
                    MessageId = x.MessageId,
                    Replies = x.MessageType == 1 ? x.Replies.ToString() : "",
                    Sender = (x.SenderId == CurrentUser.Id
                                  ? "me"
                                  : x.Sender) +
                             (x.MessageNoInThread > 1
                                  ? " (" + x.MessageNoInThread.ToString() +
                                    ")"
                                  : ""),
                    SenderId = x.SenderId,
                    StudentId = x.StudentId,
                    Recipients = x.Recipients,
                    CreatedDateTimeFullString = Util.EmailDisplayFullCreatedDateTime(x.CreatedDateTime),
                    MessageNoInThread = x.MessageNoInThread
                });
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.MessageInbox)]
        public ActionResult GetUnreadMessage()
        {
            return Json(UnreadMessage);
        }

        [ValidateInput(false)]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.MessageInbox)]
        public ActionResult ViewMessage(ViewMessageViewModel model)
        {
            var message = parameters.ParentConnectService.GetMessageById(model.MessageId);

            if (message == null)
            {
                return RedirectToAction("Inbox");
            }else
            {
                SelectedMessageId = message.MessageId;
                SelectedMessageRef = message.MessageRef;
            }

            if (message.UserId != CurrentUser.Id) // Means that this message is received from others (==> it's not the case that staff view their compose message)
            {
                UpdateUnreadMessageThread(message.MessageRef, message.UserId, CurrentUser.Id,
                                           model.StudentId);
            }

            if(CurrentUser.IsParent)
            {
                SetParentUnreadMessage(CurrentUser.Id, model.StudentId);
            }else
            {
                SetStaffUnreadMessage(CurrentUser.Id);
            }

            model = InitViewMessageViewModel(model);

            return View(model);
        }

        [ValidateInput(false)]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.MessageInbox)]
        public ActionResult ReplyMessage(ViewMessageViewModel model)
        {
            bool result = true;
            var message = parameters.ParentConnectService.GetMessageById(model.MessageId);

            if (message == null || (model.ReplyMessageType == 2 && string.IsNullOrEmpty(model.Body)))
            {
                return RedirectToAction("Inbox",
                                        new {replyMessageType = model.ReplyMessageType, replyMessageResult = false});
            }

            try
            {
                var newMessage = new Message
                                     {
                                         CreatedDateTime = DateTime.Now,
                                         Body = model.ReplyMessageType == 2 ? model.Body : "Acknowledgement",
                                         IsAcknowlegdeRequired = 0,
                                         IsDeleted = 0,
                                         MessageType = model.ReplyMessageType,
                                         MessageRef = message.MessageRef,
                                         Subject = message.Subject,
                                         UserId = CurrentUser.Id
                                     };

                parameters.ParentConnectService.ReplyMessage(newMessage, model.SenderId, model.StudentId);
            }catch(Exception)
            {
                result = false;
            }

            return RedirectToAction("Inbox",
                                    new { replyMessageType = model.ReplyMessageType, replyMessageResult = result });
        }

        [ValidateInput(false)]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.MessageInbox)]
        public ActionResult ReplyToAll(ViewMessageViewModel model)
        {
            bool result = true;
            var message = parameters.ParentConnectService.GetMessageById(model.MessageId);

            if (message == null || string.IsNullOrEmpty(model.Body))
            {
                return RedirectToAction("Inbox",
                                        new { replyMessageType = 2, replyMessageResult = false });
            }

            try
            {
                var newMessage = new Message
                {
                    CreatedDateTime = DateTime.Now,
                    Body = model.Body,
                    IsAcknowlegdeRequired = 0,
                    IsDeleted = 0,
                    MessageType = 2,
                    MessageRef = message.MessageRef,
                    Subject = message.Subject,
                    UserId = CurrentUser.Id
                };

                parameters.ParentConnectService.ReplyToAll(newMessage);
            }
            catch (Exception)
            {
                result = false;
            }

            return RedirectToAction("Inbox",
                                    new { replyMessageType = model.ReplyMessageType, replyMessageResult = result });
        }

        [ValidateInput(false)]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.MessageInbox)]
        public ActionResult MarkAllAsRead(int messageId)
        {
            bool result = true;
            try
            {
                parameters.ParentConnectService
                .MarkAllMessageThreadAsRead(messageId, CurrentUser.Id);
            }
            catch (Exception)
            {
                result = false;
            }

            return RedirectToAction("ViewMessage",
                                        new { messageId = messageId, sendActionType = "MarkAllAsRead", sendActionResult = result });
        }

        public ActionResult ShowSubMessageList(int messageId)
        {
            ViewBag.UserId = CurrentUser.Id;
            ViewBag.MessageId = messageId;
            return PartialView("_SubMessageList");
        }

        private void UpdateUnreadMessageThread(int messageRef, int senderId, int receiverId, int studentId)
        {
            // Update all unread messages of this thread as read
            parameters.ParentConnectService
                .UpdateMessageThreadAsRead(messageRef, senderId, receiverId, studentId);
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.MessageInbox)]
        public ActionResult DeleteMessage(string messageIds)
        {
            bool result = true;
            try
            {
                List<string> items = messageIds.Split(',').ToList();
                var deleteMessages = new List<MessageReceiver>();
                foreach (var item in items)
                {
                    int messageId = Convert.ToInt32(item.Split(';')[0]);
                    int studentId = Convert.ToInt32(item.Split(';')[1]);

                    deleteMessages.Add(new MessageReceiver
                                           {
                                               MessageId = messageId,
                                               StudentId = studentId,
                                               UserId = CurrentUser.Id
                                           });
                }

                parameters.ParentConnectService.DeleteMessages(deleteMessages);
            }catch(Exception)
            {
                result = false;
            }

            return Json(new { Success = result });
        }

        private ViewMessageViewModel InitViewMessageViewModel(ViewMessageViewModel inputModel)
        {
            var message = parameters.ParentConnectService.GetMessageById(inputModel.MessageId);
            Message mainMessage = null;

            if (message == null)
            {
                return null;
            }else
            {
                mainMessage = parameters.ParentConnectService.GetMessageById(message.MessageRef);
            }

            List<MessageInViewMessageViewModel> subMessages;

            if (message.UserId == CurrentUser.Id) // staff views his/her compose message
            {
                var messageInDetails = parameters.ParentConnectService
                    .GetMainMessageDetail(message.MessageRef, message.UserId);
                subMessages = ConvertToMessageInViewMessageViewModel(messageInDetails);
            }
            else
            {
                var messageInDetails = parameters.ParentConnectService
                    .GetSubThreadMessageDetail(message.MessageRef, message.UserId, CurrentUser.Id, inputModel.StudentId);
                subMessages = ConvertToMessageInViewMessageViewModel(messageInDetails);
            }

            var model = new ViewMessageViewModel
                            {
                                MessageId = message.MessageId,
                                MessageRef = message.MessageRef,
                                Messages = subMessages,
                                MessageType = message.MessageType,
                                SenderId = message.UserId,
                                StudentId = inputModel.StudentId,
                                IsViewSubThreadMessage = message.UserId != CurrentUser.Id,
                                IsDisable = mainMessage.IsDeleted == 1,
                                SendActionType = inputModel.SendActionType,
                                SendActionResult = inputModel.SendActionResult,
                                HasUnreadMessageInThread = false
                            };

            if (subMessages.Count > 0)
            {
                model.Subject = subMessages.First().Subject; // Get subject of main message
                model.IsAcknowledgeRequired = subMessages.Any(en => en.SenderId == CurrentUser.Id)
                                                  ? 0
                                                  : subMessages.First().IsAcknowledgeRequired;
                model.IsRepliedRequired = subMessages.First().ReplyEnabled;
                model.HasUnreadMessageInThread = (subMessages.FirstOrDefault(en => en.IsRead == 0) != null);
            }

            return model;
        }

        private List<MessageInViewMessageViewModel> ConvertToMessageInViewMessageViewModel(IQueryable<MessageInDetail> messageInDetails)
        {
            return messageInDetails
                .Select(x => new MessageInViewMessageViewModel
                                 {
                                     Body = x.Body,
                                     CreatedDateTimeString =
                                         x.CreatedDateTime.ToString("ddd, d MMM") + " at "
                                         + x.CreatedDateTime.ToString("h:mm")
                                         + x.CreatedDateTime.ToString("tt").ToLower(),
                                     IsRead = (x.SenderId == CurrentUser.Id ? 1 : x.IsRead),
                                     MessageId = x.MessageId,
                                     IsAcknowledgeRequired = x.IsAcknowledgeRequired,
                                     ReplyEnabled = x.ReplyEnabled,
                                     Sender = (x.SenderId == CurrentUser.Id ? "me" : x.Sender),
                                     Subject = x.Subject,
                                     SenderId = x.SenderId,
                                     StudentId = x.StudentId,
                                     Recipients = x.Recipients
                                 }).ToList();
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.PContactInfo)]
        public ActionResult ParentContactInfo()
        {
            if(!CurrentUser.IsParent)
            {
                return RedirectToAction("LogOn", "Account");    
            }

            var parent = parameters.UserService.GetUserById(CurrentUser.Id);
            var model = new EditParentViewModel {CurrentUserRoleId = CurrentUser.RoleId};
            Mapper.Map(parent, model);
            return View(model);
        }

        [HttpPost]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.PContactInfo)]
        [AjaxOnly]
        public ActionResult ParentContactInfo(EditParentViewModel model)
        {
            if(!CurrentUser.IsParent)
            {
                return RedirectToAction("LogOn", "Account");
            }

            var user = parameters.UserService.GetUserById(model.UserId);

            model.StateId = user.StateId.GetValueOrDefault();
            model.Password = user.Password;
            model.HashedPassword = user.HashedPassword;

            model.SetValidator(parameters.EditParentViewModelValidator);
            if (!IsValid(model))
            {
                return Json(new { Success = false, ErrorList = model.ValidationErrors });
            }

            Mapper.Map(model, user);
            user.ModifiedDate = DateTime.UtcNow;
            parameters.UserService.SaveUser(user);
            return Json(new { Success = true});
        }

        #region Compose message
        private SendResultViewModel SaveMessages(ComposeModel composeModel)
        {
            var model = new SendResultViewModel
            {
                ComposeModel = composeModel
            };

            if (composeModel.IsCompose)
            {
                Message msg = SaveMessage(composeModel.Subject, composeModel.HtmlContent, composeModel.UseRequestAck,composeModel.ReplyEnable);
                if (msg == null)
                {
                    return model;//faile to save message
                }
                if (msg.MessageId > 0)
                {
                    //storing who will receive message in MessageReceiver
                    //first need to get list of parents of student need to be sent
                    List<StudentInClassWithParentsViewModel> studentParents = null;
                    if (composeModel.Sendby.Equals("students"))
                    {
                        studentParents = GetStudentParentsByStudent(composeModel.SelectedStudentId, composeModel.ClassId);

                    }

                    if (composeModel.Sendby.Equals("group"))
                    {
                        if (CurrentUser.IsPublisher)
                        {
                            studentParents = GetStudentInStudentGroupWithParentForDistrictAdminAndPublisher(composeModel.SelectedStudentGroupId, null);
                        }
                        if (CurrentUser.RoleId == (int)Permissions.DistrictAdmin)
                        {
                            studentParents = GetStudentInStudentGroupWithParentForDistrictAdminAndPublisher(composeModel.SelectedStudentGroupId, CurrentUser.DistrictId);
                        }
                        if (CurrentUser.IsSchoolAdmin)
                        {
                            studentParents = GetStudentParentsByGroupForSchoolAdmin(composeModel.SelectedStudentGroupId, CurrentUser.Id, CurrentUser.DistrictId.Value);
                        }
                        if (CurrentUser.IsTeacher)
                        {
                            studentParents = GetStudentParentsByGroupForTeacher(composeModel.SelectedStudentGroupId, CurrentUser.Id, CurrentUser.DistrictId.Value);
                        }

                    }
                    if (studentParents == null)
                    {
                        return model;//faile to save message
                    }
                    //Start to save MessageReceiver and send email, sms

                    foreach (var studentParent in studentParents)
                    {
                        foreach (var parent in studentParent.ParentList)
                        {
                            MessageReceiver messageReceiver = new MessageReceiver();
                            messageReceiver.IsDeleted = 0;
                            messageReceiver.IsRead = 0;
                            messageReceiver.MessageId = msg.MessageId;
                            messageReceiver.StudentId = studentParent.StudentID;
                            messageReceiver.UserId = parent.ParentUserId; //Parent

                            parameters.ParentConnectService.SaveMessageReceiver(messageReceiver);
                            //model.CountMessageReceiver++;
                            //Change CountMessageReceiver, just count parent who has email or sms
                            if( (!string.IsNullOrEmpty(parent.Email))||(!string.IsNullOrEmpty(parent.Phone)))
                            {
                                model.CountMessageReceiver++;
                            }

                            if (model.ComposeModel.UseEmail.Equals("true"))
                            {
                                if (parent.Email.Length > 0)
                                {
                                    model.CountEmail++;
                                }
                            }
                            if (model.ComposeModel.UseSms.Equals("true"))
                            {
                                if (parent.Phone.Length > 0)
                                {
                                    model.CountSms++;
                                }
                            }

                        }
                    }
                    //Twilio must use a number to send sms,this number is stored in web.config, it's necessary to check if this number is existing or not
                }
            }
            return model;
        }

        private Message SaveMessage(string subject, string htmlContent, string useRequestAck, string replyEnable)
        {
            Message msg = new Message();
            msg.Subject = subject;
            msg.Body = htmlContent;
            msg.UserId = CurrentUser.Id;
            msg.MessageType = 1;
            msg.CreatedDateTime = DateTime.Now;
            msg.IsAcknowlegdeRequired = 0;
            if (useRequestAck.Equals("true"))
            {
                msg.IsAcknowlegdeRequired = 1;
            }
            if (replyEnable.Equals("true"))
            {
                msg.ReplyEnabled = 1;
            }
            parameters.ParentConnectService.SaveMessage(msg);
            //Update messageref
            msg.MessageRef = msg.MessageId;
            parameters.ParentConnectService.SaveMessage(msg);
            return msg;

        }

        private List<StudentInClassWithParentsViewModel> GetStudentParentsByStudent(string selectedStudentId, string classId)
        {
            List<StudentInClassWithParentsViewModel> result = new List<StudentInClassWithParentsViewModel>();

            int selectedClassId = 0;
            selectedClassId = int.Parse(classId);

            List<int> studentIdList = new List<int>();
            string[] ids = selectedStudentId.Split(',');
            if (ids != null)
            {
                foreach (var id in ids)
                {
                    studentIdList.Add(Int32.Parse(id));
                }
            }

            //Get the student along with his/her parents
            //get only active students in current district terms and whith in the class
            var students = parameters.ParentConnectService.GetActiveStudentsInClass(studentIdList, selectedClassId).Select(x => new StudentInClassWithParentsViewModel
            {
                ID = x.ID,
                FirstName = x.FirstName,
                //MiddleName = x.MiddleName,
                LastName = x.LastName,
                Gender = x.Gender,
                LocalId = x.Code,
                ParentList = ParseParents(x.Parents),
                StudentID = x.StudentID,

            });
            result = students.ToList();
            return result;
        }

        private List<StudentInClassWithParentsViewModel> GetStudentInStudentGroupWithParentForDistrictAdminAndPublisher(string selectedStudentGroupId, int? districtId)
        {
            List<StudentInClassWithParentsViewModel> result = new List<StudentInClassWithParentsViewModel>();

            int groupId = 0;
            groupId = int.Parse(selectedStudentGroupId);


            //Get the student in group along with his/her parents
            //get only active students in current district terms 
            var students = parameters.ParentConnectService.GetStudentInStudentGroupWithParentForDistrictAdminAndPublisher(groupId, districtId).Select(x => new StudentInClassWithParentsViewModel
            {
                ID = x.ReportGroupStudentID,
                FirstName = x.FirstName,
                //MiddleName = x.MiddleName,
                LastName = x.LastName,
                Gender = x.Gender,
                LocalId = x.Code,
                ParentList = ParseParents(x.Parents),
                StudentID = x.StudentID,

            });
            result = students.ToList();
            return result;
        }

        private List<StudentInClassWithParentsViewModel> GetStudentParentsByGroupForSchoolAdmin(string selectedStudentGroupId, int schoolAdminId, int districtId)
        {
            List<StudentInClassWithParentsViewModel> result = new List<StudentInClassWithParentsViewModel>();

            int groupId = 0;
            groupId = int.Parse(selectedStudentGroupId);


            //Get the student in group along with his/her parents
            //get only active students in current district terms 
            var students = parameters.ParentConnectService.GetStudentInStudentGroupWithParentForSchoolAdmin(groupId, schoolAdminId, districtId).Select(x => new StudentInClassWithParentsViewModel
            {
                ID = x.ReportGroupStudentID,
                FirstName = x.FirstName,
                //MiddleName = x.MiddleName,
                LastName = x.LastName,
                Gender = x.Gender,
                LocalId = x.Code,
                ParentList = ParseParents(x.Parents),
                StudentID = x.StudentID,

            });
            result = students.ToList();
            return result;
        }

        private List<StudentInClassWithParentsViewModel> GetStudentParentsByGroupForTeacher(string selectedStudentGroupId, int teacherId, int districtId)
        {
            List<StudentInClassWithParentsViewModel> result = new List<StudentInClassWithParentsViewModel>();

            int groupId = 0;
            groupId = int.Parse(selectedStudentGroupId);


            //Get the student in group along with his/her parents
            //get only active students in current district terms 
            var students = parameters.ParentConnectService.GetStudentInStudentGroupWithParentForTeacher(groupId, teacherId, districtId).Select(x => new StudentInClassWithParentsViewModel
            {
                ID = x.ReportGroupStudentID,
                FirstName = x.FirstName,
                //MiddleName = x.MiddleName,
                LastName = x.LastName,
                Gender = x.Gender,
                LocalId = x.Code,
                ParentList = ParseParents(x.Parents),
                StudentID = x.StudentID,

            });
            result = students.ToList();
            return result;
        }
        public List<StudentParentViewModel> ParseParents(string xmlParent)
        {
            if (string.IsNullOrWhiteSpace(xmlParent)) return new List<StudentParentViewModel>();
            var xdoc = XDocument.Parse(xmlParent);
            var result = new List<StudentParentViewModel>();

            foreach (var node in xdoc.Element("Parents").Elements("Parent"))
            {
                var p = new StudentParentViewModel();
                p.ParentUserId = GetIntValue(node.Element("ParentUserID"));
                p.Email = GetStringValue(node.Element("Email"));
                p.Phone = GetStringValue(node.Element("Phone"));
                p.FirstName = GetStringValue(node.Element("NameFirst"));
                p.LastName = GetStringValue(node.Element("NameLast"));
                result.Add(p);
            }

            return result;
        }

        private bool GetBoolValue(XElement element)
        {
            if (element == null) return false;
            return element.Value == "1" || element.Value == "true";
        }

        private string GetStringValue(XElement element)
        {
            if (element == null) return string.Empty;
            return element.Value;
        }

        private int GetIntValue(XElement element)
        {
            if (element == null) return 0;
            return Convert.ToInt32(element.Value);
        }

        public ActionResult NotifyEmailSms(ComposeModel model)
        {
            //To make this function run asynchronously, there's a way to delegate to another partial view,this partial view will call another action method to do
            //return PartialView("_SendNotifycation", model);
            int roleId = CurrentUser.RoleId;
            int currentUserId = CurrentUser.Id;
            int districtId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;

            Thread myNewThread = new Thread(() => SendNotifycation(roleId, currentUserId, districtId, model));
            myNewThread.Start();
            return PartialView("_SendNotifycation");

        }

        private void SendNotifycation(int roleId, int currentUserId, int districtId, ComposeModel model)
        {

            if (model.IsCompose)
            {
                List<StudentInClassWithParentsViewModel> studentParents = null;
                if (model.Sendby.Equals("students"))
                {
                    studentParents = GetStudentParentsByStudent(model.SelectedStudentId, model.ClassId);

                }

                if (model.Sendby.Equals("group"))
                {
                    if (roleId == (int)Permissions.Publisher)
                    {
                        studentParents =
                            GetStudentInStudentGroupWithParentForDistrictAdminAndPublisher(
                                model.SelectedStudentGroupId, null);
                    }
                    if (roleId == (int)Permissions.DistrictAdmin)
                    {
                        studentParents =
                            GetStudentInStudentGroupWithParentForDistrictAdminAndPublisher(
                                model.SelectedStudentGroupId, districtId);
                    }
                    if (roleId == (int)Permissions.SchoolAdmin)
                    {
                        studentParents = GetStudentParentsByGroupForSchoolAdmin(model.SelectedStudentGroupId,
                                                                                currentUserId,
                                                                                districtId);
                    }
                    if (roleId == (int)Permissions.Teacher)
                    {
                        studentParents = GetStudentParentsByGroupForTeacher(model.SelectedStudentGroupId,
                                                                            currentUserId,
                                                                            districtId);
                    }

                }
                //Prepare to save MessageReceiver  and send email or sms
                var emailSender = new EmailSender(parameters.DistrictConfigurationService, parameters.DistrictService, parameters.UserService, districtId, currentUserId, model.From, model.Subject);
                var smsSender = new SmsSender(parameters.DistrictConfigurationService, parameters.DistrictService, parameters.UserService, districtId, currentUserId, model.Subject);


                //Start to save MessageReceiver and send email, sms

                foreach (var studentParent in studentParents)
                {
                    foreach (var parent in studentParent.ParentList)
                    {

                        //Send email or SMS
                        if (model.UseEmail.Equals("true"))
                        {
                            if (parent.Email.Length > 0)
                            {
                                emailSender.SendGridEmail(parent.FirstName, parent.LastName, parent.Email);
                            }
                        }
                        if (model.UseSms.Equals("true") && smsSender.FromNumber.Length > 0)
                        {
                            if (parent.Phone.Length > 0)
                            {
                                //Send sms
                                smsSender.Send(parent.Phone);
                            }

                        }
                    }
                }
            }


        }
        #endregion
    }        
}
