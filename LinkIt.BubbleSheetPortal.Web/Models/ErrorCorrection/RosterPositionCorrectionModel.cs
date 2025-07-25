namespace LinkIt.BubbleSheetPortal.Web.Models.ErrorCorrection
{
    public class RosterPositionCorrectionModel
    {
        public string InputFilePath { get; set; }
        public string UrlSafeOutputFileName { get; set; }
        public int NewRosterPosition { get; set; }
    }
}