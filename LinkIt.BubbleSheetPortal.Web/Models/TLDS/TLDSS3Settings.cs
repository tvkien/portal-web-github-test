using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;

namespace LinkIt.BubbleSheetPortal.Web.Models.TLDS
{
    public class TLDSS3Settings
    {
        private int _profileId;

        public static TLDSS3Settings GetTLDSS3Settings(int prodileId)
        {
            var setting = new TLDSS3Settings();
            setting._profileId = prodileId;
            return setting;
        }
        public  string TLDSBucket
        {
            get
            {
                return LinkitConfigurationManager.GetS3Settings().TLDSBucket;//get from Vault
            }
        }

        private string _tldsFolder;
        public  string TLDSFolder
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_tldsFolder))
                {
                    _tldsFolder = ConfigurationManager.AppSettings[ContaintUtil.AppSettingTLDSFolderName];
                    if (_tldsFolder == null)
                    {
                        _tldsFolder = string.Empty;
                    }
                }
                return _tldsFolder;
            }
        }

        private string _tldsFormFolder;
        public  string TLDSFormFolder
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_tldsFormFolder))
                {
                    _tldsFormFolder = string.Format("{0}/{1}/Form", TLDSFolder.RemoveEndSlash(),_profileId);
                }
             
                return _tldsFormFolder;
            }
        }
        public string TLDSUploadStatementFolder
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_tldsFormFolder))
                {
                    _tldsFormFolder = string.Format("{0}/StatementUploaded", TLDSFolder.RemoveEndSlash());
                }

                return _tldsFormFolder;
            }
        }
        public string GetFormPath(string pdfFormFileName)
        {
            string s3FormFileFolder = string.Format("{0}/{1}", TLDSFormFolder.RemoveEndSlash(),
                pdfFormFileName);
            return s3FormFileFolder;
        }
        public string GetStatementUploadedPath(string pdfStatementUploadedFileName)
        {
            string s3FormFileFolder = string.Format("{0}/{1}", TLDSUploadStatementFolder.RemoveEndSlash(),
                pdfStatementUploadedFileName);
            return s3FormFileFolder;
        }
        private string _tldsPhotoFolder;
        public  string TLDSPhotoFolder
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_tldsPhotoFolder))
                {
                    _tldsPhotoFolder = string.Format("{0}/{1}/Photo", TLDSFolder.RemoveEndSlash(), _profileId);
                }
                
                return _tldsPhotoFolder;
            }
        }

        public string GetPhotoPath(string photoFileName)
        {
            string s3PhotoFileFolder = string.Format("{0}/{1}", TLDSPhotoFolder.RemoveEndSlash(),
                photoFileName);
            return s3PhotoFileFolder;
        }
        private string _tldsBlankTemplateSection234;
        public string TLDSBlankTemplateSection234
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_tldsBlankTemplateSection234))
                {
                    _tldsBlankTemplateSection234 = string.Format("{0}/blank_template_section234.pdf", TLDSFolder.RemoveEndSlash());
                }

                return _tldsBlankTemplateSection234;
            }
        }

        public string GetZipBatchReportPath(string zipFileName, string currentUserName)
        {
            var zipFormFolder = string.Format("{0}/Form/BatchReportZip/{1}", TLDSFolder.RemoveEndSlash(), currentUserName);
            string s3FormFileFolder = string.Format("{0}/{1}", zipFormFolder,
                zipFileName);
            return s3FormFileFolder;
        }
        public string GetZipSummaryReportPath(string zipFileName, string currentUserName)
        {
            var zipFormFolder = string.Format("{0}/Form/SummaryReportZip/{1}", TLDSFolder.RemoveEndSlash(), currentUserName);
            string s3FormFileFolder = string.Format("{0}/{1}", zipFormFolder,
                zipFileName);
            return s3FormFileFolder;
        }

        public string TLDSUploadEYALTFolder
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_tldsFormFolder))
                {
                    _tldsFormFolder = string.Format("{0}/EYALT", TLDSFolder.RemoveEndSlash());
                }

                return _tldsFormFolder;
            }
        }

        public string GetEYALTUploadedPath(int profileId, string fileName)
        {
            string s3FormFileFolder = string.Format("{0}/{1}/{2}", TLDSUploadEYALTFolder.RemoveEndSlash(), profileId, fileName);
            return s3FormFileFolder;
        }
    }
}
