using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryPrintingGroupJobRepository : IRepository<PrintingGroupJob>
    {
        private readonly List<PrintingGroupJob> table = new List<PrintingGroupJob>();
        private static int nextUniqueID = 100;

        public InMemoryPrintingGroupJobRepository()
        {
            table = AddPrintingGroupJob();
        }

        private List<PrintingGroupJob> AddPrintingGroupJob()
        {
            return new List<PrintingGroupJob>
                       {
                           new PrintingGroupJob { PrintingGroupJobID=1, GroupID=23,DateCreated=new System.DateTime(2012,5,15), CreatedUserID=4290  },
                           new PrintingGroupJob { PrintingGroupJobID=2, GroupID=11,DateCreated=new System.DateTime(2012,5,15), CreatedUserID=4290  },
                           new PrintingGroupJob { PrintingGroupJobID=3, GroupID=11,DateCreated=new System.DateTime(2012,5,15), CreatedUserID=4290  },
                           new PrintingGroupJob { PrintingGroupJobID=4, GroupID=12,DateCreated=new System.DateTime(2012,5,15), CreatedUserID=4290  },
                           new PrintingGroupJob { PrintingGroupJobID=5, GroupID=12,DateCreated=new System.DateTime(2012,5,15), CreatedUserID=4290  },
                           new PrintingGroupJob { PrintingGroupJobID=6, GroupID=12,DateCreated=new System.DateTime(2012,5,15), CreatedUserID=4290  },
                           new PrintingGroupJob { PrintingGroupJobID=7, GroupID=12,DateCreated=new System.DateTime(2012,5,15), CreatedUserID=4290  },
                           new PrintingGroupJob { PrintingGroupJobID=8, GroupID=12,DateCreated=new System.DateTime(2012,5,15), CreatedUserID=4290  },
                           new PrintingGroupJob { PrintingGroupJobID=9, GroupID=12,DateCreated=new System.DateTime(2012,5,15), CreatedUserID=4290  },
                           new PrintingGroupJob { PrintingGroupJobID=10, GroupID=12,DateCreated=new System.DateTime(2012,5,15), CreatedUserID=4290  },
                           new PrintingGroupJob { PrintingGroupJobID=11, GroupID=10,DateCreated=new System.DateTime(2012,5,15), CreatedUserID=4290  }                            
                       };
        }

        public IQueryable<PrintingGroupJob> Select()
        {
            return table.AsQueryable();
        }

        public void Save(PrintingGroupJob item)
        {
            var entity = table.FirstOrDefault(x => x.PrintingGroupJobID.Equals(item.PrintingGroupJobID));

            if (entity.IsNull())
            {
                item.PrintingGroupJobID = nextUniqueID;
                nextUniqueID++;
                table.Add(item);
            }
            else
            {
                Mapper.CreateMap<PrintingGroupJob, PrintingGroupJob>();
                Mapper.Map(item, entity);
            }
        }

        public void Delete(PrintingGroupJob item)
        {
            var entity = table.FirstOrDefault(x => x.PrintingGroupJobID.Equals(item.PrintingGroupJobID));
            if (entity.IsNotNull())
            {
                table.Remove(entity);
            }
        }
    }
}
