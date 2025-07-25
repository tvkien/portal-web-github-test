using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using S3Library;

namespace LinkIt.BubbleSheetPortal.Common
{
    public static class PassageUtil
    {
        public static List<string> GetReferenceContents(IS3Service s3Service, string xmlContent, string s3Domain, string upLoadBucketName, string AUVirtualTestROFolder)
        {
            var result = new List<string>();
            var xmlContentProcessing = new XmlContentProcessing(xmlContent);
            var refObjs = xmlContentProcessing.GetReferenceObjects();

            if (refObjs == null)
                return result;

            foreach (var refObj in refObjs)
            {
                var content = GetReferenceContent(s3Service, refObj.Key, refObj.Value, s3Domain, upLoadBucketName, AUVirtualTestROFolder);

                if (!string.IsNullOrEmpty(content))
                {
                    content = content.Replace("class=\"passage\"", "class=\"passage1\"");//New Flash version contains <div class="passage" styleName="passage"> that will lead to error in background ,temporary replace it to passage1 to avoid error
                    content = AdjustXmlContentFloatImg(content);
                    var refobjectid = string.IsNullOrWhiteSpace(refObj.Key) ? refObj.Value : refObj.Key;
                    content = HtmlUtils.ConvertTags(content, new List<string> { "passage" }, "div", string.Empty, string.Format(" refobjectid=\"{0}\"", refobjectid));
                    result.Add(content);
                }
            }

            return result;
        }

        private static string AdjustXmlContentFloatImg(string xmlContent)
        {
            var xmlContentProcessing = new XmlContentProcessing(xmlContent);
            xmlContentProcessing.AdjustXmlContentFloatImg();
            var result = xmlContentProcessing.GetXmlContent();

            return result;
        }
        public static string GetReferenceContent(IS3Service s3Service, string refObjectID, string data, string s3Domain, string upLoadBucketName, string AUVirtualTestROFolder)
        {
            var result = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(refObjectID) && !string.IsNullOrWhiteSpace(data))
                {
                    using (var client = new HttpClient())
                    {
                        //data = System.Web.HttpUtility.UrlEncode(data);//no need to encode, but make sure url start with http
                        HttpResponseMessage response = client.GetAsync(data).Result;
                        result = response.Content.ReadAsStringAsync().Result;
                        string originalResult = result;

                        var lastIndex = data.LastIndexOf("/", System.StringComparison.Ordinal);
                        var rootPath = data.Substring(0, lastIndex);

                        result = HtmlUtils.UpdateImageUrls(rootPath, result);
                        return  string.IsNullOrEmpty(result) ? originalResult: result;
                    }
                }


                bool notFound;
                result = GetS3PassageContent(s3Service, Int32.Parse(refObjectID), upLoadBucketName, AUVirtualTestROFolder, out notFound);
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }
        public static string GetS3PassageContent(IS3Service s3Service, int qtiRefObjectId, string upLoadBucketName, string AUVirtualTestROFolder, out bool notFoudJsonPassage)
        {
            notFoudJsonPassage = false;
            try
            {
                //using (var client = new HttpClient())
                //{
                //    var passageUrl = GetS3PassageXmlUrl(qtiRefObjectId, s3Domain,upLoadBucketName,AUVirtualTestROFolder);
                //    //now get passage from passage xml on S3
                //    HttpResponseMessage response = client.GetAsync(passageUrl).Result;
                //    var result = response.Content.ReadAsStringAsync().Result;
                //    if (response.StatusCode == HttpStatusCode.OK)
                //    {
                //        return result;
                //    }
                //    else
                //    {
                //        notFoudJsonPassage = true;
                //        return string.Empty;
                //    }
                //}
                var xmlPath = string.Empty;
                if (string.IsNullOrEmpty(AUVirtualTestROFolder))
                {
                    xmlPath = string.Format("RO/RO_{0}.xml", qtiRefObjectId);
                }
                else
                {
                    xmlPath = string.Format("{0}/RO/RO_{1}.xml", AUVirtualTestROFolder.RemoveEndSlash(), qtiRefObjectId);
                }
                var result = s3Service.GetReferenceContent(upLoadBucketName, xmlPath);
                if (result == null)
                    notFoudJsonPassage = true;
                return result;
            }
            catch
            {
                return string.Empty;
            }

        }

        public static string GetS3PassageXmlUrl(int qtiRefObjectId, string s3Domain, string upLoadBucketName, string AUVirtualTestROFolder)
        {
            var result = string.Empty;

            var subDomain = UrlUtil.GenerateS3Subdomain(s3Domain, upLoadBucketName);
            if (string.IsNullOrEmpty(AUVirtualTestROFolder))
            {
                result = string.Format("{0}/RO/RO_{1}.xml", subDomain.RemoveEndSlash(), qtiRefObjectId);
            }
            else
            {
                result = string.Format("{0}/{1}/RO/RO_{2}.xml", subDomain.RemoveEndSlash(),
               AUVirtualTestROFolder.RemoveEndSlash().RemoveStartSlash(), qtiRefObjectId);
            }

            return result;

        }
        public static string UpdateS3LinkForPassageMedia(string xmlContent, string s3Domain, string upLoadBucketName, string AUVirtualTestFolder)
        {
            //change audioRef="/RO/RO_6069_media/English Sentences with Audio Using the Word _Ashamed_-201503060435084447.mp3">
            // or <img class="imageupload " drawable="false" percent="10" src="..\RO\RO_6069_media\1-9706-1417056015-201503060434384817.jpg" height="330" width="600" source="" />
            //to S3 direct linkt such as  https://s3.amazonaws.com/testitemmedia/Vina/RO/RO_6069_media/1-9706-1417056015-201503060434384817.jpg
            try
            {
                var xmlContentProcessing = new XmlContentProcessing(xmlContent);

                xmlContent = xmlContentProcessing.UpdateS3LinkForPassageMedia(s3Domain.RemoveEndSlash(), upLoadBucketName,
                AUVirtualTestFolder);
                return xmlContent;
            }
            catch
            {
                return xmlContent;
            }
        }
    }
}
