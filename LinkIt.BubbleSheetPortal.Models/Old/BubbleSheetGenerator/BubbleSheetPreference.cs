using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Models.BubbleSheetGenerator
{
    public class BubbleSheetPreference
    {
        public int testExtract_gradebook { get; set; }
        public int testExtract_studentRecord { get; set; }
        public int testExtractExportRawScore { get; set; }
        public string questiongrouplabelschema { get; set; }

        public bool? isNumberQuestions { get; set; }

        public string ToXML()
        {
            var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(this.GetType());
            serializer.Serialize(stringwriter, this, emptyNamepsaces);
            return stringwriter.ToString();
        }
    }
}
