using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryBubbleSheetErrorRepository : IRepository<BubbleSheetError>
    {
        private readonly List<BubbleSheetError> table;
        private static int index = 8;

        public InMemoryBubbleSheetErrorRepository()
        {
            table = AddBubbleSheetErrors();
        }

        private List<BubbleSheetError> AddBubbleSheetErrors()
        {
            return new List<BubbleSheetError>
                       {
                           new BubbleSheetError{ BubbleSheetErrorId = 1,IsCorrected = false, BubbleSheetId = 12, FileName = "test1.pdf", RelatedImage = "singleimage.png",ErrorCode = 7, Message = "Message for Unit Test.", CreatedDate = DateTime.UtcNow},
                           new BubbleSheetError{ BubbleSheetErrorId = 3,IsCorrected = false, BubbleSheetId = 14, FileName = "test3.pdf", RelatedImage = "singleimage.png",ErrorCode = 16, Message = "Could not locate bubbles.", CreatedDate = DateTime.UtcNow},
                           new BubbleSheetError{ BubbleSheetErrorId = 4,IsCorrected = false, BubbleSheetId = 15, FileName = "test4.pdf", RelatedImage = "singleimage.png",ErrorCode = 64, Message = "Did not find correct number of questions.", CreatedDate = DateTime.UtcNow},
                           new BubbleSheetError{ BubbleSheetErrorId = 5,IsCorrected = false, BubbleSheetId = 16, FileName = "test5.pdf", RelatedImage = "singleimage.png",ErrorCode = -1, Message = "Unreadable roster position.", CreatedDate = DateTime.UtcNow},
                           new BubbleSheetError{ BubbleSheetErrorId = 6,IsCorrected = false, BubbleSheetId = 17, FileName = "test5.pdf", RelatedImage = "singleimage.png",ErrorCode = 32, Message = "Did not find correct number of bubbles for a question.", CreatedDate = DateTime.UtcNow},
                           new BubbleSheetError{ BubbleSheetErrorId = 7,IsCorrected = false, BubbleSheetId = 19, FileName = "test5.pdf", RelatedImage = "singleimage.png",ErrorCode = 8, Message = "Could not find anchors.", CreatedDate = DateTime.UtcNow},
                           new BubbleSheetError{ BubbleSheetErrorId = 2,IsCorrected = false, BubbleSheetId = 13, FileName = "test2.pdf", RelatedImage = "singleimage.png",ErrorCode = 0, Message = "Barcode could not be read.", CreatedDate = DateTime.UtcNow},
                           new BubbleSheetError{ BubbleSheetErrorId = 10,IsCorrected = false, BubbleSheetId = 20, FileName = "corrupt.pdf", RelatedImage = "corrupt.png",ErrorCode = 4, Message = "Corript File.", CreatedDate = DateTime.UtcNow},
                       };
        }

        public IQueryable<BubbleSheetError> Select()
        {
            return table.AsQueryable();
        }

        public void Save(BubbleSheetError item)
        {
            var entity = table.FirstOrDefault(x => x.BubbleSheetErrorId.Equals(item.BubbleSheetErrorId));

            if (entity.IsNull())
            {
                item.BubbleSheetErrorId = index++;
                table.Add(item);    
            }
            else
            {
                Mapper.CreateMap<BubbleSheetError, BubbleSheetError>();
                Mapper.Map(item, entity);
            }
        }

        public void Delete(BubbleSheetError item)
        {
            table.Remove(item);
        }
    }
}
