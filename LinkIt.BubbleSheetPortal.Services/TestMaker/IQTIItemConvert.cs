using System;
using LinkIt.BubbleSheetPortal.Models.TestMaker;

namespace LinkIt.BubbleSheetPortal.Services.TestMaker
{
    public interface IQTIItemConvert
    {
        QTIItemTestMaker ConvertFromXmlContent(string xmlContent);
    }
}
