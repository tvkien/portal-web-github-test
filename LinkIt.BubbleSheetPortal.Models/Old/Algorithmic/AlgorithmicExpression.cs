namespace LinkIt.BubbleSheetPortal.Models.Algorithmic
{
    public class AlgorithmicExpression
    {
        public int? QtiItemAlgorithmID { get; set; }
        public int? VirtualQuestionAlgorithmID { get; set; }
        public string Expression { get; set; }
        public int PointEarned { get; set; }
        public string Rules { get; set; }
        public int Order { get; set; }
    }
}
