using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QTIXmlUtil
    {
        private bool _inBoldTag = false;
		private bool _inItalicTag = false;
		private bool _inUnderlineTag = false;
		private bool _resetBold = false;
		private bool _resetItalic = false;
		private bool _resetUnderline = false;

        private Dictionary<string, bool> _styleClassObject = new Dictionary<string, bool>();

        public string ConvertToLinkitXML(string qti3pItemXmlContent, string urlRoot, string qtiSchemeID, ref string answerIdentifiers)
        {
            //qti3pItemXmlContent = qti3pItemXmlContent.Replace("&#160;", " ");

            var doc = new XmlDocument();
            doc.LoadXml(qti3pItemXmlContent);

            var itemXML = doc.GetElementsByTagName("itemBody")[0];

            XmlNode answerXML = null;
            if ("10".Equals(qtiSchemeID))
            {
                answerXML = GetSpecificChildNodes(itemXML, "extendedTextInteraction").FirstOrDefault().CloneNode(true);
            }
            else
            {
                answerXML = GetSpecificChildNodes(itemXML, "choiceInteraction").FirstOrDefault().CloneNode(true);
            }
            
            // Build answer identifiers string 
            answerIdentifiers = ProcessChoiceInteraction(doc, answerXML, urlRoot);

            // Do nothing if this xml content had toolName="linkitTLF"
            if (IsHavingLinkitTLFAttribute(doc))
                return doc.OuterXml;

            RemoveSpecificChildNode(itemXML, "extendedTextInteraction");
            RemoveSpecificChildNode(itemXML, "choiceInteraction");

            List<XmlNode> objectNodeList = ResolveRefObjects(doc, itemXML, urlRoot);
            //ResolveRefImgs(doc, itemXML, urlRoot);

            var childList = itemXML.ChildNodes;
            if (childList.Count == 1)
            {
                if (childList[0].LocalName == "div")
                {
                    childList = childList[0].ChildNodes;
                }
            }

            var mainBody = CreateElement(doc, "div");
            AssignAttribute(doc, mainBody, "styleName", "mainBody");
            AssignAttribute(doc, mainBody, "class", "mainBody");

             //foreach (XmlNode childXML in childList)
            //{
            //    RecursXMLFlow(doc, childXML, mainBody, urlRoot, null);
            //}
            for (int i = 0; i < childList.Count; i++)//use for loop instead of foreach (see inside RecursXMLFlow  method )
            {
                var sizeBefore = childList.Count;
                if (i >= 0 && i < sizeBefore)
                {
                    var childXML = childList[i];
                    RecursXMLFlow(doc, childXML, mainBody, urlRoot, null);

                }
               
                var sizeAfter = childList.Count;

                i -= (sizeBefore - sizeAfter);
            }

            var origItemBody = doc.GetElementsByTagName("itemBody")[0];            
            RemoveAllChildNodes(origItemBody);
            
            if(objectNodeList != null)
            {
                foreach (XmlNode item in objectNodeList)
                {
                    origItemBody.AppendChild(item);
                }
            }
            origItemBody.AppendChild(mainBody);
            mainBody.AppendChild(answerXML);

            HardCodeProcessData(doc, qtiSchemeID);
            ResolveRefImgs(doc, itemXML, urlRoot);
            return doc.OuterXml;
        }

        public string UpdateImgPathOfXmlContent(string xmlCotent, string urlRoot)
        {
            //xmlCotent = xmlCotent.Replace("&#160;", " ");

            var doc = new XmlDocument();
            doc.LoadXml(xmlCotent);
            var imgNodes = doc.GetElementsByTagName("img");
            foreach (XmlNode childNode in imgNodes)
            {
                if (childNode.Attributes["src"] != null && !childNode.Attributes["src"].Value.ToLower().StartsWith("http://"))
                {
                    childNode.Attributes["src"].Value = urlRoot + ((urlRoot.LastIndexOf("/") == urlRoot.Length - 1) ? "" : "/") + childNode.Attributes["src"].Value;
                    AssignAttribute(doc, childNode, "source", childNode.Attributes["src"].Value);
                }
            }

            var result = doc.OuterXml;

            return result;
        }

        private string ProcessChoiceInteraction(XmlDocument doc, XmlNode answerXML, string urlRoot)
        {
            // Build answer identifiers string 
            string answerIdentifiers = "";

            if (answerXML != null)
            {
                var answerList = GetSpecificChildNodes(answerXML, "simpleChoice");

                foreach (XmlNode ansXML in answerList)
                {
                    var identifier = "";
                    if (ansXML.Attributes["identifier"] != null)
                    {
                        identifier = ansXML.Attributes["identifier"].Value;
                        answerIdentifiers += identifier + ";";
                    }

                    //RemoveSpecificChildNode(ansXML, "rubricBlock");

                    var ansChildList = ansXML.ChildNodes;

                    if (ansChildList.Count == 1)
                    {
                        var ansChildXML = ansChildList[0];
                        if (ansChildXML.LocalName == "div")
                        {
                            ansChildList = ansChildXML.ChildNodes;
                        }
                    }

                    var mainAnswerBody = CreateElement(doc, "div");
                    AssignAttribute(doc, mainAnswerBody, "styleName", "answer");
                    AssignAttribute(doc, mainAnswerBody, "class", "answer");

                    foreach (XmlNode ansChildXML in ansChildList)
                    {
                        if (ansChildXML.LocalName.ToLower() != "rubricblock")
                        {
                            RecursXMLFlow(doc, ansChildXML, mainAnswerBody, urlRoot, identifier);    
                        }                        
                    }

                    // Process rubricblock separately to put rubric tag under simpleChoice tag
                    var rubricBody = CreateElement(doc, "div");
                    foreach (XmlNode ansChildXML in ansChildList)
                    {
                        if (ansChildXML.LocalName.ToLower() == "rubricblock")
                        {
                            RecursXMLFlow(doc, ansChildXML, rubricBody, urlRoot, identifier);    
                        }                        
                    }

                    RemoveAllChildNodes(ansXML);
                    ansXML.AppendChild(mainAnswerBody);

                    if (rubricBody.HasChildNodes)
                    {
                        foreach (XmlNode childNode in rubricBody.ChildNodes)
                        {
                            ansXML.AppendChild(childNode);    
                        }
                    }
                }
            }

            // Remove last character (; character)
            return answerIdentifiers.Length > 0
                       ? answerIdentifiers.Substring(0, answerIdentifiers.Length - 1)
                       : answerIdentifiers;
        }

        private void HardCodeProcessData(XmlDocument doc, string qtiSchemeID)
        {
            XmlNode processNode = doc;
            var childNodes = doc.ChildNodes;
            if(childNodes.Count == 1)
            {                
                processNode = childNodes[0];
                childNodes = childNodes[0].ChildNodes;

                if(processNode.LocalName == "assessmentItem")
                {
                    HardCodeProcessAssessmentItem(doc, processNode, qtiSchemeID);
                }
            }

            for (int i = childNodes.Count - 1; i >= 0; i--)
            {
                // Hard code to add these attributes (to match final output result of current portal)
                if (childNodes[i].LocalName == "responseDeclaration")
                {
                    HardCodeProcessResponseDeclaration(doc, childNodes[i]);                    
                }

                if (childNodes[i].LocalName != "stylesheet"
                    && childNodes[i].LocalName != "responseDeclaration"
                    && childNodes[i].LocalName != "itemBody")
                {
                    processNode.RemoveChild(childNodes[i]);
                }
            }
        }

        private void HardCodeProcessResponseDeclaration(XmlDocument doc, XmlNode item)
        {
            AssignAttribute(doc, item, "method", "default");
            AssignAttribute(doc, item, "caseSensitive", "false");
            AssignAttribute(doc, item, "type", "string");
            AssignAttribute(doc, item, "pointsValue", "1");
        }

        private void HardCodeProcessAssessmentItem(XmlDocument doc, XmlNode item, string qtiSchemeID)
        {
            AssignAttribute(doc, item, "toolName", "linkitTLF");
            AssignAttribute(doc, item, "toolVersion", "1.0");
            AssignAttribute(doc, item, "qtiSchemeID", qtiSchemeID);

            RemoveAttribute(doc, item, "title");
            RemoveAttribute(doc, item, "identifier");
        }

        private bool IsHavingLinkitTLFAttribute(XmlDocument doc)
        {
            bool result = false;
            if (doc.ChildNodes.Count == 1)
            {
                XmlNode childNode = doc.ChildNodes[0];
                if (childNode.LocalName == "assessmentItem")
                {
                    if (childNode.Attributes["toolName"] != null &&
                        childNode.Attributes["toolName"].Value == "linkitTLF")
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        private List<XmlNode> ResolveRefObjects(XmlDocument doc, XmlNode itemXML, string urlRoot )
        {
            var objectNodeList = new List<XmlNode>();
            
            foreach (XmlNode childNode in itemXML.ChildNodes)
            {
                if (childNode.HasChildNodes)
                {
                    for (int i = childNode.ChildNodes.Count -1; i >= 0; i--)// (XmlNode child in childNode.ChildNodes)
                    {
                        XmlNode child = childNode.ChildNodes[i];

                        if (child.LocalName == "object")
                        {
                            bool hasClassAttribute = false;
                            bool hasStyleNameAttribute = false;
                            foreach (XmlAttribute attribute in child.Attributes)
                            {
                                if (attribute.Name == "class")
                                {
                                    hasClassAttribute = true;
                                    attribute.Value = "referenceObject";
                                }

                                if (attribute.Name == "stylename")
                                {
                                    hasStyleNameAttribute = true;
                                    attribute.Value = "referenceObject";
                                }

                                if (attribute.Name == "data")
                                {
                                    attribute.Value = urlRoot + ((urlRoot.LastIndexOf("/") == urlRoot.Length - 1) ? "" : "/") + attribute.Value;
                                }
                            }

                            if (!hasClassAttribute)
                            {
                                var attr = doc.CreateAttribute("class");
                                attr.Value = "referenceObject";
                                child.Attributes.Append(attr);
                            }

                            if (!hasStyleNameAttribute)
                            {
                                var attr = doc.CreateAttribute("stylename");
                                attr.Value = "referenceObject";
                                child.Attributes.Append(attr);
                            }

                            // Replace object emlement with span element (objec element will be move under itembody tag)
                            objectNodeList.Add(child.CloneNode(true));
                            childNode.RemoveChild(child);
                            childNode.AppendChild(CreateElement(doc, "span"));
                        }                        
                    }
                }
            }

            return objectNodeList;
        }

        private void ResolveRefImgs(XmlDocument doc, XmlNode itemXML, string urlRoot)
        {
            var imgNodes = doc.GetElementsByTagName("img");
            foreach (XmlNode childNode in imgNodes)
            {
                if(childNode.Attributes["src"] != null)
                {
                    childNode.Attributes["src"].Value = urlRoot + ((urlRoot.LastIndexOf("/") == urlRoot.Length - 1) ? "" : "/") + childNode.Attributes["src"].Value;
                    AssignAttribute(doc, childNode, "source", childNode.Attributes["src"].Value);
                }
            }            
        }

        private List<XmlNode> GetSpecificChildNodes(XmlNode xmlNode, string localName)
        {                        
            return xmlNode.ChildNodes.Cast<XmlNode>().Where(childNode => childNode.LocalName == localName).ToList();
        }

        private void RemoveSpecificChildNode(XmlNode xmlNode, string childLocalName)
        {
            var childNodes = GetSpecificChildNodes(xmlNode, childLocalName);
            foreach (var childNode in childNodes)
            {
                xmlNode.RemoveChild(childNode);
            }
        }

        private void RecursXMLFlow(XmlDocument doc, XmlNode xml, XmlNode parent, string urlRoot, string identifier, XmlNode lastBlockElement = null)
        {
            if (lastBlockElement == null)
            {
                string pln = parent.LocalName;
                if (pln == "div" || pln == "p")
                {
                    lastBlockElement = parent;
                }
            }

            var boldSet = false;
            var italicSet = false;
            var underlineSet = false;

            XmlNode spanElement;
            XmlNode paraElement;
            XmlNode divElement;
            XmlNode listElement;
            XmlNode listItemElement;
            XmlNode nextParent = null;
            XmlNode nextBlockParent;

            List<string> formatArray;
            string formatString;

            switch (xml.NodeType)
            {
                case XmlNodeType.Text:
                    {
                        xml.Value = xml.Value;

                        if (parent.LocalName == "p")
                        {
                            spanElement = CreateElement(doc, "span");
                            spanElement.AppendChild(xml);

                            formatArray = new List<string>();
                            if (_inBoldTag) formatArray.Add("bold");
                            if (_inItalicTag) formatArray.Add("italic");
                            if (_inUnderlineTag) formatArray.Add("underline");

                            foreach (var keypair in _styleClassObject)
                            {
                                if(keypair.Value)
                                    formatArray.Add(keypair.Key);
                            }
                            
                            if (formatArray.Count > 0)
                            {
                                formatString = JoinClass(formatArray);
                                AssignAttribute(doc, spanElement, "styleName", formatString);
                                AssignAttribute(doc, spanElement, "class", formatString);
                            }

                            parent.AppendChild(spanElement);
                        }
                        else if (parent.LocalName == "span")
                        {
                            resolveSpanStyleClasses(doc, parent);
                            parent.AppendChild(xml);
                        }
                        else
                        {
                            // REVISIT -- parent may not be span for list items - but we could force it I suppose ????
                            lastBlockElement.AppendChild(xml);
                        }
                    }
                    break;
                case XmlNodeType.Element:
                    {
                        string className;
                        XmlNode newElement;
                        string subln = xml.LocalName;
                        int fs;

                        switch (subln.ToLower())
                        {
                            case "div":

                                resetStyleTags();
                                divElement = CreateElement(doc, "div");

                                CopyAttribute(doc, xml, divElement, "styleName");
                                CopyAttribute(doc, xml, divElement, "class");

                                lastBlockElement.AppendChild(divElement);
                                nextParent = divElement;
                                nextBlockParent = nextParent;
                                break;

                            case "P":

                                resetStyleTags();
                                paraElement = CreateElement(doc, "p");

                                CopyAttribute(doc, xml, paraElement, "styleName");
                                CopyAttribute(doc, xml, paraElement, "class");

                                lastBlockElement.AppendChild(paraElement);
                                nextParent = paraElement;
                                nextBlockParent = nextParent;
                                break;

                            case "span":

                                if (xml.Attributes["class"] != null)
                                {

                                    className = xml.Attributes["class"].Value;

                                    switch (className)
                                    {
                                        case "text-block":
                                        case "block-quote":
                                        case "equation-block":

                                            resetStyleTags();

                                            paraElement = CreateElement(doc, "p");
                                            AssignAttribute(doc, paraElement, "styleName", className);
                                            AssignAttribute(doc, paraElement, "class", className);

                                            if (parent.LocalName == "div")
                                            {
                                                parent.AppendChild(paraElement);
                                                nextParent = paraElement;
                                                nextBlockParent = nextParent;
                                            }
                                            else
                                            {
                                                parent = parent.ParentNode;
                                                parent.AppendChild(paraElement);
                                                nextParent = paraElement;
                                                nextBlockParent = nextParent;
                                            }
                                            break;

                                        case "class1":
                                        case "class2":
                                        case "class3":
                                        case "class4":

                                            if (_styleClassObject.ContainsKey(className))
                                            {
                                                _styleClassObject[className] = true;
                                            }else
                                            {
                                                _styleClassObject.Add(className,true);
                                            }

                                            if (lastBlockElement.LocalName != "p")
                                            {
                                                paraElement = CreateElement(doc, "p");
                                                lastBlockElement.AppendChild(paraElement);
                                                parent = paraElement;
                                                nextParent = paraElement;
                                                nextBlockParent = nextParent;
                                            }
                                            else
                                            {
                                                nextParent = lastBlockElement;
                                                nextBlockParent = nextParent;
                                            }
                                            break;

                                        default:

                                            if (_styleClassObject.ContainsKey(className))
                                            {
                                                _styleClassObject[className] = true;
                                            }
                                            else
                                            {
                                                _styleClassObject.Add(className, true);
                                            }

                                            if (lastBlockElement.LocalName != "p")
                                            {
                                                paraElement = CreateElement(doc, "p");
                                                lastBlockElement.AppendChild(paraElement);
                                                parent = paraElement;
                                                nextParent = paraElement;
                                                nextBlockParent = nextParent;
                                            }
                                            else
                                            {
                                                nextParent = lastBlockElement;
                                                nextBlockParent = nextParent;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    if (lastBlockElement.LocalName != "p")
                                    {
                                        paraElement = CreateElement(doc, "p");
                                        lastBlockElement.AppendChild(paraElement);
                                        parent = paraElement;
                                        nextParent = paraElement;
                                        nextBlockParent = nextParent;
                                    }
                                    else
                                    {
                                        nextParent = lastBlockElement;
                                        nextBlockParent = nextParent;
                                    }
                                }
                                break;

                            case "br":
                                var breakElement = CreateElement(doc, "br");
                                parent.AppendChild(breakElement);
                                nextBlockParent = lastBlockElement;

                                break;

                            case "extendedTextInteraction":
                                var extendedElement = CreateElement(doc, "extendedTextInteraction");
                                parent.AppendChild(extendedElement);
                                nextParent = extendedElement;
                                nextBlockParent = nextParent;
                                break;

                            case "rubricblock":
                                var rubricElement = CreateElement(doc, "div");

                                AssignAttribute(doc, rubricElement, "style", "display: none;");
                                AssignAttribute(doc, rubricElement, "class", "rationale");
                                AssignAttribute(doc, rubricElement, "typemessage", "rationale");
                                AssignAttribute(doc, rubricElement, "identifier", identifier);

                                parent.AppendChild(rubricElement);
                                nextParent = rubricElement;
                                nextBlockParent = nextParent;

                                break;

                            case "b":
                            case "strong":
                                _resetBold = _inBoldTag == false;
                                _inBoldTag = true;
                                boldSet = true;

                                if (parent.LocalName != "p" && parent.LocalName != "span")
                                {
                                    paraElement = CreateElement(doc, "p");
                                    parent.AppendChild(paraElement);
                                    nextParent = paraElement;
                                    nextBlockParent = nextParent;
                                }
                                else
                                {
                                    nextParent = parent;
                                    nextBlockParent = lastBlockElement;
                                }

                                break;

                            case "i":
                            case "em":
                                _resetItalic = _inItalicTag == false;
                                _inItalicTag = true;

                                italicSet = true;


                                if (parent.LocalName != "p" && parent.LocalName != "span")
                                {
                                    paraElement = CreateElement(doc, "p");
                                    parent.AppendChild(paraElement);
                                    nextParent = paraElement;
                                    nextBlockParent = nextParent;
                                }
                                else
                                {
                                    nextParent = parent;
                                    nextBlockParent = lastBlockElement;
                                }

                                break;

                            case "u":
                                _resetUnderline = _inUnderlineTag == false;
                                _inUnderlineTag = true;

                                underlineSet = true;

                                if (parent.LocalName != "p" && parent.LocalName != "span")
                                {
                                    paraElement = CreateElement(doc, "p");
                                    parent.AppendChild(paraElement);
                                    nextParent = paraElement;
                                    nextBlockParent = nextParent;
                                }
                                else
                                {
                                    nextParent = parent;
                                    nextBlockParent = lastBlockElement;
                                }

                                break;

                                //REVISIT LIST HANDLING
                            case "list":
                            case "ol": // treating ordered list as unordeed lists for now
                            case "ul":

                                resetStyleTags();

                                listElement = CreateElement(doc, "list");

                                CopyAttribute(doc, xml, listElement, "styleName");
                                CopyAttribute(doc, xml, listElement, "class");

                                parent.AppendChild(listElement);
                                nextParent = listElement;
                                nextBlockParent = nextParent;
                                break;

                            case "img":

                                var imgElement = CreateElement(doc, "img");

                                if (xml.Attributes["styleName"] != null)
                                {
                                    CopyAttribute(doc, xml, imgElement, "styleName");
                                }
                                else if (ElementIsBlockImage(parent) && parent.Attributes["styleName"] == null)
                                {
                                    AssignAttribute(doc, parent, "styleName", "imageBlock");
                                }


                                CopyAttribute(doc, xml, imgElement, "width");
                                CopyAttribute(doc, xml, imgElement, "height");
                                CopyAttribute(doc, xml, imgElement, "src");
                                CopyAttribute(doc, xml, imgElement, "source");
                                                                
                                lastBlockElement.AppendChild(imgElement);
                                nextParent = parent;
                                nextBlockParent = lastBlockElement;
                                break;

                            case "math":


                                if (parent.LocalName != "p" && parent.LocalName != "li")
                                {
                                    if (parent.LocalName == "div")
                                    {
                                        paraElement = CreateElement(doc, "p");
                                        parent.AppendChild(paraElement);
                                        parent = paraElement;
                                    }
                                    else if (parent.ParentNode.LocalName == "p")
                                    {
                                        parent = parent.ParentNode;
                                    }
                                    else
                                    {
                                        return;
                                    }
                                }

                                parent.AppendChild(xml);
                                return; //end of the line							

                            case "li":

                                if (parent.LocalName != "list")
                                {
                                    listElement = CreateElement(doc, "list");
                                    parent.AppendChild(listElement);
                                    parent = listElement;

                                }

                                listItemElement = CreateElement(doc, "li");
                                parent.AppendChild(listItemElement);
                                nextParent = listItemElement;
                                nextBlockParent = lastBlockElement;
                                break;

                            case "object":
                                lastBlockElement.AppendChild(xml);
                                nextParent = parent;
                                nextBlockParent = lastBlockElement;
                                break;
                            case "table":
                                lastBlockElement.AppendChild(xml);
                                nextParent = parent;
                                nextBlockParent = lastBlockElement;
                                break;
                            case "tbody":
                            case "tr":

                            default:
                                return;
                        }

                        for (int i = 0; i < xml.ChildNodes.Count; i++ )
                        {
                            var sizeBefore = xml.ChildNodes.Count;
                            var c = xml.ChildNodes[i];
                            if (nextParent != null)
                            {
                                RecursXMLFlow(doc, c, nextParent, urlRoot, identifier, nextBlockParent);
                            }
                            var sizeAfter = xml.ChildNodes.Count;

                            i -= (sizeBefore - sizeAfter);
                        }


                            //foreach (XmlNode c in xml.ChildNodes)
                            //{
                            //    if (nextParent != null)
                            //    {
                            //        RecursXMLFlow(doc, c, nextParent, urlRoot, nextBlockParent);
                            //    }
                            //}

                        if (boldSet && _resetBold) _inBoldTag = false;
                        if (italicSet && _resetItalic) _inItalicTag = false;
                        if (underlineSet && _resetUnderline) _inUnderlineTag = false;

                    }
                    break;
            }
        }

        private void CopyAttribute(XmlDocument doc, XmlNode source, XmlNode dest, string attributeName)
        {
            if (source.Attributes[attributeName] != null)
            {
                var attribute = doc.CreateAttribute(attributeName);
                attribute.Value = source.Attributes[attributeName].Value;
                dest.Attributes.Append(attribute);
            }
        }

        private void AssignAttribute(XmlDocument doc, XmlNode dest, string attributeName, string attributeValue)
        {
            var attribute = doc.CreateAttribute(attributeName);
            attribute.Value = attributeValue;
            dest.Attributes.Append(attribute);
        }

        private void RemoveAttribute(XmlDocument doc, XmlNode dest, string attributeName)
        {
            if (dest.Attributes[attributeName] != null)
                dest.Attributes.Remove(dest.Attributes[attributeName]);
        }

        private XmlElement CreateElement(XmlDocument doc, string elementName)
        {
            return doc.CreateElement(elementName, doc.DocumentElement.NamespaceURI);
        }

        private void resolveSpanStyleClasses(XmlDocument doc, XmlNode span)
        {
            string classString = span.Attributes["styleName"].Value;
            List<string> formatArray = classString.Split(' ').ToList();
            if (_inBoldTag)
            {
                if (!formatArray.Contains("bold")) formatArray.Add("bold");

            }
            if (_inItalicTag)
            {
                if (!formatArray.Contains("italic")) formatArray.Add("italic");

            }
            if (_inUnderlineTag)
            {
                if (!formatArray.Contains("underline")) formatArray.Add("underline");
            }



            if (formatArray.Count > 0)
            {
                var formatString = JoinClass(formatArray);
                AssignAttribute(doc, span, "styleName", formatString);
                AssignAttribute(doc, span, "class", formatString);
            }
        }

        private void resetStyleTags() {
			_inBoldTag = false;
			_inItalicTag = false;
			_inUnderlineTag = false;
			
			_styleClassObject.Clear();
		}

        private bool ElementIsBlockImage( XmlNode xml) {
            // check if xml contains an img element
            // AND if so whether it has any text
			
            if (xml.InnerText.Length > 0) return false;

            //return xml.descendants().length() < 2;
            return xml.ChildNodes.Count < 2;			
        }

        private string JoinClass(List<string> items )
        {
            string classString = "";
            foreach (string item in items)
                classString = classString + (item + " ");

            return classString.Trim();
        }

        private void RemoveAllChildNodes(XmlNode item)
        {
            while (item.FirstChild != null)
                item.RemoveChild(item.FirstChild);
        }
    }
}
