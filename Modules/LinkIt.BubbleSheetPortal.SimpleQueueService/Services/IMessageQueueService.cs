using Amazon.SQS.Model;
using LinkIt.BubbleSheetPortal.Models.DTOs.MessageQueue;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.SimpleQueueService.Services
{
    public interface IMessageQueueService
    {
        SendMessageResponse SendMessage(string queueUrl, MessageQueueDto email);
        Task<SendMessageResponse> SendMessageAsync(string queueUrl, MessageQueueDto email);
        Task<HttpStatusCode> DeleteMessageAsync(string queueUrl, string receiptHandle);
        Task<HttpStatusCode> DeleteMessageBatchAsync(string queueUrl, IEnumerable<string> receiptHandles);
        void SendMessageBatch(string queueUrl, IEnumerable<MessageQueueDto> emails);
    }
}
