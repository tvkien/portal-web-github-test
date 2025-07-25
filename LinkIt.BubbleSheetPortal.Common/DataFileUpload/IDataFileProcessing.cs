using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Common.DataFileUpload
{
    public interface IDataFileProcessing
    {
        DataFileUploaderResult Process(DataFileUploaderParameter parameter);
    }
}
