using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Services.CommonServices
{
    public class BubbleSheetCommonService
    {
        public string BuildInputFileName(string fileName)
        {
            var splittedName = fileName.Split(new char[] { '.' });
            var extension = splittedName.Last();
            var name = Path.GetFileNameWithoutExtension(fileName);
            return string.Format("{0}-{1}.{2}", name, DateTime.UtcNow.ToString("yyyy-MM-dd-hh-mm-ss"), extension);
        }
    }
}
