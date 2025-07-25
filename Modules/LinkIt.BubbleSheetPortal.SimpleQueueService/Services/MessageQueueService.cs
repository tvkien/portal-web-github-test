using Amazon.SQS;
using Amazon.SQS.Model;
using LinkIt.BubbleSheetPortal.Models.DTOs.MessageQueue;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.SimpleQueueService.Services
{
    public class MessageQueueService : IMessageQueueService
    {
        private readonly IAmazonSQS _amazonSQS;

        public MessageQueueService()
        {
            _amazonSQS = new AmazonSQSClient();
        }

        public SendMessageResponse SendMessage(string queueUrl, MessageQueueDto email)
        {
            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = JsonConvert.SerializeObject(email)
            };

            var sendMessageResponse = _amazonSQS.SendMessage(sendMessageRequest);
            return sendMessageResponse;
        }

        public async Task<SendMessageResponse> SendMessageAsync(string queueUrl, MessageQueueDto email)
        {
            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = JsonConvert.SerializeObject(email)
            };
            var sendMessageResponse = await _amazonSQS.SendMessageAsync(sendMessageRequest);
            return sendMessageResponse;
        }

        public void SendMessageBatch(string queueUrl, IEnumerable<MessageQueueDto> emails)
        {
            List<SendMessageBatchRequestEntry> sendMessageBatchRequestEntries = new List<SendMessageBatchRequestEntry>();
            var index = 1;
            foreach (var email in emails)
            {
                sendMessageBatchRequestEntries.Add(new SendMessageBatchRequestEntry
                {
                    Id = index.ToString(),
                    MessageBody = JsonConvert.SerializeObject(email),
                    MessageAttributes = new Dictionary<string, MessageAttributeValue>()
                    {
                        {"LICode", new MessageAttributeValue{ StringValue = HttpContext.Current.GetLICodeFromRequest(), DataType = "String"} }
                    }
                });
                index++;
            }
            var sendMessageBatchRequest = new SendMessageBatchRequest
            {
                QueueUrl = queueUrl,
                Entries = sendMessageBatchRequestEntries
            };
            _amazonSQS.SendMessageBatch(sendMessageBatchRequest);
        }

        public async Task<HttpStatusCode> DeleteMessageAsync(string queueUrl, string receiptHandle)
        {
            var deleteMessageRequest = new DeleteMessageRequest
            {
                QueueUrl = queueUrl,
                ReceiptHandle = receiptHandle
            };

            var deleteMessageResponse = await _amazonSQS.DeleteMessageAsync(deleteMessageRequest);
            return deleteMessageResponse.HttpStatusCode;
        }

        public async Task<HttpStatusCode> DeleteMessageBatchAsync(string queueUrl, IEnumerable<string> receiptHandles)
        {
            List<DeleteMessageBatchRequestEntry> deleteMessageBatchRequestEntries = new List<DeleteMessageBatchRequestEntry>();
            foreach (var receiptHandle in receiptHandles)
            {
                deleteMessageBatchRequestEntries.Add(new DeleteMessageBatchRequestEntry { ReceiptHandle = receiptHandle });
            }
            DeleteMessageBatchRequest deleteMessageBatchRequest = new DeleteMessageBatchRequest
            {
                QueueUrl = queueUrl,
                Entries = deleteMessageBatchRequestEntries
            };
            var deleteMessageResponse = await _amazonSQS.DeleteMessageBatchAsync(deleteMessageBatchRequest);
            return deleteMessageResponse.HttpStatusCode;
        }
    }
}
