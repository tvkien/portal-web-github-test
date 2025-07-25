using LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories;
using LinkIt.BubbleSheetPortal.Models.Isolating;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Services
{
    public class PostAnswerLogService : IPostAnswerLogService
    {
        private IPostAnswerLogDynamo _dynamoRepository;

        public PostAnswerLogService(IPostAnswerLogDynamo dynamo)
        {
            _dynamoRepository = dynamo;
        }

        public IQueryable<IsolatingPostAnswerLogDTO> GetPostAnswerLogs(int qtiOnlineTestSessionID)
        {
            var result = new List<IsolatingPostAnswerLogDTO>();
            var data = _dynamoRepository.GetPostAnswerLogs(qtiOnlineTestSessionID);
            foreach (var item in data)
            {
                var dto = new IsolatingPostAnswerLogDTO()
                {
                    DumpCol = item.DumpCol,
                    QTIOnlineTestSessionID = item.QTIOnlineTestSessionID,
                    VirtualQuestionID = item.VirtualQuestionID,
                    Answer = item.Answer,
                    AnswerImage = item.AnswerImage,
                    Timestamp = item.Timestamp,
                };
                result.Add(dto);
            }

            return result.AsQueryable();
        }
    }
}
