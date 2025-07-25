using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models.Algorithmic;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.Algorithmic
{
    public static class AlgorithmicHelper
    {     
        public static List<AlgorithmicCorrectAnswer> ConvertToAlgorithmicCorrectAnswers(int qtiSchemaID, List<AlgorithmicQuestionExpression> algorithmQuestions)
        {
            IAlgorithmicConditionParser parser;
            switch (qtiSchemaID)
            {
                // single choice, true,false
                case (int)QtiSchemaEnum.MultipleChoice:
                case (int)QtiSchemaEnum.InlineChoice:
                    parser = new AlgorithmicSingleChoiceParser();
                    break;

                // multiple choice
                case (int)QtiSchemaEnum.MultiSelect:
                case (int)QtiSchemaEnum.TextHotSpot:
                case (int)QtiSchemaEnum.ImageHotSpot:
                case (int)QtiSchemaEnum.TableHotSpot:
                case (int)QtiSchemaEnum.NumberLineHotSpot:
                case (int)QtiSchemaEnum.ChoiceMultipleVariable:
                    parser = new AlgorithmicMultipleChoiceParser();
                    break;

                // drag and drop
                case (int)QtiSchemaEnum.DragAndDrop:
                case (int)QtiSchemaEnum.DragAndDropNumerical:
                case (int)QtiSchemaEnum.DragAndDropSequence:
                    parser = new AlgorithmicDragDropStandardParser();
                    break;
                
                // fill in blank
                case (int)QtiSchemaEnum.TextEntry:
                    parser = new AlgorithmicTextEntryParser();
                    break;
                default:
                    parser = new AlgorithmicSingleChoiceParser();
                    break;

            }
            return parser.ConvertToAlgorithmicCorrectAnswers(algorithmQuestions);
        }
    }
}