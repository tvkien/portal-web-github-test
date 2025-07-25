using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.BubbleSheetReview;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class BubbleSheetStudentResultsService
    {
        private readonly IBubbleSheetStudentResultsRepository repository;
        private readonly IBubbleSheetRepository bubbleSheetRepository;

        public BubbleSheetStudentResultsService(IBubbleSheetStudentResultsRepository repository, IBubbleSheetRepository bubbleSheetRepository)
        {
            this.repository = repository;
            this.bubbleSheetRepository = bubbleSheetRepository;
        }

        public IQueryable<BubbleSheetStudentResults> GetBubbleSheetStudentResultsByTicketAndClassId(string ticket, int classId)
        {
            return repository.Select().Where(x => x.Ticket.Equals(ticket) && x.ClassId.Equals(classId));
        }

        public List<BubbleSheetStudentResults> GetStudentsOfBubbleSheetByTicket(string ticket, int classID)
        {
            var result = repository.GetStudentsOfBubbleSheetByTicket(ticket, classID);
            return result;
        }

        public List<BubbleSheetStudentResults> GetStudentsOfBubbleSheetByTicketNoStatus(string ticket, int classID)
        {
            var result = repository.GetStudentsOfBubbleSheetByTicketNoStatus(ticket, classID);
            return result;
        }

        public List<BubbleSheetStudentResults> GetBubbleSheetStudentStatus(string studentIdList, string ticket, int classID)
        {
            var result = repository.GetBubbleSheetStudentStatus(studentIdList, ticket, classID);
            return result;
        }

        public BubbleSheetReviewStatusCount GetBubbleSheetReviewStatusCount(string ticket, int classID)
        {
            var statusList = repository.GetBubbleSheetStudentStatusV2(ticket, classID);
            return new BubbleSheetReviewStatusCount
                   {
                       Finished = statusList.Count(x => x.Equals("finish", StringComparison.OrdinalIgnoreCase)),
                       Ungraded = statusList.Count(x => x.Equals("notgraded", StringComparison.OrdinalIgnoreCase)),
                       Review = statusList.Count(x => x.Equals("review", StringComparison.OrdinalIgnoreCase))
                   };
        }

        public List<BubbleSheetStudentResults> GetStudentsOfBubbleSheetByTicketWithStatusInBatch(string ticket,
            int classID)
        {
            var result = repository.GetStudentsOfBubbleSheetByTicketNoStatus(ticket, classID);
            BuildStudentStatusSegment(result, ticket, classID);
            return result;
        }

        public List<BubbleSheetStudentResults> GetStudentStatusForReviewPageByTicket(string ticket, int classID)
        {
            var listStudent =
                bubbleSheetRepository.Select()
                    .Where(x => x.Ticket.Equals(ticket) && x.ClassId.Equals(classID) && x.StudentId != null)
                    .Select(x => new BubbleSheetStudentResults
                                 {
                                     StudentId = x.StudentId.GetValueOrDefault(),
                                     AnsweredCount = 0,
                                     TotalCount = 0,
                                     Status = string.Empty
                                 }).ToList();
            BuildStudentStatusSegment(listStudent, ticket, classID);
            return listStudent;
        }

        private void BuildStudentStatusSegment(List<BubbleSheetStudentResults> data, string ticket, int classId, int batchCount = 50)
        {
            if (data == null || data.Count <= 0)
                return;

            int iCounter = 0;
            var item = data.Skip(iCounter * batchCount).Take(batchCount).ToList();
            while (item.Count > 0)
            {
                string studentIdList = string.Join(",", item.Select(o => o.StudentId));
                var studentStatus = GetBubbleSheetStudentStatus(studentIdList, ticket, classId);
                foreach (var s in studentStatus)
                {
                    var v = data.First(o => o.StudentId == s.StudentId);
                    if (string.IsNullOrEmpty(v.Status))
                    {
                        v.Status = s.Status;
                    }
                    else
                    {
                        v.Status += "|" + s.Status;
                    }
                }
                iCounter += 1;
                item = data.Skip(iCounter * batchCount).Take(batchCount).ToList();
            }

            foreach (var m in data)
            {
                m.BubbleSheetFinalStatus = repository.BuildBBSFinalStatus(m.Status);
            }
        }
    }
}