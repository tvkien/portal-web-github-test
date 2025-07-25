using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Models.Old.XpsDistrictUpload;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class XpsDistrictUploadRepository : IInsertSelect<XpsDistrictUpload>
    {
        private readonly Table<xpsDistrictUploadEntity> table;
        public XpsDistrictUploadRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<xpsDistrictUploadEntity>();
        }
        public void Save(XpsDistrictUpload item)
        {
        }

        public IQueryable<XpsDistrictUpload> Select()
        {
            return table.Select(x => new XpsDistrictUpload()
            {
                DistrictID = x.DistrictID,
                Run = x.Run,
                UploadTypeID = x.UploadTypeID,
                xpsDistrictUploadID = x.xpsDistrictUploadID,
                ClassNameType = x.ClassNameType,
                ScheduledTime = x.ScheduledTime,
            });
        }
    }
}
