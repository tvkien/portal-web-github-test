//using System.Linq;
//using LinkIt.BubbleSheetPortal.Common;
//using LinkIt.BubbleSheetPortal.Models.TestMaker;

//namespace LinkIt.BubbleSheetPortal.Services.TestMaker
//{
//    public class ComplexConvert : QTIItemConvert
//    {
//        protected override void Convert(QTIItemTestMaker qtiItem)
//        {
//            if (qtiItem == null || qtiItem.AssessmentItem == null) return;
//            var responseDeclarations = qtiItem.AssessmentItem.ResponseDeclarations;
//            if (responseDeclarations == null || responseDeclarations.Count == 0) return;
//            var responseIdentifiers = qtiItem.AssessmentItem.ResponseIdentifiers;
//            if (responseIdentifiers == null || responseIdentifiers.Count == 0) return;
//            qtiItem.CorrectAnswer = string.Empty;
//            qtiItem.ResponseIdentifier = "multi";
//            qtiItem.ResponseProcessingTypeID = (int)ResponseProcessingTypeEnum.Default;
//            qtiItem.ResponseProcessing = "<process  method=\"default\" />";
//            qtiItem.PointsPossible = 0;
//            foreach (var responseIdentifier in responseIdentifiers)
//            {
//                var responseDeclaration =
//                responseDeclarations.FirstOrDefault(o => o.Identifier == responseIdentifier.Identifier);
//                if (responseDeclaration == null || responseDeclaration.CorrectResponse == null) continue;

//                var qtiItemSubTestMaker = new QTIItemSubTestMaker
//                {
//                    QTISchemaID = GetQtiSchemaID(responseIdentifier.TagName),
//                    ResponseIdentifier = responseIdentifier.Identifier,
//                    ResponseProcessing = GetResponseProcessingXml(responseDeclaration),
//                    ResponseProcessingTypeID = (int)EnumUtils.FromDescription<ResponseProcessingTypeEnum>(responseDeclaration.Method)
//                };

//                var values = responseDeclaration.CorrectResponse.Values;
//                if (values == null || values.Count == 0) return;
//                qtiItemSubTestMaker.CorrectAnswer = values.FirstOrDefault();
//                foreach (var value in values)
//                {
//                    var qtiItemAnswerScore = new QTIItemAnswerScoreTestMaker
//                    {
//                        Answer = value,
//                        Score = ConvertValue.ToInt(responseDeclaration.PointsValue),
//                        ResponseIdentifier = responseDeclaration.Identifier
//                    };

//                    qtiItem.PointsPossible += qtiItemAnswerScore.Score;
//                    qtiItem.QTIITemAnswerScoreTestMakers.Add(qtiItemAnswerScore);
//                }
//            }
//        }
//    }
//}
