using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryBubbleSheetErrorViewRepository : IReadOnlyRepository<BubbleSheetError>
    {
        private List<BubbleSheetError> table; 

        public InMemoryBubbleSheetErrorViewRepository()
        {
            table = AddBubbleSheetErrors();
        }

        private List<BubbleSheetError> AddBubbleSheetErrors()
        {
            return new List<BubbleSheetError>
                       {
                           new BubbleSheetError{ BubbleSheetErrorId = 1, ErrorCode = 7, BubbleSheetId = 12, FileName = "test1.pdf", UploadedBy = "Pierce, Chase", RelatedImage = "singleimage.png", Message = "Message for Unit Test.", CreatedDate = DateTime.UtcNow},
                           new BubbleSheetError{ BubbleSheetErrorId = 1, ErrorCode = 0, BubbleSheetId = 12, FileName = "test1.pdf", UploadedBy = "Pierce, Chase", RelatedImage = "singleimage.png", Message = "Message for Unit Test.", CreatedDate = DateTime.UtcNow},
                           new BubbleSheetError{ BubbleSheetErrorId = 3, ErrorCode = 16, BubbleSheetId = 13, FileName = "test2.pdf", UploadedBy = "Pierce, Chase", RelatedImage = "singleimage.png", Message = "Barcode could not be read.", CreatedDate = DateTime.UtcNow},
                           new BubbleSheetError{ BubbleSheetErrorId = 3, ErrorCode = -1, BubbleSheetId = 20, FileName = "test2.pdf", UploadedBy = "Pierce, Chase", RelatedImage = "singleimage.png", Message = "Unreadable roster position.", CreatedDate = DateTime.UtcNow},
                           new BubbleSheetError{ BubbleSheetErrorId = 4, ErrorCode = 64, BubbleSheetId = 14, FileName = "test3.pdf", UploadedBy = "Pierce, Chase", RelatedImage = "singleimage.png", Message = "Could not locate bubbles.", CreatedDate = DateTime.UtcNow},
                           new BubbleSheetError{ BubbleSheetErrorId = 5, ErrorCode = -1, BubbleSheetId = 15, FileName = "test4.pdf", UploadedBy = "Pierce, Chase", RelatedImage = "singleimage.png", Message = "Did not find correct number of questions.", CreatedDate = DateTime.UtcNow},
                           new BubbleSheetError{ BubbleSheetErrorId = 6, ErrorCode = 32, BubbleSheetId = 16, FileName = "test5.pdf", UploadedBy = "Pierce, Chase", RelatedImage = "singleimage.png", Message = "Unreadable roster position.", CreatedDate = DateTime.UtcNow},
                           new BubbleSheetError{ BubbleSheetErrorId = 7, ErrorCode = 8, BubbleSheetId = 17, FileName = "test5.pdf", UploadedBy = "Pierce, Chase", RelatedImage = "singleimage.png", Message = "Did not find correct number of bubbles for a question.", CreatedDate = DateTime.UtcNow},
                           new BubbleSheetError{ BubbleSheetErrorId = 2, ErrorCode = 0, BubbleSheetId = 19, FileName = "test5.pdf", UploadedBy = "Pierce, Chase", RelatedImage = "singleimage.png", Message = "Barcode.", CreatedDate = DateTime.UtcNow},
                           new BubbleSheetError{ BubbleSheetErrorId = 8, ErrorCode = 2, BubbleSheetId = 19, FileName = "test5.pdf", UploadedBy = "Pierce, Chase", RelatedImage = "singleimage.png", Message = "File corrupt.", CreatedDate = DateTime.UtcNow},
                           new BubbleSheetError{ BubbleSheetErrorId = 10, ErrorCode = 4, BubbleSheetId = 20, FileName = "test5.pdf", UploadedBy = "Pierce, Chase", RelatedImage = "singleimage.png", Message = "File corrupt.", CreatedDate = DateTime.UtcNow}
                       };
        }

        public IQueryable<BubbleSheetError> Select()
        {
            return table.AsQueryable();
        }
    }
}
