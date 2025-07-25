using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class BankRepository : IBankRepository
    {
        private readonly Table<BankEntity> table;
        private readonly TestDataContext _testDataContext;

        public BankRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<BankEntity>();
            _testDataContext = TestDataContext.Get(connectionString);
        }

        public IQueryable<Bank> Select()
        {
            return table.Select(x => new Bank
                                {
                                    Id = x.BankID,
                                    SubjectID = x.SubjectID,
                                    Name = x.Name,
                                    CreatedByUserId = x.CreatedByUserID.GetValueOrDefault(),
                                    BankAccessID = x.BankAccessID,
                                    CreatedDate = x.CreatedDate,
                                    UpdatedDate = x.UpdatedDate,
                                    Archived = x.Archived
                                });
        }

        public void Save(Bank item)
        {
            var entity = table.FirstOrDefault(x => x.BankID.Equals(item.Id));

            if (entity == null)
            {
                entity = new BankEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);
            table.Context.SubmitChanges();
            item.Id = entity.BankID;
        }

        public void Delete(Bank item)
        {
            var entity = table.FirstOrDefault(x => x.BankID.Equals(item.Id));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(Bank item, BankEntity entity)
        {
            entity.Name = item.Name;
            entity.SubjectID = item.SubjectID;
            entity.CreatedByUserID = item.CreatedByUserId;
            entity.BankAccessID = item.BankAccessID;
            entity.CreatedDate = item.CreatedDate;
            entity.UpdatedDate = item.UpdatedDate;
            entity.Archived = item.Archived;
        }

        public BankProperty GetBankProperty(int bankId)
        {
            var result = _testDataContext.BankProperties(bankId).FirstOrDefault();
            if (result != null)
            {
                return new BankProperty()
                {
                    Id = result.BankId,
                    Name = result.Name,
                    Author = result.Author,
                    CreatedByUserId = result.CreatedByUserID ?? 0,
                    CreatedDate = result.CreatedDate ?? DateTime.MinValue,
                    UpdatedDate = result.updatedDate ?? DateTime.MinValue,
                    GradeId = result.GradeID,
                    GradeName = result.GradeName,
                    SubjectID = result.SubjectId,
                    SubjectName = result.SubjectName,
                    StateId = result.StateId,
                    Archived = result.Archived??false
                };
            }
            return  new BankProperty();
        }

        public bool CanDeleteBankByID(int bankId)
        {
            var result = _testDataContext.CanDeleteBank(bankId).FirstOrDefault();
            if (result == null || !result.Column1.HasValue)
                return false;

            return result.Column1.Value;
        }

        public void DeleteBankByID(int bankId)
        {
            _testDataContext.DeleteBankByID(bankId);
        }
        public bool CheckBankLock(int bankId, int userId, int districtId)
        {
            return _testDataContext.fnCheckBankLock(bankId, userId, districtId)??false;
        }

        public PrintTestOptions CheckPermissionPrintQuestionAndAnswerKey(int bankId, int userId, int districtId)
        {
            //TODO: code here ( call store or function ).
            var obj = _testDataContext.fnCanPrintQuestionAndAnswerKey(bankId, userId, districtId).FirstOrDefault();
            if (obj != null)
                return new PrintTestOptions()
                {
                    CanPrintQuestion = obj.CanPrintQuestion.GetValueOrDefault(),
                    CanPrintAnswerKey = obj.CanPrintAnswerKey.GetValueOrDefault()
                };
            return  new PrintTestOptions();
        }

        public List<BankOrder> GetBankOrders(int districtId)
        {
            var data = _testDataContext.GetBankOrder(districtId);
            if (data != null)
                return data.Select(o => new BankOrder
                {
                    BankId = o.BankID,
                    Order = o.Order
                }).ToList();
            return new List<BankOrder>();
        }
      
    }
}
