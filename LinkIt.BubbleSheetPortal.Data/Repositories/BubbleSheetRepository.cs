using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using AutoMapper;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class BubbleSheetRepository : IBubbleSheetRepository
    {
        private readonly Table<BubbleSheetEntity> table;

        public BubbleSheetRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = BubbleSheetDataContext.Get(connectionString).GetTable<BubbleSheetEntity>();
            Mapper.CreateMap<BubbleSheet, BubbleSheetEntity>()
                .ForMember(x => x.VirtualTestID, x => x.MapFrom(y => y.TestId))
                .ForMember(x => x.tDateTime, x => x.MapFrom(y => y.SubmittedDate));
        }

        public IQueryable<BubbleSheet> Select()
        {
            return table.Select(x => new BubbleSheet
                                             {
                                                 Id = x.BubblesheetID,
                                                 ClassId = x.ClassID??0,
                                                 SchoolId = x.SchoolID,
                                                 TeacherId = x.TeacherID,
                                                 StudentId = x.StudentID,
                                                 TestId = x.VirtualTestID,
                                                 DistrictTermId = x.DistrictTermID,
                                                 UserId = x.UserID,
                                                 SubmittedDate = x.tDateTime,
                                                 StudentIds = x.StudentIDs,
                                                 BubbleSheetCode = x.Bubblesheetcode,
                                                 BubbleSize = x.BubbleSize,
                                                 Ticket = x.Ticket,
                                                 CreatedByUserId = x.CreatedByUserID,
                                                 PrintingGroupJobID = x.PrintingGroupJobID ?? 0,
                                                 IsArchived = x.IsArchived,
                                                 IsManualEntry = x.IsManualEntry,
                                                 IsGenericSheet = x.IsGenericSheet ?? false,
                                                 TestExtract = x.TestExtract, 
                                                 ClassIds = x.ClassIDs
                                             });
        }

        public void Save(BubbleSheet item)
        {
            BubbleSheetEntity entity;

            if(item.Id.Equals(0))
            {
                entity = new BubbleSheetEntity();
                table.InsertOnSubmit(entity);
            }
            else
            {
                entity = table.FirstOrDefault(x => x.BubblesheetID.Equals(item.Id));
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.Id = entity.BubblesheetID;
        }

        public void Delete(BubbleSheet item)
        {
            var entity = table.FirstOrDefault(x => x.BubblesheetID.Equals(item.Id));

            if (!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public void ToggleArchiveBubbleSheets(IEnumerable<BubbleSheet> bubbleSheets)
        {
            var command = new StringBuilder();

            foreach (var bubbleSheet in bubbleSheets)
            {
                command.AppendLine(string.Format(@"UPDATE BubbleSheet SET IsArchived = '{0}' WHERE BubblesheetID = {1}", bubbleSheet.IsArchived ? '1' : '0', bubbleSheet.Id));
            }
            table.Context.ExecuteCommand(command.ToString());
        }

        public void UpdateBubbleSheetsWithTicket(IEnumerable<BubbleSheet> bubbleSheets, string ticket)
        {
            var command = new StringBuilder();
            
            //foreach (var bubbleSheet in bubbleSheets)
            //{
            //    command.AppendLine(string.Format(@"UPDATE BubbleSheet SET Ticket = '{0}' WHERE BubblesheetID = {1}", ticket, bubbleSheet.Id));
            //}

            command.AppendFormat("UPDATE Bubblesheet SET Ticket = '{0}' WHERE BubblesheetID IN (", ticket);
            foreach (var bubbleSheet in bubbleSheets)
            {
                command.AppendFormat("{0}, ", bubbleSheet.Id);
            }

            command.Append("-1)");
            var query = command.ToString();

            table.Context.ExecuteCommand(query);
        }

        public void Save(IList<BubbleSheet> listBubbleSheets)
        {
            Dictionary<BubbleSheetEntity, BubbleSheet> mapBubbleSheetItem = new Dictionary<BubbleSheetEntity, BubbleSheet>();
            foreach (var item in listBubbleSheets)
            {
                

                BubbleSheetEntity entity;
                if (item.Id.Equals(0))
                {
                    entity = new BubbleSheetEntity();
                    table.InsertOnSubmit(entity);
                }
                else
                {
                    entity = table.FirstOrDefault(x => x.BubblesheetID.Equals(item.Id));
                }
                if (entity != null)
                {
                    Mapper.Map(item, entity);

                    if (entity.ClassID == 0)
                        entity.ClassID = null; // support generate generic bubble sheet for large class

                    //item.Id = entity.BubblesheetID;
                    mapBubbleSheetItem.Add(entity, item);
                }
            }
            table.Context.SubmitChanges();
            foreach (var sheet in mapBubbleSheetItem)
            {
                sheet.Value.Id = sheet.Key.BubblesheetID;
            }
        }
    }
}