using System.Configuration;
using System.IO.Compression;
using System.Xml.Linq;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using LinkIt.BubbleSheetPortal.Web.Security;
using Org.BouncyCastle.Crypto.Tls;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.SAML
{
    public class SAML20Assertion
    {
        /// <summary>
        /// Build a signed XML SAML Response string to be inlcuded in an HTML Form
        /// for POSTing to a SAML Service Provider
        /// </summary>
        /// <param name="Issuer">Identity Provider - Used to match the certificate for verifying 
        ///     Response signing</param>
        /// <param name="AssertionExpirationMinutes">Assertion lifetime</param>
        /// <param name="Audience"></param>
        /// <param name="Subject"></param>
        /// <param name="Recipient"></param>
        /// <param name="Attributes">Dictionary of attributes to send through for user SSO</param>
        /// <param name="SigningCert">X509 Certificate used to sign Assertion</param>
        /// <returns></returns>
        public static string CreateSAML20Response(
            SAMLRequest samlRequest,
            X509Certificate2 SigningCert)
        {
            var AssertionExpirationMinutes = 1;
            var Subject = "Authenticate";

            Dictionary<string, string> samlAttributes = new Dictionary<string, string>();

            samlAttributes.Add("SectorID", samlRequest.SectorId);
            samlAttributes.Add("UserID", samlRequest.UserId);
            samlAttributes.Add("ActionType", samlRequest.ActionType);
            samlAttributes.Add("ReturnURL", samlRequest.ReturnUrl);
            samlAttributes.Add("ErrorURL", samlRequest.ErrorUrl);

            var response = new SamlToken
            {
                RequestedSecurityToken = new ResponseType(),
                Lifetime = new Lifetime
                {
                    Created = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddMinutes(AssertionExpirationMinutes)
                },
                AppliesTo = new AppliesTo
                {
                    EndpointReference = new EndpointReference
                    {
                        Address = samlRequest.Issuer
                    }
                }
            };

            response.RequestedSecurityToken.Items = new AssertionType[] { CreateSAML20Assertion(samlRequest.Issuer, AssertionExpirationMinutes, samlRequest.Audience, Subject, samlRequest.Recipient, samlAttributes) };
                        
            XmlDocument XMLResponse = SerializeAndSignSAMLResponse(response, SigningCert);
            
            return XMLResponse.OuterXml;            
        }

        /// <summary>
        /// Creates a SAML 2.0 Assertion Segment for a Response
        /// Simple implmenetation assuming a list of string key and value pairs
        /// </summary>
        /// <param name="Issuer"></param>
        /// <param name="AssertionExpirationMinutes"></param>
        /// <param name="Audience"></param>
        /// <param name="Subject"></param>
        /// <param name="Recipient"></param>
        /// <param name="Attributes">Dictionary of string key, string value pairs</param>
        /// <returns>Assertion to sign and include in Response</returns>
        private static AssertionType CreateSAML20Assertion(string Issuer,
            int AssertionExpirationMinutes,
            string Audience,
            string Subject,
            string Recipient,
            Dictionary<string, string> Attributes)
        {
            AssertionType NewAssertion = new AssertionType()
            {
                Version = "2.0",
                IssueInstant = System.DateTime.UtcNow,
                ID = "_" + System.Guid.NewGuid().ToString()
            };

            // Create Issuer
            NewAssertion.Issuer = new NameIDType() { Value = Issuer.Trim() };

            // Create Assertion Subject
            SubjectType subject = new SubjectType();
            NameIDType subjectNameIdentifier = new NameIDType() { Value = Subject.Trim(), Format = "urn:oasis:names:tc:SAML:1.1:nameid-format:persistent" };
            SubjectConfirmationType subjectConfirmation = new SubjectConfirmationType() { Method = "urn:oasis:names:tc:SAML:2.0:cm:bearer", SubjectConfirmationData = new SubjectConfirmationDataType() { NotOnOrAfter = DateTime.UtcNow.AddMinutes(AssertionExpirationMinutes), Recipient = Recipient } };
            subject.Items = new object[] { subjectNameIdentifier, subjectConfirmation };
            NewAssertion.Subject = subject;

            // Create Assertion Conditions
            ConditionsType conditions = new ConditionsType();
            conditions.NotBefore = DateTime.UtcNow;
            conditions.NotBeforeSpecified = true;
            conditions.NotOnOrAfter = DateTime.UtcNow.AddMinutes(AssertionExpirationMinutes);
            conditions.NotOnOrAfterSpecified = true;
            conditions.Items = new ConditionAbstractType[] { new AudienceRestrictionType() { Audience = new string[] { Audience.Trim() } } };
            NewAssertion.Conditions = conditions;

            // Add AuthnStatement and Attributes as Items
            AuthnStatementType authStatement = new AuthnStatementType() { AuthnInstant = DateTime.UtcNow, SessionIndex = NewAssertion.ID };
            AuthnContextType context = new AuthnContextType();
            context.ItemsElementName = new ItemsChoiceType5[] { ItemsChoiceType5.AuthnContextClassRef };
            context.Items = new object[] { "urn:oasis:names:tc:SAML:2.0:ac:classes:unspecified" };
            authStatement.AuthnContext = context;

            AttributeStatementType attributeStatement = new AttributeStatementType();
            attributeStatement.Items = new AttributeType[Attributes.Count];
            int i = 0;
            foreach (KeyValuePair<string, string> attribute in Attributes)
            {
                attributeStatement.Items[i] = new AttributeType()
                {
                    Name = attribute.Key,
                    AttributeValue = new object[] { attribute.Value },
                    NameFormat = "urn:oasis:names:tc:SAML:2.0:attrname-format:basic"
                };
                i++;
            }

            // TODO:
            //NewAssertion.Items = new StatementAbstractType[] { authStatement, attributeStatement };
            NewAssertion.Items = new StatementAbstractType[] { attributeStatement };

            return NewAssertion;
        }

        /// <summary>
        /// Accepts SAML Response, serializes it to XML and signs using the supplied certificate
        /// </summary>
        /// <param name="Response">SAML 2.0 Response</param>
        /// <param name="SigningCert">X509 certificate</param>
        /// <returns>XML Document with computed signature</returns>
        private static XmlDocument SerializeAndSignSAMLResponse(SamlToken samlToken, X509Certificate2 SigningCert)
        {
            // Set serializer and writers for action
            XmlSerializer responseSerializer = new XmlSerializer(samlToken.RequestedSecurityToken.GetType());
            StringWriter stringWriter = new StringWriter();
            XmlWriter responseWriter = XmlTextWriter.Create(stringWriter,
                new XmlWriterSettings() {OmitXmlDeclaration = true, Indent = true, Encoding = Encoding.UTF8});
            responseSerializer.Serialize(responseWriter, samlToken.RequestedSecurityToken);
            responseWriter.Close();
            XmlDocument xmlResponse = new XmlDocument();
            xmlResponse.LoadXml(stringWriter.ToString());

            CertificateUtility.AppendSignatureToXMLDocument(ref xmlResponse,
                "#" + ((AssertionType) samlToken.RequestedSecurityToken.Items[0]).ID, SigningCert);

            using (XmlReader reader = XmlReader.Create(new StringReader(xmlResponse.OuterXml)))
            {
                samlToken.RequestedSecurityToken = (ResponseType) responseSerializer.Deserialize(reader);
            }

            responseSerializer = new XmlSerializer(samlToken.GetType());
            stringWriter = new StringWriter();
            responseWriter = XmlTextWriter.Create(stringWriter,
                new XmlWriterSettings() {OmitXmlDeclaration = true, Indent = true, Encoding = Encoding.UTF8});
            responseSerializer.Serialize(responseWriter, samlToken);
            responseWriter.Close();
            xmlResponse = new XmlDocument();
            xmlResponse.LoadXml(stringWriter.ToString());

            return xmlResponse;
        }

        public static SAMLReponse ParseAzureSAMLResponse(string samlReponse, X509Certificate2 SigningCert, PassThroughVDETMessageViewModel obj, SAMLConfiguration configuration)
        {
            try
            {
                if (ValidateX509CertificateSignature(samlReponse, SigningCert))
                {
                    return ParseAzureReponseXML(samlReponse, configuration);
                }
                else
                {
                    //Invalid Signcher
                    obj.Status = Util.PassThroughVDETStatusFail;
                    obj.Code = Util.PassThroughVDETCode104;
                    obj.Message = Util.PassThroughVDETMessage104;
                }
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                //SAML invalid Format
                obj.Status = Util.PassThroughVDETStatusFail;
                obj.Code = Util.PassThroughVDETCode105;
                obj.Message = Util.PassThroughVDETMessage105;
            }

            return null;
        }

        private static SAMLReponse ParseAzureReponseXML(string xml, SAMLConfiguration configuration)
        {
            var samlReponse = new SAMLReponse();

            var doc = new XmlDocument();
            doc.Load(new StringReader(xml));

            var conditions = doc.GetElementsByTagName("Conditions");
            if (conditions.Count == 1)
            {
                XmlNode condition = conditions[0];
                if (condition.Attributes != null)
                {
                    samlReponse.NotBefore = DateTime.Parse(condition.Attributes["NotBefore"].Value);
                    samlReponse.NotOnOrAfter = DateTime.Parse(condition.Attributes["NotOnOrAfter"].Value);
                }                
            }

            var statusNode = doc.GetElementsByTagName("samlp:StatusCode")[0];
            if (statusNode != null)
            {
                samlReponse.StatusCode = statusNode.Attributes["Value"].Value;
            }

            if (configuration.Client == SSOClient.Vdet)
            {
                var attributeNodes = doc.GetElementsByTagName("Attribute");
                GetUserInformationForVDET(configuration, attributeNodes, samlReponse);
            }
            else if (configuration.Client == SSOClient.Cecv)
            {
                var attributeNodes = doc.GetElementsByTagName("saml:Attribute");
                GetUserInformationForCECV(configuration, attributeNodes, samlReponse);
            }

            return samlReponse;
        }

        private static void GetUserInformationForVDET(SAMLConfiguration configuration, XmlNodeList attributeNodes,
            SAMLReponse samlReponse)
        {
            foreach (XmlNode node in attributeNodes)
            {
                if (node.Attributes["Name"].Value == "http://schemas.microsoft.com/identity/claims/tenantid")
                    samlReponse.TenantId = node.ChildNodes[0].InnerText;

                if (node.Attributes["Name"].Value == "http://schemas.microsoft.com/identity/claims/objectidentifier")
                    samlReponse.ObjectIdentifier = node.ChildNodes[0].InnerText;

                //if (node.Attributes["Name"].Value == ConfigurationManager.AppSettings["InsightSamlKeyUserId"])
                if (node.Attributes["Name"].Value == configuration.UserIdKey)
                    samlReponse.UserId = node.ChildNodes[0].InnerText;

                //if (node.Attributes["Name"].Value == ConfigurationManager.AppSettings["InsightSamlKeyRole"])
                if (node.Attributes["Name"].Value == configuration.RoleKey)
                    samlReponse.SectorId = node.ChildNodes[0].InnerText;
            }
        }

        private static void GetUserInformationForCECV(SAMLConfiguration configuration, XmlNodeList attributeNodes,
            SAMLReponse samlReponse)
        {
            foreach (XmlNode node in attributeNodes)
            {
                //if (node.Attributes["Format"].Value == "urn:oasis:names:tc:SAML:2.0:nameid-format:email")
                //    samlReponse.UserId = node.ChildNodes[0].InnerText;

                //samlReponse.TenantId = "";
                //samlReponse.SectorId = configuration.SectorID;
                //samlReponse.ObjectIdentifier = "";

                //if (node.Attributes["Name"].Value == "http://schemas.microsoft.com/identity/claims/objectidentifier")
                //    samlReponse.ObjectIdentifier = node.ChildNodes[0].InnerText;

                if (node.Attributes["Name"].Value == configuration.UserIdKey)
                    samlReponse.UserId = node.ChildNodes[0].InnerText;

                if (node.Attributes["Name"].Value == configuration.RoleKey)
                    samlReponse.SectorId = node.ChildNodes[0].InnerText;
            }
        }

        public static SAMLRequest ParseSAMLResponse(string samlReponse, X509Certificate2 SigningCert, PassThroughVDETMessageViewModel obj)
        {
            try
            {
                if (ValidateX509CertificateSignature(samlReponse, SigningCert))
                {
                    var samlToken = new SamlToken();

                    XmlSerializer responseSerializer = new XmlSerializer(samlToken.GetType());
                    using (XmlReader reader = XmlReader.Create(new StringReader(samlReponse)))
                    {
                        samlToken = (SamlToken)responseSerializer.Deserialize(reader);
                    }

                    var assertionType = (AssertionType)samlToken.RequestedSecurityToken.Items[0];

                    if (assertionType.Conditions.NotBefore <= DateTime.UtcNow &&
                        assertionType.Conditions.NotOnOrAfter >= DateTime.UtcNow)
                    {
                        var samlRequest = new SAMLRequest();
                        var statementAbstractType = (AttributeStatementType)assertionType.Items[0];

                        foreach (AttributeType item in statementAbstractType.Items)
                        {
                            switch (item.Name)
                            {
                                case "SectorID":
                                    {
                                        samlRequest.SectorId = item.AttributeValue[0] == null ? string.Empty : item.AttributeValue[0].ToString();
                                        break;
                                    }
                                case "UserID":
                                    {
                                        samlRequest.UserId = item.AttributeValue[0] == null ? string.Empty : item.AttributeValue[0].ToString();
                                        break;
                                    }
                                case "ActionType":
                                    {
                                        samlRequest.ActionType = item.AttributeValue[0] == null ? string.Empty : item.AttributeValue[0].ToString();
                                        break;
                                    }
                                case "ReturnURL":
                                    {
                                        samlRequest.ReturnUrl = item.AttributeValue[0] == null ? string.Empty : item.AttributeValue[0].ToString();
                                        break;
                                    }
                                case "ErrorURL":
                                    {
                                        samlRequest.ErrorUrl = item.AttributeValue[0] == null ? string.Empty : item.AttributeValue[0].ToString();
                                        break;
                                    }

                                default:
                                    {
                                        break;
                                    }
                            }
                        }

                        return samlRequest;
                    }
                    else
                    { 
                        //tocken expire
                        obj.Status = Util.PassThroughVDETStatusFail;
                        obj.Code = Util.PassThroughVDETCode103;
                        obj.Message = Util.PassThroughVDETMessage103;
                    }
                }
                else
                { 
                    //Invalid Signcher
                    obj.Status = Util.PassThroughVDETStatusFail;
                    obj.Code = Util.PassThroughVDETCode104;
                    obj.Message = Util.PassThroughVDETMessage104;
                }
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                //SAML invalid Format
                obj.Status = Util.PassThroughVDETStatusFail;
                obj.Code = Util.PassThroughVDETCode105;
                obj.Message = Util.PassThroughVDETMessage105;
            }

            return null;
        }

        public static bool ValidateX509CertificateSignature(string samlToken , X509Certificate2 SigningCert)
        {
            XmlDocument SAMLResponse = new XmlDocument();
            SAMLResponse.PreserveWhitespace = true;
            SAMLResponse.LoadXml(samlToken);

            XmlNodeList XMLSignatures = SAMLResponse.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#");

            // Checking If the Response or the Assertion has been signed once and only once.
            if (XMLSignatures.Count != 1) return false;
            SignedXml SignedSAML = new SignedXml(SAMLResponse);
            SignedSAML.LoadXml((XmlElement)XMLSignatures[0]);

            return SignedSAML.CheckSignature(SigningCert, true);
        }

        public static string CreateSsoAuthenticationRequest(SAMLConfiguration configuration)
        {
            string xml = @"<samlp:AuthnRequest xmlns=""urn:oasis:names:tc:SAML:2.0:assertion"" ID=""#ID""
                                                 Version=""2.0"" IssueInstant=""#DATE""
                                                 xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol"">
                                    <Issuer xmlns=""urn:oasis:names:tc:SAML:2.0:assertion"">#ISSUER</Issuer>
                             </samlp:AuthnRequest>";

            var id = "_" + Guid.NewGuid();
            var issueInstant = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            var issuer = configuration.Issuer;

            xml = xml.Replace("#ID", id);
            xml = xml.Replace("#DATE", issueInstant);
            xml = xml.Replace("#ISSUER", issuer);
            xml = xml.Replace("\r\n", "");


            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new StreamWriter(new DeflateStream(memoryStream, CompressionMode.Compress, true), new UTF8Encoding(false)))
                {
                    writer.Write(xml);
                }

                return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length, Base64FormattingOptions.None);
            }
        }

        public static string CreateAzureVdetRequest()
        {
            string xml = @"<samlp:AuthnRequest xmlns=""urn:oasis:names:tc:SAML:2.0:assertion"" ID=""#ID""
                                                 Version=""2.0"" IssueInstant=""#DATE""
                                                 xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol"">
                                    <Issuer xmlns=""urn:oasis:names:tc:SAML:2.0:assertion"">#ISSUER</Issuer>
                             </samlp:AuthnRequest>";

            var id = "_" + Guid.NewGuid();
            var issueInstant = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            var issuer = ConfigurationManager.AppSettings["AzureVdetIssuer"];

            xml = xml.Replace("#ID", id);
            xml = xml.Replace("#DATE", issueInstant);
            xml = xml.Replace("#ISSUER", issuer);
            xml = xml.Replace("\r\n", "");


            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(new DeflateStream(memoryStream, CompressionMode.Compress, true), new UTF8Encoding(false));
            writer.Write(xml);
            writer.Close();
            
            var azureRequest = Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length, Base64FormattingOptions.None);
            return azureRequest;
        }

        public static bool ValidateX509CertificateSignature(string certOneString, string certTwoString)
        {
            using (X509Certificate certOne = new X509Certificate2(Encoding.UTF8.GetBytes(certOneString)))
            using (X509Certificate certTwo = new X509Certificate2(Encoding.UTF8.GetBytes(certTwoString)))
            {
                return certOne.Equals(certTwo);
            }
        }

        public static NYCSamlResponse ParseNYCSamlResponse(string samlResponse)
        {
            byte[] samlBytes = Convert.FromBase64String(samlResponse);
            string decodedSamlResponse = Encoding.UTF8.GetString(samlBytes);

            var response = new NYCSamlResponse();

            if (string.IsNullOrEmpty(decodedSamlResponse))
            {
                return response;
            }

            var doc = XDocument.Parse(decodedSamlResponse);

            var certificate = doc.Descendants().Where(e => e.Name.LocalName == "X509Certificate").FirstOrDefault()?.Value;

            if (!string.IsNullOrEmpty(certificate))
            {
                response.CertString = certificate;
            }

            var assertion = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "Assertion");

            if (assertion == null)
            {
                return response;
            }

            response.Email = assertion.Descendants()
                .Where(e => e.Name.LocalName == "NameID")
                .FirstOrDefault()?.Value;

            var attributes = assertion.Descendants()
                                       .Where(e => e.Name.LocalName == "Attribute")
                                       .Select(attr => new
                                       {
                                           Name = attr.Attribute("Name")?.Value,
                                           Value = attr.Descendants().FirstOrDefault()?.Value
                                       });

            response.UserName = attributes?.FirstOrDefault(x => x.Name.Equals("username", StringComparison.InvariantCultureIgnoreCase))?.Value;
            response.FirstName = attributes?.FirstOrDefault(x => x.Name.Equals("firstname", StringComparison.InvariantCultureIgnoreCase))?.Value;
            response.LastName = attributes?.FirstOrDefault(x => x.Name.Equals("lastname", StringComparison.InvariantCultureIgnoreCase))?.Value;

            return response;
        }
    }
}
