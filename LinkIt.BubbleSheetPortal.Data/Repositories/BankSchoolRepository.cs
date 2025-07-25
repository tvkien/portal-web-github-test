using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class BankSchoolRepository : IRepository<BankSchool>
    {
        private readonly Table<BankSchoolEntity> table;
        private readonly Table<BankSchoolView> view;

        public BankSchoolRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table =TestDataContext.Get(connectionString).GetTable<BankSchoolEntity>();
            view = TestDataContext.Get(connectionString).GetTable<BankSchoolView>();
        }
        public IQueryable<BankSchool> Select()
        {
            return view.Select(x => new BankSchool()
                    {
                        BankSchoolId = x.BankSchoolID,
                        BankId = x.BankID,
                        SchoolId = x.SchoolID,
                        Name = x.Name,//School Name
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        EditedByUserId = x.EditedByUserID,
                        DistrictId = x.DistrictID,
                        DistrictName = x.DistrictName
                    });
        }

        public void Save(BankSchool item)
        {
            var entity = table.FirstOrDefault(x => x.BankSchoolID.Equals(item.BankSchoolId));

            if (entity.IsNull())
            {
                entity = new BankSchoolEntity();
                table.InsertOnSubmit(entity);
            }
            MapBankSchool(item, entity);
            table.Context.SubmitChanges();
            item.BankSchoolId = entity.BankSchoolID;
        }

        public void Delete(BankSchool item)
        {
            if (item.IsNotNull())
            {
                var entity = table.FirstOrDefault(x => x.BankSchoolID.Equals(item.BankSchoolId));
                if (entity != null)
                {
                    table.DeleteOnSubmit(entity);
                    table.Context.SubmitChanges();
                }
            }
        }

        private void MapBankSchool(BankSchool source, BankSchoolEntity destination)
        {
            destination.BankSchoolID = source.BankSchoolId;
            destination.BankID = source.BankId;
            destination.SchoolID = source.SchoolId;
            destination.StartDate = source.StartDate;
            destination.EndDate = source.EndDate;
            destination.EditedByUserID = source.EditedByUserId;
        }
    }
}
