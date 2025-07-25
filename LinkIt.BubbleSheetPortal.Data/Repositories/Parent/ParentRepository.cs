using System;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.Parent
{
    public class ParentRepository : IRepository<ParentDto>
    {
        private readonly Table<ParentEntity> table;

        public ParentRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<ParentEntity>();
        }

        public IQueryable<ParentDto> Select()
        {
            return table.Select(x => new ParentDto
            {
                ParentID = x.ParentID,
                Code = x.Code,
                CreatedBy = x.CreatedBy.GetValueOrDefault(),
                CreatedDate = x.CreatedDate.GetValueOrDefault(),
                DistrictID = x.DistrictID,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                ModifiedBy = x.ModifiedBy,
                ModifiedDate = x.ModifiedDate.GetValueOrDefault(),
                UserID = x.UserID                 
            });
        }

        public void Save(ParentDto item)
        {
            var entity = table.FirstOrDefault(x => x.ParentID.Equals(item.ParentID));

            if (entity == null)
            {
                entity = new ParentEntity();                
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);
            table.Context.SubmitChanges();
            item.ParentID = entity.ParentID;
        }

        public void Delete(ParentDto item)
        {
            var entity = table.FirstOrDefault(x => x.ParentID.Equals(item.ParentID));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(ParentDto item, ParentEntity entity)
        {            
            entity.DistrictID = item.DistrictID;
            entity.Code = item.Code;
            entity.CreatedBy = item.CreatedBy;
            entity.CreatedDate = item.CreatedDate;
            entity.ModifiedBy = item.ModifiedBy;
            entity.ModifiedDate = DateTime.UtcNow;
            entity.Email = item.Email;
            entity.FirstName = item.FirstName;
            entity.LastName = item.LastName;
            entity.UserID = item.UserID;            
        }         
    }
}
