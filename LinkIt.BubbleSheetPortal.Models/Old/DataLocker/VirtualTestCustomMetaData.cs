using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.Old.DataLocker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace LinkIt.BubbleSheetPortal.Models.DataLocker
{
    public class VirtualTestCustomMetaData
    {
        public static string Raw = "Raw";
        public static string Scaled = "Scaled";
        public static string Percent = "Percent";
        public static string Percentile = "Percentile";
        public static string CustomN_1 = "CustomN_1";
        public static string CustomN_2 = "CustomN_2";
        public static string CustomN_3 = "CustomN_3";
        public static string CustomN_4 = "CustomN_4";
        public static string CustomA_1 = "CustomA_1";
        public static string CustomA_2 = "CustomA_2";
        public static string CustomA_3 = "CustomA_3";
        public static string CustomA_4 = "CustomA_4";
        public static string Artifact = "Artifact";
        public static string NOTE_COMMENT = "NOTE_COMMENT";

        public int VirtualTestCustomMetaDataID { get; set; }
        public int VirtualTestCustomScoreID { get; set; }
        public int? VirtualTestCustomSubScoreID { get; set; }
        public string ScoreType { get; set; }//one of these values: Raw, Scaled, Percent, Percentile, CustomN_1, CustomN_2, CustomN_3, CustomN_4, CustomA_1, CustomA_2, CustomA_3, CustomA_4
        public string MetaData { get; set; }
        public int? Order { get; set; }
    }

    public static class VirtualTestCustomMetaDataExtension
    {
        public static VirtualTestCustomMetaModel ParseMetaToObject(this VirtualTestCustomMetaData meta)
        {
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            var obj = json_serializer.Deserialize<VirtualTestCustomMetaModel>(meta.MetaData);
            obj.Order = meta.Order;
            return obj;
        }

        public static VirtualTestCustomMetaModel ParseMetaToObject(this VirtualTestCustomMetaData meta, string metaData)
        {
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            var obj = json_serializer.Deserialize<VirtualTestCustomMetaModel>(metaData);
            return obj;
        }
    }
}
