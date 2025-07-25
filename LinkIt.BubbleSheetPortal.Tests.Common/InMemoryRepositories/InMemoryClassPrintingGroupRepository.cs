using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories 
{
    public class InMemoryClassPrintingGroupRepository : IRepository<ClassPrintingGroup>
    {
        private readonly List<ClassPrintingGroup> table = new List<ClassPrintingGroup>();
        private static int nextUniqueID = 1;

        public InMemoryClassPrintingGroupRepository()
        {
            table = AddClassPrintingGroup();
        }

        private List<ClassPrintingGroup> AddClassPrintingGroup()
        {
            return new List<ClassPrintingGroup>
                       {
                           new ClassPrintingGroup { ClassGroupID=38, ClassID=39692, GroupID=10 },
                           new ClassPrintingGroup { ClassGroupID=39, ClassID=34662, GroupID=10 },
                           new ClassPrintingGroup { ClassGroupID=35, ClassID=7284, GroupID=11 },
                           new ClassPrintingGroup { ClassGroupID=36, ClassID=263386, GroupID=11 },
                           new ClassPrintingGroup { ClassGroupID=50, ClassID=163485, GroupID=12 },
                           new ClassPrintingGroup { ClassGroupID=51, ClassID=163484, GroupID=12 },
                           new ClassPrintingGroup { ClassGroupID=52, ClassID=7117, GroupID=12 },
                           new ClassPrintingGroup { ClassGroupID=53, ClassID=7118, GroupID=12 },
                           new ClassPrintingGroup { ClassGroupID=54, ClassID=7119, GroupID=12 },
                       };
        }

        public IQueryable<ClassPrintingGroup> Select()
        {
            return table.AsQueryable();
        }

        public void Save(ClassPrintingGroup item)
        {
            var entity = table.FirstOrDefault(x => x.ClassGroupID.Equals(item.ClassGroupID));

            if (entity.IsNull())
            {
                item.ClassGroupID = nextUniqueID;
                nextUniqueID++;
                table.Add(item);
            }
            else
            {
                Mapper.CreateMap<ClassPrintingGroup, ClassPrintingGroup>();
                Mapper.Map(item, entity);
            }
        }

        public void Delete(ClassPrintingGroup item)
        {
            var entity = table.FirstOrDefault(x => x.ClassGroupID == item.ClassGroupID);
            if (entity.IsNotNull())
            {
                table.Remove(entity);
            }
        }
    }
}
