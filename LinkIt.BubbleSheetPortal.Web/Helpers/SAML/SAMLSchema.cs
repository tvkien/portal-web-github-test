﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.SAML
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute("Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class SignatureType
    {

        private SignedInfoType signedInfoField;

        private SignatureValueType signatureValueField;

        private KeyInfoType keyInfoField;

        private ObjectType[] objectField;

        private string idField;

        /// <remarks/>
        public SignedInfoType SignedInfo
        {
            get
            {
                return this.signedInfoField;
            }
            set
            {
                this.signedInfoField = value;
            }
        }

        /// <remarks/>
        public SignatureValueType SignatureValue
        {
            get
            {
                return this.signatureValueField;
            }
            set
            {
                this.signatureValueField = value;
            }
        }

        /// <remarks/>
        public KeyInfoType KeyInfo
        {
            get
            {
                return this.keyInfoField;
            }
            set
            {
                this.keyInfoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Object")]
        public ObjectType[] Object
        {
            get
            {
                return this.objectField;
            }
            set
            {
                this.objectField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
        public SignatureType()
        {
            xmlns.Add("ds", "http://www.w3.org/2000/09/xmldsig#");
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute("SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class SignedInfoType
    {

        private CanonicalizationMethodType canonicalizationMethodField;

        private SignatureMethodType signatureMethodField;

        private ReferenceType[] referenceField;

        private string idField;

        /// <remarks/>
        public CanonicalizationMethodType CanonicalizationMethod
        {
            get
            {
                return this.canonicalizationMethodField;
            }
            set
            {
                this.canonicalizationMethodField = value;
            }
        }

        /// <remarks/>
        public SignatureMethodType SignatureMethod
        {
            get
            {
                return this.signatureMethodField;
            }
            set
            {
                this.signatureMethodField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Reference")]
        public ReferenceType[] Reference
        {
            get
            {
                return this.referenceField;
            }
            set
            {
                this.referenceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute("CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class CanonicalizationMethodType
    {

        private System.Xml.XmlNode[] anyField;

        private string algorithmField;

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlNode[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Algorithm
        {
            get
            {
                return this.algorithmField;
            }
            set
            {
                this.algorithmField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class X509IssuerSerialType
    {

        private string x509IssuerNameField;

        private string x509SerialNumberField;

        /// <remarks/>
        public string X509IssuerName
        {
            get
            {
                return this.x509IssuerNameField;
            }
            set
            {
                this.x509IssuerNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "integer")]
        public string X509SerialNumber
        {
            get
            {
                return this.x509SerialNumberField;
            }
            set
            {
                this.x509SerialNumberField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute("SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class SignatureMethodType
    {

        private string hMACOutputLengthField;

        private System.Xml.XmlNode[] anyField;

        private string algorithmField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "integer")]
        public string HMACOutputLength
        {
            get
            {
                return this.hMACOutputLengthField;
            }
            set
            {
                this.hMACOutputLengthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlNode[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Algorithm
        {
            get
            {
                return this.algorithmField;
            }
            set
            {
                this.algorithmField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute("Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class ReferenceType
    {

        private TransformType[] transformsField;

        private DigestMethodType digestMethodField;

        private byte[] digestValueField;

        private string idField;

        private string uRIField;

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Transform", IsNullable = false)]
        public TransformType[] Transforms
        {
            get
            {
                return this.transformsField;
            }
            set
            {
                this.transformsField = value;
            }
        }

        /// <remarks/>
        public DigestMethodType DigestMethod
        {
            get
            {
                return this.digestMethodField;
            }
            set
            {
                this.digestMethodField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
        public byte[] DigestValue
        {
            get
            {
                return this.digestValueField;
            }
            set
            {
                this.digestValueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string URI
        {
            get
            {
                return this.uRIField;
            }
            set
            {
                this.uRIField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute("Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class TransformType
    {

        private object[] itemsField;

        private string[] textField;

        private string algorithmField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("XPath", typeof(string))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Algorithm
        {
            get
            {
                return this.algorithmField;
            }
            set
            {
                this.algorithmField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute("DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class DigestMethodType
    {

        private System.Xml.XmlNode[] anyField;

        private string algorithmField;

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlNode[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Algorithm
        {
            get
            {
                return this.algorithmField;
            }
            set
            {
                this.algorithmField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute("SignatureValue", Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class SignatureValueType
    {

        private string idField;

        private byte[] valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute(DataType = "base64Binary")]
        public byte[] Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute("KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class KeyInfoType
    {

        private object[] itemsField;

        private ItemsChoiceType2[] itemsElementNameField;

        private string[] textField;

        private string idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("KeyName", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("KeyValue", typeof(KeyValueType))]
        [System.Xml.Serialization.XmlElementAttribute("MgmtData", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("PGPData", typeof(PGPDataType))]
        [System.Xml.Serialization.XmlElementAttribute("RetrievalMethod", typeof(RetrievalMethodType))]
        [System.Xml.Serialization.XmlElementAttribute("SPKIData", typeof(SPKIDataType))]
        [System.Xml.Serialization.XmlElementAttribute("X509Data", typeof(X509DataType))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType2[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute("KeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class KeyValueType
    {

        private object itemField;

        private string[] textField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("DSAKeyValue", typeof(DSAKeyValueType))]
        [System.Xml.Serialization.XmlElementAttribute("RSAKeyValue", typeof(RSAKeyValueType))]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute("DSAKeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class DSAKeyValueType
    {

        private byte[] pField;

        private byte[] qField;

        private byte[] gField;

        private byte[] yField;

        private byte[] jField;

        private byte[] seedField;

        private byte[] pgenCounterField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
        public byte[] P
        {
            get
            {
                return this.pField;
            }
            set
            {
                this.pField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
        public byte[] Q
        {
            get
            {
                return this.qField;
            }
            set
            {
                this.qField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
        public byte[] G
        {
            get
            {
                return this.gField;
            }
            set
            {
                this.gField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
        public byte[] Y
        {
            get
            {
                return this.yField;
            }
            set
            {
                this.yField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
        public byte[] J
        {
            get
            {
                return this.jField;
            }
            set
            {
                this.jField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
        public byte[] Seed
        {
            get
            {
                return this.seedField;
            }
            set
            {
                this.seedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
        public byte[] PgenCounter
        {
            get
            {
                return this.pgenCounterField;
            }
            set
            {
                this.pgenCounterField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute("RSAKeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class RSAKeyValueType
    {

        private byte[] modulusField;

        private byte[] exponentField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
        public byte[] Modulus
        {
            get
            {
                return this.modulusField;
            }
            set
            {
                this.modulusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
        public byte[] Exponent
        {
            get
            {
                return this.exponentField;
            }
            set
            {
                this.exponentField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute("PGPData", Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class PGPDataType
    {

        private object[] itemsField;

        private ItemsChoiceType1[] itemsElementNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("PGPKeyID", typeof(byte[]), DataType = "base64Binary")]
        [System.Xml.Serialization.XmlElementAttribute("PGPKeyPacket", typeof(byte[]), DataType = "base64Binary")]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType1[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#", IncludeInSchema = false)]
    public enum ItemsChoiceType1
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("##any:")]
        Item,

        /// <remarks/>
        PGPKeyID,

        /// <remarks/>
        PGPKeyPacket,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute("RetrievalMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class RetrievalMethodType
    {

        private TransformType[] transformsField;

        private string uRIField;

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Transform", IsNullable = false)]
        public TransformType[] Transforms
        {
            get
            {
                return this.transformsField;
            }
            set
            {
                this.transformsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string URI
        {
            get
            {
                return this.uRIField;
            }
            set
            {
                this.uRIField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute("SPKIData", Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class SPKIDataType
    {

        private byte[][] sPKISexpField;

        private System.Xml.XmlElement anyField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("SPKISexp", DataType = "base64Binary")]
        public byte[][] SPKISexp
        {
            get
            {
                return this.sPKISexpField;
            }
            set
            {
                this.sPKISexpField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute("X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class X509DataType
    {

        private object[] itemsField;

        private ItemsChoiceType[] itemsElementNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("X509CRL", typeof(byte[]), DataType = "base64Binary")]
        [System.Xml.Serialization.XmlElementAttribute("X509Certificate", typeof(byte[]), DataType = "base64Binary")]
        [System.Xml.Serialization.XmlElementAttribute("X509IssuerSerial", typeof(X509IssuerSerialType))]
        [System.Xml.Serialization.XmlElementAttribute("X509SKI", typeof(byte[]), DataType = "base64Binary")]
        [System.Xml.Serialization.XmlElementAttribute("X509SubjectName", typeof(string))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#", IncludeInSchema = false)]
    public enum ItemsChoiceType
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("##any:")]
        Item,

        /// <remarks/>
        X509CRL,

        /// <remarks/>
        X509Certificate,

        /// <remarks/>
        X509IssuerSerial,

        /// <remarks/>
        X509SKI,

        /// <remarks/>
        X509SubjectName,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#", IncludeInSchema = false)]
    public enum ItemsChoiceType2
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("##any:")]
        Item,

        /// <remarks/>
        KeyName,

        /// <remarks/>
        KeyValue,

        /// <remarks/>
        MgmtData,

        /// <remarks/>
        PGPData,

        /// <remarks/>
        RetrievalMethod,

        /// <remarks/>
        SPKIData,

        /// <remarks/>
        X509Data,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute("Object", Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class ObjectType
    {

        private System.Xml.XmlNode[] anyField;

        private string idField;

        private string mimeTypeField;

        private string encodingField;

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlNode[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MimeType
        {
            get
            {
                return this.mimeTypeField;
            }
            set
            {
                this.mimeTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Encoding
        {
            get
            {
                return this.encodingField;
            }
            set
            {
                this.encodingField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute("Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class TransformsType
    {

        private TransformType[] transformField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Transform")]
        public TransformType[] Transform
        {
            get
            {
                return this.transformField;
            }
            set
            {
                this.transformField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute("Manifest", Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class ManifestType
    {

        private ReferenceType[] referenceField;

        private string idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Reference")]
        public ReferenceType[] Reference
        {
            get
            {
                return this.referenceField;
            }
            set
            {
                this.referenceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute("SignatureProperties", Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class SignaturePropertiesType
    {

        private SignaturePropertyType[] signaturePropertyField;

        private string idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("SignatureProperty")]
        public SignaturePropertyType[] SignatureProperty
        {
            get
            {
                return this.signaturePropertyField;
            }
            set
            {
                this.signaturePropertyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute("SignatureProperty", Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class SignaturePropertyType
    {

        private System.Xml.XmlElement[] itemsField;

        private string[] textField;

        private string targetField;

        private string idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Target
        {
            get
            {
                return this.targetField;
            }
            set
            {
                this.targetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2001/04/xmlenc#")]
    [System.Xml.Serialization.XmlRootAttribute("CipherData", Namespace = "http://www.w3.org/2001/04/xmlenc#", IsNullable = false)]
    public partial class CipherDataType
    {

        private object itemField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("CipherReference", typeof(CipherReferenceType))]
        [System.Xml.Serialization.XmlElementAttribute("CipherValue", typeof(byte[]), DataType = "base64Binary")]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2001/04/xmlenc#")]
    [System.Xml.Serialization.XmlRootAttribute("CipherReference", Namespace = "http://www.w3.org/2001/04/xmlenc#", IsNullable = false)]
    public partial class CipherReferenceType
    {

        private TransformsType1 itemField;

        private string uRIField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Transforms")]
        public TransformsType1 Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string URI
        {
            get
            {
                return this.uRIField;
            }
            set
            {
                this.uRIField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName = "TransformsType", Namespace = "http://www.w3.org/2001/04/xmlenc#")]
    public partial class TransformsType1
    {

        private TransformType[] transformField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public TransformType[] Transform
        {
            get
            {
                return this.transformField;
            }
            set
            {
                this.transformField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2001/04/xmlenc#")]
    [System.Xml.Serialization.XmlRootAttribute("EncryptedData", Namespace = "http://www.w3.org/2001/04/xmlenc#", IsNullable = false)]
    public partial class EncryptedDataType : EncryptedType
    {
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(EncryptedKeyType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(EncryptedDataType))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2001/04/xmlenc#")]
    public abstract partial class EncryptedType
    {

        private EncryptionMethodType encryptionMethodField;

        private KeyInfoType keyInfoField;

        private CipherDataType cipherDataField;

        private EncryptionPropertiesType encryptionPropertiesField;

        private string idField;

        private string typeField;

        private string mimeTypeField;

        private string encodingField;

        /// <remarks/>
        public EncryptionMethodType EncryptionMethod
        {
            get
            {
                return this.encryptionMethodField;
            }
            set
            {
                this.encryptionMethodField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyInfoType KeyInfo
        {
            get
            {
                return this.keyInfoField;
            }
            set
            {
                this.keyInfoField = value;
            }
        }

        /// <remarks/>
        public CipherDataType CipherData
        {
            get
            {
                return this.cipherDataField;
            }
            set
            {
                this.cipherDataField = value;
            }
        }

        /// <remarks/>
        public EncryptionPropertiesType EncryptionProperties
        {
            get
            {
                return this.encryptionPropertiesField;
            }
            set
            {
                this.encryptionPropertiesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MimeType
        {
            get
            {
                return this.mimeTypeField;
            }
            set
            {
                this.mimeTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Encoding
        {
            get
            {
                return this.encodingField;
            }
            set
            {
                this.encodingField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2001/04/xmlenc#")]
    [System.Xml.Serialization.XmlRootAttribute("EncryptionMethod", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata", IsNullable = false)]
    public partial class EncryptionMethodType
    {

        private string keySizeField;

        private byte[] oAEPparamsField;

        private System.Xml.XmlNode[] anyField;

        private string algorithmField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "integer")]
        public string KeySize
        {
            get
            {
                return this.keySizeField;
            }
            set
            {
                this.keySizeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
        public byte[] OAEPparams
        {
            get
            {
                return this.oAEPparamsField;
            }
            set
            {
                this.oAEPparamsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlNode[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Algorithm
        {
            get
            {
                return this.algorithmField;
            }
            set
            {
                this.algorithmField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2001/04/xmlenc#")]
    [System.Xml.Serialization.XmlRootAttribute("EncryptionProperties", Namespace = "http://www.w3.org/2001/04/xmlenc#", IsNullable = false)]
    public partial class EncryptionPropertiesType
    {

        private EncryptionPropertyType[] encryptionPropertyField;

        private string idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("EncryptionProperty")]
        public EncryptionPropertyType[] EncryptionProperty
        {
            get
            {
                return this.encryptionPropertyField;
            }
            set
            {
                this.encryptionPropertyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2001/04/xmlenc#")]
    [System.Xml.Serialization.XmlRootAttribute("EncryptionProperty", Namespace = "http://www.w3.org/2001/04/xmlenc#", IsNullable = false)]
    public partial class EncryptionPropertyType
    {

        private System.Xml.XmlElement[] itemsField;

        private string[] textField;

        private string targetField;

        private string idField;

        private System.Xml.XmlAttribute[] anyAttrField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Target
        {
            get
            {
                return this.targetField;
            }
            set
            {
                this.targetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr
        {
            get
            {
                return this.anyAttrField;
            }
            set
            {
                this.anyAttrField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2001/04/xmlenc#")]
    [System.Xml.Serialization.XmlRootAttribute("EncryptedKey", Namespace = "http://www.w3.org/2001/04/xmlenc#", IsNullable = false)]
    public partial class EncryptedKeyType : EncryptedType
    {

        private ReferenceList referenceListField;

        private string carriedKeyNameField;

        private string recipientField;

        /// <remarks/>
        public ReferenceList ReferenceList
        {
            get
            {
                return this.referenceListField;
            }
            set
            {
                this.referenceListField = value;
            }
        }

        /// <remarks/>
        public string CarriedKeyName
        {
            get
            {
                return this.carriedKeyNameField;
            }
            set
            {
                this.carriedKeyNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Recipient
        {
            get
            {
                return this.recipientField;
            }
            set
            {
                this.recipientField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2001/04/xmlenc#")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2001/04/xmlenc#", IsNullable = false)]
    public partial class ReferenceList
    {

        private ReferenceType1[] itemsField;

        private ItemsChoiceType3[] itemsElementNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DataReference", typeof(ReferenceType1))]
        [System.Xml.Serialization.XmlElementAttribute("KeyReference", typeof(ReferenceType1))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public ReferenceType1[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType3[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName = "ReferenceType", Namespace = "http://www.w3.org/2001/04/xmlenc#")]
    public partial class ReferenceType1
    {

        private System.Xml.XmlElement[] anyField;

        private string uRIField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string URI
        {
            get
            {
                return this.uRIField;
            }
            set
            {
                this.uRIField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2001/04/xmlenc#", IncludeInSchema = false)]
    public enum ItemsChoiceType3
    {

        /// <remarks/>
        DataReference,

        /// <remarks/>
        KeyReference,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2001/04/xmlenc#")]
    [System.Xml.Serialization.XmlRootAttribute("AgreementMethod", Namespace = "http://www.w3.org/2001/04/xmlenc#", IsNullable = false)]
    public partial class AgreementMethodType
    {

        private byte[] kANonceField;

        private System.Xml.XmlNode[] anyField;

        private KeyInfoType originatorKeyInfoField;

        private KeyInfoType recipientKeyInfoField;

        private string algorithmField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("KA-Nonce", DataType = "base64Binary")]
        public byte[] KANonce
        {
            get
            {
                return this.kANonceField;
            }
            set
            {
                this.kANonceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlNode[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }

        /// <remarks/>
        public KeyInfoType OriginatorKeyInfo
        {
            get
            {
                return this.originatorKeyInfoField;
            }
            set
            {
                this.originatorKeyInfoField = value;
            }
        }

        /// <remarks/>
        public KeyInfoType RecipientKeyInfo
        {
            get
            {
                return this.recipientKeyInfoField;
            }
            set
            {
                this.recipientKeyInfoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Algorithm
        {
            get
            {
                return this.algorithmField;
            }
            set
            {
                this.algorithmField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [System.Xml.Serialization.XmlRootAttribute("BaseID", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public abstract partial class BaseIDAbstractType
    {

        private string nameQualifierField;

        private string sPNameQualifierField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NameQualifier
        {
            get
            {
                return this.nameQualifierField;
            }
            set
            {
                this.nameQualifierField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SPNameQualifier
        {
            get
            {
                return this.sPNameQualifierField;
            }
            set
            {
                this.sPNameQualifierField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [System.Xml.Serialization.XmlRootAttribute("NameID", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public partial class NameIDType
    {

        private string nameQualifierField;

        private string sPNameQualifierField;

        private string formatField;

        private string sPProvidedIDField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NameQualifier
        {
            get
            {
                return this.nameQualifierField;
            }
            set
            {
                this.nameQualifierField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SPNameQualifier
        {
            get
            {
                return this.sPNameQualifierField;
            }
            set
            {
                this.sPNameQualifierField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Format
        {
            get
            {
                return this.formatField;
            }
            set
            {
                this.formatField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SPProvidedID
        {
            get
            {
                return this.sPProvidedIDField;
            }
            set
            {
                this.sPProvidedIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [System.Xml.Serialization.XmlRootAttribute("EncryptedID", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public partial class EncryptedElementType
    {

        private EncryptedDataType encryptedDataField;

        private EncryptedKeyType[] encryptedKeyField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2001/04/xmlenc#")]
        public EncryptedDataType EncryptedData
        {
            get
            {
                return this.encryptedDataField;
            }
            set
            {
                this.encryptedDataField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("EncryptedKey", Namespace = "http://www.w3.org/2001/04/xmlenc#")]
        public EncryptedKeyType[] EncryptedKey
        {
            get
            {
                return this.encryptedKeyField;
            }
            set
            {
                this.encryptedKeyField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [System.Xml.Serialization.XmlRootAttribute("Assertion", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public partial class AssertionType
    {

        private NameIDType issuerField;

        private SignatureType signatureField;

        private SubjectType subjectField;

        private ConditionsType conditionsField;

        private AdviceType adviceField;

        private StatementAbstractType[] itemsField;

        private string versionField;

        private string idField;

        private System.DateTime issueInstantField;

        /// <remarks/>
        //[System.Xml.Serialization.XmlElement(Namespace="saml")]
        public NameIDType Issuer
        {
            get
            {
                return this.issuerField;
            }
            set
            {
                this.issuerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureType Signature
        {
            get
            {
                return this.signatureField;
            }
            set
            {
                this.signatureField = value;
            }
        }

        /// <remarks/>
        public SubjectType Subject
        {
            get
            {
                return this.subjectField;
            }
            set
            {
                this.subjectField = value;
            }
        }

        /// <remarks/>
        public ConditionsType Conditions
        {
            get
            {
                return this.conditionsField;
            }
            set
            {
                this.conditionsField = value;
            }
        }

        /// <remarks/>
        public AdviceType Advice
        {
            get
            {
                return this.adviceField;
            }
            set
            {
                this.adviceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AttributeStatement", typeof(AttributeStatementType))]
        [System.Xml.Serialization.XmlElementAttribute("AuthnStatement", typeof(AuthnStatementType))]
        [System.Xml.Serialization.XmlElementAttribute("AuthzDecisionStatement", typeof(AuthzDecisionStatementType))]
        [System.Xml.Serialization.XmlElementAttribute("Statement", typeof(StatementAbstractType))]
        public StatementAbstractType[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime IssueInstant
        {
            get
            {
                return this.issueInstantField;
            }
            set
            {
                this.issueInstantField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [System.Xml.Serialization.XmlRootAttribute("Subject", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public partial class SubjectType
    {

        private object[] itemsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("BaseID", typeof(BaseIDAbstractType))]
        [System.Xml.Serialization.XmlElementAttribute("EncryptedID", typeof(EncryptedElementType))]
        [System.Xml.Serialization.XmlElementAttribute("NameID", typeof(NameIDType))]
        [System.Xml.Serialization.XmlElementAttribute("SubjectConfirmation", typeof(SubjectConfirmationType))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [System.Xml.Serialization.XmlRootAttribute("SubjectConfirmation", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public partial class SubjectConfirmationType
    {

        private object itemField;

        private SubjectConfirmationDataType subjectConfirmationDataField;

        private string methodField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("BaseID", typeof(BaseIDAbstractType))]
        [System.Xml.Serialization.XmlElementAttribute("EncryptedID", typeof(EncryptedElementType))]
        [System.Xml.Serialization.XmlElementAttribute("NameID", typeof(NameIDType))]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }

        /// <remarks/>
        public SubjectConfirmationDataType SubjectConfirmationData
        {
            get
            {
                return this.subjectConfirmationDataField;
            }
            set
            {
                this.subjectConfirmationDataField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Method
        {
            get
            {
                return this.methodField;
            }
            set
            {
                this.methodField = value;
            }
        }
    }

    //SAMLString = stringWriter.ToString();
    //SAMLString = SAMLString.Replace("SubjectConfirmationData",
    //    string.Format("SubjectConfirmationData NotOnOrAfter=\"{0:o}\" Recipient=\"{1}\"",
    //    DateTime.UtcNow.AddMinutes(5), Recipient));
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KeyInfoConfirmationDataType))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [System.Xml.Serialization.XmlRootAttribute("SubjectConfirmationData", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public partial class SubjectConfirmationDataType
    {
        private string recipient;

        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Recipient
        {
            get
            {
                return this.recipient;
            }
            set
            {
                this.recipient = value;
            }
        }

        private System.DateTime notOnOrAfterField;
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime NotOnOrAfter
        {
            get
            {
                return this.notOnOrAfterField;
            }
            set
            {
                this.notOnOrAfterField = value;
            }
        }

        private string[] textField;
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public partial class KeyInfoConfirmationDataType : SubjectConfirmationDataType
    {
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [System.Xml.Serialization.XmlRootAttribute("Conditions", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public partial class ConditionsType
    {

        private ConditionAbstractType[] itemsField;

        private System.DateTime notBeforeField;

        private bool notBeforeFieldSpecified;

        private System.DateTime notOnOrAfterField;

        private bool notOnOrAfterFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AudienceRestriction", typeof(AudienceRestrictionType))]
        [System.Xml.Serialization.XmlElementAttribute("Condition", typeof(ConditionAbstractType))]
        [System.Xml.Serialization.XmlElementAttribute("OneTimeUse", typeof(OneTimeUseType))]
        [System.Xml.Serialization.XmlElementAttribute("ProxyRestriction", typeof(ProxyRestrictionType))]
        public ConditionAbstractType[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime NotBefore
        {
            get
            {
                return this.notBeforeField;
            }
            set
            {
                this.notBeforeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool NotBeforeSpecified
        {
            get
            {
                return this.notBeforeFieldSpecified;
            }
            set
            {
                this.notBeforeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime NotOnOrAfter
        {
            get
            {
                return this.notOnOrAfterField;
            }
            set
            {
                this.notOnOrAfterField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool NotOnOrAfterSpecified
        {
            get
            {
                return this.notOnOrAfterFieldSpecified;
            }
            set
            {
                this.notOnOrAfterFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [System.Xml.Serialization.XmlRootAttribute("AudienceRestriction", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public partial class AudienceRestrictionType : ConditionAbstractType
    {

        private string[] audienceField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Audience", DataType = "anyURI")]
        public string[] Audience
        {
            get
            {
                return this.audienceField;
            }
            set
            {
                this.audienceField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ProxyRestrictionType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(OneTimeUseType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AudienceRestrictionType))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [System.Xml.Serialization.XmlRootAttribute("Condition", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public abstract partial class ConditionAbstractType
    {
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [System.Xml.Serialization.XmlRootAttribute("OneTimeUse", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public partial class OneTimeUseType : ConditionAbstractType
    {
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [System.Xml.Serialization.XmlRootAttribute("ProxyRestriction", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public partial class ProxyRestrictionType : ConditionAbstractType
    {

        private string[] audienceField;

        private string countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Audience", DataType = "anyURI")]
        public string[] Audience
        {
            get
            {
                return this.audienceField;
            }
            set
            {
                this.audienceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "nonNegativeInteger")]
        public string Count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [System.Xml.Serialization.XmlRootAttribute("Advice", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public partial class AdviceType
    {

        private object[] itemsField;

        private ItemsChoiceType4[] itemsElementNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("Assertion", typeof(AssertionType))]
        [System.Xml.Serialization.XmlElementAttribute("AssertionIDRef", typeof(string), DataType = "NCName")]
        [System.Xml.Serialization.XmlElementAttribute("AssertionURIRef", typeof(string), DataType = "anyURI")]
        [System.Xml.Serialization.XmlElementAttribute("EncryptedAssertion", typeof(EncryptedElementType))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType4[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IncludeInSchema = false)]
    public enum ItemsChoiceType4
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("##any:")]
        Item,

        /// <remarks/>
        Assertion,

        /// <remarks/>
        AssertionIDRef,

        /// <remarks/>
        AssertionURIRef,

        /// <remarks/>
        EncryptedAssertion,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [System.Xml.Serialization.XmlRootAttribute("AttributeStatement", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public partial class AttributeStatementType : StatementAbstractType
    {

        private object[] itemsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Attribute", typeof(AttributeType))]
        [System.Xml.Serialization.XmlElementAttribute("EncryptedAttribute", typeof(EncryptedElementType))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(RequestedAttributeType))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [System.Xml.Serialization.XmlRootAttribute("Attribute", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public partial class AttributeType
    {

        private object[] attributeValueField;

        private string nameField;

        private string nameFormatField;

        private string friendlyNameField;

        private System.Xml.XmlAttribute[] anyAttrField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AttributeValue", IsNullable = true)]
        public object[] AttributeValue
        {
            get
            {
                return this.attributeValueField;
            }
            set
            {
                this.attributeValueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string NameFormat
        {
            get
            {
                return this.nameFormatField;
            }
            set
            {
                this.nameFormatField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string FriendlyName
        {
            get
            {
                return this.friendlyNameField;
            }
            set
            {
                this.friendlyNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr
        {
            get
            {
                return this.anyAttrField;
            }
            set
            {
                this.anyAttrField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    [System.Xml.Serialization.XmlRootAttribute("RequestedAttribute", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata", IsNullable = false)]
    public partial class RequestedAttributeType : AttributeType
    {

        private bool isRequiredField;

        private bool isRequiredFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool isRequired
        {
            get
            {
                return this.isRequiredField;
            }
            set
            {
                this.isRequiredField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool isRequiredSpecified
        {
            get
            {
                return this.isRequiredFieldSpecified;
            }
            set
            {
                this.isRequiredFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AttributeStatementType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AuthzDecisionStatementType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AuthnStatementType))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [System.Xml.Serialization.XmlRootAttribute("Statement", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public abstract partial class StatementAbstractType
    {
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [System.Xml.Serialization.XmlRootAttribute("AuthnStatement", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public partial class AuthnStatementType : StatementAbstractType
    {

        private SubjectLocalityType subjectLocalityField;

        private AuthnContextType authnContextField;

        private System.DateTime authnInstantField;

        private string sessionIndexField;

        private System.DateTime sessionNotOnOrAfterField;

        private bool sessionNotOnOrAfterFieldSpecified;

        /// <remarks/>
        public SubjectLocalityType SubjectLocality
        {
            get
            {
                return this.subjectLocalityField;
            }
            set
            {
                this.subjectLocalityField = value;
            }
        }

        /// <remarks/>
        public AuthnContextType AuthnContext
        {
            get
            {
                return this.authnContextField;
            }
            set
            {
                this.authnContextField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime AuthnInstant
        {
            get
            {
                return this.authnInstantField;
            }
            set
            {
                this.authnInstantField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SessionIndex
        {
            get
            {
                return this.sessionIndexField;
            }
            set
            {
                this.sessionIndexField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime SessionNotOnOrAfter
        {
            get
            {
                return this.sessionNotOnOrAfterField;
            }
            set
            {
                this.sessionNotOnOrAfterField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SessionNotOnOrAfterSpecified
        {
            get
            {
                return this.sessionNotOnOrAfterFieldSpecified;
            }
            set
            {
                this.sessionNotOnOrAfterFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [System.Xml.Serialization.XmlRootAttribute("SubjectLocality", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public partial class SubjectLocalityType
    {

        private string addressField;

        private string dNSNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DNSName
        {
            get
            {
                return this.dNSNameField;
            }
            set
            {
                this.dNSNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [System.Xml.Serialization.XmlRootAttribute("AuthnContext", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public partial class AuthnContextType
    {

        private object[] itemsField;

        private ItemsChoiceType5[] itemsElementNameField;

        private string[] authenticatingAuthorityField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AuthnContextClassRef", typeof(string), DataType = "anyURI")]
        [System.Xml.Serialization.XmlElementAttribute("AuthnContextDecl", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("AuthnContextDeclRef", typeof(string), DataType = "anyURI")]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType5[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AuthenticatingAuthority", DataType = "anyURI")]
        public string[] AuthenticatingAuthority
        {
            get
            {
                return this.authenticatingAuthorityField;
            }
            set
            {
                this.authenticatingAuthorityField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IncludeInSchema = false)]
    public enum ItemsChoiceType5
    {

        /// <remarks/>
        AuthnContextClassRef,

        /// <remarks/>
        AuthnContextDecl,

        /// <remarks/>
        AuthnContextDeclRef,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [System.Xml.Serialization.XmlRootAttribute("AuthzDecisionStatement", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public partial class AuthzDecisionStatementType : StatementAbstractType
    {

        private ActionType[] actionField;

        private EvidenceType evidenceField;

        private string resourceField;

        private DecisionType decisionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Action")]
        public ActionType[] Action
        {
            get
            {
                return this.actionField;
            }
            set
            {
                this.actionField = value;
            }
        }

        /// <remarks/>
        public EvidenceType Evidence
        {
            get
            {
                return this.evidenceField;
            }
            set
            {
                this.evidenceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Resource
        {
            get
            {
                return this.resourceField;
            }
            set
            {
                this.resourceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public DecisionType Decision
        {
            get
            {
                return this.decisionField;
            }
            set
            {
                this.decisionField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [System.Xml.Serialization.XmlRootAttribute("Action", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public partial class ActionType
    {

        private string namespaceField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Namespace
        {
            get
            {
                return this.namespaceField;
            }
            set
            {
                this.namespaceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    [System.Xml.Serialization.XmlRootAttribute("Evidence", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
    public partial class EvidenceType
    {

        private object[] itemsField;

        private ItemsChoiceType6[] itemsElementNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Assertion", typeof(AssertionType))]
        [System.Xml.Serialization.XmlElementAttribute("AssertionIDRef", typeof(string), DataType = "NCName")]
        [System.Xml.Serialization.XmlElementAttribute("AssertionURIRef", typeof(string), DataType = "anyURI")]
        [System.Xml.Serialization.XmlElementAttribute("EncryptedAssertion", typeof(EncryptedElementType))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType6[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IncludeInSchema = false)]
    public enum ItemsChoiceType6
    {

        /// <remarks/>
        Assertion,

        /// <remarks/>
        AssertionIDRef,

        /// <remarks/>
        AssertionURIRef,

        /// <remarks/>
        EncryptedAssertion,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public enum DecisionType
    {

        /// <remarks/>
        Permit,

        /// <remarks/>
        Deny,

        /// <remarks/>
        Indeterminate,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("Extensions", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class ExtensionsType
    {

        private System.Xml.XmlElement[] anyField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("Status", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class StatusType
    {

        private StatusCodeType statusCodeField;

        private string statusMessageField;

        private StatusDetailType statusDetailField;

        /// <remarks/>
        public StatusCodeType StatusCode
        {
            get
            {
                return this.statusCodeField;
            }
            set
            {
                this.statusCodeField = value;
            }
        }

        /// <remarks/>
        public string StatusMessage
        {
            get
            {
                return this.statusMessageField;
            }
            set
            {
                this.statusMessageField = value;
            }
        }

        /// <remarks/>
        public StatusDetailType StatusDetail
        {
            get
            {
                return this.statusDetailField;
            }
            set
            {
                this.statusDetailField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("StatusCode", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class StatusCodeType
    {

        private StatusCodeType statusCodeField;

        private string valueField;

        /// <remarks/>
        public StatusCodeType StatusCode
        {
            get
            {
                return this.statusCodeField;
            }
            set
            {
                this.statusCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("StatusDetail", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class StatusDetailType
    {

        private System.Xml.XmlElement[] anyField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("AssertionIDRequest", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class AssertionIDRequestType : RequestAbstractType
    {

        private string[] assertionIDRefField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AssertionIDRef", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", DataType = "NCName")]
        public string[] AssertionIDRef
        {
            get
            {
                return this.assertionIDRefField;
            }
            set
            {
                this.assertionIDRefField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(NameIDMappingRequestType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(LogoutRequestType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ManageNameIDRequestType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ArtifactResolveType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AuthnRequestType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(SubjectQueryAbstractType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AuthzDecisionQueryType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AttributeQueryType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AuthnQueryType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AssertionIDRequestType))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    public abstract partial class RequestAbstractType
    {

        private NameIDType issuerField;

        private SignatureType signatureField;

        private ExtensionsType extensionsField;

        private string idField;

        private string versionField;

        private System.DateTime issueInstantField;

        private string destinationField;

        private string consentField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public NameIDType Issuer
        {
            get
            {
                return this.issuerField;
            }
            set
            {
                this.issuerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureType Signature
        {
            get
            {
                return this.signatureField;
            }
            set
            {
                this.signatureField = value;
            }
        }

        /// <remarks/>
        public ExtensionsType Extensions
        {
            get
            {
                return this.extensionsField;
            }
            set
            {
                this.extensionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime IssueInstant
        {
            get
            {
                return this.issueInstantField;
            }
            set
            {
                this.issueInstantField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Destination
        {
            get
            {
                return this.destinationField;
            }
            set
            {
                this.destinationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Consent
        {
            get
            {
                return this.consentField;
            }
            set
            {
                this.consentField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AuthzDecisionQueryType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AttributeQueryType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AuthnQueryType))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("SubjectQuery", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public abstract partial class SubjectQueryAbstractType : RequestAbstractType
    {

        private SubjectType subjectField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public SubjectType Subject
        {
            get
            {
                return this.subjectField;
            }
            set
            {
                this.subjectField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("AuthnQuery", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class AuthnQueryType : SubjectQueryAbstractType
    {

        private RequestedAuthnContextType requestedAuthnContextField;

        private string sessionIndexField;

        /// <remarks/>
        public RequestedAuthnContextType RequestedAuthnContext
        {
            get
            {
                return this.requestedAuthnContextField;
            }
            set
            {
                this.requestedAuthnContextField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SessionIndex
        {
            get
            {
                return this.sessionIndexField;
            }
            set
            {
                this.sessionIndexField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("RequestedAuthnContext", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class RequestedAuthnContextType
    {

        private string[] itemsField;

        private ItemsChoiceType7[] itemsElementNameField;

        private AuthnContextComparisonType comparisonField;

        private bool comparisonFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AuthnContextClassRef", typeof(string), Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", DataType = "anyURI")]
        [System.Xml.Serialization.XmlElementAttribute("AuthnContextDeclRef", typeof(string), Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", DataType = "anyURI")]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public string[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType7[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public AuthnContextComparisonType Comparison
        {
            get
            {
                return this.comparisonField;
            }
            set
            {
                this.comparisonField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ComparisonSpecified
        {
            get
            {
                return this.comparisonFieldSpecified;
            }
            set
            {
                this.comparisonFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IncludeInSchema = false)]
    public enum ItemsChoiceType7
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("urn:oasis:names:tc:SAML:2.0:assertion:AuthnContextClassRef")]
        AuthnContextClassRef,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("urn:oasis:names:tc:SAML:2.0:assertion:AuthnContextDeclRef")]
        AuthnContextDeclRef,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    public enum AuthnContextComparisonType
    {

        /// <remarks/>
        exact,

        /// <remarks/>
        minimum,

        /// <remarks/>
        maximum,

        /// <remarks/>
        better,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("AttributeQuery", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class AttributeQueryType : SubjectQueryAbstractType
    {

        private AttributeType[] attributeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Attribute", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public AttributeType[] Attribute
        {
            get
            {
                return this.attributeField;
            }
            set
            {
                this.attributeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("AuthzDecisionQuery", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class AuthzDecisionQueryType : SubjectQueryAbstractType
    {

        private ActionType[] actionField;

        private EvidenceType evidenceField;

        private string resourceField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Action", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public ActionType[] Action
        {
            get
            {
                return this.actionField;
            }
            set
            {
                this.actionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public EvidenceType Evidence
        {
            get
            {
                return this.evidenceField;
            }
            set
            {
                this.evidenceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Resource
        {
            get
            {
                return this.resourceField;
            }
            set
            {
                this.resourceField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("AuthnRequest", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class AuthnRequestType : RequestAbstractType
    {

        private SubjectType subjectField;

        private NameIDPolicyType nameIDPolicyField;

        private ConditionsType conditionsField;

        private RequestedAuthnContextType requestedAuthnContextField;

        private ScopingType scopingField;

        private bool forceAuthnField;

        private bool forceAuthnFieldSpecified;

        private bool isPassiveField;

        private bool isPassiveFieldSpecified;

        private string protocolBindingField;

        private ushort assertionConsumerServiceIndexField;

        private bool assertionConsumerServiceIndexFieldSpecified;

        private string assertionConsumerServiceURLField;

        private ushort attributeConsumingServiceIndexField;

        private bool attributeConsumingServiceIndexFieldSpecified;

        private string providerNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public SubjectType Subject
        {
            get
            {
                return this.subjectField;
            }
            set
            {
                this.subjectField = value;
            }
        }

        /// <remarks/>
        public NameIDPolicyType NameIDPolicy
        {
            get
            {
                return this.nameIDPolicyField;
            }
            set
            {
                this.nameIDPolicyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public ConditionsType Conditions
        {
            get
            {
                return this.conditionsField;
            }
            set
            {
                this.conditionsField = value;
            }
        }

        /// <remarks/>
        public RequestedAuthnContextType RequestedAuthnContext
        {
            get
            {
                return this.requestedAuthnContextField;
            }
            set
            {
                this.requestedAuthnContextField = value;
            }
        }

        /// <remarks/>
        public ScopingType Scoping
        {
            get
            {
                return this.scopingField;
            }
            set
            {
                this.scopingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool ForceAuthn
        {
            get
            {
                return this.forceAuthnField;
            }
            set
            {
                this.forceAuthnField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ForceAuthnSpecified
        {
            get
            {
                return this.forceAuthnFieldSpecified;
            }
            set
            {
                this.forceAuthnFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool IsPassive
        {
            get
            {
                return this.isPassiveField;
            }
            set
            {
                this.isPassiveField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsPassiveSpecified
        {
            get
            {
                return this.isPassiveFieldSpecified;
            }
            set
            {
                this.isPassiveFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string ProtocolBinding
        {
            get
            {
                return this.protocolBindingField;
            }
            set
            {
                this.protocolBindingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort AssertionConsumerServiceIndex
        {
            get
            {
                return this.assertionConsumerServiceIndexField;
            }
            set
            {
                this.assertionConsumerServiceIndexField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AssertionConsumerServiceIndexSpecified
        {
            get
            {
                return this.assertionConsumerServiceIndexFieldSpecified;
            }
            set
            {
                this.assertionConsumerServiceIndexFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string AssertionConsumerServiceURL
        {
            get
            {
                return this.assertionConsumerServiceURLField;
            }
            set
            {
                this.assertionConsumerServiceURLField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort AttributeConsumingServiceIndex
        {
            get
            {
                return this.attributeConsumingServiceIndexField;
            }
            set
            {
                this.attributeConsumingServiceIndexField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AttributeConsumingServiceIndexSpecified
        {
            get
            {
                return this.attributeConsumingServiceIndexFieldSpecified;
            }
            set
            {
                this.attributeConsumingServiceIndexFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ProviderName
        {
            get
            {
                return this.providerNameField;
            }
            set
            {
                this.providerNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("NameIDPolicy", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class NameIDPolicyType
    {

        private string formatField;

        private string sPNameQualifierField;

        private bool allowCreateField;

        private bool allowCreateFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Format
        {
            get
            {
                return this.formatField;
            }
            set
            {
                this.formatField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SPNameQualifier
        {
            get
            {
                return this.sPNameQualifierField;
            }
            set
            {
                this.sPNameQualifierField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool AllowCreate
        {
            get
            {
                return this.allowCreateField;
            }
            set
            {
                this.allowCreateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AllowCreateSpecified
        {
            get
            {
                return this.allowCreateFieldSpecified;
            }
            set
            {
                this.allowCreateFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("Scoping", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class ScopingType
    {

        private IDPListType iDPListField;

        private string[] requesterIDField;

        private string proxyCountField;

        /// <remarks/>
        public IDPListType IDPList
        {
            get
            {
                return this.iDPListField;
            }
            set
            {
                this.iDPListField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("RequesterID", DataType = "anyURI")]
        public string[] RequesterID
        {
            get
            {
                return this.requesterIDField;
            }
            set
            {
                this.requesterIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "nonNegativeInteger")]
        public string ProxyCount
        {
            get
            {
                return this.proxyCountField;
            }
            set
            {
                this.proxyCountField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("IDPList", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class IDPListType
    {

        private IDPEntryType[] iDPEntryField;

        private string getCompleteField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("IDPEntry")]
        public IDPEntryType[] IDPEntry
        {
            get
            {
                return this.iDPEntryField;
            }
            set
            {
                this.iDPEntryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "anyURI")]
        public string GetComplete
        {
            get
            {
                return this.getCompleteField;
            }
            set
            {
                this.getCompleteField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("IDPEntry", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class IDPEntryType
    {

        private string providerIDField;

        private string nameField;

        private string locField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string ProviderID
        {
            get
            {
                return this.providerIDField;
            }
            set
            {
                this.providerIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Loc
        {
            get
            {
                return this.locField;
            }
            set
            {
                this.locField = value;
            }
        }
    }

    /// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    //[System.Xml.Serialization.XmlRootAttribute("RequestedSecurityToken", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class ResponseType : StatusResponseType
    {

        private object[] itemsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Assertion", typeof(AssertionType), Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        [System.Xml.Serialization.XmlElementAttribute("EncryptedAssertion", typeof(EncryptedElementType), Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }
        //[XmlNamespaceDeclarations]
        //public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
        public ResponseType()
        {
            //xmlns.Add("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
            //xmlns.Add("ds", "http://www.w3.org/2000/09/xmldsig#");
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("RequestSecurityTokenResponse", Namespace = "http://schemas.xmlsoap.org/ws/2005/02/trust", IsNullable = false)]
    public partial class SamlToken
    {
        [XmlElementAttribute("Lifetime", typeof(Lifetime), Namespace = "http://schemas.xmlsoap.org/ws/2005/02/trust")]
        public Lifetime Lifetime { get; set; }

        [XmlElementAttribute("AppliesTo", typeof(AppliesTo), Namespace = "http://schemas.xmlsoap.org/ws/2004/09/policy")]
        public AppliesTo AppliesTo { get; set; }

        [XmlElementAttribute("RequestedSecurityToken", typeof(ResponseType), Namespace = "http://schemas.xmlsoap.org/ws/2005/02/trust")]
        public ResponseType RequestedSecurityToken { get; set; }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
        public SamlToken()
        {
            xmlns.Add("t", "http://schemas.xmlsoap.org/ws/2005/02/trust");
        }
    }

    public partial class Lifetime
    {
        [XmlElementAttribute("Created", typeof(DateTime), Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
        public DateTime Created { get; set; }

        [XmlElementAttribute("Expires", typeof(DateTime), Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
        public DateTime Expires { get; set; }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
        public Lifetime()
        {
            xmlns.Add("wsu", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
        }
    }

    public partial class AppliesTo
    {
        [XmlElementAttribute("EndpointReference", typeof(EndpointReference), Namespace = "http://www.w3.org/2005/08/addressing")]
        public EndpointReference EndpointReference { get; set; }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
        public AppliesTo()
        {
            xmlns.Add("wsp", "http://schemas.xmlsoap.org/ws/2004/09/policy");
        }
    }

    public partial class EndpointReference
    {
        [XmlElementAttribute("Address", typeof(string), Namespace = "http://www.w3.org/2005/08/addressing")]
        public string Address { get; set; }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
        public EndpointReference()
        {
            xmlns.Add("wsa", "http://www.w3.org/2005/08/addressing");
        }
    }

    /// <remarks/>
    //[System.Xml.Serialization.XmlIncludeAttribute(typeof(NameIDMappingResponseType))]
    //[System.Xml.Serialization.XmlIncludeAttribute(typeof(ArtifactResponseType))]
    //[System.Xml.Serialization.XmlIncludeAttribute(typeof(ResponseType))]
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    //[System.Xml.Serialization.XmlRootAttribute("ManageNameIDResponse", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class StatusResponseType
    {

        private NameIDType issuerField;

        private SignatureType signatureField;

        private ExtensionsType extensionsField;

        private StatusType statusField;

        private string idField;

        private string inResponseToField;

        //private string versionField;

        //private System.DateTime issueInstantField;

        //private string destinationField;

        private string consentField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Issuer", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public NameIDType Issuer
        {
            get
            {
                return this.issuerField;
            }
            set
            {
                this.issuerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureType Signature
        {
            get
            {
                return this.signatureField;
            }
            set
            {
                this.signatureField = value;
            }
        }

        /// <remarks/>
        public ExtensionsType Extensions
        {
            get
            {
                return this.extensionsField;
            }
            set
            {
                this.extensionsField = value;
            }
        }

        /// <remarks/>
        public StatusType Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "NCName")]
        public string InResponseTo
        {
            get
            {
                return this.inResponseToField;
            }
            set
            {
                this.inResponseToField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        //public string Version
        //{
        //    get
        //    {
        //        return this.versionField;
        //    }
        //    set
        //    {
        //        this.versionField = value;
        //    }
        //}

        ///// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        //public System.DateTime IssueInstant
        //{
        //    get
        //    {
        //        return this.issueInstantField;
        //    }
        //    set
        //    {
        //        this.issueInstantField = value;
        //    }
        //}

        ///// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        //public string Destination
        //{
        //    get
        //    {
        //        return this.destinationField;
        //    }
        //    set
        //    {
        //        this.destinationField = value;
        //    }
        //}

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Consent
        {
            get
            {
                return this.consentField;
            }
            set
            {
                this.consentField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("ArtifactResolve", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class ArtifactResolveType : RequestAbstractType
    {

        private string artifactField;

        /// <remarks/>
        public string Artifact
        {
            get
            {
                return this.artifactField;
            }
            set
            {
                this.artifactField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("ArtifactResponse", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class ArtifactResponseType : StatusResponseType
    {

        private System.Xml.XmlElement anyField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("ManageNameIDRequest", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class ManageNameIDRequestType : RequestAbstractType
    {

        private object itemField;

        private object item1Field;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("EncryptedID", typeof(EncryptedElementType), Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        [System.Xml.Serialization.XmlElementAttribute("NameID", typeof(NameIDType), Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("NewEncryptedID", typeof(EncryptedElementType))]
        [System.Xml.Serialization.XmlElementAttribute("NewID", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("Terminate", typeof(TerminateType))]
        public object Item1
        {
            get
            {
                return this.item1Field;
            }
            set
            {
                this.item1Field = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("Terminate", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class TerminateType
    {
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("LogoutRequest", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class LogoutRequestType : RequestAbstractType
    {

        private object itemField;

        private string[] sessionIndexField;

        private string reasonField;

        private System.DateTime notOnOrAfterField;

        private bool notOnOrAfterFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("BaseID", typeof(BaseIDAbstractType), Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        [System.Xml.Serialization.XmlElementAttribute("EncryptedID", typeof(EncryptedElementType), Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        [System.Xml.Serialization.XmlElementAttribute("NameID", typeof(NameIDType), Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("SessionIndex")]
        public string[] SessionIndex
        {
            get
            {
                return this.sessionIndexField;
            }
            set
            {
                this.sessionIndexField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Reason
        {
            get
            {
                return this.reasonField;
            }
            set
            {
                this.reasonField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime NotOnOrAfter
        {
            get
            {
                return this.notOnOrAfterField;
            }
            set
            {
                this.notOnOrAfterField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool NotOnOrAfterSpecified
        {
            get
            {
                return this.notOnOrAfterFieldSpecified;
            }
            set
            {
                this.notOnOrAfterFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("NameIDMappingRequest", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class NameIDMappingRequestType : RequestAbstractType
    {

        private object itemField;

        private NameIDPolicyType nameIDPolicyField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("BaseID", typeof(BaseIDAbstractType), Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        [System.Xml.Serialization.XmlElementAttribute("EncryptedID", typeof(EncryptedElementType), Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        [System.Xml.Serialization.XmlElementAttribute("NameID", typeof(NameIDType), Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }

        /// <remarks/>
        public NameIDPolicyType NameIDPolicy
        {
            get
            {
                return this.nameIDPolicyField;
            }
            set
            {
                this.nameIDPolicyField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    [System.Xml.Serialization.XmlRootAttribute("NameIDMappingResponse", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IsNullable = false)]
    public partial class NameIDMappingResponseType : StatusResponseType
    {

        private object itemField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("EncryptedID", typeof(EncryptedElementType), Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        [System.Xml.Serialization.XmlElementAttribute("NameID", typeof(NameIDType), Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName = "ExtensionsType", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    [System.Xml.Serialization.XmlRootAttribute("Extensions", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata", IsNullable = false)]
    public partial class ExtensionsType1
    {

        private System.Xml.XmlElement[] anyField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    [System.Xml.Serialization.XmlRootAttribute("EntitiesDescriptor", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata", IsNullable = false)]
    public partial class EntitiesDescriptorType
    {

        private SignatureType signatureField;

        private ExtensionsType1 extensionsField;

        private object[] itemsField;

        private System.DateTime validUntilField;

        private bool validUntilFieldSpecified;

        private string cacheDurationField;

        private string idField;

        private string nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureType Signature
        {
            get
            {
                return this.signatureField;
            }
            set
            {
                this.signatureField = value;
            }
        }

        /// <remarks/>
        public ExtensionsType1 Extensions
        {
            get
            {
                return this.extensionsField;
            }
            set
            {
                this.extensionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("EntitiesDescriptor", typeof(EntitiesDescriptorType))]
        [System.Xml.Serialization.XmlElementAttribute("EntityDescriptor", typeof(EntityDescriptorType))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime validUntil
        {
            get
            {
                return this.validUntilField;
            }
            set
            {
                this.validUntilField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool validUntilSpecified
        {
            get
            {
                return this.validUntilFieldSpecified;
            }
            set
            {
                this.validUntilFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "duration")]
        public string cacheDuration
        {
            get
            {
                return this.cacheDurationField;
            }
            set
            {
                this.cacheDurationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    [System.Xml.Serialization.XmlRootAttribute("EntityDescriptor", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata", IsNullable = false)]
    public partial class EntityDescriptorType
    {

        private SignatureType signatureField;

        private ExtensionsType1 extensionsField;

        private object[] itemsField;

        private OrganizationType organizationField;

        private ContactType[] contactPersonField;

        private AdditionalMetadataLocationType[] additionalMetadataLocationField;

        private string entityIDField;

        private System.DateTime validUntilField;

        private bool validUntilFieldSpecified;

        private string cacheDurationField;

        private string idField;

        private System.Xml.XmlAttribute[] anyAttrField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureType Signature
        {
            get
            {
                return this.signatureField;
            }
            set
            {
                this.signatureField = value;
            }
        }

        /// <remarks/>
        public ExtensionsType1 Extensions
        {
            get
            {
                return this.extensionsField;
            }
            set
            {
                this.extensionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AffiliationDescriptor", typeof(AffiliationDescriptorType))]
        [System.Xml.Serialization.XmlElementAttribute("AttributeAuthorityDescriptor", typeof(AttributeAuthorityDescriptorType))]
        [System.Xml.Serialization.XmlElementAttribute("AuthnAuthorityDescriptor", typeof(AuthnAuthorityDescriptorType))]
        [System.Xml.Serialization.XmlElementAttribute("IDPSSODescriptor", typeof(IDPSSODescriptorType))]
        [System.Xml.Serialization.XmlElementAttribute("PDPDescriptor", typeof(PDPDescriptorType))]
        [System.Xml.Serialization.XmlElementAttribute("RoleDescriptor", typeof(RoleDescriptorType))]
        [System.Xml.Serialization.XmlElementAttribute("SPSSODescriptor", typeof(SPSSODescriptorType))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        public OrganizationType Organization
        {
            get
            {
                return this.organizationField;
            }
            set
            {
                this.organizationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ContactPerson")]
        public ContactType[] ContactPerson
        {
            get
            {
                return this.contactPersonField;
            }
            set
            {
                this.contactPersonField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AdditionalMetadataLocation")]
        public AdditionalMetadataLocationType[] AdditionalMetadataLocation
        {
            get
            {
                return this.additionalMetadataLocationField;
            }
            set
            {
                this.additionalMetadataLocationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string entityID
        {
            get
            {
                return this.entityIDField;
            }
            set
            {
                this.entityIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime validUntil
        {
            get
            {
                return this.validUntilField;
            }
            set
            {
                this.validUntilField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool validUntilSpecified
        {
            get
            {
                return this.validUntilFieldSpecified;
            }
            set
            {
                this.validUntilFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "duration")]
        public string cacheDuration
        {
            get
            {
                return this.cacheDurationField;
            }
            set
            {
                this.cacheDurationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr
        {
            get
            {
                return this.anyAttrField;
            }
            set
            {
                this.anyAttrField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    [System.Xml.Serialization.XmlRootAttribute("AffiliationDescriptor", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata", IsNullable = false)]
    public partial class AffiliationDescriptorType
    {

        private SignatureType signatureField;

        private ExtensionsType1 extensionsField;

        private string[] affiliateMemberField;

        private KeyDescriptorType[] keyDescriptorField;

        private string affiliationOwnerIDField;

        private System.DateTime validUntilField;

        private bool validUntilFieldSpecified;

        private string cacheDurationField;

        private string idField;

        private System.Xml.XmlAttribute[] anyAttrField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureType Signature
        {
            get
            {
                return this.signatureField;
            }
            set
            {
                this.signatureField = value;
            }
        }

        /// <remarks/>
        public ExtensionsType1 Extensions
        {
            get
            {
                return this.extensionsField;
            }
            set
            {
                this.extensionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AffiliateMember", DataType = "anyURI")]
        public string[] AffiliateMember
        {
            get
            {
                return this.affiliateMemberField;
            }
            set
            {
                this.affiliateMemberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("KeyDescriptor")]
        public KeyDescriptorType[] KeyDescriptor
        {
            get
            {
                return this.keyDescriptorField;
            }
            set
            {
                this.keyDescriptorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string affiliationOwnerID
        {
            get
            {
                return this.affiliationOwnerIDField;
            }
            set
            {
                this.affiliationOwnerIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime validUntil
        {
            get
            {
                return this.validUntilField;
            }
            set
            {
                this.validUntilField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool validUntilSpecified
        {
            get
            {
                return this.validUntilFieldSpecified;
            }
            set
            {
                this.validUntilFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "duration")]
        public string cacheDuration
        {
            get
            {
                return this.cacheDurationField;
            }
            set
            {
                this.cacheDurationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr
        {
            get
            {
                return this.anyAttrField;
            }
            set
            {
                this.anyAttrField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    [System.Xml.Serialization.XmlRootAttribute("KeyDescriptor", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata", IsNullable = false)]
    public partial class KeyDescriptorType
    {

        private KeyInfoType keyInfoField;

        private EncryptionMethodType[] encryptionMethodField;

        private KeyTypes useField;

        private bool useFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyInfoType KeyInfo
        {
            get
            {
                return this.keyInfoField;
            }
            set
            {
                this.keyInfoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("EncryptionMethod")]
        public EncryptionMethodType[] EncryptionMethod
        {
            get
            {
                return this.encryptionMethodField;
            }
            set
            {
                this.encryptionMethodField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public KeyTypes use
        {
            get
            {
                return this.useField;
            }
            set
            {
                this.useField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool useSpecified
        {
            get
            {
                return this.useFieldSpecified;
            }
            set
            {
                this.useFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    public enum KeyTypes
    {

        /// <remarks/>
        encryption,

        /// <remarks/>
        signing,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    [System.Xml.Serialization.XmlRootAttribute("AttributeAuthorityDescriptor", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata", IsNullable = false)]
    public partial class AttributeAuthorityDescriptorType : RoleDescriptorType
    {

        private EndpointType[] attributeServiceField;

        private EndpointType[] assertionIDRequestServiceField;

        private string[] nameIDFormatField;

        private string[] attributeProfileField;

        private AttributeType[] attributeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AttributeService")]
        public EndpointType[] AttributeService
        {
            get
            {
                return this.attributeServiceField;
            }
            set
            {
                this.attributeServiceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AssertionIDRequestService")]
        public EndpointType[] AssertionIDRequestService
        {
            get
            {
                return this.assertionIDRequestServiceField;
            }
            set
            {
                this.assertionIDRequestServiceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("NameIDFormat", DataType = "anyURI")]
        public string[] NameIDFormat
        {
            get
            {
                return this.nameIDFormatField;
            }
            set
            {
                this.nameIDFormatField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AttributeProfile", DataType = "anyURI")]
        public string[] AttributeProfile
        {
            get
            {
                return this.attributeProfileField;
            }
            set
            {
                this.attributeProfileField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Attribute", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public AttributeType[] Attribute
        {
            get
            {
                return this.attributeField;
            }
            set
            {
                this.attributeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(IndexedEndpointType))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    [System.Xml.Serialization.XmlRootAttribute("SingleLogoutService", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata", IsNullable = false)]
    public partial class EndpointType
    {

        private System.Xml.XmlElement[] anyField;

        private string bindingField;

        private string locationField;

        private string responseLocationField;

        private System.Xml.XmlAttribute[] anyAttrField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Binding
        {
            get
            {
                return this.bindingField;
            }
            set
            {
                this.bindingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string ResponseLocation
        {
            get
            {
                return this.responseLocationField;
            }
            set
            {
                this.responseLocationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr
        {
            get
            {
                return this.anyAttrField;
            }
            set
            {
                this.anyAttrField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AttributeAuthorityDescriptorType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(PDPDescriptorType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AuthnAuthorityDescriptorType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(SSODescriptorType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(SPSSODescriptorType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(IDPSSODescriptorType))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    [System.Xml.Serialization.XmlRootAttribute("RoleDescriptor", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata", IsNullable = false)]
    public abstract partial class RoleDescriptorType
    {

        private SignatureType signatureField;

        private ExtensionsType1 extensionsField;

        private KeyDescriptorType[] keyDescriptorField;

        private OrganizationType organizationField;

        private ContactType[] contactPersonField;

        private string idField;

        private System.DateTime validUntilField;

        private bool validUntilFieldSpecified;

        private string cacheDurationField;

        private string[] protocolSupportEnumerationField;

        private string errorURLField;

        private System.Xml.XmlAttribute[] anyAttrField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureType Signature
        {
            get
            {
                return this.signatureField;
            }
            set
            {
                this.signatureField = value;
            }
        }

        /// <remarks/>
        public ExtensionsType1 Extensions
        {
            get
            {
                return this.extensionsField;
            }
            set
            {
                this.extensionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("KeyDescriptor")]
        public KeyDescriptorType[] KeyDescriptor
        {
            get
            {
                return this.keyDescriptorField;
            }
            set
            {
                this.keyDescriptorField = value;
            }
        }

        /// <remarks/>
        public OrganizationType Organization
        {
            get
            {
                return this.organizationField;
            }
            set
            {
                this.organizationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ContactPerson")]
        public ContactType[] ContactPerson
        {
            get
            {
                return this.contactPersonField;
            }
            set
            {
                this.contactPersonField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime validUntil
        {
            get
            {
                return this.validUntilField;
            }
            set
            {
                this.validUntilField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool validUntilSpecified
        {
            get
            {
                return this.validUntilFieldSpecified;
            }
            set
            {
                this.validUntilFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "duration")]
        public string cacheDuration
        {
            get
            {
                return this.cacheDurationField;
            }
            set
            {
                this.cacheDurationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string[] protocolSupportEnumeration
        {
            get
            {
                return this.protocolSupportEnumerationField;
            }
            set
            {
                this.protocolSupportEnumerationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string errorURL
        {
            get
            {
                return this.errorURLField;
            }
            set
            {
                this.errorURLField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr
        {
            get
            {
                return this.anyAttrField;
            }
            set
            {
                this.anyAttrField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    [System.Xml.Serialization.XmlRootAttribute("Organization", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata", IsNullable = false)]
    public partial class OrganizationType
    {

        private ExtensionsType1 extensionsField;

        private localizedNameType[] organizationNameField;

        private localizedNameType[] organizationDisplayNameField;

        private localizedURIType[] organizationURLField;

        private System.Xml.XmlAttribute[] anyAttrField;

        /// <remarks/>
        public ExtensionsType1 Extensions
        {
            get
            {
                return this.extensionsField;
            }
            set
            {
                this.extensionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("OrganizationName")]
        public localizedNameType[] OrganizationName
        {
            get
            {
                return this.organizationNameField;
            }
            set
            {
                this.organizationNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("OrganizationDisplayName")]
        public localizedNameType[] OrganizationDisplayName
        {
            get
            {
                return this.organizationDisplayNameField;
            }
            set
            {
                this.organizationDisplayNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("OrganizationURL")]
        public localizedURIType[] OrganizationURL
        {
            get
            {
                return this.organizationURLField;
            }
            set
            {
                this.organizationURLField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr
        {
            get
            {
                return this.anyAttrField;
            }
            set
            {
                this.anyAttrField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    [System.Xml.Serialization.XmlRootAttribute("OrganizationName", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata", IsNullable = false)]
    public partial class localizedNameType
    {

        private string langField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get
            {
                return this.langField;
            }
            set
            {
                this.langField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    [System.Xml.Serialization.XmlRootAttribute("OrganizationURL", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata", IsNullable = false)]
    public partial class localizedURIType
    {

        private string langField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get
            {
                return this.langField;
            }
            set
            {
                this.langField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute(DataType = "anyURI")]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    [System.Xml.Serialization.XmlRootAttribute("ContactPerson", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata", IsNullable = false)]
    public partial class ContactType
    {

        private ExtensionsType1 extensionsField;

        private string companyField;

        private string givenNameField;

        private string surNameField;

        private string[] emailAddressField;

        private string[] telephoneNumberField;

        private ContactTypeType contactTypeField;

        private System.Xml.XmlAttribute[] anyAttrField;

        /// <remarks/>
        public ExtensionsType1 Extensions
        {
            get
            {
                return this.extensionsField;
            }
            set
            {
                this.extensionsField = value;
            }
        }

        /// <remarks/>
        public string Company
        {
            get
            {
                return this.companyField;
            }
            set
            {
                this.companyField = value;
            }
        }

        /// <remarks/>
        public string GivenName
        {
            get
            {
                return this.givenNameField;
            }
            set
            {
                this.givenNameField = value;
            }
        }

        /// <remarks/>
        public string SurName
        {
            get
            {
                return this.surNameField;
            }
            set
            {
                this.surNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("EmailAddress", DataType = "anyURI")]
        public string[] EmailAddress
        {
            get
            {
                return this.emailAddressField;
            }
            set
            {
                this.emailAddressField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("TelephoneNumber")]
        public string[] TelephoneNumber
        {
            get
            {
                return this.telephoneNumberField;
            }
            set
            {
                this.telephoneNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ContactTypeType contactType
        {
            get
            {
                return this.contactTypeField;
            }
            set
            {
                this.contactTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr
        {
            get
            {
                return this.anyAttrField;
            }
            set
            {
                this.anyAttrField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    public enum ContactTypeType
    {

        /// <remarks/>
        technical,

        /// <remarks/>
        support,

        /// <remarks/>
        administrative,

        /// <remarks/>
        billing,

        /// <remarks/>
        other,
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(SPSSODescriptorType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(IDPSSODescriptorType))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    public abstract partial class SSODescriptorType : RoleDescriptorType
    {

        private IndexedEndpointType[] artifactResolutionServiceField;

        private EndpointType[] singleLogoutServiceField;

        private EndpointType[] manageNameIDServiceField;

        private string[] nameIDFormatField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ArtifactResolutionService")]
        public IndexedEndpointType[] ArtifactResolutionService
        {
            get
            {
                return this.artifactResolutionServiceField;
            }
            set
            {
                this.artifactResolutionServiceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("SingleLogoutService")]
        public EndpointType[] SingleLogoutService
        {
            get
            {
                return this.singleLogoutServiceField;
            }
            set
            {
                this.singleLogoutServiceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ManageNameIDService")]
        public EndpointType[] ManageNameIDService
        {
            get
            {
                return this.manageNameIDServiceField;
            }
            set
            {
                this.manageNameIDServiceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("NameIDFormat", DataType = "anyURI")]
        public string[] NameIDFormat
        {
            get
            {
                return this.nameIDFormatField;
            }
            set
            {
                this.nameIDFormatField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    [System.Xml.Serialization.XmlRootAttribute("ArtifactResolutionService", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata", IsNullable = false)]
    public partial class IndexedEndpointType : EndpointType
    {

        private ushort indexField;

        private bool isDefaultField;

        private bool isDefaultFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort index
        {
            get
            {
                return this.indexField;
            }
            set
            {
                this.indexField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool isDefault
        {
            get
            {
                return this.isDefaultField;
            }
            set
            {
                this.isDefaultField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool isDefaultSpecified
        {
            get
            {
                return this.isDefaultFieldSpecified;
            }
            set
            {
                this.isDefaultFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    [System.Xml.Serialization.XmlRootAttribute("AuthnAuthorityDescriptor", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata", IsNullable = false)]
    public partial class AuthnAuthorityDescriptorType : RoleDescriptorType
    {

        private EndpointType[] authnQueryServiceField;

        private EndpointType[] assertionIDRequestServiceField;

        private string[] nameIDFormatField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AuthnQueryService")]
        public EndpointType[] AuthnQueryService
        {
            get
            {
                return this.authnQueryServiceField;
            }
            set
            {
                this.authnQueryServiceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AssertionIDRequestService")]
        public EndpointType[] AssertionIDRequestService
        {
            get
            {
                return this.assertionIDRequestServiceField;
            }
            set
            {
                this.assertionIDRequestServiceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("NameIDFormat", DataType = "anyURI")]
        public string[] NameIDFormat
        {
            get
            {
                return this.nameIDFormatField;
            }
            set
            {
                this.nameIDFormatField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    [System.Xml.Serialization.XmlRootAttribute("IDPSSODescriptor", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata", IsNullable = false)]
    public partial class IDPSSODescriptorType : SSODescriptorType
    {

        private EndpointType[] singleSignOnServiceField;

        private EndpointType[] nameIDMappingServiceField;

        private EndpointType[] assertionIDRequestServiceField;

        private string[] attributeProfileField;

        private AttributeType[] attributeField;

        private bool wantAuthnRequestsSignedField;

        private bool wantAuthnRequestsSignedFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("SingleSignOnService")]
        public EndpointType[] SingleSignOnService
        {
            get
            {
                return this.singleSignOnServiceField;
            }
            set
            {
                this.singleSignOnServiceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("NameIDMappingService")]
        public EndpointType[] NameIDMappingService
        {
            get
            {
                return this.nameIDMappingServiceField;
            }
            set
            {
                this.nameIDMappingServiceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AssertionIDRequestService")]
        public EndpointType[] AssertionIDRequestService
        {
            get
            {
                return this.assertionIDRequestServiceField;
            }
            set
            {
                this.assertionIDRequestServiceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AttributeProfile", DataType = "anyURI")]
        public string[] AttributeProfile
        {
            get
            {
                return this.attributeProfileField;
            }
            set
            {
                this.attributeProfileField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Attribute", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public AttributeType[] Attribute
        {
            get
            {
                return this.attributeField;
            }
            set
            {
                this.attributeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool WantAuthnRequestsSigned
        {
            get
            {
                return this.wantAuthnRequestsSignedField;
            }
            set
            {
                this.wantAuthnRequestsSignedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool WantAuthnRequestsSignedSpecified
        {
            get
            {
                return this.wantAuthnRequestsSignedFieldSpecified;
            }
            set
            {
                this.wantAuthnRequestsSignedFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    [System.Xml.Serialization.XmlRootAttribute("PDPDescriptor", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata", IsNullable = false)]
    public partial class PDPDescriptorType : RoleDescriptorType
    {

        private EndpointType[] authzServiceField;

        private EndpointType[] assertionIDRequestServiceField;

        private string[] nameIDFormatField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AuthzService")]
        public EndpointType[] AuthzService
        {
            get
            {
                return this.authzServiceField;
            }
            set
            {
                this.authzServiceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AssertionIDRequestService")]
        public EndpointType[] AssertionIDRequestService
        {
            get
            {
                return this.assertionIDRequestServiceField;
            }
            set
            {
                this.assertionIDRequestServiceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("NameIDFormat", DataType = "anyURI")]
        public string[] NameIDFormat
        {
            get
            {
                return this.nameIDFormatField;
            }
            set
            {
                this.nameIDFormatField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    [System.Xml.Serialization.XmlRootAttribute("SPSSODescriptor", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata", IsNullable = false)]
    public partial class SPSSODescriptorType : SSODescriptorType
    {

        private IndexedEndpointType[] assertionConsumerServiceField;

        private AttributeConsumingServiceType[] attributeConsumingServiceField;

        private bool authnRequestsSignedField;

        private bool authnRequestsSignedFieldSpecified;

        private bool wantAssertionsSignedField;

        private bool wantAssertionsSignedFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AssertionConsumerService")]
        public IndexedEndpointType[] AssertionConsumerService
        {
            get
            {
                return this.assertionConsumerServiceField;
            }
            set
            {
                this.assertionConsumerServiceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AttributeConsumingService")]
        public AttributeConsumingServiceType[] AttributeConsumingService
        {
            get
            {
                return this.attributeConsumingServiceField;
            }
            set
            {
                this.attributeConsumingServiceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool AuthnRequestsSigned
        {
            get
            {
                return this.authnRequestsSignedField;
            }
            set
            {
                this.authnRequestsSignedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AuthnRequestsSignedSpecified
        {
            get
            {
                return this.authnRequestsSignedFieldSpecified;
            }
            set
            {
                this.authnRequestsSignedFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool WantAssertionsSigned
        {
            get
            {
                return this.wantAssertionsSignedField;
            }
            set
            {
                this.wantAssertionsSignedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool WantAssertionsSignedSpecified
        {
            get
            {
                return this.wantAssertionsSignedFieldSpecified;
            }
            set
            {
                this.wantAssertionsSignedFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    [System.Xml.Serialization.XmlRootAttribute("AttributeConsumingService", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata", IsNullable = false)]
    public partial class AttributeConsumingServiceType
    {

        private localizedNameType[] serviceNameField;

        private localizedNameType[] serviceDescriptionField;

        private RequestedAttributeType[] requestedAttributeField;

        private ushort indexField;

        private bool isDefaultField;

        private bool isDefaultFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ServiceName")]
        public localizedNameType[] ServiceName
        {
            get
            {
                return this.serviceNameField;
            }
            set
            {
                this.serviceNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ServiceDescription")]
        public localizedNameType[] ServiceDescription
        {
            get
            {
                return this.serviceDescriptionField;
            }
            set
            {
                this.serviceDescriptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("RequestedAttribute")]
        public RequestedAttributeType[] RequestedAttribute
        {
            get
            {
                return this.requestedAttributeField;
            }
            set
            {
                this.requestedAttributeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort index
        {
            get
            {
                return this.indexField;
            }
            set
            {
                this.indexField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool isDefault
        {
            get
            {
                return this.isDefaultField;
            }
            set
            {
                this.isDefaultField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool isDefaultSpecified
        {
            get
            {
                return this.isDefaultFieldSpecified;
            }
            set
            {
                this.isDefaultFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:oasis:names:tc:SAML:2.0:metadata")]
    [System.Xml.Serialization.XmlRootAttribute("AdditionalMetadataLocation", Namespace = "urn:oasis:names:tc:SAML:2.0:metadata", IsNullable = false)]
    public partial class AdditionalMetadataLocationType
    {

        private string namespaceField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string @namespace
        {
            get
            {
                return this.namespaceField;
            }
            set
            {
                this.namespaceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute(DataType = "anyURI")]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }
}