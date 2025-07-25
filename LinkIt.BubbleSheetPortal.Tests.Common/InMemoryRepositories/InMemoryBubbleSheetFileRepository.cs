using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryBubbleSheetFileRepository : IBubbleSheetFileRepository
    {
        private readonly List<BubbleSheetFile> table;
        private readonly List<BubbleSheetFile> bubbleSheetFileTicketTable; 
        private static int index = 6;

        public InMemoryBubbleSheetFileRepository()
        {
            table = AddBubbleSheetFiles();
            bubbleSheetFileTicketTable = AddBubbleSheetFileTicketEntries();
        }

        private List<BubbleSheetFile> AddBubbleSheetFiles()
        {
            return new List<BubbleSheetFile>
                       {
                           new BubbleSheetFile {BubbleSheetFileId = 1, BubbleSheetId = 123, InputFilePath = "path", Barcode1 = "10060000123", Barcode2 = "10001120517", Date = DateTime.UtcNow},
                           new BubbleSheetFile {BubbleSheetFileId = 2, BubbleSheetId = 124, InputFilePath = "partition1", OutputFileName = "row1", Barcode1 = "10060000124", Barcode2 = "10001120517", Date = DateTime.UtcNow},
                           new BubbleSheetFile {BubbleSheetFileId = 3, BubbleSheetId = 125, Barcode1 = "10060000125", Barcode2 = "10001120517", Date = DateTime.UtcNow},
                           new BubbleSheetFile {BubbleSheetFileId = 4, BubbleSheetId = 126, Barcode1 = "10060000126", Barcode2 = "10001120517", Date = DateTime.UtcNow},
                           new BubbleSheetFile {BubbleSheetFileId = 5, RosterPosition = 1, InputFileName = "file", UserId = 1, CreatedDate = DateTime.UtcNow, BubbleSheetId = 127, OutputFileName = "file", FileDisposition = "Created", IsConfirmed = false},
                           new BubbleSheetFile{ BubbleSheetFileId = 250, BubbleSheetId = 250, }
                       };
        }

        private List<BubbleSheetFile> AddBubbleSheetFileTicketEntries()
        {
            return new List<BubbleSheetFile>
                       {
                           new BubbleSheetFile {BubbleSheetFileId = 1, BubbleSheetId = 123, Ticket = "ticket1", IsGenericSheet = true, IsUnmappedGeneric = false, InputFilePath = "path", Barcode1 = "10060000123", Barcode2 = "10001120517", Date = DateTime.UtcNow},
                           new BubbleSheetFile {BubbleSheetFileId = 2, BubbleSheetId = 124, Ticket = "ticket1", IsGenericSheet = true, IsUnmappedGeneric = false, InputFilePath = "partition1", OutputFileName = "row1", Barcode1 = "10060000124", Barcode2 = "10001120517", Date = DateTime.UtcNow},
                           new BubbleSheetFile {BubbleSheetFileId = 3, BubbleSheetId = 125, Ticket = "ticket1", IsGenericSheet = true, IsUnmappedGeneric = false, Barcode1 = "10060000125", Barcode2 = "10001120517", Date = DateTime.UtcNow},
                           new BubbleSheetFile {BubbleSheetFileId = 4, BubbleSheetId = 126, Ticket = "ticket2", IsUnmappedGeneric = false, Barcode1 = "10060000126", Barcode2 = "10001120517", Date = DateTime.UtcNow},
                           new BubbleSheetFile {BubbleSheetFileId = 5, BubbleSheetId = 126, Ticket = "ticket2", IsUnmappedGeneric = true, Barcode1 = "10060000126", Barcode2 = "10001120517", Date = DateTime.UtcNow},
                           new BubbleSheetFile {BubbleSheetFileId = 6, BubbleSheetId = 126, Ticket = "ticket2", IsUnmappedGeneric = false, Barcode1 = "10060000126", Barcode2 = "10001120517", Date = DateTime.UtcNow},
                           new BubbleSheetFile {BubbleSheetFileId = 7, BubbleSheetId = 126, Ticket = "ticket2", IsUnmappedGeneric = false, Barcode1 = "10060000126", Barcode2 = "10001120517", Date = DateTime.UtcNow},
                       };
        }

        public IQueryable<BubbleSheetFile> Select()
        {
            return table.AsQueryable();
        }

        public IQueryable<BubbleSheetFile> SelectFromBubbleSheetFileTicketView()
        {
            return bubbleSheetFileTicketTable.AsQueryable();
        }

        public void Save(BubbleSheetFile item)
        {
            var entity = table.FirstOrDefault(x => x.BubbleSheetFileId.Equals(item.BubbleSheetFileId));

            if (entity.IsNull())
            {
                item.BubbleSheetFileId = index;
                index++;
                table.Add(item);
            }
            else
            {
                Mapper.CreateMap<School, School>();
                Mapper.Map(item, entity);
            }
        }

        public int SATGetDistinctPageCount(string ticket, int classID, int studentID)
        {
            throw new NotImplementedException();
        }

        public GenericBubbleSheet GetGenericFileInfor(int bubblesheetFileId)
        {
            throw new NotImplementedException();
        }
    }
}