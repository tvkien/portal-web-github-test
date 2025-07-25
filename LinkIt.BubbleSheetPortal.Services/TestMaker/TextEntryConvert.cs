//using System.Linq;
//using LinkIt.BubbleSheetPortal.Common;
//using LinkIt.BubbleSheetPortal.Models.TestMaker;

//namespace LinkIt.BubbleSheetPortal.Services.TestMaker
//{
//    public class TextEntryConvert : QTIItemConvert
//    {
//        protected override void Convert(QTIItemTestMaker qtiItem)
//        {
//            if (qtiItem == null || qtiItem.AssessmentItem == null) return;
//            var responseIdentifiers = qtiItem.AssessmentItem.ResponseIdentifiers;
//            var textEntryInteraction = EnumUtils.GetDescription(typeof(QTISchemaEnum));
//            if (responseIdentifiers == null) return;

//            var textEntryIdentifier = responseIdentifiers.FirstOrDefault(o => o.TagName == textEntryInteraction);
//            if (textEntryIdentifier == null) return;

//            var responseDeclarations = qtiItem.AssessmentItem.ResponseDeclarations;
//            if (responseDeclarations == null || responseDeclarations.Count == 0) return;
//            var responseDeclaration =
//                responseDeclarations.FirstOrDefault(o => o.Identifier == textEntryIdentifier.Identifier);
//            if (responseDeclaration == null || responseDeclaration.CorrectResponse == null) return;

//            var values = responseDeclaration.CorrectResponse.Values;
//            if (values == null || values.Count == 0) return;
//            qtiItem.CorrectAnswer = values.FirstOrDefault();
//            qtiItem.PointsPossible = ConvertValue.ToInt(responseDeclaration.PointsValue);
//            qtiItem.ResponseIdentifier = responseDeclaration.Identifier;
//            qtiItem.ResponseProcessingTypeID =
//                (int) EnumUtils.FromDescription<ResponseProcessingTypeEnum>(responseDeclaration.Method);
//            qtiItem.ResponseProcessing = GetResponseProcessingXml(responseDeclaration);
//            foreach (var value in values)
//            {
//                var qtiItemAnswerScore = new QTIItemAnswerScoreTestMaker
//                {
//                    Answer = value,
//                    Score = ConvertValue.ToInt(responseDeclaration.PointsValue),
//                    ResponseIdentifier = responseDeclaration.Identifier
//                };

//                qtiItem.QTIITemAnswerScoreTestMakers.Add(qtiItemAnswerScore);
//            }
//        }
//    }
//}
