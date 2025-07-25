using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.DynamoConnector
{
    public class DocumentModelTransform
    {
        public static T Transform<T>(Document document)
        {
            if (document == null) return default(T);

            var result = JsonConvert.DeserializeObject<T>(document.ToJson());

            return result;
        }

        public static List<T> Transform<T>(List<Document> documents)
        {
            if (documents == null) return null;
            var result = new List<T>();
            foreach (var document in documents)
            {
                var data = Transform<T>(document);
                if (data == null) continue;
                result.Add(data);
            }

            return result;
        }

        public static DynamoDBList Transform<T>(List<T> items)
        {
            if (items == null) return null;

            var result = new DynamoDBList();
            foreach(var item in items)
            {
                var json = JsonConvert.SerializeObject(item);
                var doc = Document.FromJson(json);
                result.Add(doc);
            }

            return result;
        }
    }
}
