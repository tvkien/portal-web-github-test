using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class BankDistrictRepository : IRepository<BankDistrict>
    {
        private readonly Table<BankDistrictEntity> table;
        private readonly Table<BankDistrictView> view;

        public BankDistrictRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table =TestDataContext.Get(connectionString).GetTable<BankDistrictEntity>();
            view = TestDataContext.Get(connectionString).GetTable<BankDistrictView>();
        }
        public IQueryable<BankDistrict> Select()
        {
            return view.Select(x => new BankDistrict()
                    {
                        BankDistrictId = x.BankDistrictID,
                        BankId = x.BankID,
                        DistrictId =  x.DistrictID,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        EditedByUserId = x.EditedByUserID,
                        BankDistrictAccessId = x.BankDistrictAccessID,
                        BankName = x.BankName,
                        Name = x.Name,//DistrictName
                        StateId = x.StateID,
                        LiCode = x.LICode,
                        Hide = x.Hide??false
                    });
        }

        public void Save(BankDistrict item)
        {
            var entity = table.FirstOrDefault(x => x.BankDistrictID.Equals(item.BankDistrictId));

            if (entity.IsNull())
            {
                entity = new BankDistrictEntity();
                table.InsertOnSubmit(entity);
            }
            MapBankDistrict(item, entity);
            table.Context.SubmitChanges();
            item.BankDistrictId = entity.BankDistrictID;
        }

        public void Delete(BankDistrict item)
        {
            if (item.IsNotNull())
            {
                var entity = table.FirstOrDefault(x => x.BankDistrictID.Equals(item.BankDistrictId));
                if (entity != null)
                {
                    table.DeleteOnSubmit(entity);
                    table.Context.SubmitChanges();
                }
            }
        }

        private void MapBankDistrict(BankDistrict source, BankDistrictEntity destination)
        {
            destination.BankID = source.BankId;
            destination.DistrictID = source.DistrictId;
            destination.StartDate = source.StartDate;
            destination.EndDate = source.EndDate;
            destination.EditedByUserID = source.EditedByUserId;
            destination.BankDistrictAccessID = source.BankDistrictAccessId;
            destination.Hide = source.Hide;
        }
    }
}
