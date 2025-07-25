using System.ComponentModel;

namespace LinkIt.BubbleSheetPortal.Models.TestMaker
{
    public enum QTISchemaEnum
    {
        [Description("choiceInteraction")]
        Choice = 1,

        [Description("choiceInteraction")]
        ChoiceMultiple = 3,

        [Description("inlineChoiceInteraction")]
        InlineChoice = 8,

        [Description("textEntryInteraction")]
        TextEntry = 9,

        [Description("extendedTextInteraction")]
        ExtendedText = 10,

        [Description("complex")]
        UploadComposite = 21,

        [Description("partialCredit")]
        DragAndDropPartialCredit = 30,

        [Description("textHotSpot")]
        TextHotSpot = 31,
       

        [Description("imageHotSpot")]
        ImageHotSpot = 32,         
        
        [Description("tableHotSpot")]
        TableHotSpot = 33,

        [Description("numberLineHotSpot")]
        NumberLineHotSpot = 34,
        [Description("dragDropNumerical")]
        DragDropNumerical = 35,
        [Description("dragDropSequence")]
        DragDropSequence = 36,

        [Description("choiceInteraction")]
        ChoiceMultipleVariable = 37
       
    }
}
