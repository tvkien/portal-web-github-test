using System;

namespace LinkIt.BubbleSheetPortal.Common.Enum
{
    [Flags]
    public enum NavigatorKeywords
    {
        None = 1 << 0,
        Year = 1 << 1,
        Category = 1 << 2,
        Keyword = 1 << 3,
        Period = 1 << 4,
        Type = 1 << 5,
        School = 1 << 6,
        Suffix = 1 << 7,
    }
}
