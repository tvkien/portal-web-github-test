using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Common
{
    public class ETLXmlSerialization<T> where T : class, new()
    {
        public string SerializeObjectToXml(T obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();            
            UTF8Encoding encoding = new UTF8Encoding(false);
            XmlTextWriter xmlWriter = new XmlTextWriter(ms, encoding);

            serializer.Serialize(xmlWriter, obj);
            ms = (MemoryStream)xmlWriter.BaseStream;
            string xml = Encoding.UTF8.GetString(ms.ToArray());

            ms.Dispose();
            return xml;
        }

        public T DeserializeXmlToObject(string xml)
        {
            T obj = new T();
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            try
            {
                XmlReader xmlReader = XmlReader.Create(new StringReader(xml));
                obj = (T)serializer.Deserialize(xmlReader);

                return obj;
            }
            catch
            {
                return null;
            }
        }

        public string SerializeObjectToXmlWithOutHeader(object obj)
        {
            string retval = null;
            var nsSerializer = new XmlSerializerNamespaces();
            nsSerializer.Add("", "");
            if (obj != null)
            {
                StringBuilder sb = new StringBuilder();
                using (XmlWriter writer = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true }))
                {
                    new XmlSerializer(obj.GetType()).Serialize(writer, obj, nsSerializer);
                }
                retval = sb.ToString();
            }
            return retval;
        }
    }
}