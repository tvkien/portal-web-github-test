using System;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;
using AutoMapper;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTI3PSubjectGradeLicenseRepository : IReadOnlyRepository<QTI3PSubjectGradeLicenses>
    {
        private readonly Table<QTI3PSubjectGradeLicenseEntity> table;

        public QTI3PSubjectGradeLicenseRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<QTI3PSubjectGradeLicenseEntity>();
        }

        public IQueryable<QTI3PSubjectGradeLicenses> Select()
        {
            return table.Select(x => new QTI3PSubjectGradeLicenses
            {
                       QTI3PSubjectGradeLicensesID = x.QTI3pSubjectGradeLicensesID,
                       QTI3pLicensesID = x.QTI3pLicensesID,
                       Subject = x.Subject,
                       GradeID = x.GradeID,
                       DistrictID = x.DistrictID,
                       Status = x.Status
                    });
        }
    }
}
