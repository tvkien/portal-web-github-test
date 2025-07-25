namespace LinkIt.BubbleSheetPortal.Models
{
    public enum ErrorCode
    {
        BarcodeCouldNotBeRead = 0, 
        CouldNotFindAnchors = 8,    
        DidNotFindCorrectNumberOfQuestions = 64, 
        CouldNotFindAllQuestions = 4,  
        DidNotFindCorrectNumberOfBubblesForQuestion = 32,   
        FileWasCorrupt = 2, 
        FileWasUnreadable = 1,  
        CouldNotLocateBubbles = 16,  
        UnreadableRosterPosition = -1
    }
}
