using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class BubbleSheetPageRepository : IBubbleSheetPageRepository
    {
        private readonly Table<BubbleSheetProcessingSheetPageEntity> table;
        private readonly BubbleSheetDataContext _context;

        public BubbleSheetPageRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            this._context = BubbleSheetDataContext.Get(connectionString);
            table = _context.GetTable<BubbleSheetProcessingSheetPageEntity>();
        }

        public IQueryable<BubbleSheetPage> Select()
        {
            return table.Select(x => new BubbleSheetPage
                                     {
                                         PageNumber = x.PageNumber,
                                         PageNumberSub = x.PageNumberSub,
                                         SheetPageId = x.SheetPageId,
                                         Ticket = x.Ticket,
                                         SheetPageIdText = x.SheetPageIdText
                                     });
        }


        public IEnumerable<string> SearchSheetPageIdText(string id, int take)
        {
            var query = this.Select().Where(x => x.SheetPageIdText.StartsWith(id)).Select(x => x.SheetPageIdText).Take(take);

            var dbCommand = _context.GetCommand(query);

            foreach (DbParameter dbCommandParameter in dbCommand.Parameters)
            {
                dbCommandParameter.DbType = DbType.AnsiString; // SheetPageIdText is varchar(36)
                dbCommandParameter.Size = 36;
            }

            _context.Connection.Open();
            DbDataReader reader = dbCommand.ExecuteReader();
            List<string> result = _context.Translate<string>(reader).ToList();
            _context.Connection.Close();

            return result;
        }
    }
}
