using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.BubbleSheetReview;
using Lokad.Cloud.Storage;


namespace LinkIt.BubbleSheetPortal.Services
{
    public class ReadResultService
    {
        public CloudEntity<ReadResult> CreateNewBubbleSheetFileCloudEntity(BubbleSheetFile bubbleSheetFile)
        {
            return new CloudEntity<ReadResult>
            {
                PartitionKey = bubbleSheetFile.InputFilePath,
                RowKey = bubbleSheetFile.UrlSafeOutputFileName,
                Value = new ReadResult
                {
                    Barcode1 = bubbleSheetFile.Barcode1,
                    Barcode2 = bubbleSheetFile.Barcode2,
                    UserId = bubbleSheetFile.UserId.GetValueOrDefault(),
                    InputFileName = bubbleSheetFile.InputFileName,
                    InputPath = bubbleSheetFile.InputFilePath,
                    OutputFile = bubbleSheetFile.OutputFileName,
                    DistrictId = bubbleSheetFile.DistrictId.GetValueOrDefault(),
                    RosterPosition = bubbleSheetFile.RosterPosition.ToString(),
                    IsRoster = bubbleSheetFile.RosterPosition > 0
                }
            };
        }

        public ReadResult CreateNewBubbleSheetFileReadResult(BubbleSheetFile bubbleSheetFile)
        {
            return new ReadResult
            {
                Barcode1 = bubbleSheetFile.Barcode1,
                Barcode2 = bubbleSheetFile.Barcode2,
                UserId = bubbleSheetFile.UserId.GetValueOrDefault(),
                InputFileName = bubbleSheetFile.InputFileName,
                InputPath = bubbleSheetFile.InputFilePath,
                OutputFile = bubbleSheetFile.OutputFileName,
                DistrictId = bubbleSheetFile.DistrictId.GetValueOrDefault(),
                RosterPosition = bubbleSheetFile.RosterPosition.ToString(),
                IsRoster = bubbleSheetFile.RosterPosition > 0
            };
        }

        public ReadResult CreateNewBubbleSheetFile(BubbleSheetFile bubbleSheetFile)
        {
            return new ReadResult
                   {
                       Barcode1 = bubbleSheetFile.Barcode1,
                       Barcode2 = bubbleSheetFile.Barcode2,
                       UserId = bubbleSheetFile.UserId.GetValueOrDefault(),
                       InputFileName = bubbleSheetFile.InputFileName,
                       InputPath = bubbleSheetFile.InputFilePath,
                       OutputFile = bubbleSheetFile.OutputFileName,
                       DistrictId = bubbleSheetFile.DistrictId.GetValueOrDefault(),
                       RosterPosition = bubbleSheetFile.RosterPosition.ToString(),
                       IsRoster = bubbleSheetFile.RosterPosition > 0
                   };
        }

        public void SetBubbleSheetCloudEntityQuestions(CloudEntity<ReadResult> bubbleSheetCloudEntity, int questionCount)
        {
            var readResultQuestions = new List<ReadResultItem>();

            for (var i = 0; i < questionCount; i++)
            {
                readResultQuestions.Add(new ReadResultItem { ProblemNumber = (i + 1) });
            }
            bubbleSheetCloudEntity.Value.Questions = readResultQuestions.ToList();
        }

        public void SetBubbleSheetReadResultQuestions(ReadResult bubbleSheetCloudEntity, int questionCount)
        {
            var readResultQuestions = new List<ReadResultItem>();

            for (var i = 0; i < questionCount; i++)
            {
                readResultQuestions.Add(new ReadResultItem { ProblemNumber = (i + 1) });
            }
            bubbleSheetCloudEntity.Questions = readResultQuestions.ToList();
        }

        public void SetBubbleSheetQuestions(ReadResult bubbleSheetCloudEntity, int questionCount)
        {
            var readResultQuestions = new List<ReadResultItem>();

            for (var i = 0; i < questionCount; i++)
            {
                readResultQuestions.Add(new ReadResultItem { ProblemNumber = (i + 1) });
            }
            bubbleSheetCloudEntity.Questions = readResultQuestions.ToList();
        }

        public GradeRequest CreateGradeRequest(CloudEntity<ReadResult> bubbleSheetFileCloudEntity)
        {
            var gradeRequest = new GradeRequest { ReadResults = new List<ReadResult>() };
            gradeRequest.ReadResults.Add(bubbleSheetFileCloudEntity.Value);

            return gradeRequest;
        }
    }
}