using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data;
using LinkIt.BubbleSheetPortal.InteractiveRubric.DataContext;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Entities;

namespace LinkIt.BubbleSheetPortal.InteractiveRubric.Repositories
{
    public class RubricQuestionCategoryRepository : IRubricQuestionCategoryRepository
    {
        private readonly Table<RubricQuestionCategoryEntity> table;

        public RubricQuestionCategoryRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = RubricDataContext.Get(connectionString).GetTable<RubricQuestionCategoryEntity>();
        }

        public IQueryable<RubricQuestionCategory> Select()
        {
            return table.Select(x => new RubricQuestionCategory
            {
                CategoryCode = x.CategoryCode,
                CategoryName = x.CategoryName,
                CreatedBy = x.CreatedBy,
                CreatedDate = x.CreatedDate,
                OrderNumber = x.OrderNumber,
                RubricQuestionCategoryID = x.RubricQuestionCategoryID,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate,
                VirtualQuestionID = x.VirtualQuestionID,
                PointsPossible = x.PointsPossible ?? 0
            });
        }

        public void Save(RubricQuestionCategory item)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(RubricQuestionCategory item)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<RubricQuestionCategory> Insert(List<RubricQuestionCategory> rubricQuestionCategories)
        {
            foreach (var item in rubricQuestionCategories)
            {
                var entity = new RubricQuestionCategoryEntity
                {
                    VirtualQuestionID = item.VirtualQuestionID,
                    CategoryCode = string.IsNullOrEmpty(item.CategoryCode) ? item.CategoryName : item.CategoryCode,
                    CategoryName = item.CategoryName,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    OrderNumber = item.OrderNumber,
                    PointsPossible = item.PointsPossible
                };
                table.InsertOnSubmit(entity);
                table.Context.SubmitChanges();
                item.RubricQuestionCategoryID = entity.RubricQuestionCategoryID;
            }

            return rubricQuestionCategories;
        }

        public IEnumerable<RubricQuestionCategory> Update(IEnumerable<RubricQuestionCategory> rubricQuestionCategories)
        {
            foreach (var item in rubricQuestionCategories)
            {
                var entity = table.FirstOrDefault(x => x.RubricQuestionCategoryID.Equals(item.RubricQuestionCategoryID));
                entity.CategoryName = item.CategoryName;
                entity.CategoryCode = string.IsNullOrEmpty(item.CategoryCode) ? item.CategoryName : item.CategoryCode;
                entity.OrderNumber = item.OrderNumber;
                entity.UpdatedBy = item.UpdatedBy;
                entity.UpdatedDate = System.DateTime.Now;
                entity.PointsPossible = item.PointsPossible;
            }
            table.Context.SubmitChanges();
            return rubricQuestionCategories;
        }

        public void Delete(IEnumerable<RubricQuestionCategory> rubricQuestionCategories)
        {
            var rubricQuestionCategoryIds = rubricQuestionCategories.Select(x => x.RubricQuestionCategoryID).ToArray();
            var entities = table.Where(x => rubricQuestionCategoryIds.Contains(x.RubricQuestionCategoryID)).ToArray();
            if (entities?.Length > 0)
            {
                table.DeleteAllOnSubmit(entities);

                table.Context.SubmitChanges();
            }
        }

        public void Delete(IEnumerable<int> rubricQuestionCategoryIds)
        {
            var entities = table.Where(x => rubricQuestionCategoryIds.Contains(x.RubricQuestionCategoryID)).ToArray();
            if (entities?.Length > 0)
            {
                table.DeleteAllOnSubmit(entities);

                table.Context.SubmitChanges();
            }
        }
    }
}
