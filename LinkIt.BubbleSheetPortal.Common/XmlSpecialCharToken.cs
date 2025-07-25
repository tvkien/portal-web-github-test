using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Common
{
    public class XmlSpecialCharToken
    {
        private static Dictionary<string, string> SpecialCharDictionary = new Dictionary<string, string>()
        {   //some html characters that cause error when loading into xml
            //see full list here: http://www.degraeve.com/reference/specialcharacters.php
            {"&lsquo;","‘"},
            {"&rsquo;","’"},
            {"&sbquo;","‚"},
            {"&ldquo;","“"},
            {"&rdquo;","”"},
            //{"&bdquo;","„"},
            //{"&dagger;","†"},
            //{"&Dagger;","‡"},
            //{"&permil;","‰"},
            //{"&lsaquo;","‹"},
            //{"&rsaquo;","›"},
            //{"&spades;","♠"},
            //{"&clubs;","♣"},
            //{"&hearts;","♥"},
            //{"&diams;","♦"},
            //{"&oline;","‾"},
            //{"&larr;","←"},
            //{"&uarr;","↑"},
            //{"&rarr;","→"},
            //{"&darr;","↓"},
            //{"&trade;","™"},
            //{"&#x2122;","™"}, //will be processed with &# for all special chars start with &#
            //{"&quot;","\""},
            //{"&amp;","&"},
            //{"&frasl;","/"},
            //{"&lt;","<"},
            //{"&gt;",">"},
            //{"&hellip;","…"},
             {"&times;", "Ã—" },
            {"&ndash;","–"},
            {"&mdash;","—"},
            //{"&iexcl;","¡"},
            //{"&cent;","¢"},
            //{"&pound;","£"},
            //{"&curren;","¤"},
            //{"&yen;","¥"},
            //{"&brvbar;","¦"},
            //{"&brkbar;","¦"},
            //{"&sect;","§"},
            //{"&uml;","¨"},
            //{"&die;","¨"},
            //{"&copy;","©"},
            //{"&ordf;","ª"},
            //{"&laquo;","«"},
            //{"&not;","¬"},
            //{"&shy;",""},
            //{"&reg;","®"},
            //{"&macr;","¯"},
            //{"&hibar;","¯"},
            //{"&deg;","°"},
            //{"&plusmn;","±"},
            //{"&sup2;","²"},
            //{"&sup3;","³"},
            //{"&acute;","´"},
            //{"&micro;","µ"},
            //{"&para;","¶"},
            //{"&middot;","·"},
            //{"&cedil;","¸"},
            //{"&sup1;","¹"},
            //{"&ordm;","º"},
            //{"&raquo;","»"},

            //{"&frac14;","¼"},
            //{"&frac12;","½"},
            //{"&frac34;","¾"},
            //{"&iquest;","¿"},

            //{"&Agrave;","À"},//special chars on pallet,not process here
            //{"&Aacute;","Á"},
            //{"&Acirc;","Â"},
            //{"&Atilde;","Ã"},
            //{"&Auml;","Ä"},
            //{"&Aring;","Å"},
            //{"&AElig;","Æ"},

            //{"&Ccedil;","Ç"},

            //{"&Egrave;","È"},
            //{"&Eacute;","É"},
            //{"&Ecirc;","Ê"},
            //{"&Euml;","Ë"},

            //{"&Igrave;","Ì"},
            //{"&Iacute;","Í"},
            //{"&Icirc;","Î"},

            //{"&Iuml;","Ï"},
            //{"&ETH;","Ð"},
            //{"&Ntilde;","Ñ"},

            //{"&Ograve;","Ò"},
            //{"&Oacute;","Ó"},
            //{"&Ocirc;","Ô"},
            //{"&Otilde;","Õ"},
            //{"&Ouml;","Ö"},

            //{"&times;","×"},
            //{"&Oslash;","Ø"},

            //{"&Ugrave;","Ù"},
            //{"&Uacute;","Ú"},
            //{"&Ucirc;","Û"},
            //{"&Uuml;","Ü"},

            //{"&Yacute;","Ý"},
            //{"&THORN;","Þ"},
            //{"&szlig;","ß"},

            //{"&agrave;","à"},
            //{"&aacute;","á"},
            //{"&acirc;","â"},
            //{"&atilde;","ã"},
            //{"&auml;","ä"},
            //{"&aring;","å"},
            //{"&aelig;","æ"},

            //{"&ccedil;","ç"},

            //{"&egrave;","è"},
            //{"&eacute;","é"},
            //{"&ecirc;","ê"},
            //{"&euml;","ë"},

            //{"&igrave;","ì"},
            //{"&iacute;","í"},
            //{"&icirc;","î"},
            //{"&iuml;","ï"},

            //{"&eth;","ð"},
            //{"&ntilde;","ñ"},
            //{"&ograve;","ò"},
            //{"&oacute;","ó"},
            //{"&ocirc;","ô"},
            //{"&otilde;","õ"},
            //{"&ouml;","ö"},
            //{"&divide;","÷"},
            //{"&oslash;","ø"},

            //{"&ugrave;","ù"},
            //{"&uacute;","ú"},
            //{"&ucirc;","û"},
            //{"&uuml;","ü"},

            //{"&yacute;","ý"},
            //{"&thorn;","þ"},
            //{"&yuml;","ÿ"},
            //{"&Alpha;","Α"},
            //{"&alpha;","α"},
            //{"&Beta;","Β"},
            //{"&beta;","β"},
            //{"&Gamma;","Γ"},
            //{"&gamma;","γ"},
            //{"&Delta;","Δ"},
            //{"&delta;","δ"},
            //{"&Epsilon;","Ε"},
            //{"&epsilon;","ε"},
            //{"&Zeta;","Ζ"},
            //{"&zeta;","ζ"},
            //{"&Eta;","Η"},
            //{"&eta;","η"},
            //{"&Theta;","Θ"},
            //{"&theta;","θ"},
            //{"&Iota;","Ι"},
            //{"&iota;","ι"},
            //{"&Kappa;","Κ"},
            //{"&kappa;","κ"},
            //{"&Lambda;","Λ"},
            //{"&lambda;","λ"},
            //{"&Mu;","Μ"},
            //{"&mu;","μ"},
            //{"&Nu;","Ν"},
            //{"&nu;","Ξ"},
            //{"&Xi;",""},
            //{"&xi;","ξ"},
            //{"&Omicron;","Ο"},
            //{"&omicron;","ο"},
            //{"&Pi;","Π"},
            //{"&pi;","π"},
            //{"&Rho;","Ρ"},
            //{"&rho;","ρ"},
            //{"&sigma;","σ"},
            //{"&Tau;","Τ"},
            //{"&tau;","τ"},
            //{"&Upsilon;","Υ"},
            //{"&upsilon;","υ"},
            //{"&Phi;","Φ"},
            //{"&phi;","φ"},
            //{"&Chi;","Χ"},
            //{"&chi;","χ"},
            //{"&Psi;","Ψ"},
            //{"&psi;","ψ"},
            //{"&Omega;","Ω"},
            //{"&omega;","ω"},
            {"&bull;","•"}
        };
        Dictionary<string, string> _specialCharReplacementDic =
        new Dictionary<string, string>();
        public XmlSpecialCharToken(bool useCDATA=false)
        {
            if (useCDATA)
            {
                _specialCharReplacementDic.Add("&#", string.Format("<![CDATA[{0}]]>", Guid.NewGuid().ToString()));
                foreach (var item in XmlSpecialCharToken.SpecialCharDictionary)
                {
                    _specialCharReplacementDic.Add(item.Key, string.Format("<![CDATA[{0}]]>", Guid.NewGuid().ToString()));
                }
            }
            else
            {
                //Sometime special charaters are inside a CDATA block already, so use using a nother CDATA will cause error
                _specialCharReplacementDic.Add("&#", Guid.NewGuid().ToString());
                foreach (var item in XmlSpecialCharToken.SpecialCharDictionary)
                {
                    _specialCharReplacementDic.Add(item.Key, Guid.NewGuid().ToString());
                }
            }
        }

        public Dictionary<string, string> SpecialCharReplacementDic
        {
            get { return _specialCharReplacementDic; }
        }

       

    }
}
