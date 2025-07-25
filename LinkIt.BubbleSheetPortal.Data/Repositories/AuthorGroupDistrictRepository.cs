using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class AuthorGroupDistrictRepository : IRepository<AuthorGroupDistrict>
    {
        private readonly Table<AuthorGroupDistrictEntity> table;
        private readonly AssessmentDataContext dbContext;

        public AuthorGroupDistrictRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<AuthorGroupDistrictEntity>();
            dbContext = AssessmentDataContext.Get(connectionString);

            Mapper.CreateMap<AuthorGroupDistrict, AuthorGroupDistrictEntity>();
        }

        public IQueryable<AuthorGroupDistrict> Select()
        {
            return table.Select(x => new AuthorGroupDistrict
                                     {
                                         AuthorGroupId = x.AuthorGroupID,
                                         DistrictId = x.DistrictID
                                     });
        }

        public void Save(AuthorGroupDistrict item)
        {
            var entity =
                table.FirstOrDefault(x => x.AuthorGroupID.Equals(item.AuthorGroupId) && x.DistrictID.Equals(item.DistrictId));

            if (entity.IsNull())
            {
                entity = new AuthorGroupDistrictEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
        }

        public void Delete(AuthorGroupDistrict item)
        {
            var entity =
                table.FirstOrDefault(x => x.AuthorGroupID.Equals(item.AuthorGroupId) && x.DistrictID.Equals(item.DistrictId));
            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}