namespace LinkIt.BubbleSheetPortal.Models.DTOs.UserGroup
{
    public class XLIModuleDto
    {
        public int Id { get; set; }
        public int XLIAreaId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public string DisplayTooltip { get; set; }
        public int ModuleOrder { get; set; }
    }
}
