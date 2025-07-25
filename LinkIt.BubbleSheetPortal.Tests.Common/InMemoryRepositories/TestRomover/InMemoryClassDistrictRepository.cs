using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories.TestRomover
{
    public class InMemoryClassDistrictRepository : IReadOnlyRepository<ClassDistrict>
    {
        private readonly List<ClassDistrict> table = new List<ClassDistrict>();

        public InMemoryClassDistrictRepository()
        {
            table = AddInMemoryClassDistrictRepository();
        }

        private List<ClassDistrict> AddInMemoryClassDistrictRepository()
        {
            return new List<ClassDistrict>
                    {                           
                      new ClassDistrict{ClassId = 1, DistrictId = 272, Name = "PLTW 7 9(B) 10", VirtualTestSourceId = 3}   ,
                      new ClassDistrict{ClassId = 2, DistrictId = 272, Name = "PLTW 7 9(B) 9", VirtualTestSourceId = 1}   ,
                      new ClassDistrict{ClassId = 3, DistrictId = 272, Name = "PLTW 7 9(B) 8", VirtualTestSourceId = 3}   ,
                      new ClassDistrict{ClassId = 4, DistrictId = 272, Name = "PLTW 7 9(B) 7", VirtualTestSourceId = 2}   ,
                      new ClassDistrict{ClassId = 5, DistrictId = 272, Name = "PLTW 7 9(B) 6", VirtualTestSourceId = 3}   ,
                      new ClassDistrict{ClassId = 6, DistrictId = 272, Name = "PLTW 7 9(B) 5", VirtualTestSourceId = 3}   ,
                      new ClassDistrict{ClassId = 7, DistrictId = 272, Name = "PLTW 7 9(B) 4", VirtualTestSourceId = 3}   ,
                      new ClassDistrict{ClassId = 8, DistrictId = 272, Name = "PLTW 7 9(B) 3", VirtualTestSourceId = 4}   ,
                      new ClassDistrict{ClassId = 9, DistrictId = 272, Name = "PLTW 7 9(B) 2", VirtualTestSourceId = 3}   ,
                      new ClassDistrict{ClassId = 10, DistrictId = 272, Name = "PLTW 7 9(B) 1", VirtualTestSourceId = 5}   
                    };
        }

        public IQueryable<ClassDistrict> Select()
        {
            return table.AsQueryable();
        }
    }
}
