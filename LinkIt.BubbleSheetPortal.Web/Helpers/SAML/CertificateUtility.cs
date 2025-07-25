using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Web;
using System.Xml;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.SAML
{
    public class CertificateUtility
    {
        /// <summary>
        /// Use an X509 certificate to append a computed signature to an XML serialized Response
        /// </summary>
        /// <param name="XMLSerializedSAMLResponse"></param>
        /// <param name="ReferenceURI">Assertion ID from SAML Response</param>
        /// <param name="SigningCert">X509 Certificate for signing</param>
        /// <remarks>Referenced this article:
        ///     http://www.west-wind.com/weblog/posts/2008/Feb/23/Digitally-Signing-an-XML-Document-and-Verifying-the-Signature
        /// </remarks>
        public static void AppendSignatureToXMLDocument(ref XmlDocument XMLSerializedSAMLResponse, String ReferenceURI, X509Certificate2 SigningCert)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(XMLSerializedSAMLResponse.NameTable);
            ns.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
            XmlElement xeAssertion = XMLSerializedSAMLResponse.DocumentElement.SelectSingleNode("saml:Assertion", ns) as XmlElement;

            //SignedXml signedXML = new SignedXml(XMLSerializedSAMLResponse);
            SignedXml signedXML = new SignedXml(xeAssertion);

            signedXML.SigningKey = SigningCert.PrivateKey;
            signedXML.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;

            Reference reference = new Reference();
            reference.Uri = ReferenceURI;
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigExcC14NTransform());
            signedXML.AddReference(reference);
            signedXML.ComputeSignature();

            XmlElement signature = signedXML.GetXml();            
            if (xeAssertion != null)
                xeAssertion.AppendChild(signature);

            //XmlElement xeResponse = XMLSerializedSAMLResponse.DocumentElement;
            //xeResponse.AppendChild(signature);
        }
    }
}