namespace LinkIt.BubbleSheetPortal.Models.DTOs.SSO
{
    public class ClassLinkMapping
    {
        public int[] LinkItRoleIDs { get; set; }

        public Mapping[] Mapping { get; set; }
    }

    public class Mapping
    {
        public int Priority { get; set; }

        public string ClassLinkField { get; set; }

        public string LinkItField { get; set; }
    }
}
