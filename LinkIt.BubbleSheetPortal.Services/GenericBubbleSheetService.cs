using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class GenericBubbleSheetService
    {
        private readonly IReadOnlyRepository<GenericBubbleSheet> repository;
        private readonly IReadOnlyRepository<GenericBubbleSheetACTSAT> _repositoryGenericBubblesheetACTSAT;

        public GenericBubbleSheetService(IReadOnlyRepository<GenericBubbleSheet> repository, IReadOnlyRepository<GenericBubbleSheetACTSAT> repositoryGenericBubblesheetACTSAT)
        {
            this.repository = repository;
            _repositoryGenericBubblesheetACTSAT = repositoryGenericBubblesheetACTSAT;
        }

        public IQueryable<GenericBubbleSheet> GetGenericSheetsByTicket(string ticket, int? classId)
        {
            if (classId.HasValue)
            {
                return repository.Select().Where(x => x.Ticket.Equals(ticket) && x.ClassID == classId.Value);
            }
            return repository.Select().Where(x => x.Ticket.Equals(ticket));
        }

        public GenericBubbleSheet GetGenericSheetByBubbleSheetId(int bubbleSheetFileId)
        {
            return repository.Select().FirstOrDefault(x => x.BubbleSheetFileId.Equals(bubbleSheetFileId));
        }

        public GenericBubbleSheetACTSAT GetGenericSheetACTSATByBubbleSheetId(int bubbleSheetFileId)
        {
            return _repositoryGenericBubblesheetACTSAT.Select().FirstOrDefault(x => x.BubbleSheetFileId.Equals(bubbleSheetFileId));
        }

        public IQueryable<GenericBubbleSheetACTSAT> GetGenericSheetsACTSATByTicketAndClassId(string ticket, int classId)
        {
            return
                _repositoryGenericBubblesheetACTSAT.Select()
                    .Where(x => x.Ticket.Equals(ticket) && (classId == 0 || x.ClassID == classId));
        }

        public GenericBubbleSheetACTSAT GetGenericSheetACTSATByBubbleSheetIdAndTicket(string ticket, int bubbleSheetFileId)
        {
            return _repositoryGenericBubblesheetACTSAT.Select().FirstOrDefault(x => x.Ticket.Equals(ticket) && x.BubbleSheetFileId.Equals(bubbleSheetFileId));
        }
    }
}
