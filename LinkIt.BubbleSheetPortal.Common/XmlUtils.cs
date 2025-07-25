using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Common
{
    public abstract class XmlUtils
    {
        /// -------------------------------------------------------------------
        /// <summary>
        /// Adds an <see cref="XmlAttribute"/> to a <see cref="XmlNode"/>.
        /// </summary>
        /// <param name="node">Node in which an attribute is to be added</param>
        /// <param name="attributeName">attribute name</param>
        /// <param name="attributeValue">value of the attribute</param>
        /// <returns>true if successful, false otherwise</returns>
        /// -------------------------------------------------------------------
        public static void AddAttribute(XmlNode node,
                                        string attributeName,
                                        string attributeValue)
        {
            node.Attributes.Append(CreateAttribute(node,
                                                    attributeName,
                                                    attributeValue));
        }

        /// -----------------------------------------------------------
        /// <summary>
        /// Create an <see cref="XmlAttribute"/>.
        /// </summary>
        /// <param name="xmlDocument">XmlDocument object</param>
        /// <param name="attrName">name of the attribute</param>
        /// <param name="attrValue">value of the attribute</param>
        /// <returns>an instance of an XmlAttribute object</returns>
        /// -----------------------------------------------------------
        public static XmlAttribute CreateAttribute(XmlDocument xmlDocument,
                                                    string attrName,
                                                    string attrValue)
        {
            XmlAttribute oAtt = xmlDocument.CreateAttribute(attrName);
            oAtt.Value = attrValue;
            return oAtt;
        }

        /// -----------------------------------------------------------
        /// <summary>
        /// Create an <see cref="XmlAttribute"/>
        /// </summary>
        /// <param name="node">Node to use for creating an attribute</param>
        /// <param name="attrName">name of the attribute</param>
        /// <param name="attrValue">value of the attribute</param>
        /// <returns>an instance of an XmlAttribute object</returns>
        /// -----------------------------------------------------------
        public static XmlAttribute CreateAttribute(XmlNode node,
                                                    string attrName,
                                                    string attrValue)
        {
            return (CreateAttribute(node.OwnerDocument, attrName, attrValue));
        }

        /// -----------------------------------------------------------
        /// <summary>
        /// Adds an <see cref="XmlAttribute"/> to a <see cref="XmlNode"/>.
        /// </summary>
        /// <param name="node">Node in which an attribute is to be added</param>
        /// <param name="namespaceUri"></param>
        /// <param name="attributeName">attribute name</param>
        /// <param name="attributeValue">value of the attribute</param>
        /// <returns>true if successful, false otherwise</returns>
        /// -----------------------------------------------------------
        public static void AddAttribute(XmlNode node,
                                        string namespaceUri,
                                        string attributeName,
                                        string attributeValue)
        {
            XmlAttribute attribute = node.OwnerDocument.CreateAttribute(attributeName, namespaceUri);
            attribute.Value = attributeValue;
            node.Attributes.Append(attribute);
        }

        public static string GetNodeAttribute(XmlNode node, string attributeName)
        {
            try
            {
                if (node == null || string.IsNullOrEmpty(attributeName)) return null;
                if (node.Attributes == null) return null;
                if (node.Attributes[attributeName] == null) return null;
                var result = node.Attributes[attributeName].Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetNodeAttributeCaseInSensitive(XmlNode node, string attributeName)
        {
            try
            {
                if (node == null) return null;
                if (node.Attributes == null) return null;
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    if (!string.Equals(attribute.Name, attributeName, StringComparison.CurrentCultureIgnoreCase)) continue;
                    return attribute.Value;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool IsNodeAttributeExisted(XmlNode node, string attributeName)
        {
            try
            {
                if (node == null || string.IsNullOrEmpty(attributeName)) return false;
                if (node.Attributes == null) return false;
                if (node.Attributes[attributeName] == null) return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void SetNodeAttribute(XmlNode node, string attributeName, string value)
        {
            if (node == null || string.IsNullOrEmpty(attributeName)) return;
            if (node.Attributes == null) return;
            var xmlAttribute = node.Attributes[attributeName];
            if (xmlAttribute == null)
            {
                AddAttribute(node, attributeName, value);
                xmlAttribute = node.Attributes[attributeName];
            }

            xmlAttribute.Value = value;
        }

        public static string RemoveSelfClosingTags(string xml)
        {
            char[] seperators = { ' ', '\t', '\r', '\n' };

            var prevIndex = -1;
            while (xml.Contains("/>"))
            {
                var selfCloseIndex = xml.IndexOf("/>", System.StringComparison.Ordinal);
                if (prevIndex == selfCloseIndex)
                    return xml; // we are in a loop...

                prevIndex = selfCloseIndex;

                var tagStartIndex = -1;

                var tag = "";

                //really? no backwards indexof?
                for (var i = selfCloseIndex; i > 0; i--)
                {
                    if (xml[i] == '<')
                    {
                        tagStartIndex = i;
                        break;
                    }
                }

                var tagEndIndex = xml.IndexOfAny(seperators, tagStartIndex);
                var tagLength = tagEndIndex - tagStartIndex;
                tag = xml.Substring(tagStartIndex + 1, tagLength - 1);

                if ("br".Equals(tag, StringComparison.InvariantCultureIgnoreCase))
                {
                    xml = xml.Substring(0, selfCloseIndex) + " style='display:none;'></" + tag + ">" + xml.Substring(selfCloseIndex + 2);
                }
                else
                {
                    xml = xml.Substring(0, selfCloseIndex) + "></" + tag + ">" + xml.Substring(selfCloseIndex + 2);
                }
            }

            return xml;
        }

        public static string Serialize<T>(T obj)
        {
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
            var ms = new MemoryStream();
            var writer = XmlWriter.Create(ms, settings);
            var serializer = new XmlSerializer(typeof(T));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            serializer.Serialize(writer, obj, ns);

            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            var sr = new StreamReader(ms);
            var result = sr.ReadToEnd();

            return result;
        }

        public static T Deserialize<T>(string xml) where T : class
        {
            if (string.IsNullOrWhiteSpace(xml)) return null;
            try
            {
                var deSerializer = new XmlSerializer(typeof(T));
                T model;
                using (TextReader reader = new StringReader(xml))
                {
                    model = deSerializer.Deserialize(reader) as T;
                }

                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static int GetIntValue(XElement element)
        {
            if (element == null) return 0;
            return Convert.ToInt32(element.Value);
        }

        public static int? GetInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            int result;
            if (int.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }

        public static int? GetInt(XElement element)
        {
            var str = element == null ? null : element.Value;
            return GetInt(str);
        }

        public static string GetStringValue(XElement element)
        {
            if (element == null) return string.Empty;
            return element.Value;
        }

        public static int? GetIntNullable(XmlAttributeCollection attrs, string attrName)
        {
            if (attrs == null) return null;
            if (string.IsNullOrWhiteSpace(attrName)) return null;
            if (attrs[attrName] == null || attrs[attrName].Value == null) return null;
            var value = attrs[attrName].Value.Trim();
            var result = 0;
            if (int.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }

        public static bool GetBoolValue(XElement element)
        {
            if (element == null) return false;
            return element.Value == "1" || element.Value == "true";
        }

        public static DateTime? GetDateTime(XElement element)
        {
            if (element == null) return null;
            if (string.IsNullOrWhiteSpace(element.Value)) return null;
            DateTime result;
            if (DateTime.TryParse(element.Value, out result))
            {
                return result.ToUniversalTime();
            }

            return null;
        }

        public static string UpdateImageUrlsToLinkit(string xmlContent, int qtiGroupId)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlContent);
            var nodes = doc.GetElementsByTagName("img");
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes == null) continue;
                var src = GetNodeAttribute(node, "src");
                if (string.IsNullOrWhiteSpace(src))
                {
                    src = GetNodeAttribute(node, "source");
                }

                if (string.IsNullOrWhiteSpace(src)) continue;
                if (src.StartsWith("http") || src.StartsWith("www.") || src.StartsWith("nwea.s3.amazonaws.com")) // check nwea.s3.amazonaws.com in case this file stays on Amazon server
                {
                    src = src.Replace(" ", "%20");
                }
                else
                {
                    if (src.StartsWith("/"))
                    {
                        src = src.Substring(1, src.Length - 1);
                    }
                    src = string.Format("/ItemSet_{0}/{1}", qtiGroupId, src);
                    src = src.Replace(" ", "%20");
                }

                src = src.Replace("\\/", "\\");

                SetNodeAttribute(node, "src", src);
                SetNodeAttribute(node, "source", string.Empty);
            }
            return doc.OuterXml;
        }

        public static void SetOrUpdateNodeAttribute(ref XmlNode node, string attributeName, string value)
        {
            if (node == null || string.IsNullOrEmpty(attributeName)) return;
            if (node.Attributes == null) return;
            var xmlAttribute = node.Attributes[attributeName];

            if (xmlAttribute == null)
            {
                XmlAttribute oAtt = node.OwnerDocument.CreateAttribute(attributeName);
                oAtt.Value = value;
                node.Attributes.Append(oAtt);
            }
            else
            {
                xmlAttribute.Value = value;
            }
        }

        public static XmlNode GetSingleChildNodeByName(XmlNode parentNode, string childNodeName)
        {
            XmlNode childNode = null;
            foreach (XmlNode node in parentNode.ChildNodes)
            {
                if (node.Name == childNodeName)
                {
                    childNode = node;
                    break;
                }
            }
            return childNode;
        }

        public static void RemoveNodeByName(ref XmlNode node, string nodeName)
        {
            XmlNode removedNode = GetSingleChildNodeByName(node, nodeName);
            if (removedNode != null)
            {
                node.RemoveChild(removedNode);
            }
        }

        public static void RemoveAllNodeByName(ref XmlNode node, string nodeName)
        {
            var removedNodes = node.OwnerDocument.GetElementsByTagName(nodeName);
            if (removedNodes != null)
            {
                for (int i = 0; i < removedNodes.Count; i++)
                {
                    node.RemoveChild(removedNodes[i]);
                }
            }
        }

        /// <summary>
        /// Insert a new node as the first child node
        /// </summary>
        /// <param name="parentNode">The parent node</param>
        /// <param name="newChildNode">New node will be inserted as the first child</param>
        public static void InsertFirstChild(ref XmlNode parentNode, XmlNode newChildNode)
        {
            var firstChild = parentNode.FirstChild;
            if (firstChild != null)
            {
                parentNode.InsertBefore(newChildNode, firstChild);
            }
            else
            {
                parentNode.AppendChild(newChildNode);
            }
        }

        /// <summary>
        /// Move all child node(s) of one node (source node) to another node ( destination node)
        /// </summary>
        /// <param name="sourceNode">The source node</param>
        /// <param name="destinationNode">The destination node</param>
        public static void MoveChildNodes(ref XmlNode sourceNode, ref XmlNode destinationNode)
        {
            do
            {
                var firstNode = sourceNode.FirstChild;
                if (firstNode != null)
                {
                    destinationNode.AppendChild(firstNode);
                    //sourceNode.RemoveChild(firstNode);//no need to do this, firstNode will be automatically removed from sourceNode
                }
            } while (sourceNode.FirstChild != null);
        }

        public static string CleanUpHtmlToXml(string content)
        {
            var result = content.Replace("<br>", "<br/>");
            //because <img has no close tag so it is necessary to add / to the end > to make />
            string pattern = "<img[^<,>]+(?>)";
            string replacement = "$0/";
            Regex rgx = new Regex(pattern);
            result = rgx.Replace(result, replacement);
            result = result.Replace("//>", "/>");

            var temp = string.Format("<TEMP_ROOT>{0}</TEMP_ROOT>", result);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(temp);
            ReplaceNodeNames(doc, "sub", "span", "smallText sub", "sub");
            ReplaceNodeNames(doc, "sup", "span", "smallText sup", "sup");
            ReplaceNodeNames(doc, "b", "span", "bold", "bold");
            ReplaceNodeNames(doc, "u", "span", "underline", "underline");
            result = doc.FirstChild.InnerXml;
            return result;
        }

        public static string ReplaceNodeNames(XmlDocument doc, string nodeName, string newNodeName, string className, string styleName)
        {
            var oldNodes = doc.GetElementsByTagName(nodeName);
            for (int i = 0; i < oldNodes.Count; i++)
            {
                var newNode = doc.CreateElement(newNodeName);
                if (oldNodes[i].Attributes != null)
                {
                    //copy attributes
                    for (int j = 0; j < oldNodes[i].Attributes.Count; j++)
                    {
                        newNode.Attributes.Append(oldNodes[i].Attributes[j]);
                    }
                    //copy child nodes
                    for (int k = 0; k < oldNodes[i].ChildNodes.Count; k++)
                    {
                        newNode.AppendChild(oldNodes[i].ChildNodes[k].CloneNode(true));
                    }
                    //set attribute class
                    if (newNode.Attributes["class"] == null)
                    {
                        var classAtt = doc.CreateAttribute("class");
                        classAtt.Value = className;
                        newNode.Attributes.Append(classAtt);
                    }
                    else
                    {
                        newNode.Attributes["class"].Value = newNode.Attributes["class"].Value ??
                                                            string.Empty + " " + className;
                    }
                    //set attribute class
                    if (newNode.Attributes["styleName"] == null)
                    {
                        var styleNameAtt = doc.CreateAttribute("styleName");
                        styleNameAtt.Value = styleName;
                        newNode.Attributes.Append(styleNameAtt);
                    }
                    else
                    {
                        newNode.Attributes["styleName"].Value = newNode.Attributes["styleName"].Value ??
                                                            string.Empty + " " + styleName;
                    }
                    oldNodes[i].ParentNode.ReplaceChild(newNode, oldNodes[i]);
                }
            }
            return doc.OuterXml;
        }

        private static void MoveToAnotherParent(ref XmlNode childNode, ref XmlNode newParentNode)
        {
            var firstChild = newParentNode.FirstChild;
            newParentNode.InsertBefore(firstChild, childNode);
        }

        public static List<XmlNode> GetChildNodeListByName(XmlNode parentNode, string childNodeName)
        {
            List<XmlNode> childNodeList = new List<XmlNode>();
            foreach (XmlNode node in parentNode.ChildNodes)
            {
                if (node.Name == childNodeName)
                {
                    childNodeList.Add(node);
                }
            }
            return childNodeList;
        }

        public static void RecurseGetChildNodeListByName(XmlNode parentNode, string childNodeName, bool findChildNodeName, List<XmlNode> childNodeList)
        {
            //List<XmlNode> childNodeList = new List<XmlNode>();
            //var findChildNodeName = false;
            if (parentNode.HasChildNodes)
            {
                foreach (XmlNode node in parentNode.ChildNodes)
                {
                    if (node.Name == childNodeName)
                    {
                        findChildNodeName = true;
                        childNodeList.Add(node);
                    }
                }
                if (!findChildNodeName)
                    RecurseGetChildNodeListByName(parentNode.FirstChild, childNodeName, findChildNodeName, childNodeList);
            }
            if (parentNode.NextSibling != null)
            {
                if (!findChildNodeName)
                    RecurseGetChildNodeListByName(parentNode.NextSibling, childNodeName, findChildNodeName, childNodeList);
            }
            // return childNodeList;
        }

        public static XmlNode RemoveAllNamespaces(XmlNode documentElement)
        {
            var xmlnsPattern = "\\s+xmlns\\s*(:\\w)?\\s*=\\s*\\\"(?<url>[^\\\"]*)\\\"";
            var outerXml = documentElement.OuterXml;
            var matchCol = Regex.Matches(outerXml, xmlnsPattern);
            foreach (var match in matchCol)
                outerXml = outerXml.Replace(match.ToString(), "");

            var result = new XmlDocument();
            result.LoadXml(outerXml);

            return result;
        }

        public static string RemoveAllNamespacesPrefix(string xmlContent)
        {
            xmlContent = Regex.Replace(xmlContent, @"(<\s*\/?)\s*(\w+):(\w+)", "$1$3");
            return xmlContent;
        }

        public static XmlNode Strip(XmlNode documentElement)
        {
            var namespaceManager = new XmlNamespaceManager(documentElement.OwnerDocument.NameTable);
            foreach (var nspace in namespaceManager.GetNamespacesInScope(XmlNamespaceScope.All))
            {
                namespaceManager.RemoveNamespace(nspace.Key, nspace.Value);
            }

            return documentElement;
        }

        public static string BuildXml<T>(T obj)
        {
            using (var sw = new StringWriter())
            {
                var xs = new XmlSerializer(typeof(T));
                xs.Serialize(sw, obj);
                try
                {
                    var xml = sw.ToString();
                    return xml;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public static bool IsValidXml(string xmlString)
        {
            Regex tagsWithData = new Regex("<\\w+>[^<]+</\\w+>");

            if (string.IsNullOrEmpty(xmlString) || !tagsWithData.IsMatch(xmlString))
            {
                return false;
            }
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmlString);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsDrawable(string xmlContent)
        {
            var doc = new XmlContentProcessing(xmlContent);
            if (doc != null)
            {
                string drawable = doc.GetDrawable();
                if (drawable.ToLower() == "true")
                {
                    return true;
                }
            }
            return false;
        }

        public static string SanitizeAmpersands(string html)
        {
            // Fix unencoded '&' in <object data="...">
            html = Regex.Replace(
                html,
                @"<object\b[^>]*\bdata=""([^""]*)""",
                match =>
                {
                    var fullMatch = match.Value;
                    var attrValue = match.Groups[1].Value;

                    if (string.IsNullOrEmpty(attrValue))
                        return fullMatch;

                    var fixedAttr = Regex.Replace(attrValue, @"&(?![a-zA-Z]+;|#\d+;)", "&amp;");
                    return fullMatch.Replace(attrValue, fixedAttr);
                },
                RegexOptions.IgnoreCase
            );

            // Fix unencoded '&' in <img href="..."> or <img alt="...">
            html = Regex.Replace(
                html,
                @"<img\b[^>]*\b(?<attr>href|alt)=""(?<value>[^""]*)""",
                m =>
                {
                    var fullMatch = m.Value;
                    var attrName = m.Groups["attr"].Value;
                    var attrValue = m.Groups["value"].Value;

                    if (string.IsNullOrEmpty(attrValue))
                        return fullMatch;

                    var fixedValue = Regex.Replace(attrValue, @"&(?![a-zA-Z]+;|#\d+;)", "&amp;");
                    return fullMatch.Replace(attrValue, fixedValue);
                },
                RegexOptions.IgnoreCase
            );

            return html;
        }
    }
}
