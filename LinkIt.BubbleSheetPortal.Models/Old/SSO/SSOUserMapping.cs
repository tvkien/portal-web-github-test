namespace LinkIt.BubbleSheetPortal.Models.SSO
{
    public class SSOUserMapping
    {
        public int SSOUserMappingID { get; set; }
        public int UserID { get; set; }
        public string ADUsername { get; set; }
        public int DistrictID { get; set; }
        public virtual User User { get; set; }
        public virtual District District { get; set; }
        public string Type { get; set; }
    }
}
