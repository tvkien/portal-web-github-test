using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S3Library.Entities;
using S3Library.Utilities;

namespace S3Library.Services
{
    public class S3PortalLinkService: BaseService
    {
        public S3PortalLinkService(string connectionString) : base(connectionString)
        {
        }

        public string InsertS3PortalLink(int serviceType, int? districtID, string bucketName, string filePath)
        {
            try
            {
                string portalKey = Utility.BuildPortalKey();
                var s3PortalLink = new S3PortalLink
                                    {
                                        ServiceType = serviceType,
                                        DistrictID = districtID,
                                        BucketName = bucketName,
                                        FilePath = filePath,
                                        DateCreated = DateTime.Now,
                                        PortalKey = portalKey
                                    };
                _context.S3PortalLink.AddObject(s3PortalLink);
                _context.SaveChanges();
                return portalKey;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public S3PortalLink GetS3PortalLinkById(int s3PortalLinkId)
        {
            try
            {
                return _context.S3PortalLink.FirstOrDefault(x => x.S3PortalLinkID == s3PortalLinkId);                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public S3PortalLink GetS3PortalLinkByKey(string portalKey)
        {
            try
            {
                return _context.S3PortalLink.FirstOrDefault(x => String.Equals(x.PortalKey, portalKey));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
